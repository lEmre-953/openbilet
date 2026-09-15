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
    internal class Luxyalova
    {
        private readonly main _main;
        string WaitLoading = @"(document.querySelector('.loading') || document.querySelector('.loader') || document.querySelector('.page-loader') || document.querySelector('.preloader') || document.querySelector('.spinner') || document.querySelector('.overlay') || document.querySelector('[class*=""loading""]') || document.querySelector('[class*=""loader""]')) != null";
        string ResultCheck = "document.querySelectorAll('.journey-items').length > 0";
        public async Task<List<SeferBilgisi>> LuxYalovaSeferleriGetir()
        {
            string script = @"
            (function() {
            var seferler = [];
            var firma = 'Lüx Yalova';
            var items = document.querySelectorAll('.journey-items');

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
        
                var form = item.querySelector('form[action=""/satin-al""]');
                if (!form) continue; 

                var idInput = form.querySelector('input[name=""JourneyId""]');
                var seferId = idInput ? idInput.value : '';

                var dateInput = form.querySelector('input[name=""Date""]');
                var saat = dateInput ? dateInput.value.split(' ')[1] : '';

                var originInput = form.querySelector('input[name=""OriginText""]');
                var destInput = form.querySelector('input[name=""DestinationText""]');
                var aciklama = (originInput ? originInput.value : '') + ' - ' + (destInput ? destInput.value : '');

                var mainSpans = Array.from(item.children).filter(e => e.tagName === 'SPAN');
        
                var koltukTipi = mainSpans.length > 3 ? mainSpans[3].innerText.trim() : 'Standart';
        
                var fiyat = '';
                if (mainSpans.length > 4) {
                    fiyat = mainSpans[4].innerText.replace(/TRY|TL/gi, '').trim();
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
        public Luxyalova(ChromiumWebBrowser browser)
        {
            _main = new main(browser);
        }

        public main Cefsharp => _main;
        public async Task<List<SeferBilgisi>> SeferAra(string kalkisMetin, string varisMetin, string tarih)
        {
            tarih = tarih.Replace(".", "-");
            kalkisMetin = Cefsharp.ResolveStop(kalkisMetin, "Luxyalova");
            varisMetin = Cefsharp.ResolveStop(varisMetin, "Luxyalova");
            await Cefsharp.WaitForJsReadyAsync();
            await Task.Delay(300);

            Cefsharp.ExecuteJavaScript($"document.querySelector('input[name=\"Date\"]').value='{tarih}';");
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
            await Cefsharp.WaitForSeferResultsAsync(WaitLoading, ResultCheck);
            var sonuc = await LuxYalovaSeferleriGetir();
            return sonuc;
        }
    }
}
