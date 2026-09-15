using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace openbilet
{
    /// <summary>
    /// Aranabilir ComboBox - yazarken filtreleme ve "Aranıyor..." mesajı desteği
    /// </summary>
    public class SearchableComboBox : ComboBox
    {
        private List<string> _allItems = new List<string>();
        private const string ARANIYOR = "Aranıyor...";
        private const string SONUC_BULUNAMADI = "Sonuç bulunamadı";
        private bool _isUpdating;
        private string _lastSearchText = "";
        private Timer _searchTimer;
        private string _pendingSearchText;

        public SearchableComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDown;
            AutoCompleteMode = AutoCompleteMode.None;
            _searchTimer = new Timer { Interval = 250 };
            _searchTimer.Tick += SearchTimer_Tick;
            KeyDown += SearchableComboBox_KeyDown;
            TextChanged += SearchableComboBox_TextChanged;
            DropDown += SearchableComboBox_DropDown;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _searchTimer?.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Tüm şehirleri ekle (filtreleme için kaynak liste)
        /// </summary>
        public void SetItems(IEnumerable<string> items)
        {
            if (items == null) return;
            _allItems = items.ToList();
            _isUpdating = true;
            try
            {
                Items.Clear();
                Items.AddRange(_allItems.ToArray());
                _lastSearchText = "";
            }
            finally
            {
                _isUpdating = false;
            }
        }

        public event EventHandler SearchStarted;
        public event EventHandler SearchCompleted;

        private void SearchableComboBox_DropDown(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            if (_allItems.Count == 0) return;

            string searchText = Text ?? "";
            if (string.IsNullOrEmpty(searchText))
            {
                SearchStarted?.Invoke(this, EventArgs.Empty);
                _searchTimer.Stop();
                _pendingSearchText = "";
                _isUpdating = true;
                try
                {
                    Items.Clear();
                    Items.Add(ARANIYOR);
                }
                catch { }
                finally
                {
                    _isUpdating = false;
                }
                _searchTimer.Start();
            }
        }

        private void SearchableComboBox_TextChanged(object sender, EventArgs e)
        {
            if (_isUpdating) return;
            if (_allItems.Count == 0) return;

            string searchText = Text ?? "";
            if (searchText == _lastSearchText) return;

            _searchTimer.Stop();
            _pendingSearchText = searchText;
            SearchStarted?.Invoke(this, EventArgs.Empty);

            _isUpdating = true;
            try
            {
                Items.Clear();
                Items.Add(ARANIYOR);
                DroppedDown = true;
            }
            catch { }
            finally
            {
                _isUpdating = false;
            }

            _searchTimer.Start();
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            _searchTimer.Stop();

            string searchText = _pendingSearchText ?? "";
            _lastSearchText = searchText;

            _isUpdating = true;
            try
            {
                Items.Clear();

                var filtered = string.IsNullOrWhiteSpace(searchText)
                    ? _allItems
                    : _allItems.Where(s => s.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                foreach (var item in filtered)
                    Items.Add(item);

                if (filtered.Count == 0)
                    Items.Add(SONUC_BULUNAMADI);

                Text = searchText;
                if (searchText.Length > 0 && IsHandleCreated)
                {
                    try
                    {
                        SelectionStart = searchText.Length;
                        SelectionLength = 0;
                    }
                    catch { }
                }
            }
            catch { }
            finally
            {
                _isUpdating = false;
            }

            SearchCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void SearchableComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && DroppedDown)
            {
                e.SuppressKeyPress = true;
                if (SelectedIndex >= 0 && SelectedItem != null)
                {
                    string selected = SelectedItem.ToString();
                    if (selected != ARANIYOR && selected != SONUC_BULUNAMADI)
                    {
                        _isUpdating = true;
                        try
                        {
                            Text = selected;
                            DroppedDown = false;
                        }
                        finally
                        {
                            _isUpdating = false;
                        }
                    }
                }
            }
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            if (SelectedItem != null)
            {
                string selected = SelectedItem.ToString();
                if (selected == ARANIYOR || selected == SONUC_BULUNAMADI)
                {
                    SelectedIndex = -1;
                }
            }
            base.OnSelectionChangeCommitted(e);
        }
    }
}
