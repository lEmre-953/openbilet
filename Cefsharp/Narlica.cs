using CefSharp;
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
    public class Narlica
    {
        private readonly main _main;
        string WaitLoading = "document.querySelector('.loading') != null || document.querySelector('.overlay') != null || document.querySelector('.preloader') != null";
        string ResultCheck = "document.querySelectorAll('.journey-items').length > 0";
        public Narlica(ChromiumWebBrowser browser)
        {
            _main = new main(browser);
        }
        public main Cefsharp => _main;
        public async Task<List<SeferBilgisi>> NarlicaSeferleriGetir()
        {
            string script = @"
        (function() {
            var seferler = [];
            var items = document.querySelectorAll('.journey-items');
            var firma = 'Narlıca';
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var form = item.querySelector('form[action=""/satin-al""]');
                if (!form) continue;

                var idInput = form.querySelector('input[name=""JourneyId""]');
                var seferId = idInput ? idInput.value : '';

                var dateInput = form.querySelector('input[name=""Date""]');
                var saat = dateInput ? dateInput.value.split(' ')[1] : '';

                // Güzergah ve notları birleştirme
                var originInput = form.querySelector('input[name=""OriginText""]');
                var destInput = form.querySelector('input[name=""DestinationText""]');
                var guzergah = (originInput ? originInput.value : '') + ' - ' + (destInput ? destInput.value : '');
        
                var noteEl = item.querySelector('.note');
                var noteBilgisi = noteEl ? noteEl.innerText.trim() : '';
                var aciklama = noteBilgisi ? (guzergah + ' | ' + noteBilgisi) : guzergah;

                // Görünür span'lardan koltuk tipi ve fiyat okuma
                var mainSpans = Array.from(item.querySelectorAll('span.generic-textfont'));
        
                // 4. span (index 3) Koltuk Tipi
                var koltukTipi = mainSpans.length > 3 ? mainSpans[3].innerText.trim() : 'Standart';
        
                // 5. span (index 4) Fiyat
                var fiyat = '';
                if (mainSpans.length > 4) {
                    fiyat = mainSpans[4].innerText.replace(/TRY|TL|\n/gi, '').trim();
                }

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
            kalkisMetin = Cefsharp.ResolveStop(kalkisMetin, "Narlıca");
            varisMetin = Cefsharp.ResolveStop(varisMetin, "Narlıca ");

            await Cefsharp.WaitForJsReadyAsync();

            Cefsharp.ExecuteJavaScript($"document.querySelector('input[name=\"date\"]').value='{tarih}';");
            Cefsharp.ExecuteJavaScript($"document.querySelector('input[name=\"date\"]').dispatchEvent(new Event('blur'))");
            Cefsharp.ExecuteJavaScript($"document.querySelector('input[name=\"date\"]').dispatchEvent(new Event('change'))");
            await Task.Delay(150);

            string kalkisid = Cefsharp.IdFinder("Origin", "option", kalkisMetin);
            Cefsharp.SetValue("Origin", kalkisid);
            Cefsharp.Dispatch("Origin", "change");
            await Task.Delay(150);

            string varisid = Cefsharp.IdFinder("Destination", "option", varisMetin);
            Cefsharp.SetValue("Destination", varisid);
            Cefsharp.Dispatch("Destination", "change");
            await Task.Delay(150);

            Cefsharp.ExecuteJavaScript("document.querySelector('button[type=\"submit\"].button.full-width').click();");
            await Task.Delay(150);
            await Cefsharp.WaitForSeferResultsAsync(WaitLoading, ResultCheck);
            var sonuc = await NarlicaSeferleriGetir();
            return sonuc;
        }
    }
}

