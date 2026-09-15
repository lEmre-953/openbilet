using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace openbilet
{
    public class LoginForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        private Panel pnlLeft, pnlRight, pnlLogin, pnlRegister;
        private TextBox txtLoginKullanici, txtLoginSifre;
        private TextBox txtRegAd, txtRegSoyad, txtRegEmail, txtRegTelefon, txtRegKullanici, txtRegSifre;
        private Label lblLoginHata, lblRegHata;

        public int GirisYapanKullaniciId { get; private set; }
        public string GirisYapanKullaniciAdi { get; private set; }

        public LoginForm()
        {
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Open Bilet - Giriş";
            Size = new Size(900, 550);
            MinimumSize = new Size(900, 550);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(236, 240, 241);

            pnlLeft = new Panel
            {
                Dock = DockStyle.Left,
                Width = 340,
                BackColor = Color.FromArgb(41, 128, 185)
            };
            pnlLeft.Paint += PnlLeft_Paint;

            pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(40, 30, 40, 30)
            };

            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);

            BuildLoginPanel();
            BuildRegisterPanel();

            ShowLogin();
        }

        private void PnlLeft_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (var brush = new LinearGradientBrush(pnlLeft.ClientRectangle,
                Color.FromArgb(41, 128, 185), Color.FromArgb(44, 62, 80), 90f))
            {
                g.FillRectangle(brush, pnlLeft.ClientRectangle);
            }

            using (var titleFont = new Font("Segoe UI", 22, FontStyle.Bold))
            using (var subFont = new Font("Segoe UI", 10))
            {
                var sf = new StringFormat { Alignment = StringAlignment.Center };

                g.DrawString("Açık Bilet", titleFont, Brushes.White,
                    new RectangleF(0, 140, pnlLeft.Width, 50), sf);

                g.DrawString("Otobüs Sefer Karşılaştırma", subFont, Brushes.White,
                    new RectangleF(0, 195, pnlLeft.Width, 30), sf);

                g.DrawString("7 firma • Tek ekran • En iyi fiyat", subFont,
                    new SolidBrush(Color.FromArgb(180, 255, 255, 255)),
                    new RectangleF(0, 225, pnlLeft.Width, 30), sf);
            }
        }

        private void BuildLoginPanel()
        {
            pnlLogin = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            int y = 30;

            var lblBaslik = new Label
            {
                Text = "Giriş Yap",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(0, y),
                AutoSize = true
            };
            y += 60;

            var lblAltBaslik = new Label
            {
                Text = "Hesabınıza giriş yapın",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(127, 140, 141),
                Location = new Point(0, y),
                AutoSize = true
            };
            y += 50;

            txtLoginKullanici = CreateTextBox("Kullanıcı Adı", y);
            y += 60;

            txtLoginSifre = CreateTextBox("Şifre", y, true);
            y += 70;

            lblLoginHata = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(231, 76, 60),
                Font = new Font("Segoe UI", 8.5f),
                Location = new Point(0, y - 15),
                AutoSize = true
            };

            var btnGiris = CreateButton("Giriş Yap", Color.FromArgb(41, 128, 185), y);
            btnGiris.Click += BtnGiris_Click;
            y += 65;

            var lblKayitLink = new LinkLabel
            {
                Text = "Hesabınız yok mu? Kayıt Olun",
                Font = new Font("Segoe UI", 9),
                LinkColor = Color.FromArgb(41, 128, 185),
                Location = new Point(0, y),
                AutoSize = true
            };
            lblKayitLink.Click += (s, e) => ShowRegister();

            pnlLogin.Controls.AddRange(new Control[] {
                lblBaslik, lblAltBaslik, txtLoginKullanici, txtLoginSifre,
                lblLoginHata, btnGiris, lblKayitLink
            });
        }

        private void BuildRegisterPanel()
        {
            pnlRegister = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

            int y = 10;

            var lblBaslik = new Label
            {
                Text = "Kayıt Ol",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(0, y),
                AutoSize = true
            };
            y += 50;

            txtRegAd = CreateTextBox("Ad", y);
            txtRegSoyad = CreateTextBox("Soyad", y);
            txtRegSoyad.Location = new Point(250, y);
            txtRegAd.Size = new Size(220, 35);
            txtRegSoyad.Size = new Size(220, 35);
            y += 55;

            txtRegEmail = CreateTextBox("E-posta", y);
            y += 55;

            txtRegTelefon = CreateTextBox("Telefon", y);
            y += 55;

            txtRegKullanici = CreateTextBox("Kullanıcı Adı", y);
            y += 55;

            txtRegSifre = CreateTextBox("Şifre", y, true);
            y += 60;

            lblRegHata = new Label
            {
                Text = "",
                ForeColor = Color.FromArgb(231, 76, 60),
                Font = new Font("Segoe UI", 8.5f),
                Location = new Point(0, y - 10),
                AutoSize = true
            };

            var btnKayit = CreateButton("Kayıt Ol", Color.FromArgb(39, 174, 96), y);
            btnKayit.Click += BtnKayit_Click;
            y += 60;

            var lblGirisLink = new LinkLabel
            {
                Text = "Zaten hesabınız var mı? Giriş Yapın",
                Font = new Font("Segoe UI", 9),
                LinkColor = Color.FromArgb(41, 128, 185),
                Location = new Point(0, y),
                AutoSize = true
            };
            lblGirisLink.Click += (s, e) => ShowLogin();

            pnlRegister.Controls.AddRange(new Control[] {
                lblBaslik, txtRegAd, txtRegSoyad, txtRegEmail,
                txtRegTelefon, txtRegKullanici, txtRegSifre,
                lblRegHata, btnKayit, lblGirisLink
            });
        }

        private TextBox CreateTextBox(string placeholder, int y, bool isPassword = false)
        {
            var txt = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Size = new Size(480, 35),
                Location = new Point(0, y),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 247, 249),
                ForeColor = Color.FromArgb(44, 62, 80)
            };

            if (isPassword)
                txt.UseSystemPasswordChar = true;

            // Windows native placeholder (cue banner) — şifre alanında da düzgün çalışır
            SendMessage(txt.Handle, EM_SETCUEBANNER, (IntPtr)1, placeholder);

            return txt;
        }

        private Button CreateButton(string text, Color color, int y)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(480, 45),
                Location = new Point(0, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private string GetTextBoxValue(TextBox txt)
        {
            return txt.Text.Trim();
        }

        private void ShowLogin()
        {
            pnlRight.Controls.Clear();
            pnlRight.Controls.Add(pnlLogin);
            lblLoginHata.Text = "";
            Text = "Açık Bilet - Giriş";
        }

        private void ShowRegister()
        {
            pnlRight.Controls.Clear();
            pnlRight.Controls.Add(pnlRegister);
            lblRegHata.Text = "";
            Text = "Açık Bilet - Kayıt";
        }

        private void BtnGiris_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = GetTextBoxValue(txtLoginKullanici);
            string sifre = GetTextBoxValue(txtLoginSifre);

            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                lblLoginHata.Text = "Kullanıcı adı ve şifre boş bırakılamaz.";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(AppSettings.MySqlConnectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        "SELECT id, ad FROM users WHERE kullanici_adi = @user AND sifre = @pass LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@user", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@pass", sifre);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            GirisYapanKullaniciId = reader.GetInt32("id");
                            GirisYapanKullaniciAdi = reader.GetString("ad");
                            DialogResult = DialogResult.OK;
                            Close();
                        }
                        else
                        {
                            lblLoginHata.Text = "Kullanıcı adı veya şifre hatalı.";
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                lblLoginHata.Text = "Bağlantı hatası: " + ex.Message;
            }
        }

        private void BtnKayit_Click(object sender, EventArgs e)
        {
            string ad = GetTextBoxValue(txtRegAd);
            string soyad = GetTextBoxValue(txtRegSoyad);
            string email = GetTextBoxValue(txtRegEmail);
            string telefon = GetTextBoxValue(txtRegTelefon);
            string kullaniciAdi = GetTextBoxValue(txtRegKullanici);
            string sifre = GetTextBoxValue(txtRegSifre);

            if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) ||
                string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(sifre))
            {
                lblRegHata.Text = "Ad, soyad, kullanıcı adı ve şifre zorunludur.";
                return;
            }

            try
            {
                using (var conn = new MySqlConnection(AppSettings.MySqlConnectionString))
                {
                    conn.Open();

                    var checkCmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM users WHERE kullanici_adi = @user", conn);
                    checkCmd.Parameters.AddWithValue("@user", kullaniciAdi);
                    long count = (long)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        lblRegHata.Text = "Bu kullanıcı adı zaten kullanılıyor.";
                        return;
                    }

                    var cmd = new MySqlCommand(
                        @"INSERT INTO users (ad, soyad, email, telefon, kullanici_adi, sifre)
                          VALUES (@ad, @soyad, @email, @telefon, @user, @pass)", conn);
                    cmd.Parameters.AddWithValue("@ad", ad);
                    cmd.Parameters.AddWithValue("@soyad", soyad);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@telefon", telefon);
                    cmd.Parameters.AddWithValue("@user", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@pass", sifre);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Kayıt başarılı! Giriş yapabilirsiniz.", "Başarılı",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowLogin();
                    txtLoginKullanici.Text = kullaniciAdi;
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                lblRegHata.Text = "Kayıt hatası: " + ex.Message;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (pnlRight.Controls.Contains(pnlLogin))
                    BtnGiris_Click(this, EventArgs.Empty);
                else
                    BtnKayit_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
