using MySql.Data.MySqlClient;
using openbilet.Services;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace openbilet
{
    public class OdemeForm : Form
    {

        // Sefer bilgileri
        private readonly string _firma;
        private readonly string _kalkis;
        private readonly string _varis;
        private readonly string _saat;
        private readonly string _fiyat;
        private readonly string _koltukTipi;
        private readonly string _tarih;
        private readonly int _kullaniciId;
        private readonly string _kullaniciAdi;

        // Kart alanları
        private TextBox txtKartSahibi, txtKartNo, txtCvc;
        private ComboBox cmbAy, cmbYil;

        // Alıcı alanları
        private TextBox txtAd, txtSoyad, txtEmail, txtTelefon;

        // Durum
        private Label lblSonuc;
        private Button btnOde;

        public OdemeForm(string firma, string kalkis, string varis, string saat,
            string fiyat, string koltukTipi, string tarih, int kullaniciId, string kullaniciAdi)
        {
            _firma = firma;
            _kalkis = kalkis;
            _varis = varis;
            _saat = saat;
            _fiyat = fiyat;
            _koltukTipi = koltukTipi;
            _tarih = tarih;
            _kullaniciId = kullaniciId;
            _kullaniciAdi = kullaniciAdi;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Ödeme - " + _firma;
            Size = new Size(560, 780);
            MinimumSize = new Size(560, 780);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(245, 247, 249);

            var pnlMain = new Panel
            {
                AutoScroll = true,
                Dock = DockStyle.Fill,
                Padding = new Padding(30, 20, 30, 20)
            };

            int y = 10;

            // ── Sefer Bilgisi Kartı ──
            var pnlSefer = CreateCard(10, y, 475, 130);
            pnlSefer.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new LinearGradientBrush(pnlSefer.ClientRectangle,
                    Color.FromArgb(41, 128, 185), Color.FromArgb(44, 62, 80), 0f))
                    e.Graphics.FillRoundedRectangle(brush, pnlSefer.ClientRectangle, 10);

                var sf = new StringFormat();
                using (var fBold = new Font("Segoe UI", 13, FontStyle.Bold))
                using (var fNormal = new Font("Segoe UI", 10))
                using (var fSmall = new Font("Segoe UI", 9))
                {
                    e.Graphics.DrawString(_firma, fBold, Brushes.White, 20, 12);
                    e.Graphics.DrawString($"{_kalkis}  →  {_varis}", fNormal, Brushes.White, 20, 42);
                    e.Graphics.DrawString($"Saat: {_saat}   |   {_koltukTipi}", fSmall,
                        new SolidBrush(Color.FromArgb(200, 255, 255, 255)), 20, 70);

                    using (var fPrice = new Font("Segoe UI", 16, FontStyle.Bold))
                        e.Graphics.DrawString(_fiyat, fPrice, Brushes.White, 350, 45);
                }
            };
            pnlMain.Controls.Add(pnlSefer);
            y += 145;

            // ── Sandbox Uyarısı ──
            var lblSandbox = new Label
            {
                Text = "TEST MODU - Gerçek ödeme alınmaz",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34),
                BackColor = Color.FromArgb(253, 245, 230),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(475, 28),
                Location = new Point(10, y)
            };
            pnlMain.Controls.Add(lblSandbox);
            y += 40;

            // ── Kart Bilgileri Bölümü ──
            pnlMain.Controls.Add(CreateSectionLabel("Kart Bilgileri", y));
            y += 30;

            pnlMain.Controls.Add(CreateFieldLabel("Kart Sahibi", y));
            y += 22;
            txtKartSahibi = CreateTextBox(y, 475);
            pnlMain.Controls.Add(txtKartSahibi);
            y += 42;

            pnlMain.Controls.Add(CreateFieldLabel("Kart Numarası", y));
            y += 22;
            txtKartNo = CreateTextBox(y, 475);
            txtKartNo.MaxLength = 19;
            txtKartNo.KeyPress += TxtKartNo_KeyPress;
            pnlMain.Controls.Add(txtKartNo);
            y += 42;

            // SKT ve CVC yan yana
            pnlMain.Controls.Add(CreateFieldLabel("Son Kullanma Tarihi", y));
            var lblCvcLabel = CreateFieldLabel("CVC", y);
            lblCvcLabel.Location = new Point(300, y);
            pnlMain.Controls.Add(lblCvcLabel);
            y += 22;

            cmbAy = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(100, 32),
                Location = new Point(10, y),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            for (int m = 1; m <= 12; m++)
                cmbAy.Items.Add(m.ToString("D2"));
            cmbAy.SelectedIndex = 0;

            cmbYil = new ComboBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(100, 32),
                Location = new Point(120, y),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            int currentYear = DateTime.Now.Year;
            for (int yr = currentYear; yr <= currentYear + 10; yr++)
                cmbYil.Items.Add(yr.ToString());
            cmbYil.SelectedIndex = 0;

            txtCvc = CreateTextBox(y, 165);
            txtCvc.Location = new Point(300, y);
            txtCvc.MaxLength = 4;
            txtCvc.UseSystemPasswordChar = true;
            txtCvc.KeyPress += (s, e) => { if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) e.Handled = true; };

            pnlMain.Controls.AddRange(new Control[] { cmbAy, cmbYil, txtCvc });
            y += 50;

            // ── Alıcı Bilgileri Bölümü ──
            pnlMain.Controls.Add(CreateSectionLabel("Alıcı Bilgileri", y));
            y += 30;

            // Ad ve Soyad yan yana
            pnlMain.Controls.Add(CreateFieldLabel("Ad", y));
            var lblSoyadLabel = CreateFieldLabel("Soyad", y);
            lblSoyadLabel.Location = new Point(250, y);
            pnlMain.Controls.Add(lblSoyadLabel);
            y += 22;

            txtAd = CreateTextBox(y, 228);
            txtSoyad = CreateTextBox(y, 228);
            txtSoyad.Location = new Point(250, y);
            pnlMain.Controls.AddRange(new Control[] { txtAd, txtSoyad });
            y += 42;

            pnlMain.Controls.Add(CreateFieldLabel("E-posta", y));
            y += 22;
            txtEmail = CreateTextBox(y, 475);
            pnlMain.Controls.Add(txtEmail);
            y += 42;

            pnlMain.Controls.Add(CreateFieldLabel("Telefon", y));
            y += 22;
            txtTelefon = CreateTextBox(y, 475);
            txtTelefon.Text = "+90";
            pnlMain.Controls.Add(txtTelefon);
            y += 50;

            // ── Test Kart Bilgisi ──
            var lblTestKart = new Label
            {
                Text = "Test Kartı: 5528 7900 0000 0008  |  SKT: 12/2030  |  CVC: 123",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(10, y),
                AutoSize = true
            };
            pnlMain.Controls.Add(lblTestKart);
            y += 30;

            // ── Sonuç Label ──
            lblSonuc = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                Location = new Point(10, y),
                Size = new Size(475, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlMain.Controls.Add(lblSonuc);
            y += 30;

            // ── Ödeme Butonu ──
            btnOde = new Button
            {
                Text = $"Ödemeyi Tamamla ({_fiyat})",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(475, 50),
                Location = new Point(10, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnOde.FlatAppearance.BorderSize = 0;
            btnOde.Click += BtnOde_Click;
            pnlMain.Controls.Add(btnOde);

            Controls.Add(pnlMain);
        }

        private void BtnOde_Click(object sender, EventArgs e)
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(txtKartSahibi.Text))
            { ShowHata("Kart sahibi adını girin."); return; }

            string kartNo = txtKartNo.Text.Replace(" ", "");
            if (kartNo.Length < 15 || kartNo.Length > 16)
            { ShowHata("Geçerli bir kart numarası girin."); return; }

            if (string.IsNullOrWhiteSpace(txtCvc.Text) || txtCvc.Text.Length < 3)
            { ShowHata("Geçerli bir CVC girin."); return; }

            if (string.IsNullOrWhiteSpace(txtAd.Text) || string.IsNullOrWhiteSpace(txtSoyad.Text))
            { ShowHata("Ad ve soyad zorunludur."); return; }

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !txtEmail.Text.Contains("@"))
            { ShowHata("Geçerli bir e-posta girin."); return; }

            btnOde.Enabled = false;
            btnOde.Text = "İşleniyor...";
            lblSonuc.Text = "";
            lblSonuc.ForeColor = Color.FromArgb(41, 128, 185);
            lblSonuc.Text = "Ödeme işleniyor, lütfen bekleyin...";
            Application.DoEvents();

            try
            {
                var sonuc = IyzicoService.OdemeYap(
                    kartSahibi: txtKartSahibi.Text.Trim(),
                    kartNumarasi: txtKartNo.Text,
                    sonKullanmaAy: cmbAy.SelectedItem.ToString(),
                    sonKullanmaYil: cmbYil.SelectedItem.ToString(),
                    cvc: txtCvc.Text.Trim(),
                    aliciAd: txtAd.Text.Trim(),
                    aliciSoyad: txtSoyad.Text.Trim(),
                    aliciEmail: txtEmail.Text.Trim(),
                    aliciTelefon: txtTelefon.Text.Trim(),
                    firmaAdi: _firma,
                    seferBilgisi: $"{_kalkis} → {_varis} | {_saat}",
                    fiyat: _fiyat
                );

                if (sonuc.Status == "success")
                {
                    lblSonuc.ForeColor = Color.FromArgb(39, 174, 96);
                    lblSonuc.Text = "Ödeme başarılı! E-posta gönderiliyor...";
                    Application.DoEvents();

                    BiletKaydet(sonuc.PaymentId);

                    // Bileti e-posta ile gönder (arka planda, UI bloklanmaz)
                    string aliciEmail = txtEmail.Text.Trim();
                    string aliciAdSoyad = $"{txtAd.Text.Trim()} {txtSoyad.Text.Trim()}";
                    bool epostaBasarili = EmailService.BiletEpostaGonder(
                        aliciEmail: aliciEmail,
                        aliciAdSoyad: aliciAdSoyad,
                        firma: _firma,
                        kalkis: _kalkis,
                        varis: _varis,
                        tarih: _tarih,
                        saat: _saat,
                        fiyat: _fiyat,
                        koltukTipi: _koltukTipi,
                        odemeId: sonuc.PaymentId
                    );

                    string epostaMesaj = epostaBasarili
                        ? $"\n\nBilet detayları {aliciEmail} adresine gönderildi."
                        : $"\n\n(E-posta gönderilemedi, ancak biletiniz kaydedildi.)";

                    MessageBox.Show(
                        $"Ödeme başarıyla tamamlandı!\n\n" +
                        $"Firma: {_firma}\n" +
                        $"Güzergah: {_kalkis} → {_varis}\n" +
                        $"Saat: {_saat}\n" +
                        $"Tutar: {_fiyat}\n" +
                        $"Ödeme ID: {sonuc.PaymentId}" +
                        epostaMesaj,
                        "Ödeme Başarılı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    string detay = $"Status: {sonuc.Status}\n" +
                                   $"StatusCode: {sonuc.StatusCode}\n" +
                                   $"ErrorCode: {sonuc.ErrorCode}\n" +
                                   $"ErrorGroup: {sonuc.ErrorGroup}\n" +
                                   $"ErrorMessage: {sonuc.ErrorMessage}";
                    lblSonuc.ForeColor = Color.FromArgb(231, 76, 60);
                    lblSonuc.Text = "Ödeme başarısız: " + (sonuc.ErrorMessage ?? "Bilinmeyen hata");
                    MessageBox.Show(detay, "iyzico Hata Detayı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnOde.Enabled = true;
                    btnOde.Text = $"Ödemeyi Tamamla ({_fiyat})";
                }
            }
            catch (Exception ex)
            {
                lblSonuc.ForeColor = Color.FromArgb(231, 76, 60);
                lblSonuc.Text = "Hata: " + ex.Message;
                btnOde.Enabled = true;
                btnOde.Text = $"Ödemeyi Tamamla ({_fiyat})";
            }
        }

        private void BiletKaydet(string odemeId)
        {
            try
            {
                using (var conn = new MySqlConnection(AppSettings.MySqlConnectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        @"INSERT INTO tickets (user_id, firma, kalkis, varis, tarih, saat, fiyat, koltuk_tipi, odeme_id)
                          VALUES (@uid, @firma, @kalkis, @varis, @tarih, @saat, @fiyat, @koltuk, @odeme)", conn);
                    cmd.Parameters.AddWithValue("@uid", _kullaniciId);
                    cmd.Parameters.AddWithValue("@firma", _firma);
                    cmd.Parameters.AddWithValue("@kalkis", _kalkis);
                    cmd.Parameters.AddWithValue("@varis", _varis);
                    cmd.Parameters.AddWithValue("@tarih", _tarih);
                    cmd.Parameters.AddWithValue("@saat", _saat);
                    cmd.Parameters.AddWithValue("@fiyat", _fiyat);
                    cmd.Parameters.AddWithValue("@koltuk", _koltukTipi);
                    cmd.Parameters.AddWithValue("@odeme", odemeId ?? "");
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch { }
        }

        private void ShowHata(string mesaj)
        {
            lblSonuc.ForeColor = Color.FromArgb(231, 76, 60);
            lblSonuc.Text = mesaj;
        }

        // Kart numarası formatlama (4'lü gruplar)
        private void TxtKartNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            if (char.IsDigit(e.KeyChar))
            {
                string raw = txtKartNo.Text.Replace(" ", "");
                if (raw.Length >= 16) { e.Handled = true; return; }

                raw += e.KeyChar;
                string formatted = "";
                for (int i = 0; i < raw.Length; i++)
                {
                    if (i > 0 && i % 4 == 0) formatted += " ";
                    formatted += raw[i];
                }

                txtKartNo.Text = formatted;
                txtKartNo.SelectionStart = formatted.Length;
                e.Handled = true;
            }
        }

        // ── UI Yardımcı Metodlar ──

        private Panel CreateCard(int x, int y, int w, int h)
        {
            return new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Color.Transparent
            };
        }

        private Label CreateSectionLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(10, y),
                AutoSize = true
            };
        }

        private Label CreateFieldLabel(string text, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(10, y),
                AutoSize = true
            };
        }

        private TextBox CreateTextBox(int y, int width)
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 10),
                Size = new Size(width, 32),
                Location = new Point(10, y),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
        }
    }

    // Rounded rectangle extension
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (var path = new GraphicsPath())
            {
                path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
                path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
                path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }
    }
}
