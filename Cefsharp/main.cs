using CefSharp;
using CefSharp.WinForms;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using openbilet.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace openbilet
{
    public class main
    {
        public main(ChromiumWebBrowser browser)
        {
            _browser = browser;
        }

        private readonly ChromiumWebBrowser _browser;
        private bool CanExecuteJs => _browser.IsBrowserInitialized && _browser.CanExecuteJavascriptInMainFrame;
     
        public async Task  WaitForJsReadyAsync(int timeoutMs = 20000)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < timeoutMs)
            {
                var frame = _browser.GetMainFrame();
                if (frame != null && CanExecuteJs) return;
                await Task.Delay(50);
            }
            await Task.Delay(100);
        }
        public string ResolveStop(string comboBoxDegeri, string firmaAdi)
        {
            try
            {
                var conn = new MySqlConnection(AppSettings.MySqlConnectionString);
                conn.Open();

                var cmd = new MySqlCommand(
                    "SELECT id FROM stops WHERE UPPER(canonical_name) = @canonical AND is_active = 1 LIMIT 1",
                    conn);

                string canonical = comboBoxDegeri.Contains("(")
                    ? comboBoxDegeri.Split('(')[1].Replace(")", "").Trim()
                    : comboBoxDegeri.Trim();

                if (canonical == "MERKEZ")
                    canonical = comboBoxDegeri.Split('(')[0].Trim();

                cmd.Parameters.AddWithValue("@canonical", canonical);
                var stopIdObj = cmd.ExecuteScalar();
                if (stopIdObj == null) return comboBoxDegeri;

                int stopId = Convert.ToInt32(stopIdObj);

                cmd = new MySqlCommand(
                    @"SELECT alias_name FROM stop_aliases
              WHERE stop_id = @stopId
                AND (company = @firma OR company IS NULL)
              ORDER BY company DESC
              LIMIT 1",
                    conn);

                cmd.Parameters.AddWithValue("@stopId", stopId);
                cmd.Parameters.AddWithValue("@firma", firmaAdi);

                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    return result.ToString();


                conn.Close();
                return canonical;
            }
            catch
            {
                return comboBoxDegeri;
            }
        }
        public static string FirmaLinkGetir(string firmaAdi)
        {
            var conn = new MySqlConnection(AppSettings.MySqlConnectionString);
            conn.Open();
            var cmd = new MySqlCommand(
                "SELECT scrape_url FROM companies WHERE name = @firma", conn);
            cmd.Parameters.AddWithValue("@firma", firmaAdi);
            var result = cmd.ExecuteScalar();
            conn.Close();
            return result.ToString();
        }
        public async Task<JavascriptResponse> EvaluateScriptAsync(string script)
        {
            return await _browser.GetMainFrame().EvaluateScriptAsync(script);
        }

        public void ExecuteJavaScript(string jsCode)
        {
            if (!CanExecuteJs) return;
            _browser.GetMainFrame()?.ExecuteJavaScriptAsync(jsCode);
        }

        public async Task WaitForSeferResultsAsync(string loadingSelector, string resultSelector = null, int timeoutMs = 15000)
        {
            string combinedCheck = resultSelector != null
                ? $"({loadingSelector}) ? 'L' : (({resultSelector}) ? 'R' : 'N')"
                : $"({loadingSelector}) ? 'L' : 'N'";

            var deadline = DateTime.Now.AddMilliseconds(2500);
            bool loadingSeen = false;
            while (DateTime.Now < deadline)
            {
                string state = await EvalStringAsync(combinedCheck);
                if (state == "L") { loadingSeen = true; break; }
                if (state == "R") { await Task.Delay(100); return; }
                await Task.Delay(80);
            }

            if (loadingSeen)
            {
                deadline = DateTime.Now.AddMilliseconds(timeoutMs);
                while (DateTime.Now < deadline)
                {
                    if (!await EvalBoolAsync(loadingSelector))
                        break;
                    await Task.Delay(120);
                }
            }

            if (resultSelector != null)
            {
                deadline = DateTime.Now.AddMilliseconds(4000);
                while (DateTime.Now < deadline)
                {
                    if (await EvalBoolAsync(resultSelector))
                        break;
                    await Task.Delay(120);
                }
            }

            await Task.Delay(100);
        }

        public async Task<string> EvalStringAsync(string script)
        {
            try
            {
                var response = await EvaluateScriptAsync(script);
                if (response != null && response.Success && response.Result != null)
                    return response.Result.ToString();
            }
            catch { }
            return "";
        }

        public async Task<bool> EvalBoolAsync(string script)
        {
            try
            {
                var response = await EvaluateScriptAsync(script);
                if (response != null && response.Success && response.Result != null)
                    return Convert.ToBoolean(response.Result);
            }
            catch { }
            return false;
        }

        public void SetValue(string id, string value)
        {
            if (!CanExecuteJs) return;
            _browser.GetMainFrame()?.ExecuteJavaScriptAsync($"document.getElementById('{id}').value = '{value}';");
        }
        public string IdFinder(string id, string TagName, string Text)
        {
            if (!CanExecuteJs) return null;

            string script = $@"
        (function() {{
            var x = document.getElementById('{id}').getElementsByTagName('{TagName}');
            for (var i = 0; i < x.length; i++) {{
                if (x[i].innerText.includes('{Text}')) {{
                    return x[i].value;
                }}
            }}
            return null;
        }})();";
            
            var response = _browser.GetMainFrame().EvaluateScriptAsync(script).GetAwaiter().GetResult();
            if (response.Success && response.Result != null)
            {
                return response.Result.ToString();
            }
            else
            {
                return null;
            }
        }

        public void Dispatch(string id, string Eventtype)
        {
            if (!CanExecuteJs) return;
            _browser.GetMainFrame()?.ExecuteJavaScriptAsync($"document.getElementById('{id}').dispatchEvent(new Event('{Eventtype}'));");
        }
        public void Click(string id)
        {
            if (!CanExecuteJs) return;
            _browser.GetMainFrame()?.ExecuteJavaScriptAsync($"document.getElementById('{id}').click();");
        }
    }
}
