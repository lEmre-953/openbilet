using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace openbilet
{
    public class TestForm : Form
    {
        private readonly Dictionary<string, Panel> _panels;
        private readonly Dictionary<string, Button> _siteButtons = new Dictionary<string, Button>();
        private readonly HashSet<string> _visibleSites = new HashSet<string>();
        private Panel _pnlBrowsers;
        private TextBox _txtLog;
        private readonly Form _ownerForm;

        public TestForm(Dictionary<string, Panel> browserPanels, Form ownerForm)
        {
            _panels = new Dictionary<string, Panel>(browserPanels);
            _ownerForm = ownerForm;
            Owner = ownerForm;
            InitializeUI();
        }

        private void InitializeUI()
        {
            Text = "Test Paneli - Site Görüntüleyici";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(44, 62, 80);
            FormBorderStyle = FormBorderStyle.Sizable;

            var pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(10, 5, 10, 5)
            };

            var btnYardim = new Button
            {
                Text = "Tümünü Göster",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                Size = new Size(120, 36),
                Location = new Point(10, 7),
                Cursor = Cursors.Hand
            };
            btnYardim.FlatAppearance.BorderSize = 0;
            btnYardim.Click += (s, e) => TumunuGoster();

            var btnGizle = new Button
            {
                Text = "Tümünü Gizle",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                Size = new Size(120, 36),
                Location = new Point(140, 7),
                Cursor = Cursors.Hand
            };
            btnGizle.FlatAppearance.BorderSize = 0;
            btnGizle.Click += (s, e) => TumunuGizle();

            pnlTop.Controls.Add(btnYardim);
            pnlTop.Controls.Add(btnGizle);

            int x = 280;
            var siteNames = new[] { "Efetur", "Duzceguven", "Metro", "Pamukkale", "Luxyalova", "Narlıca", "AliOsmanUlusoy" };
            var colors = new[]
            {
                Color.FromArgb(41, 128, 185),
                Color.FromArgb(39, 174, 96),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(241, 196, 15),
                Color.FromArgb(230, 126, 34),
                Color.FromArgb(26, 188, 156),
                Color.FromArgb(52, 152, 219)
            };

            for (int i = 0; i < siteNames.Length; i++)
            {
                var name = siteNames[i];
                if (!_panels.ContainsKey(name)) continue;

                var btn = new Button
                {
                    Text = name,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = colors[i],
                    ForeColor = Color.White,
                    Size = new Size(100, 36),
                    Location = new Point(x, 7),
                    Cursor = Cursors.Hand,
                    Tag = name
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += SiteButton_Click;
                _siteButtons[name] = btn;
                pnlTop.Controls.Add(btn);
                x += 110;
            }

            _pnlBrowsers = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(52, 73, 94)
            };

            var splitter = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 450,
                Panel1 = { BackColor = Color.FromArgb(44, 62, 80) },
                Panel2 = { BackColor = Color.FromArgb(44, 62, 80) }
            };

            _txtLog = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(33, 47, 60),
                ForeColor = Color.FromArgb(236, 240, 241),
                Font = new Font("Consolas", 9),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(10)
            };

            splitter.Panel1.Controls.Add(_pnlBrowsers);
            splitter.Panel2.Controls.Add(_txtLog);

            Controls.Add(splitter);
            Controls.Add(pnlTop);

            Log("Test paneli açıldı. Üstteki butonlarla görüntülemek istediğiniz siteleri seçin.");
            Log("Arka planda çalışan tarayıcılar burada görüntülenir.");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            foreach (var kvp in _panels)
            {
                if (kvp.Value.Parent != null)
                {
                    kvp.Value.Parent.Controls.Remove(kvp.Value);
                }
            }
        }

        private static readonly Dictionary<string, int> _panelOffsets = new Dictionary<string, int>
        {
            { "Efetur", 0 }, { "Duzceguven", 500 }, { "Metro", 600 }, { "Pamukkale", 700 }, { "Luxyalova", 800 }, { "Narlıca", 900 },{ "AliOsmanUlusoy", 1000 }
        };

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (var kvp in _panels)
            {
                _pnlBrowsers.Controls.Remove(kvp.Value);
                if (_ownerForm != null)
                {
                    var offset = _panelOffsets.ContainsKey(kvp.Key) ? _panelOffsets[kvp.Key] : 0;
                    kvp.Value.Visible = true;
                    kvp.Value.Location = new Point(-500 + offset, -500);
                    kvp.Value.Size = new Size(400, 300);
                    _ownerForm.Controls.Add(kvp.Value);
                }
            }
            base.OnFormClosing(e);
        }

        private void SiteButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var name = (string)btn.Tag;
            ToggleSite(name);
        }

        private void ToggleSite(string name)
        {
            if (!_panels.ContainsKey(name)) return;

            var panel = _panels[name];
            if (_visibleSites.Contains(name))
            {
                _pnlBrowsers.Controls.Remove(panel);
                _visibleSites.Remove(name);
                Log($"[Gizlendi] {name}");
            }
            else
            {
                foreach (var p in _panels.Values)
                    _pnlBrowsers.Controls.Remove(p);
                _visibleSites.Clear();

                panel.Dock = DockStyle.Fill;
                panel.Visible = true;
                _pnlBrowsers.Controls.Add(panel);
                _visibleSites.Add(name);
                Log($"[Gösterildi] {name} - Tam ekran");
            }

            GuncelleButonRenkleri();
        }

        private void TumunuGoster()
        {
            Log("Tek site seçin - tam ekran görüntüleme için şirket butonuna tıklayın.");
        }

        private void TumunuGizle()
        {
            foreach (var name in _visibleSites)
            {
                if (_panels.ContainsKey(name))
                    _pnlBrowsers.Controls.Remove(_panels[name]);
            }
            _visibleSites.Clear();
            GuncelleButonRenkleri();
            Log("[Tümü gizlendi]");
        }

        private void GuncelleButonRenkleri()
        {
            var siteNames = new[] { "Efetur", "Duzceguven", "Metro", "Pamukkale", "Luxyalova", "Narlıca", "AliOsmanUlusoy" };
            var colorsActive = new[]
            {
                Color.FromArgb(41, 128, 185),
                Color.FromArgb(39, 174, 96),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(241, 196, 15),
                Color.FromArgb(230, 126, 34),
                Color.FromArgb(26, 188, 156),
                Color.FromArgb(26, 188, 156)
            };
            var colorInactive = Color.FromArgb(127, 140, 141);

            for (int i = 0; i < siteNames.Length; i++)
            {
                if (_siteButtons.TryGetValue(siteNames[i], out var btn))
                {
                    btn.BackColor = _visibleSites.Contains(siteNames[i]) ? colorsActive[i] : colorInactive;
                }
            }
        }

        private void Log(string mesaj)
        {
            var line = $"[{DateTime.Now:HH:mm:ss}] {mesaj}\r\n";
            _txtLog.AppendText(line);
            _txtLog.SelectionStart = _txtLog.Text.Length;
            _txtLog.ScrollToCaret();
        }

        public void LogIslem(string site, string islem)
        {
            if (IsDisposed || !IsHandleCreated) return;
            if (InvokeRequired)
            {
                try { BeginInvoke(new Action(() => Log($"{site}: {islem}"))); }
                catch { }
            }
            else
            {
                Log($"{site}: {islem}");
            }
        }
    }
}
