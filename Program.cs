using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace openbilet
{
    internal static class Program
    {
        /// <summary>
        /// Uygulamanın ana girdi noktası.
        /// </summary>
        [STAThread]
        static void Main()
        {
            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-gpu", "1");
            settings.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            Cef.Initialize(settings);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                var loginForm = new LoginForm();
                if (loginForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    break;

                var mainForm = new Form1(loginForm.GirisYapanKullaniciId, loginForm.GirisYapanKullaniciAdi);
                Application.Run(mainForm);

                if (!mainForm.LogoutRequested)
                    break;
            }
        }
    }
}
