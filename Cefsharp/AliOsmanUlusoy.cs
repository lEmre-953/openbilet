using CefSharp.WinForms;
using Newtonsoft.Json;
using openbilet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openbilet.Cefsharp
{
    internal class AliOsmanUlusoy
    {
        private readonly main _main;
        string WaitLoading = "document.getElementById('load_full_page') != null || document.querySelector('.loading-overlay') != null || document.querySelector('.page-loading') != null";
        string ResultCheck = "document.querySelectorAll('.bilet-detay').length > 0";
        public AliOsmanUlusoy(ChromiumWebBrowser browser)
        {
            _main = new main(browser);
        }
        public main Cefsharp => _main;
        public async Task<List<SeferBilgisi>> AliOsmanUlusoySeferleriGetir()
        {
            string script = @"
        (function() {
            var seferler = [];
            var firma = 'AliOsmanUlusoy';
            var items = document.querySelectorAll('.bilet-detay');

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var rawId = item.id || '';
                var seferId = rawId.replace('journey-', '');
                if (!seferId) continue;
                var timeEl = item.querySelector('.tab2 .time');
                var saat = timeEl ? timeEl.innerText.trim() : '';
                var priceEl = item.querySelector('.tab7');
                var fiyat = priceEl ? priceEl.innerText.replace(/TRY|TL|\n/gi, '').trim() : '';
                var seatTypeEl = item.querySelector('.tab4');
                var koltukTipi = seatTypeEl ? seatTypeEl.innerText.trim() : 'Standart';
                var fromEl = item.querySelector('.tab3 .list-from');
                var toEl = item.querySelector('.tab3 .list-to');
                var guzergah = (fromEl ? fromEl.innerText.trim() : '') + ' > ' + (toEl ? toEl.innerText.trim() : '');
                var nightInfoEl = item.querySelector('.tab3 .night-info');
                var nightInfo = nightInfoEl ? nightInfoEl.innerText.trim() : '';
                var aciklama = nightInfo ? (guzergah + ' | ' + nightInfo) : guzergah;

                seferler.push({
                    Firma: firma,
                    Id: seferId,
                    Saat: saat,
                    KoltukTipi: koltukTipi,
                    Fiyat: fiyat,
                    Aciklama: aciklama
                });
            }
            
            return JSON.stringify(seferler);
        })();";

            var response = await Cefsharp.EvaluateScriptAsync(script);
            if (response == null) return new List<SeferBilgisi>();

            if (response.Success && response.Result != null)
            {
                string jsonResult = response.Result.ToString();
                List<SeferBilgisi> seferListesi = JsonConvert.DeserializeObject<List<SeferBilgisi>>(jsonResult);
                return seferListesi;
            }

            return new List<SeferBilgisi>();
        }

        public async Task<List<SeferBilgisi>> SeferAra(string kalkisMetin, string varisMetin, string tarih)
        {

            tarih = tarih.Replace(".", "-");
            DateTime dt = DateTime.ParseExact(tarih, "dd-MM-yyyy", null);
            string yeniFormat = dt.ToString("yyyy-MM-dd");
            kalkisMetin = Cefsharp.ResolveStop(kalkisMetin, "Ali Osman Ulusoy");
            varisMetin = Cefsharp.ResolveStop(varisMetin, "Ali Osman Ulusoy");

            await Cefsharp.WaitForJsReadyAsync();

            Cefsharp.ExecuteJavaScript($"document.querySelector('input[name=\"checkin_date\"]').value='{yeniFormat}';");
            Cefsharp.ExecuteJavaScript($"document.querySelector('input[name=\"checkin_date\"]').dispatchEvent(new Event('blur'))");
            Cefsharp.ExecuteJavaScript($"document.querySelector('input[name=\"checkin_date\"]').dispatchEvent(new Event('change'))");
            await Task.Delay(800);

            string kalkisid = Cefsharp.IdFinder("from", "option", kalkisMetin);
            Cefsharp.SetValue("from", kalkisid);
            Cefsharp.Dispatch("from", "change");
            await Task.Delay(150);

            string varisid = Cefsharp.IdFinder("to", "option", varisMetin);
            Cefsharp.SetValue("to", varisid);
            Cefsharp.Dispatch("to", "change");
            await Task.Delay(150);

            Cefsharp.ExecuteJavaScript("document.querySelector('input[value=\"SEFER ARA\"]').click();");
            await Cefsharp.WaitForSeferResultsAsync(WaitLoading, ResultCheck);
            var sonuc = await AliOsmanUlusoySeferleriGetir();
            return sonuc;
        }
    }
}

