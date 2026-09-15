using CefSharp.WinForms;
using MySql.Data.MySqlClient;
using openbilet.Cefsharp;
using openbilet.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace openbilet
{
    public partial class Form1 : Form
    {
        private Efetur _efetur;
        private Duzceguven _duzceguven;
        private Metro _metro;
        private Pamukkale _pamukkale;
        private Luxyalova _luxyalova;
        private Narlica _narlica;
        private AliOsmanUlusoy _AliOsmanUlusoy;

        private readonly Dictionary<string, Panel> _browserPanels = new Dictionary<string, Panel>();

        public bool LogoutRequested { get; private set; }

        public static void DuraklariYukle(ComboBox nereden, ComboBox nereye)
        {
            try
            {
                var conn = new MySqlConnection(AppSettings.MySqlConnectionString);
                conn.Open();

                var cmd = new MySqlCommand(
                    "SELECT city, canonical_name FROM stops WHERE is_active = 1 ORDER BY city, canonical_name",
                    conn);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string durak;
                    if (reader.GetString("city") != reader.GetString("canonical_name"))
                    {
                        durak = string.Format("{0} ({1})",
                        reader.GetString("city").ToUpper(new CultureInfo("tr-TR")),
                        reader.GetString("canonical_name").ToUpper(new CultureInfo("tr-TR")));
                        nereden.Items.Add(durak);
                        nereye.Items.Add(durak);
                    }
                    else 
                    {
                        durak = string.Format("{0} ({1})",
                        reader.GetString("city").ToUpper(new CultureInfo("tr-TR")),
                        "MERKEZ");
                        nereden.Items.Add(durak);
                        nereye.Items.Add(durak);
                    }
                    
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Duraklar yüklenemedi: " + ex.Message);
            }
        }
        private ChromiumWebBrowser SiteTarayicisiOlustur(string name, string url, int offsetX)
        {
            var panel = new Panel
            {
                Size = new Size(400, 300),
                Location = new Point(-500 + offsetX, -500),
                Visible = true,
                Tag = name
            };
            var browser = new ChromiumWebBrowser(url) { Dock = DockStyle.Fill };
            panel.Controls.Add(browser);
            this.Controls.Add(panel);
            _browserPanels[name] = panel;
            return browser;
        }
        private readonly int _kullaniciId;
        private readonly string _kullaniciAdi;

        public Form1(int kullaniciId = 0, string kullaniciAdi = null)
        {
            _kullaniciId = kullaniciId;
            _kullaniciAdi = kullaniciAdi;
            InitializeComponent();
            if (!string.IsNullOrEmpty(_kullaniciAdi))
                lblTitle.Text = $"Otobüs Seferleri - Hoş geldin, {_kullaniciAdi}!";
            var browserEfetur = SiteTarayicisiOlustur("Efetur", main.FirmaLinkGetir("Efetur"), 0);
            var browserDuzceguven = SiteTarayicisiOlustur("Duzceguven", main.FirmaLinkGetir("Düzcegüven"), 500);
            var browserMetro = SiteTarayicisiOlustur("Metro", main.FirmaLinkGetir("Metro"), 600);
            var browserPamukkale = SiteTarayicisiOlustur("Pamukkale", main.FirmaLinkGetir("Pamukkale"), 700);
            var browserluxyalova = SiteTarayicisiOlustur("Luxyalova", main.FirmaLinkGetir("Lüxyalova"), 800);
            var browserNarlica = SiteTarayicisiOlustur("Narlıca", main.FirmaLinkGetir("Narlıca"), 900);
            var browserAliOsmanUlusoy = SiteTarayicisiOlustur("AliOsmanUlusoy", main.FirmaLinkGetir("AliOsmanUlusoy"), 1000);
            _efetur = new Efetur(browserEfetur);
            _duzceguven = new Duzceguven(browserDuzceguven);
            _metro = new Metro(browserMetro);
            _pamukkale = new Pamukkale(browserPamukkale);
            _luxyalova =  new Luxyalova(browserluxyalova);
            _narlica = new Narlica(browserNarlica);
            _AliOsmanUlusoy = new AliOsmanUlusoy(browserAliOsmanUlusoy);
            SetupForm(); 
        }

        private void SetupForm()
        {
            dgvSeferler.Columns.Clear();
            dgvSeferler.Columns.Add("Firma", "Firma");
            dgvSeferler.Columns.Add("Kalkis", "Kalkış");
            dgvSeferler.Columns.Add("Varis", "Varış");
            dgvSeferler.Columns.Add("Saat", "Saat");
            dgvSeferler.Columns.Add("Fiyat", "Fiyat");
            dgvSeferler.Columns.Add("Koltuk Tipi", "Koltuk Tipi");
            dgvSeferler.Columns.Add("Açıklama", "Açıklama");
            dgvSeferler.SelectionChanged += DgvSeferler_SelectionChanged;
            DuraklariYukle(cmbNereden, cmbNereye);
        }

        private void DgvSeferler_SelectionChanged(object sender, EventArgs e)
        {
            bool seciliMi = dgvSeferler.SelectedRows.Count > 0 && dgvSeferler.Rows.Count > 0;
            btnSatinAl.Enabled = seciliMi;

            if (seciliMi)
            {
                var row = dgvSeferler.SelectedRows[0];
                string fiyat = row.Cells["Fiyat"].Value?.ToString() ?? "";
                btnSatinAl.Text = string.IsNullOrWhiteSpace(fiyat) ? "Satın Al" : $"Satın Al ({fiyat})";
            }
            else
            {
                btnSatinAl.Text = "Satın Al";
            }
        }

        private void btnSatinAl_Click(object sender, EventArgs e)
        {
            if (dgvSeferler.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen bir sefer seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvSeferler.SelectedRows[0];
            string firma = row.Cells["Firma"].Value?.ToString() ?? "-";
            string kalkis = row.Cells["Kalkis"].Value?.ToString() ?? "-";
            string varis = row.Cells["Varis"].Value?.ToString() ?? "-";
            string saat = row.Cells["Saat"].Value?.ToString() ?? "-";
            string fiyat = row.Cells["Fiyat"].Value?.ToString() ?? "0";
            string koltukTipi = row.Cells["Koltuk Tipi"].Value?.ToString() ?? "-";

            if (string.IsNullOrWhiteSpace(fiyat) || fiyat == "-" || fiyat == "0")
            {
                MessageBox.Show("Bu seferin fiyat bilgisi bulunamadı.", "Uyarı",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tarih = dtpTarih.Value.ToString("yyyy-MM-dd");
            var odemeForm = new OdemeForm(firma, kalkis, varis, saat, fiyat, koltukTipi, tarih, _kullaniciId, _kullaniciAdi);
            odemeForm.ShowDialog(this);
        }

        private async void btnAra_Click_1(object sender, EventArgs e)
        {
            string nereden = cmbNereden.Text.ToUpper(new CultureInfo("tr-TR"));
            string nereye = cmbNereye.Text.ToUpper(new CultureInfo("tr-TR"));
            if (nereden == "" || nereye == "")
            {
                MessageBox.Show("Kalkış veya Varış Seçin");
                return;
            }

            lblStatus.Text = "Tüm sitelerde aranıyor...";
            lblStatus.ForeColor = Color.FromArgb(41, 128, 185);
            btnAra.Enabled = false;
            dgvSeferler.Rows.Clear();


            string tarih = dtpTarih.Value.ToString("dd/MM/yyyy");

            try
            {
                async Task<List<SeferBilgisi>> GuvenliAra(string site, Func<Task<List<SeferBilgisi>>> ara)
                {
                    try
                    {
                        OnIslemLog?.Invoke(site, "Arama başlatılıyor...");
                        var sonuc = await ara();
                        OnIslemLog?.Invoke(site, $"{sonuc?.Count ?? 0} sefer bulundu");
                        return sonuc ?? new List<SeferBilgisi>();
                    }
                    catch (Exception ex)
                    {
                        OnIslemLog?.Invoke(site, $"Hata: {ex.Message}");
                        return new List<SeferBilgisi>();
                    }
                }

                var gorevler = new[]
                {
                    GuvenliAra("Efetur", () => _efetur.SeferAra(nereden, nereye, tarih)),
                    GuvenliAra("Duzceguven", () => _duzceguven.SeferAra(nereden, nereye, tarih)),
                    GuvenliAra("Metro", () => _metro.SeferAra(nereden, nereye, tarih)),
                    GuvenliAra("Pamukkale", () => _pamukkale.SeferAra(nereden, nereye, tarih)),
                    GuvenliAra("Luxyalova", () => _luxyalova.SeferAra(nereden, nereye, tarih)),
                    GuvenliAra("Narlıca", () => _narlica.SeferAra(nereden, nereye, tarih)),
                    GuvenliAra("AliOsmanUlusoy", () => _AliOsmanUlusoy.SeferAra(nereden, nereye, tarih))
                };

                var sonuclar = await Task.WhenAll(gorevler);
                var tumSeferler = sonuclar.SelectMany(s => s ?? new List<SeferBilgisi>()).ToList();
                foreach (var sefer in tumSeferler)
                {
                    dgvSeferler.Rows.Add(
                        sefer.Firma ?? "-",
                        nereden,
                        nereye,
                        sefer.Saat,
                        (sefer.Fiyat ?? "") + (string.IsNullOrEmpty(sefer.Fiyat) ? "" : " TL"),
                        sefer.KoltukTipi ?? "-",
                        sefer.Aciklama
                        );
                }

                lblStatus.Text = tumSeferler.Count > 0
                    ? $"{tumSeferler.Count} sefer bulundu"
                    : "Sefer bulunamadı.";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Arama sırasında hata oluştu.";
                MessageBox.Show($"Hata: {ex.Message}", "Hata");
            }
            finally
            {
                btnAra.Enabled = true;
            }
        }

        private TestForm _testForm;
        internal Action<string, string> OnIslemLog;

        private void btnTest_Click(object sender, EventArgs e)
        {
            if (_testForm != null && !_testForm.IsDisposed)
            {
                _testForm.BringToFront();
                return;
            }
            _testForm = new TestForm(_browserPanels, this);
            _testForm.FormClosed += (s, args) =>
            {
                OnIslemLog = null;
                _testForm = null;
            };
            OnIslemLog = _testForm.LogIslem;
            _testForm.Show();
        }

        private void btnBiletlerim_Click(object sender, EventArgs e)
        {
            var biletlerimForm = new BiletlerimForm(_kullaniciId, _kullaniciAdi);
            biletlerimForm.ShowDialog(this);
        }

        private void btnCikis_Click(object sender, EventArgs e)
        {
            var sonuc = MessageBox.Show("Çıkış yapmak istediğinize emin misiniz?", "Çıkış",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (sonuc == DialogResult.Yes)
            {
                LogoutRequested = true;
                Close();
            }
        }

        private void pnlResults_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
