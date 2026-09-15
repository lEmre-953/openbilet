using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using openbilet.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace openbilet
{
    internal class Efetur
    {
        private readonly main _main;
        string WaitLoading = "document.querySelector('.preloader') != null || document.querySelector('.loading') != null";
        string ResultCheck = "document.querySelectorAll(\"tr[id^='Sef']:not([id^='Sefack'])\").length > 0";
        public Efetur(ChromiumWebBrowser browser)
        {
            _main = new main(browser);
        }
        public main Cefsharp => _main;
        public async Task<List<SeferBilgisi>> EfeturSeferleriGetir()
        {
            string script = @"
        (function() {
            var seferler = [];
            var rows = document.querySelectorAll(""tr[id^='Sef']:not([id^='Sefack'])"");
            var firma = 'Efetur' 
            for(var i = 0; i < rows.length; i++) {
                var row = rows[i];
                var seferId = row.id.replace('Sef', ''); 
                
                var saat = row.cells[1] ? row.cells[1].innerText.trim() : '';
                var koltukTipi = row.cells[3] ? row.cells[3].innerText.trim() : '';
                
                var fiyatMetni = row.cells[5] ? row.cells[5].innerText.trim() : '';
                var fiyat = fiyatMetni.split(' ')[0]; 
                
                var aciklamaSatiri = document.getElementById('Sefack' + seferId);
                var aciklama = '';
                if(aciklamaSatiri && aciklamaSatiri.cells.length > 1) {
                    aciklama = aciklamaSatiri.cells[1].innerText.trim();
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
            tarih = tarih.Replace(".", "/");
            kalkisMetin = Cefsharp.ResolveStop(kalkisMetin, "Efetur");
            varisMetin = Cefsharp.ResolveStop(varisMetin, "Efetur");
            await Cefsharp.WaitForJsReadyAsync();

            Cefsharp.SetValue("Tarih", tarih);
            Cefsharp.Dispatch("Tarih", "blur");
            Cefsharp.Dispatch("Tarih", "change");
            await Task.Delay(150);

            string kalkisid = Cefsharp.IdFinder("Kalkis", "option", kalkisMetin);
            Cefsharp.SetValue("Kalkis", kalkisid);
            Cefsharp.Dispatch("Kalkis", "change");
            await Task.Delay(150);

            string varisid = Cefsharp.IdFinder("Varis", "option", varisMetin);
            Cefsharp.SetValue("Varis", varisid);
            Cefsharp.Dispatch("Varis", "change");
            await Task.Delay(150);

            Cefsharp.Click("seferListele");
            await Task.Delay(150);
            await Cefsharp.WaitForSeferResultsAsync(WaitLoading, ResultCheck);
            var sonuc = await EfeturSeferleriGetir();
            return sonuc;
        }
    }
}
