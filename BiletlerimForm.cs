using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;

namespace openbilet
{
    public class BiletlerimForm : Form
    {
        private readonly int _kullaniciId;
        private readonly string _kullaniciAdi;
        private FlowLayoutPanel _flowPanel;
        private Label _lblBos;

        public BiletlerimForm(int kullaniciId, string kullaniciAdi)
        {
            _kullaniciId = kullaniciId;
            _kullaniciAdi = kullaniciAdi;
            InitializeUI();
            BiletleriYukle();
        }

        private void InitializeUI()
        {
            Text = "Biletlerim - " + _kullaniciAdi;
            Size = new Size(750, 650);
            MinimumSize = new Size(750, 500);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(236, 240, 241);

            // Header
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(41, 128, 185)
            };

            var lblTitle = new Label
            {
                Text = "Biletlerim",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitle);

            // Bilet kartları scroll paneli
            _flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(30, 20, 30, 20),
                BackColor = Color.FromArgb(236, 240, 241),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            _lblBos = new Label
            {
                Text = "Henüz bilet satın almadınız.",
                Font = new Font("Segoe UI", 12),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Padding = new Padding(20)
            };

            Controls.Add(_flowPanel);
            Controls.Add(pnlHeader);
        }

        private void BiletleriYukle()
        {
            _flowPanel.Controls.Clear();
            var biletler = BiletleriGetir();

            if (biletler.Count == 0)
            {
                _flowPanel.Controls.Add(_lblBos);
                return;
            }

            foreach (var bilet in biletler)
            {
                var kart = BiletKartiOlustur(bilet);
                _flowPanel.Controls.Add(kart);
            }
        }

        private List<BiletModel> BiletleriGetir()
        {
            var liste = new List<BiletModel>();
            try
            {
                using (var conn = new MySqlConnection(AppSettings.MySqlConnectionString))
                {
                    conn.Open();
                    var cmd = new MySqlCommand(
                        @"SELECT firma, kalkis, varis, tarih, saat, fiyat, koltuk_tipi, odeme_id, satin_alma_tarihi
                          FROM tickets WHERE user_id = @uid ORDER BY tarih DESC, saat DESC", conn);
                    cmd.Parameters.AddWithValue("@uid", _kullaniciId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var bilet = new BiletModel
                            {
                                Firma = reader.GetString("firma"),
                                Kalkis = reader.GetString("kalkis"),
                                Varis = reader.GetString("varis"),
                                Tarih = reader.GetDateTime("tarih"),
                                Saat = reader.GetString("saat"),
                                Fiyat = reader.GetString("fiyat"),
                                KoltukTipi = reader.GetString("koltuk_tipi"),
                                OdemeId = reader.GetString("odeme_id"),
                                SatinAlmaTarihi = reader.GetDateTime("satin_alma_tarihi")
                            };

                            // Süresi geçmiş mi kontrol et
                            DateTime seferZamani = bilet.Tarih.Date;
                            if (TimeSpan.TryParse(bilet.Saat, out TimeSpan saatSpan))
                                seferZamani = seferZamani.Add(saatSpan);
                            else
                                seferZamani = seferZamani.AddHours(23).AddMinutes(59);

                            bilet.SuresiGecmis = seferZamani < DateTime.Now;
                            liste.Add(bilet);
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Biletler yüklenemedi: " + ex.Message);
            }
            return liste;
        }

        private Panel BiletKartiOlustur(BiletModel bilet)
        {
            var kart = new Panel
            {
                Size = new Size(650, 140),
                Margin = new Padding(0, 0, 0, 15),
                BackColor = Color.Transparent,
                Tag = bilet
            };

            kart.Paint += (s, e) => BiletKartiCiz(e.Graphics, kart, bilet);
            return kart;
        }

        private void BiletKartiCiz(Graphics g, Panel kart, BiletModel bilet)
        {
            using (var bmp = new Bitmap(kart.Width, kart.Height))
            {
                using (var bg = Graphics.FromImage(bmp))
                {
                    bg.SmoothingMode = SmoothingMode.AntiAlias;
                    bg.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    var rect = new Rectangle(0, 0, kart.Width - 1, kart.Height - 1);

                    using (var path = RoundedRect(rect, 12))
                    {
                        bg.FillPath(Brushes.White, path);
                        using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                            bg.DrawPath(pen, path);
                    }

                    // Sol renkli şerit
                    Color seritRenk = bilet.SuresiGecmis
                        ? Color.FromArgb(149, 165, 166)
                        : Color.FromArgb(41, 128, 185);

                    using (var path = RoundedRect(new Rectangle(0, 0, 8, kart.Height - 1), 12))
                        bg.FillPath(new SolidBrush(seritRenk), path);
                    bg.FillRectangle(new SolidBrush(seritRenk), 4, 0, 8, kart.Height);

                    int x = 25, y = 14;

                    // Firma adı
                    using (var f = new Font("Segoe UI", 14, FontStyle.Bold))
                        bg.DrawString(bilet.Firma, f, new SolidBrush(Color.FromArgb(44, 62, 80)), x, y);

                    // Durum etiketi
                    string durum = bilet.SuresiGecmis ? "Süresi Geçti" : "Aktif";
                    Color durumRenk = bilet.SuresiGecmis
                        ? Color.FromArgb(231, 76, 60)
                        : Color.FromArgb(39, 174, 96);
                    using (var f = new Font("Segoe UI", 9, FontStyle.Bold))
                    {
                        var durumSize = bg.MeasureString(durum, f);
                        float dx = kart.Width - durumSize.Width - 25;
                        // Etiket arka planı
                        Color bgColor = bilet.SuresiGecmis
                            ? Color.FromArgb(40, 231, 76, 60)
                            : Color.FromArgb(40, 39, 174, 96);
                        bg.FillRoundedRectangle(new SolidBrush(bgColor),
                            new Rectangle((int)dx - 8, y, (int)durumSize.Width + 16, 26), 6);
                        bg.DrawString(durum, f, new SolidBrush(durumRenk), dx, y + 3);
                    }

                    y += 42;

                    // Güzergah
                    using (var f = new Font("Segoe UI", 11))
                    {
                        string guzergah = $"{bilet.Kalkis}  →  {bilet.Varis}";
                        bg.DrawString(guzergah, f, new SolidBrush(Color.FromArgb(52, 73, 94)), x, y);
                    }

                    y += 32;

                    // Detaylar satırı
                    using (var f = new Font("Segoe UI", 9))
                    {
                        var detayRenk = new SolidBrush(Color.FromArgb(127, 140, 141));
                        string tarihStr = bilet.Tarih.ToString("dd MMMM yyyy", new CultureInfo("tr-TR"));
                        bg.DrawString($"📅 {tarihStr}     ⏰ {bilet.Saat}     💺 {bilet.KoltukTipi}", f, detayRenk, x, y);
                    }

                    y += 26;

                    // Fiyat
                    using (var f = new Font("Segoe UI", 13, FontStyle.Bold))
                    {
                        string fiyatStr = bilet.Fiyat.Contains("TL") ? bilet.Fiyat : bilet.Fiyat + " TL";
                        bg.DrawString(fiyatStr, f, new SolidBrush(Color.FromArgb(41, 128, 185)), x, y);
                    }

                    // Ödeme ID (sağ alt)
                    using (var f = new Font("Segoe UI", 7.5f))
                    {
                        string idStr = "ID: " + (bilet.OdemeId.Length > 12 ? bilet.OdemeId.Substring(0, 12) + "..." : bilet.OdemeId);
                        var idSize = bg.MeasureString(idStr, f);
                        bg.DrawString(idStr, f, new SolidBrush(Color.FromArgb(180, 180, 180)),
                            kart.Width - idSize.Width - 20, kart.Height - 25);
                    }
                }

                if (bilet.SuresiGecmis)
                {
                    // Blur efekti: küçült ve tekrar büyüt
                    int kucukW = Math.Max(1, bmp.Width / 6);
                    int kucukH = Math.Max(1, bmp.Height / 6);

                    using (var kucuk = new Bitmap(kucukW, kucukH))
                    {
                        using (var gk = Graphics.FromImage(kucuk))
                        {
                            gk.InterpolationMode = InterpolationMode.HighQualityBilinear;
                            gk.DrawImage(bmp, 0, 0, kucukW, kucukH);
                        }

                        g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                        g.DrawImage(kucuk, 0, 0, kart.Width, kart.Height);
                    }

                    // Üzerine yarı saydam beyaz katman
                    using (var overlay = new SolidBrush(Color.FromArgb(100, 255, 255, 255)))
                    using (var path = RoundedRect(new Rectangle(0, 0, kart.Width - 1, kart.Height - 1), 12))
                        g.FillPath(overlay, path);

                    // "Süresi Geçti" damgası
                    using (var f = new Font("Segoe UI", 18, FontStyle.Bold))
                    {
                        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        var stampRect = new RectangleF(0, 0, kart.Width, kart.Height);

                        // Gölge
                        g.DrawString("SÜRESİ GEÇTİ", f,
                            new SolidBrush(Color.FromArgb(60, 0, 0, 0)),
                            new RectangleF(2, 2, kart.Width, kart.Height), sf);
                        // Ana metin
                        g.DrawString("SÜRESİ GEÇTİ", f,
                            new SolidBrush(Color.FromArgb(200, 192, 57, 43)), stampRect, sf);
                    }
                }
                else
                {
                    g.DrawImage(bmp, 0, 0);
                }
            }
        }

        private GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private class BiletModel
        {
            public string Firma { get; set; }
            public string Kalkis { get; set; }
            public string Varis { get; set; }
            public DateTime Tarih { get; set; }
            public string Saat { get; set; }
            public string Fiyat { get; set; }
            public string KoltukTipi { get; set; }
            public string OdemeId { get; set; }
            public DateTime SatinAlmaTarihi { get; set; }
            public bool SuresiGecmis { get; set; }
        }
    }
}
