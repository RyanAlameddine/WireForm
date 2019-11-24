namespace WireForm
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.toolBox = new System.Windows.Forms.ComboBox();
            this.gateBox = new System.Windows.Forms.ComboBox();
            this.gatePicBox = new System.Windows.Forms.PictureBox();
            this.GateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.alskjdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sdfsdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SelectionSettings = new System.Windows.Forms.ListBox();
            this.SelectionSettingValue = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.gatePicBox)).BeginInit();
            this.GateMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBox
            // 
            this.toolBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolBox.FormattingEnabled = true;
            this.toolBox.Items.AddRange(new object[] {
            "Wire Painter",
            "Gate Controller"});
            this.toolBox.Location = new System.Drawing.Point(667, 12);
            this.toolBox.Name = "toolBox";
            this.toolBox.Size = new System.Drawing.Size(121, 21);
            this.toolBox.TabIndex = 0;
            this.toolBox.SelectedIndexChanged += new System.EventHandler(this.toolBox_SelectedIndexChanged);
            // 
            // gateBox
            // 
            this.gateBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gateBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gateBox.FormattingEnabled = true;
            this.gateBox.Location = new System.Drawing.Point(667, 39);
            this.gateBox.Name = "gateBox";
            this.gateBox.Size = new System.Drawing.Size(121, 21);
            this.gateBox.TabIndex = 1;
            this.gateBox.Visible = false;
            this.gateBox.SelectedIndexChanged += new System.EventHandler(this.GateBox_SelectedIndexChanged);
            // 
            // gatePicBox
            // 
            this.gatePicBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gatePicBox.BackColor = System.Drawing.Color.Transparent;
            this.gatePicBox.Location = new System.Drawing.Point(667, 62);
            this.gatePicBox.Margin = new System.Windows.Forms.Padding(2);
            this.gatePicBox.Name = "gatePicBox";
            this.gatePicBox.Size = new System.Drawing.Size(120, 83);
            this.gatePicBox.TabIndex = 5;
            this.gatePicBox.TabStop = false;
            this.gatePicBox.Paint += new System.Windows.Forms.PaintEventHandler(this.GatePicBox_Paint);
            this.gatePicBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GatePicBox_MouseClick);
            // 
            // GateMenu
            // 
            this.GateMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.GateMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alskjdfToolStripMenuItem,
            this.sdfsdfToolStripMenuItem});
            this.GateMenu.Name = "contextMenuStrip1";
            this.GateMenu.Size = new System.Drawing.Size(109, 48);
            // 
            // alskjdfToolStripMenuItem
            // 
            this.alskjdfToolStripMenuItem.Name = "alskjdfToolStripMenuItem";
            this.alskjdfToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.alskjdfToolStripMenuItem.Text = "alskjdf";
            // 
            // sdfsdfToolStripMenuItem
            // 
            this.sdfsdfToolStripMenuItem.Name = "sdfsdfToolStripMenuItem";
            this.sdfsdfToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
            this.sdfsdfToolStripMenuItem.Text = "sdfsdf";
            // 
            // SelectionSettings
            // 
            this.SelectionSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectionSettings.FormattingEnabled = true;
            this.SelectionSettings.Location = new System.Drawing.Point(667, 149);
            this.SelectionSettings.Margin = new System.Windows.Forms.Padding(2);
            this.SelectionSettings.Name = "SelectionSettings";
            this.SelectionSettings.Size = new System.Drawing.Size(121, 225);
            this.SelectionSettings.TabIndex = 6;
            this.SelectionSettings.Visible = false;
            this.SelectionSettings.SelectedIndexChanged += new System.EventHandler(this.SelectionSettings_SelectedIndexChanged);
            // 
            // SelectionSettingValue
            // 
            this.SelectionSettingValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectionSettingValue.FormattingEnabled = true;
            this.SelectionSettingValue.Location = new System.Drawing.Point(667, 379);
            this.SelectionSettingValue.Name = "SelectionSettingValue";
            this.SelectionSettingValue.Size = new System.Drawing.Size(121, 21);
            this.SelectionSettingValue.TabIndex = 7;
            this.SelectionSettingValue.Visible = false;
            this.SelectionSettingValue.SelectedIndexChanged += new System.EventHandler(this.SelectionSettingsValue_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SelectionSettingValue);
            this.Controls.Add(this.SelectionSettings);
            this.Controls.Add(this.gatePicBox);
            this.Controls.Add(this.gateBox);
            this.Controls.Add(this.toolBox);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseWheel);
            ((System.ComponentModel.ISupportInitialize)(this.gatePicBox)).EndInit();
            this.GateMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox toolBox;
        private System.Windows.Forms.ComboBox gateBox;
        private System.Windows.Forms.PictureBox gatePicBox;
        private System.Windows.Forms.ContextMenuStrip GateMenu;
        private System.Windows.Forms.ToolStripMenuItem alskjdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sdfsdfToolStripMenuItem;
        private System.Windows.Forms.ListBox SelectionSettings;
        private System.Windows.Forms.ComboBox SelectionSettingValue;
    }
}

