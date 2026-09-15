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
    public class Metro
    {
        private readonly main _main;
        string WaitLoading = "document.querySelector('.loading-container') != null || document.querySelector('.ajaxloading') != null || document.querySelector('img[src*=\"ajaxloading\"]') != null";
        string ResultCheck = "document.querySelectorAll('.journey-item').length > 0";
        public Metro(ChromiumWebBrowser browser)
        {
            _main = new main(browser);
        }

        public async Task<List<SeferBilgisi>> MetroSeferleriGetir()
        {
            string script = @"
        (function() {
            var seferler = [];
            var firma = 'Metro';
            var items = document.querySelectorAll('.journey-item');

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                var secButonu = item.querySelector('.ngSelectJourney');
                if (!secButonu) continue;

                var seferId = '';
                var fiyat = '';
                var saat = '';
                var koltukTipi = 'Bilinmiyor';
                var aciklama = '';

                if (secButonu) {
                    seferId = secButonu.getAttribute('data-journeyno') || '';
                    fiyat = secButonu.getAttribute('data-price') || '';
                    saat = secButonu.getAttribute('data-stophour') || '';
                } else {
                    var saatEl = item.querySelector('.journey-item-hour');
                    if (saatEl) saat = saatEl.innerText.trim();

                    var fiyatEl = item.querySelector('.price');
                    if (fiyatEl) fiyat = fiyatEl.innerText.trim();
                }
                if (item.querySelector('.SUIT-logo')) {
                    koltukTipi = 'SUIT (2+1)';
                } else if (item.querySelector('.CIPSUIT-logo')) {
                    koltukTipi = 'CIP SUIT (2+1)';
                } else {
                    koltukTipi = 'Standart';
                }

                if (secButonu) {
                    aciklama = secButonu.getAttribute('data-guzergah') || '';
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

        public main Cefsharp => _main;
        public async Task<List<SeferBilgisi>> SeferAra(string kalkisMetin, string varisMetin, string tarih)
        {
            tarih = tarih.Replace(".","/");
            kalkisMetin = Cefsharp.ResolveStop(kalkisMetin, "Metro");
            varisMetin = Cefsharp.ResolveStop(varisMetin, "Metro");
            await Cefsharp.WaitForJsReadyAsync();

            Cefsharp.SetValue("inpSearchJourneyBusBoardingDate", tarih);
            Cefsharp.Dispatch("inpSearchJourneyBusBoardingDate", "blur");
            Cefsharp.Dispatch("inpSearchJourneyBusBoardingDate", "change");
            await Task.Delay(150);

            string kalkisid = Cefsharp.IdFinder("selectBoardingTerminal", "option", kalkisMetin);
            Cefsharp.SetValue("selectBoardingTerminal", kalkisid);
            Cefsharp.Dispatch("selectBoardingTerminal", "change");
            await Task.Delay(150);

            string varisid = Cefsharp.IdFinder("selectLandingTerminal", "option", varisMetin);
            Cefsharp.SetValue("selectLandingTerminal", varisid);
            Cefsharp.Dispatch("selectLandingTerminal", "change");
            await Task.Delay(150);

            Cefsharp.Click("btnIndexSearchJourneys");
            await Cefsharp.WaitForSeferResultsAsync(WaitLoading, ResultCheck);

            return await MetroSeferleriGetir();
        }
    }
}

