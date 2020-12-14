
namespace GlucoseTrayCore.Views
{
    partial class Settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radio_dexcom = new System.Windows.Forms.RadioButton();
            this.radio_nightscout = new System.Windows.Forms.RadioButton();
            this.label_glucose_fetch = new System.Windows.Forms.Label();
            this.datasource_grid = new System.Windows.Forms.TableLayoutPanel();
            this.dexcom_settings_grid = new System.Windows.Forms.TableLayoutPanel();
            this.radio_dexcom_server_us_share2 = new System.Windows.Forms.RadioButton();
            this.label_dexcom_username = new System.Windows.Forms.Label();
            this.label_dexcom_password = new System.Windows.Forms.Label();
            this.textBox_dexcom_username = new System.Windows.Forms.TextBox();
            this.label_dexcom_server = new System.Windows.Forms.Label();
            this.radio_dexcom_server_us_share1 = new System.Windows.Forms.RadioButton();
            this.radio_dexcom_server_international = new System.Windows.Forms.RadioButton();
            this.maskedText_dexcom_password = new System.Windows.Forms.MaskedTextBox();
            this.nightscout_grid = new System.Windows.Forms.TableLayoutPanel();
            this.label_nightscout_url = new System.Windows.Forms.Label();
            this.label_nightscout_token = new System.Windows.Forms.Label();
            this.textBox_nightscout_url = new System.Windows.Forms.TextBox();
            this.textBox_nightscout_token = new System.Windows.Forms.TextBox();
            this.glucose_unit_grid = new System.Windows.Forms.TableLayoutPanel();
            this.radio_glucose_unit_mmol = new System.Windows.Forms.RadioButton();
            this.label_glucose_unit = new System.Windows.Forms.Label();
            this.radio_glucose_unit_mg = new System.Windows.Forms.RadioButton();
            this.glucose_values_grid = new System.Windows.Forms.TableLayoutPanel();
            this.label_glucose_thresholds = new System.Windows.Forms.Label();
            this.label_glucose_high = new System.Windows.Forms.Label();
            this.label_glucose_warning_high = new System.Windows.Forms.Label();
            this.label_glucose_warning_low = new System.Windows.Forms.Label();
            this.label_glucose_low = new System.Windows.Forms.Label();
            this.label_glucose_critical = new System.Windows.Forms.Label();
            this.numeric_glucose_high = new System.Windows.Forms.NumericUpDown();
            this.numeric_glucose_warning_high = new System.Windows.Forms.NumericUpDown();
            this.numeric_glucose_warning_low = new System.Windows.Forms.NumericUpDown();
            this.numeric_glucose_low = new System.Windows.Forms.NumericUpDown();
            this.numeric_glucose_critical = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.numeric_polling_threshold = new System.Windows.Forms.NumericUpDown();
            this.label_polling_threshold = new System.Windows.Forms.Label();
            this.label_stale_results = new System.Windows.Forms.Label();
            this.label_log_level = new System.Windows.Forms.Label();
            this.comboBox_log_level = new System.Windows.Forms.ComboBox();
            this.label_debug_mode = new System.Windows.Forms.Label();
            this.checkBox_debug_mode = new System.Windows.Forms.CheckBox();
            this.label_db_location = new System.Windows.Forms.Label();
            this.numeric_stale_results = new System.Windows.Forms.NumericUpDown();
            this.textBox_db_location_result = new System.Windows.Forms.TextBox();
            this.button_save = new System.Windows.Forms.Button();
            this.datasource_grid.SuspendLayout();
            this.dexcom_settings_grid.SuspendLayout();
            this.nightscout_grid.SuspendLayout();
            this.glucose_unit_grid.SuspendLayout();
            this.glucose_values_grid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_high)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_warning_high)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_warning_low)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_low)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_critical)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_polling_threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_stale_results)).BeginInit();
            this.SuspendLayout();
            // 
            // radio_dexcom
            // 
            this.radio_dexcom.AutoSize = true;
            this.radio_dexcom.Dock = System.Windows.Forms.DockStyle.Right;
            this.radio_dexcom.Location = new System.Drawing.Point(45, 41);
            this.radio_dexcom.Name = "radio_dexcom";
            this.radio_dexcom.Size = new System.Drawing.Size(102, 33);
            this.radio_dexcom.TabIndex = 0;
            this.radio_dexcom.TabStop = true;
            this.radio_dexcom.Text = "Dexcom";
            this.radio_dexcom.UseVisualStyleBackColor = true;
            this.radio_dexcom.CheckedChanged += new System.EventHandler(this.radio_dexcom_CheckedChanged);
            // 
            // radio_nightscout
            // 
            this.radio_nightscout.AutoSize = true;
            this.radio_nightscout.Dock = System.Windows.Forms.DockStyle.Right;
            this.radio_nightscout.Location = new System.Drawing.Point(173, 41);
            this.radio_nightscout.Name = "radio_nightscout";
            this.radio_nightscout.Size = new System.Drawing.Size(124, 33);
            this.radio_nightscout.TabIndex = 1;
            this.radio_nightscout.TabStop = true;
            this.radio_nightscout.Text = "Nightscout";
            this.radio_nightscout.UseVisualStyleBackColor = true;
            this.radio_nightscout.CheckedChanged += new System.EventHandler(this.radio_nightscout_CheckedChanged);
            // 
            // label_glucose_fetch
            // 
            this.label_glucose_fetch.AutoSize = true;
            this.datasource_grid.SetColumnSpan(this.label_glucose_fetch, 2);
            this.label_glucose_fetch.Location = new System.Drawing.Point(3, 0);
            this.label_glucose_fetch.Name = "label_glucose_fetch";
            this.label_glucose_fetch.Size = new System.Drawing.Size(168, 25);
            this.label_glucose_fetch.TabIndex = 2;
            this.label_glucose_fetch.Text = "Glucose Datasource";
            // 
            // datasource_grid
            // 
            this.datasource_grid.ColumnCount = 2;
            this.datasource_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.33333F));
            this.datasource_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.66667F));
            this.datasource_grid.Controls.Add(this.label_glucose_fetch, 0, 0);
            this.datasource_grid.Controls.Add(this.radio_nightscout, 1, 1);
            this.datasource_grid.Controls.Add(this.radio_dexcom, 0, 1);
            this.datasource_grid.Location = new System.Drawing.Point(12, 12);
            this.datasource_grid.Name = "datasource_grid";
            this.datasource_grid.RowCount = 2;
            this.datasource_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.datasource_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.datasource_grid.Size = new System.Drawing.Size(300, 77);
            this.datasource_grid.TabIndex = 3;
            // 
            // dexcom_settings_grid
            // 
            this.dexcom_settings_grid.ColumnCount = 2;
            this.dexcom_settings_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dexcom_settings_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dexcom_settings_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.dexcom_settings_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.dexcom_settings_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.dexcom_settings_grid.Controls.Add(this.radio_dexcom_server_us_share2, 1, 3);
            this.dexcom_settings_grid.Controls.Add(this.label_dexcom_username, 0, 0);
            this.dexcom_settings_grid.Controls.Add(this.label_dexcom_password, 1, 0);
            this.dexcom_settings_grid.Controls.Add(this.textBox_dexcom_username, 0, 1);
            this.dexcom_settings_grid.Controls.Add(this.label_dexcom_server, 0, 2);
            this.dexcom_settings_grid.Controls.Add(this.radio_dexcom_server_us_share1, 1, 2);
            this.dexcom_settings_grid.Controls.Add(this.radio_dexcom_server_international, 1, 4);
            this.dexcom_settings_grid.Controls.Add(this.maskedText_dexcom_password, 1, 1);
            this.dexcom_settings_grid.Location = new System.Drawing.Point(319, 12);
            this.dexcom_settings_grid.Name = "dexcom_settings_grid";
            this.dexcom_settings_grid.RowCount = 5;
            this.dexcom_settings_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 39.47368F));
            this.dexcom_settings_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60.52632F));
            this.dexcom_settings_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.dexcom_settings_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.dexcom_settings_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.dexcom_settings_grid.Size = new System.Drawing.Size(363, 176);
            this.dexcom_settings_grid.TabIndex = 4;
            this.dexcom_settings_grid.Visible = false;
            // 
            // radio_dexcom_server_us_share2
            // 
            this.radio_dexcom_server_us_share2.AutoSize = true;
            this.radio_dexcom_server_us_share2.Location = new System.Drawing.Point(184, 108);
            this.radio_dexcom_server_us_share2.Name = "radio_dexcom_server_us_share2";
            this.radio_dexcom_server_us_share2.Size = new System.Drawing.Size(123, 28);
            this.radio_dexcom_server_us_share2.TabIndex = 5;
            this.radio_dexcom_server_us_share2.TabStop = true;
            this.radio_dexcom_server_us_share2.Text = "US Share 2";
            this.radio_dexcom_server_us_share2.UseVisualStyleBackColor = true;
            // 
            // label_dexcom_username
            // 
            this.label_dexcom_username.AutoSize = true;
            this.label_dexcom_username.Location = new System.Drawing.Point(3, 0);
            this.label_dexcom_username.Name = "label_dexcom_username";
            this.label_dexcom_username.Size = new System.Drawing.Size(161, 25);
            this.label_dexcom_username.TabIndex = 0;
            this.label_dexcom_username.Text = "Dexcom Username";
            // 
            // label_dexcom_password
            // 
            this.label_dexcom_password.AutoSize = true;
            this.label_dexcom_password.Location = new System.Drawing.Point(184, 0);
            this.label_dexcom_password.Name = "label_dexcom_password";
            this.label_dexcom_password.Size = new System.Drawing.Size(157, 25);
            this.label_dexcom_password.TabIndex = 1;
            this.label_dexcom_password.Text = "Dexcom Password";
            // 
            // textBox_dexcom_username
            // 
            this.textBox_dexcom_username.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_dexcom_username.Location = new System.Drawing.Point(3, 31);
            this.textBox_dexcom_username.Name = "textBox_dexcom_username";
            this.textBox_dexcom_username.Size = new System.Drawing.Size(175, 31);
            this.textBox_dexcom_username.TabIndex = 2;
            // 
            // label_dexcom_server
            // 
            this.label_dexcom_server.AutoSize = true;
            this.label_dexcom_server.Location = new System.Drawing.Point(3, 72);
            this.label_dexcom_server.Name = "label_dexcom_server";
            this.label_dexcom_server.Size = new System.Drawing.Size(131, 25);
            this.label_dexcom_server.TabIndex = 4;
            this.label_dexcom_server.Text = "Dexcom Server";
            // 
            // radio_dexcom_server_us_share1
            // 
            this.radio_dexcom_server_us_share1.AutoSize = true;
            this.radio_dexcom_server_us_share1.Location = new System.Drawing.Point(184, 75);
            this.radio_dexcom_server_us_share1.Name = "radio_dexcom_server_us_share1";
            this.radio_dexcom_server_us_share1.Size = new System.Drawing.Size(123, 27);
            this.radio_dexcom_server_us_share1.TabIndex = 4;
            this.radio_dexcom_server_us_share1.TabStop = true;
            this.radio_dexcom_server_us_share1.Text = "US Share 1";
            this.radio_dexcom_server_us_share1.UseVisualStyleBackColor = true;
            // 
            // radio_dexcom_server_international
            // 
            this.radio_dexcom_server_international.AutoSize = true;
            this.radio_dexcom_server_international.Location = new System.Drawing.Point(184, 142);
            this.radio_dexcom_server_international.Name = "radio_dexcom_server_international";
            this.radio_dexcom_server_international.Size = new System.Drawing.Size(136, 29);
            this.radio_dexcom_server_international.TabIndex = 6;
            this.radio_dexcom_server_international.TabStop = true;
            this.radio_dexcom_server_international.Text = "International";
            this.radio_dexcom_server_international.UseVisualStyleBackColor = true;
            // 
            // maskedText_dexcom_password
            // 
            this.maskedText_dexcom_password.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maskedText_dexcom_password.Location = new System.Drawing.Point(184, 31);
            this.maskedText_dexcom_password.Name = "maskedText_dexcom_password";
            this.maskedText_dexcom_password.Size = new System.Drawing.Size(176, 31);
            this.maskedText_dexcom_password.TabIndex = 3;
            this.maskedText_dexcom_password.UseSystemPasswordChar = true;
            // 
            // nightscout_grid
            // 
            this.nightscout_grid.ColumnCount = 2;
            this.nightscout_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.nightscout_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.nightscout_grid.Controls.Add(this.label_nightscout_url, 0, 0);
            this.nightscout_grid.Controls.Add(this.label_nightscout_token, 0, 1);
            this.nightscout_grid.Controls.Add(this.textBox_nightscout_url, 1, 0);
            this.nightscout_grid.Controls.Add(this.textBox_nightscout_token, 1, 1);
            this.nightscout_grid.Location = new System.Drawing.Point(319, 12);
            this.nightscout_grid.Name = "nightscout_grid";
            this.nightscout_grid.RowCount = 2;
            this.nightscout_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.nightscout_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.nightscout_grid.Size = new System.Drawing.Size(454, 97);
            this.nightscout_grid.TabIndex = 5;
            this.nightscout_grid.Visible = false;
            // 
            // label_nightscout_url
            // 
            this.label_nightscout_url.AutoSize = true;
            this.label_nightscout_url.Location = new System.Drawing.Point(3, 0);
            this.label_nightscout_url.Name = "label_nightscout_url";
            this.label_nightscout_url.Size = new System.Drawing.Size(126, 25);
            this.label_nightscout_url.TabIndex = 0;
            this.label_nightscout_url.Text = "Nightscout Url";
            // 
            // label_nightscout_token
            // 
            this.label_nightscout_token.AutoSize = true;
            this.label_nightscout_token.Location = new System.Drawing.Point(3, 48);
            this.label_nightscout_token.Name = "label_nightscout_token";
            this.label_nightscout_token.Size = new System.Drawing.Size(208, 25);
            this.label_nightscout_token.TabIndex = 1;
            this.label_nightscout_token.Text = "Nightscout Access Token";
            // 
            // textBox_nightscout_url
            // 
            this.textBox_nightscout_url.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_nightscout_url.Location = new System.Drawing.Point(230, 3);
            this.textBox_nightscout_url.Name = "textBox_nightscout_url";
            this.textBox_nightscout_url.Size = new System.Drawing.Size(221, 31);
            this.textBox_nightscout_url.TabIndex = 2;
            // 
            // textBox_nightscout_token
            // 
            this.textBox_nightscout_token.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_nightscout_token.Location = new System.Drawing.Point(230, 51);
            this.textBox_nightscout_token.Name = "textBox_nightscout_token";
            this.textBox_nightscout_token.Size = new System.Drawing.Size(221, 31);
            this.textBox_nightscout_token.TabIndex = 3;
            // 
            // glucose_unit_grid
            // 
            this.glucose_unit_grid.ColumnCount = 2;
            this.glucose_unit_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.16835F));
            this.glucose_unit_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.83165F));
            this.glucose_unit_grid.Controls.Add(this.radio_glucose_unit_mmol, 1, 1);
            this.glucose_unit_grid.Controls.Add(this.label_glucose_unit, 0, 0);
            this.glucose_unit_grid.Controls.Add(this.radio_glucose_unit_mg, 0, 1);
            this.glucose_unit_grid.Location = new System.Drawing.Point(12, 110);
            this.glucose_unit_grid.Name = "glucose_unit_grid";
            this.glucose_unit_grid.RowCount = 2;
            this.glucose_unit_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.glucose_unit_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.glucose_unit_grid.Size = new System.Drawing.Size(297, 78);
            this.glucose_unit_grid.TabIndex = 6;
            // 
            // radio_glucose_unit_mmol
            // 
            this.radio_glucose_unit_mmol.AutoSize = true;
            this.radio_glucose_unit_mmol.Dock = System.Windows.Forms.DockStyle.Right;
            this.radio_glucose_unit_mmol.Location = new System.Drawing.Point(184, 42);
            this.radio_glucose_unit_mmol.Name = "radio_glucose_unit_mmol";
            this.radio_glucose_unit_mmol.Size = new System.Drawing.Size(110, 33);
            this.radio_glucose_unit_mmol.TabIndex = 8;
            this.radio_glucose_unit_mmol.TabStop = true;
            this.radio_glucose_unit_mmol.Text = "MMOG/L";
            this.radio_glucose_unit_mmol.UseVisualStyleBackColor = true;
            this.radio_glucose_unit_mmol.CheckedChanged += new System.EventHandler(this.radio_glucose_unit_mmol_CheckedChanged);
            // 
            // label_glucose_unit
            // 
            this.label_glucose_unit.AutoSize = true;
            this.glucose_unit_grid.SetColumnSpan(this.label_glucose_unit, 2);
            this.label_glucose_unit.Location = new System.Drawing.Point(3, 0);
            this.label_glucose_unit.Name = "label_glucose_unit";
            this.label_glucose_unit.Size = new System.Drawing.Size(111, 25);
            this.label_glucose_unit.TabIndex = 0;
            this.label_glucose_unit.Text = "Glucose Unit";
            // 
            // radio_glucose_unit_mg
            // 
            this.radio_glucose_unit_mg.AutoSize = true;
            this.radio_glucose_unit_mg.Dock = System.Windows.Forms.DockStyle.Right;
            this.radio_glucose_unit_mg.Location = new System.Drawing.Point(53, 42);
            this.radio_glucose_unit_mg.Name = "radio_glucose_unit_mg";
            this.radio_glucose_unit_mg.Size = new System.Drawing.Size(93, 33);
            this.radio_glucose_unit_mg.TabIndex = 7;
            this.radio_glucose_unit_mg.TabStop = true;
            this.radio_glucose_unit_mg.Text = "MG/DL";
            this.radio_glucose_unit_mg.UseVisualStyleBackColor = true;
            this.radio_glucose_unit_mg.CheckedChanged += new System.EventHandler(this.radio_glucose_unit_mg_CheckedChanged);
            // 
            // glucose_values_grid
            // 
            this.glucose_values_grid.ColumnCount = 2;
            this.glucose_values_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 61.06557F));
            this.glucose_values_grid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.93443F));
            this.glucose_values_grid.Controls.Add(this.label_glucose_thresholds, 0, 0);
            this.glucose_values_grid.Controls.Add(this.label_glucose_high, 0, 1);
            this.glucose_values_grid.Controls.Add(this.label_glucose_warning_high, 0, 2);
            this.glucose_values_grid.Controls.Add(this.label_glucose_warning_low, 0, 3);
            this.glucose_values_grid.Controls.Add(this.label_glucose_low, 0, 4);
            this.glucose_values_grid.Controls.Add(this.label_glucose_critical, 0, 5);
            this.glucose_values_grid.Controls.Add(this.numeric_glucose_high, 1, 1);
            this.glucose_values_grid.Controls.Add(this.numeric_glucose_warning_high, 1, 2);
            this.glucose_values_grid.Controls.Add(this.numeric_glucose_warning_low, 1, 3);
            this.glucose_values_grid.Controls.Add(this.numeric_glucose_low, 1, 4);
            this.glucose_values_grid.Controls.Add(this.numeric_glucose_critical, 1, 5);
            this.glucose_values_grid.Enabled = false;
            this.glucose_values_grid.Location = new System.Drawing.Point(12, 218);
            this.glucose_values_grid.Name = "glucose_values_grid";
            this.glucose_values_grid.RowCount = 6;
            this.glucose_values_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.glucose_values_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.glucose_values_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.glucose_values_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.glucose_values_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.glucose_values_grid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.glucose_values_grid.Size = new System.Drawing.Size(244, 247);
            this.glucose_values_grid.TabIndex = 7;
            // 
            // label_glucose_thresholds
            // 
            this.label_glucose_thresholds.AutoSize = true;
            this.glucose_values_grid.SetColumnSpan(this.label_glucose_thresholds, 2);
            this.label_glucose_thresholds.Location = new System.Drawing.Point(3, 0);
            this.label_glucose_thresholds.Name = "label_glucose_thresholds";
            this.label_glucose_thresholds.Size = new System.Drawing.Size(165, 25);
            this.label_glucose_thresholds.TabIndex = 0;
            this.label_glucose_thresholds.Text = "Glucose Thresholds";
            // 
            // label_glucose_high
            // 
            this.label_glucose_high.AutoSize = true;
            this.label_glucose_high.Location = new System.Drawing.Point(3, 45);
            this.label_glucose_high.Name = "label_glucose_high";
            this.label_glucose_high.Size = new System.Drawing.Size(50, 25);
            this.label_glucose_high.TabIndex = 1;
            this.label_glucose_high.Text = "High";
            // 
            // label_glucose_warning_high
            // 
            this.label_glucose_warning_high.AutoSize = true;
            this.label_glucose_warning_high.Location = new System.Drawing.Point(3, 85);
            this.label_glucose_warning_high.Name = "label_glucose_warning_high";
            this.label_glucose_warning_high.Size = new System.Drawing.Size(121, 25);
            this.label_glucose_warning_high.TabIndex = 2;
            this.label_glucose_warning_high.Text = "High Warning";
            // 
            // label_glucose_warning_low
            // 
            this.label_glucose_warning_low.AutoSize = true;
            this.label_glucose_warning_low.Location = new System.Drawing.Point(3, 125);
            this.label_glucose_warning_low.Name = "label_glucose_warning_low";
            this.label_glucose_warning_low.Size = new System.Drawing.Size(115, 25);
            this.label_glucose_warning_low.TabIndex = 3;
            this.label_glucose_warning_low.Text = "Low Warning";
            // 
            // label_glucose_low
            // 
            this.label_glucose_low.AutoSize = true;
            this.label_glucose_low.Location = new System.Drawing.Point(3, 165);
            this.label_glucose_low.Name = "label_glucose_low";
            this.label_glucose_low.Size = new System.Drawing.Size(44, 25);
            this.label_glucose_low.TabIndex = 4;
            this.label_glucose_low.Text = "Low";
            // 
            // label_glucose_critical
            // 
            this.label_glucose_critical.AutoSize = true;
            this.label_glucose_critical.Location = new System.Drawing.Point(3, 205);
            this.label_glucose_critical.Name = "label_glucose_critical";
            this.label_glucose_critical.Size = new System.Drawing.Size(114, 25);
            this.label_glucose_critical.TabIndex = 5;
            this.label_glucose_critical.Text = "Critically Low";
            // 
            // numeric_glucose_high
            // 
            this.numeric_glucose_high.Location = new System.Drawing.Point(151, 48);
            this.numeric_glucose_high.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numeric_glucose_high.Name = "numeric_glucose_high";
            this.numeric_glucose_high.Size = new System.Drawing.Size(89, 31);
            this.numeric_glucose_high.TabIndex = 9;
            this.numeric_glucose_high.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // numeric_glucose_warning_high
            // 
            this.numeric_glucose_warning_high.Location = new System.Drawing.Point(151, 88);
            this.numeric_glucose_warning_high.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numeric_glucose_warning_high.Name = "numeric_glucose_warning_high";
            this.numeric_glucose_warning_high.Size = new System.Drawing.Size(89, 31);
            this.numeric_glucose_warning_high.TabIndex = 10;
            this.numeric_glucose_warning_high.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            // 
            // numeric_glucose_warning_low
            // 
            this.numeric_glucose_warning_low.Location = new System.Drawing.Point(151, 128);
            this.numeric_glucose_warning_low.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numeric_glucose_warning_low.Name = "numeric_glucose_warning_low";
            this.numeric_glucose_warning_low.Size = new System.Drawing.Size(89, 31);
            this.numeric_glucose_warning_low.TabIndex = 11;
            this.numeric_glucose_warning_low.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // numeric_glucose_low
            // 
            this.numeric_glucose_low.Location = new System.Drawing.Point(151, 168);
            this.numeric_glucose_low.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numeric_glucose_low.Name = "numeric_glucose_low";
            this.numeric_glucose_low.Size = new System.Drawing.Size(89, 31);
            this.numeric_glucose_low.TabIndex = 12;
            this.numeric_glucose_low.Value = new decimal(new int[] {
            70,
            0,
            0,
            0});
            // 
            // numeric_glucose_critical
            // 
            this.numeric_glucose_critical.Location = new System.Drawing.Point(151, 208);
            this.numeric_glucose_critical.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.numeric_glucose_critical.Name = "numeric_glucose_critical";
            this.numeric_glucose_critical.Size = new System.Drawing.Size(89, 31);
            this.numeric_glucose_critical.TabIndex = 13;
            this.numeric_glucose_critical.Value = new decimal(new int[] {
            55,
            0,
            0,
            0});
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.5443F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.4557F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 168F));
            this.tableLayoutPanel1.Controls.Add(this.numeric_polling_threshold, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_polling_threshold, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label_stale_results, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label_log_level, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBox_log_level, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label_debug_mode, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.checkBox_debug_mode, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label_db_location, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.numeric_stale_results, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox_db_location_result, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(306, 218);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(482, 199);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // numeric_polling_threshold
            // 
            this.numeric_polling_threshold.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numeric_polling_threshold.Location = new System.Drawing.Point(316, 3);
            this.numeric_polling_threshold.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numeric_polling_threshold.Name = "numeric_polling_threshold";
            this.numeric_polling_threshold.Size = new System.Drawing.Size(163, 31);
            this.numeric_polling_threshold.TabIndex = 14;
            this.numeric_polling_threshold.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label_polling_threshold
            // 
            this.label_polling_threshold.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label_polling_threshold, 2);
            this.label_polling_threshold.Location = new System.Drawing.Point(3, 0);
            this.label_polling_threshold.Name = "label_polling_threshold";
            this.label_polling_threshold.Size = new System.Drawing.Size(185, 25);
            this.label_polling_threshold.TabIndex = 0;
            this.label_polling_threshold.Text = "Polling Rate (seconds)";
            // 
            // label_stale_results
            // 
            this.label_stale_results.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label_stale_results, 2);
            this.label_stale_results.Location = new System.Drawing.Point(3, 40);
            this.label_stale_results.Name = "label_stale_results";
            this.label_stale_results.Size = new System.Drawing.Size(235, 40);
            this.label_stale_results.TabIndex = 2;
            this.label_stale_results.Text = "Consider Results State After (minutes)";
            // 
            // label_log_level
            // 
            this.label_log_level.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label_log_level, 2);
            this.label_log_level.Location = new System.Drawing.Point(3, 80);
            this.label_log_level.Name = "label_log_level";
            this.label_log_level.Size = new System.Drawing.Size(122, 25);
            this.label_log_level.TabIndex = 4;
            this.label_log_level.Text = "Logging Level";
            // 
            // comboBox_log_level
            // 
            this.comboBox_log_level.FormattingEnabled = true;
            this.comboBox_log_level.Location = new System.Drawing.Point(316, 83);
            this.comboBox_log_level.Name = "comboBox_log_level";
            this.comboBox_log_level.Size = new System.Drawing.Size(137, 33);
            this.comboBox_log_level.TabIndex = 16;
            // 
            // label_debug_mode
            // 
            this.label_debug_mode.AutoSize = true;
            this.label_debug_mode.Location = new System.Drawing.Point(3, 120);
            this.label_debug_mode.Name = "label_debug_mode";
            this.label_debug_mode.Size = new System.Drawing.Size(118, 25);
            this.label_debug_mode.TabIndex = 6;
            this.label_debug_mode.Text = "Debug Mode";
            // 
            // checkBox_debug_mode
            // 
            this.checkBox_debug_mode.AutoSize = true;
            this.checkBox_debug_mode.Dock = System.Windows.Forms.DockStyle.Top;
            this.checkBox_debug_mode.Location = new System.Drawing.Point(186, 123);
            this.checkBox_debug_mode.Name = "checkBox_debug_mode";
            this.checkBox_debug_mode.Size = new System.Drawing.Size(124, 21);
            this.checkBox_debug_mode.TabIndex = 17;
            this.checkBox_debug_mode.UseVisualStyleBackColor = true;
            // 
            // label_db_location
            // 
            this.label_db_location.AutoSize = true;
            this.label_db_location.Location = new System.Drawing.Point(3, 160);
            this.label_db_location.Name = "label_db_location";
            this.label_db_location.Size = new System.Drawing.Size(158, 25);
            this.label_db_location.TabIndex = 8;
            this.label_db_location.Text = "Database Location";
            // 
            // numeric_stale_results
            // 
            this.numeric_stale_results.Location = new System.Drawing.Point(316, 43);
            this.numeric_stale_results.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numeric_stale_results.Name = "numeric_stale_results";
            this.numeric_stale_results.Size = new System.Drawing.Size(137, 31);
            this.numeric_stale_results.TabIndex = 15;
            this.numeric_stale_results.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // textBox_db_location_result
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textBox_db_location_result, 2);
            this.textBox_db_location_result.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_db_location_result.Location = new System.Drawing.Point(186, 163);
            this.textBox_db_location_result.Name = "textBox_db_location_result";
            this.textBox_db_location_result.Size = new System.Drawing.Size(293, 31);
            this.textBox_db_location_result.TabIndex = 18;
            this.textBox_db_location_result.Text = "C:\\temp\\glucosetray.db";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(341, 524);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(189, 34);
            this.button_save.TabIndex = 9;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 582);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.dexcom_settings_grid);
            this.Controls.Add(this.glucose_values_grid);
            this.Controls.Add(this.glucose_unit_grid);
            this.Controls.Add(this.nightscout_grid);
            this.Controls.Add(this.datasource_grid);
            this.Name = "Settings";
            this.Text = "Settings";
            this.datasource_grid.ResumeLayout(false);
            this.datasource_grid.PerformLayout();
            this.dexcom_settings_grid.ResumeLayout(false);
            this.dexcom_settings_grid.PerformLayout();
            this.nightscout_grid.ResumeLayout(false);
            this.nightscout_grid.PerformLayout();
            this.glucose_unit_grid.ResumeLayout(false);
            this.glucose_unit_grid.PerformLayout();
            this.glucose_values_grid.ResumeLayout(false);
            this.glucose_values_grid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_high)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_warning_high)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_warning_low)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_low)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_glucose_critical)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_polling_threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_stale_results)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radio_dexcom;
        private System.Windows.Forms.RadioButton radio_nightscout;
        private System.Windows.Forms.Label label_glucose_fetch;
        private System.Windows.Forms.TableLayoutPanel datasource_grid;
        private System.Windows.Forms.TableLayoutPanel dexcom_settings_grid;
        private System.Windows.Forms.Label label_dexcom_username;
        private System.Windows.Forms.Label label_dexcom_password;
        private System.Windows.Forms.TextBox textBox_dexcom_username;
        private System.Windows.Forms.Label label_dexcom_server;
        private System.Windows.Forms.RadioButton radio_dexcom_server_us_share1;
        private System.Windows.Forms.RadioButton radio_dexcom_server_us_share2;
        private System.Windows.Forms.RadioButton radio_dexcom_server_international;
        private System.Windows.Forms.TableLayoutPanel nightscout_grid;
        private System.Windows.Forms.Label label_nightscout_url;
        private System.Windows.Forms.Label label_nightscout_token;
        private System.Windows.Forms.TextBox textBox_nightscout_url;
        private System.Windows.Forms.TextBox textBox_nightscout_token;
        private System.Windows.Forms.TableLayoutPanel glucose_unit_grid;
        private System.Windows.Forms.RadioButton radio_glucose_unit_mmol;
        private System.Windows.Forms.Label label_glucose_unit;
        private System.Windows.Forms.RadioButton radio_glucose_unit_mg;
        private System.Windows.Forms.TableLayoutPanel glucose_values_grid;
        private System.Windows.Forms.Label label_glucose_thresholds;
        private System.Windows.Forms.Label label_glucose_high;
        private System.Windows.Forms.Label label_glucose_warning_high;
        private System.Windows.Forms.MaskedTextBox maskedText_dexcom_password;
        private System.Windows.Forms.Label label_glucose_warning_low;
        private System.Windows.Forms.Label label_glucose_low;
        private System.Windows.Forms.Label label_glucose_critical;
        private System.Windows.Forms.NumericUpDown numeric_glucose_high;
        private System.Windows.Forms.NumericUpDown numeric_glucose_warning_high;
        private System.Windows.Forms.NumericUpDown numeric_glucose_warning_low;
        private System.Windows.Forms.NumericUpDown numeric_glucose_low;
        private System.Windows.Forms.NumericUpDown numeric_glucose_critical;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label_polling_threshold;
        private System.Windows.Forms.Label label_stale_results;
        private System.Windows.Forms.NumericUpDown numeric_stale_results;
        private System.Windows.Forms.Label label_log_level;
        private System.Windows.Forms.ComboBox comboBox_log_level;
        private System.Windows.Forms.Label label_debug_mode;
        private System.Windows.Forms.CheckBox checkBox_debug_mode;
        private System.Windows.Forms.Label label_db_location;
        private System.Windows.Forms.NumericUpDown numeric_polling_threshold;
        private System.Windows.Forms.TextBox textBox_db_location_result;
        private System.Windows.Forms.Button button_save;
    }
}