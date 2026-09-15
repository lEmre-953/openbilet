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
    internal class Duzceguven
    {
        private readonly main _main;
        string WaitLoading = "document.querySelector('.page-load') != null || document.querySelector('.preloader') != null || document.querySelector('.overlay') != null";
        string ResultCheck = "document.querySelectorAll('.booking-item').length > 0";
        public async Task<List<SeferBilgisi>> DuzceguvenSeferleriGetir()
        {
            string script = @"
        (function() {
            var seferler = [];
            var firma = 'Düzce Güven' 
            var items = document.querySelectorAll('.booking-item');

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var seferId = item.getAttribute('data-id');
                if (!seferId) continue;
                var saatEl = item.querySelector('.booking-item-clock b');
                var saat = saatEl ? saatEl.innerText.trim() : '';
                var fiyatEl = item.querySelector('.booking-item-price');
                var fiyat = fiyatEl ? fiyatEl.innerText.replace('TL', '').trim() : '';
                var koltukTipi = '';
                if (item.querySelector('.mdl21')) {
                    koltukTipi = '2+1';
                } else if (item.querySelector('.mdl22')) {
                    koltukTipi = '2+2';
                } else {
                    koltukTipi = 'Standart';
                }
                var aciklamaEl = item.querySelector('.travel-defination');
                var aciklama = aciklamaEl ? aciklamaEl.innerText.trim() : '';
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
        public Duzceguven(ChromiumWebBrowser browser)
        {
            _main = new main(browser);
        }

        public main Cefsharp => _main;
        public async Task<List<SeferBilgisi>> SeferAra(string kalkisMetin, string varisMetin, string tarih)
        {
            tarih = tarih.Replace(".","/");
            kalkisMetin = Cefsharp.ResolveStop(kalkisMetin, "Düzcegüven");
            varisMetin = Cefsharp.ResolveStop(varisMetin, "Düzcegüven");

            await Cefsharp.WaitForJsReadyAsync();

            Cefsharp.SetValue("seferTarih", tarih);
            Cefsharp.Dispatch("seferTarih", "blur");
            Cefsharp.Dispatch("seferTarih", "change");
            await Task.Delay(150);

            string kalkisid = Cefsharp.IdFinder("departure", "option", kalkisMetin);
            Cefsharp.SetValue("departure", kalkisid);
            Cefsharp.Dispatch("departure", "change");
            await Task.Delay(150);

            string varisid = Cefsharp.IdFinder("arrival", "option", varisMetin);
            Cefsharp.SetValue("arrival", varisid);
            Cefsharp.Dispatch("arrival", "change");
            await Task.Delay(150);

            Cefsharp.ExecuteJavaScript("document.querySelector('input[value=\"Sefer Listele\"]').click();");
            await Task.Delay(150);
            await Cefsharp.WaitForSeferResultsAsync(WaitLoading, ResultCheck);

            return await DuzceguvenSeferleriGetir();
        }
    }
}
