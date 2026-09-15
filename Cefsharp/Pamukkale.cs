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
    internal class Pamukkale
    {
        private readonly main _main;
        string WaitLoading = "document.querySelector('.page-loader') != null || document.querySelector('.loading') != null || document.querySelector('.preloader') != null";
        string ResultCheck = "document.querySelectorAll('.sefer-list-kutu').length > 0";
        public Pamukkale(ChromiumWebBrowser browser)
        {
            _main = new main(browser);
        }
        
        public async Task<List<SeferBilgisi>> PamukkaleSeferleriGetir()
        {
            string script = @"
        (function() {
            var seferler = [];
            var firma = 'Pamukkale';
            var items = document.querySelectorAll('.sefer-list-kutu');

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                
                var dataElementi = item.querySelector('.sefer-list-kutu-tarih');
                
                if (!dataElementi) continue;

                var seferId = dataElementi.getAttribute('data-id') || '';
                if (!seferId) continue; // ID'si olmayan bozuk verileri atla

                var fiyat = dataElementi.getAttribute('data-fiyat') || '';

                var saatEl = item.querySelector('.sefersaat');
                var saat = saatEl ? saatEl.innerText.trim() : '';

                var aracImg = item.querySelector('.aracmodel img');
                var koltukTipi = aracImg ? (aracImg.getAttribute('title') || aracImg.getAttribute('alt')) : 'Standart';
                var aciklamaEl = item.querySelector('.biletaciklama');
                var aciklama = aciklamaEl ? aciklamaEl.innerText.trim().replace(/\n/g, ' ') : '';

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
        public main Cefsharp => _main;
        public async Task<List<SeferBilgisi>> SeferAra(string kalkisMetin, string varisMetin, string tarih)
        {
            Cefsharp.Click("allow-push-notification");
            tarih = tarih.Replace(".", "/");
            kalkisMetin = Cefsharp.ResolveStop(kalkisMetin, "Pamukkale");
            varisMetin = Cefsharp.ResolveStop(varisMetin, "Pamukkale");
            await Cefsharp.WaitForJsReadyAsync();

            string kalkisid = Cefsharp.IdFinder("kalkis-durak-list", "option", kalkisMetin);
            Cefsharp.SetValue("kalkis-durak-list", kalkisid);
            Cefsharp.Dispatch("kalkis-durak-list", "change");
            await Task.Delay(300);

            string varisid = Cefsharp.IdFinder("varis-durak-list", "option", varisMetin);
            Cefsharp.SetValue("varis-durak-list", varisid);
            Cefsharp.Dispatch("varis-durak-list", "change");
            await Task.Delay(300);

            Cefsharp.SetValue("tarihInputMain1", tarih);
            Cefsharp.Dispatch("tarihInputMain1", "blur");
            Cefsharp.Dispatch("tarihInputMain1", "change");
            await Task.Delay(150);

            Cefsharp.ExecuteJavaScript("document.querySelector('input[value=\"SEFER SORGULA\"]').click();");
            Cefsharp.Click("allow-push-notification");
            await Cefsharp.WaitForSeferResultsAsync(WaitLoading, ResultCheck);

            return await PamukkaleSeferleriGetir();
        }
    }
}

