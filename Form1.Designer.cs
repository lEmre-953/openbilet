namespace openbilet
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnTest = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnAra = new System.Windows.Forms.Button();
            this.dtpTarih = new System.Windows.Forms.DateTimePicker();
            this.lblTarih = new System.Windows.Forms.Label();
            this.lblNereye = new System.Windows.Forms.Label();
            this.lblNereden = new System.Windows.Forms.Label();
            this.pnlResults = new System.Windows.Forms.Panel();
            this.dgvSeferler = new System.Windows.Forms.DataGridView();
            this.lblResultsTitle = new System.Windows.Forms.Label();
            this.cmbNereye = new openbilet.SearchableComboBox();
            this.cmbNereden = new openbilet.SearchableComboBox();
            this.pnlHeader.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeferler)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnBiletlerim = new System.Windows.Forms.Button();
            this.btnCikis = new System.Windows.Forms.Button();
            this.pnlHeader.Controls.Add(this.btnCikis);
            this.pnlHeader.Controls.Add(this.btnBiletlerim);
            this.pnlHeader.Controls.Add(this.btnTest);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1305, 108);
            this.pnlHeader.TabIndex = 0;
            // 
            // btnTest
            // 
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTest.FlatAppearance.BorderSize = 0;
            this.btnTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTest.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTest.ForeColor = System.Drawing.Color.White;
            this.btnTest.Location = new System.Drawing.Point(1180, 35);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(100, 38);
            this.btnTest.TabIndex = 1;
            this.btnTest.Text = "Test";
            this.btnTest.UseVisualStyleBackColor = false;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            //
            // btnBiletlerim
            //
            this.btnBiletlerim.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBiletlerim.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnBiletlerim.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBiletlerim.FlatAppearance.BorderSize = 0;
            this.btnBiletlerim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBiletlerim.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnBiletlerim.ForeColor = System.Drawing.Color.White;
            this.btnBiletlerim.Location = new System.Drawing.Point(1050, 35);
            this.btnBiletlerim.Name = "btnBiletlerim";
            this.btnBiletlerim.Size = new System.Drawing.Size(120, 38);
            this.btnBiletlerim.TabIndex = 2;
            this.btnBiletlerim.Text = "Biletlerim";
            this.btnBiletlerim.UseVisualStyleBackColor = false;
            this.btnBiletlerim.Click += new System.EventHandler(this.btnBiletlerim_Click);
            //
            // btnCikis
            //
            this.btnCikis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCikis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnCikis.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCikis.FlatAppearance.BorderSize = 0;
            this.btnCikis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCikis.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnCikis.ForeColor = System.Drawing.Color.White;
            this.btnCikis.Location = new System.Drawing.Point(920, 35);
            this.btnCikis.Name = "btnCikis";
            this.btnCikis.Size = new System.Drawing.Size(120, 38);
            this.btnCikis.TabIndex = 3;
            this.btnCikis.Text = "Çıkış Yap";
            this.btnCikis.UseVisualStyleBackColor = false;
            this.btnCikis.Click += new System.EventHandler(this.btnCikis_Click);
            //
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1305, 108);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Otobüs Seferleri";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.pnlSearch.Controls.Add(this.lblStatus);
            this.pnlSearch.Controls.Add(this.btnAra);
            this.pnlSearch.Controls.Add(this.dtpTarih);
            this.pnlSearch.Controls.Add(this.lblTarih);
            this.pnlSearch.Controls.Add(this.cmbNereye);
            this.pnlSearch.Controls.Add(this.lblNereye);
            this.pnlSearch.Controls.Add(this.cmbNereden);
            this.pnlSearch.Controls.Add(this.lblNereden);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(0, 108);
            this.pnlSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(30, 31, 30, 31);
            this.pnlSearch.Size = new System.Drawing.Size(1305, 246);
            this.pnlSearch.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(140)))), ((int)(((byte)(141)))));
            this.lblStatus.Location = new System.Drawing.Point(240, 157);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 25);
            this.lblStatus.TabIndex = 7;
            // 
            // btnAra
            // 
            this.btnAra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnAra.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAra.FlatAppearance.BorderSize = 0;
            this.btnAra.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAra.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAra.ForeColor = System.Drawing.Color.White;
            this.btnAra.Location = new System.Drawing.Point(39, 138);
            this.btnAra.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAra.Name = "btnAra";
            this.btnAra.Size = new System.Drawing.Size(180, 62);
            this.btnAra.TabIndex = 6;
            this.btnAra.Text = "Sefer Ara";
            this.btnAra.UseVisualStyleBackColor = false;
            this.btnAra.Click += new System.EventHandler(this.btnAra_Click_1);
            // 
            // dtpTarih
            // 
            this.dtpTarih.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpTarih.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTarih.Location = new System.Drawing.Point(962, 71);
            this.dtpTarih.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dtpTarih.Name = "dtpTarih";
            this.dtpTarih.Size = new System.Drawing.Size(268, 34);
            this.dtpTarih.TabIndex = 5;
            // 
            // lblTarih
            // 
            this.lblTarih.AutoSize = true;
            this.lblTarih.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblTarih.Location = new System.Drawing.Point(957, 38);
            this.lblTarih.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTarih.Name = "lblTarih";
            this.lblTarih.Size = new System.Drawing.Size(53, 28);
            this.lblTarih.TabIndex = 4;
            this.lblTarih.Text = "Tarih";
            // 
            // lblNereye
            // 
            this.lblNereye.AutoSize = true;
            this.lblNereye.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblNereye.Location = new System.Drawing.Point(609, 38);
            this.lblNereye.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNereye.Name = "lblNereye";
            this.lblNereye.Size = new System.Drawing.Size(74, 28);
            this.lblNereye.TabIndex = 2;
            this.lblNereye.Text = "Nereye";
            // 
            // lblNereden
            // 
            this.lblNereden.AutoSize = true;
            this.lblNereden.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblNereden.Location = new System.Drawing.Point(240, 38);
            this.lblNereden.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNereden.Name = "lblNereden";
            this.lblNereden.Size = new System.Drawing.Size(87, 28);
            this.lblNereden.TabIndex = 0;
            this.lblNereden.Text = "Nereden";
            // 
            // pnlResults
            // 
            this.btnSatinAl = new System.Windows.Forms.Button();
            this.pnlResults.Controls.Add(this.btnSatinAl);
            this.pnlResults.Controls.Add(this.dgvSeferler);
            this.pnlResults.Controls.Add(this.lblResultsTitle);
            this.pnlResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlResults.Location = new System.Drawing.Point(0, 354);
            this.pnlResults.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pnlResults.Name = "pnlResults";
            this.pnlResults.Padding = new System.Windows.Forms.Padding(30, 31, 30, 31);
            this.pnlResults.Size = new System.Drawing.Size(1305, 415);
            this.pnlResults.TabIndex = 2;
            this.pnlResults.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlResults_Paint);
            // 
            // dgvSeferler
            // 
            this.dgvSeferler.AllowUserToAddRows = false;
            this.dgvSeferler.AllowUserToDeleteRows = false;
            this.dgvSeferler.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSeferler.BackgroundColor = System.Drawing.Color.White;
            this.dgvSeferler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSeferler.Location = new System.Drawing.Point(39, 69);
            this.dgvSeferler.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvSeferler.MultiSelect = false;
            this.dgvSeferler.Name = "dgvSeferler";
            this.dgvSeferler.ReadOnly = true;
            this.dgvSeferler.RowHeadersVisible = false;
            this.dgvSeferler.RowHeadersWidth = 62;
            this.dgvSeferler.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSeferler.Size = new System.Drawing.Size(1210, 300);
            this.dgvSeferler.TabIndex = 1;
            // 
            // lblResultsTitle
            // 
            this.lblResultsTitle.AutoSize = true;
            this.lblResultsTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblResultsTitle.Location = new System.Drawing.Point(34, 15);
            this.lblResultsTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResultsTitle.Name = "lblResultsTitle";
            this.lblResultsTitle.Size = new System.Drawing.Size(136, 30);
            this.lblResultsTitle.TabIndex = 0;
            this.lblResultsTitle.Text = "Sefer Listesi";
            //
            // btnSatinAl
            //
            this.btnSatinAl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSatinAl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.btnSatinAl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSatinAl.FlatAppearance.BorderSize = 0;
            this.btnSatinAl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSatinAl.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSatinAl.ForeColor = System.Drawing.Color.White;
            this.btnSatinAl.Location = new System.Drawing.Point(1069, 10);
            this.btnSatinAl.Name = "btnSatinAl";
            this.btnSatinAl.Size = new System.Drawing.Size(180, 45);
            this.btnSatinAl.TabIndex = 2;
            this.btnSatinAl.Text = "Satın Al";
            this.btnSatinAl.UseVisualStyleBackColor = false;
            this.btnSatinAl.Enabled = false;
            this.btnSatinAl.Click += new System.EventHandler(this.btnSatinAl_Click);
            //
            // cmbNereye
            // 
            this.cmbNereye.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbNereye.FormattingEnabled = true;
            this.cmbNereye.Location = new System.Drawing.Point(614, 69);
            this.cmbNereye.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbNereye.Name = "cmbNereye";
            this.cmbNereye.Size = new System.Drawing.Size(298, 36);
            this.cmbNereye.TabIndex = 3;
            // 
            // cmbNereden
            // 
            this.cmbNereden.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbNereden.FormattingEnabled = true;
            this.cmbNereden.Location = new System.Drawing.Point(245, 69);
            this.cmbNereden.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmbNereden.Name = "cmbNereden";
            this.cmbNereden.Size = new System.Drawing.Size(298, 36);
            this.cmbNereden.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1305, 769);
            this.Controls.Add(this.pnlResults);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.pnlHeader);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(889, 662);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Açık Bilet - Otobüs Seferleri";
            this.pnlHeader.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlResults.ResumeLayout(false);
            this.pnlResults.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSeferler)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblNereden;
        private SearchableComboBox cmbNereden;
        private System.Windows.Forms.Label lblNereye;
        private SearchableComboBox cmbNereye;
        private System.Windows.Forms.Label lblTarih;
        private System.Windows.Forms.DateTimePicker dtpTarih;
        private System.Windows.Forms.Button btnAra;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlResults;
        private System.Windows.Forms.Label lblResultsTitle;
        private System.Windows.Forms.DataGridView dgvSeferler;
        private System.Windows.Forms.Button btnSatinAl;
        private System.Windows.Forms.Button btnBiletlerim;
        private System.Windows.Forms.Button btnCikis;
    }
}
