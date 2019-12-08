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
            this.SelectionSettings = new System.Windows.Forms.ListBox();
            this.SelectionSettingValue = new System.Windows.Forms.ComboBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileButton = new System.Windows.Forms.ToolStripMenuItem();
            this.newButton = new System.Windows.Forms.ToolStripMenuItem();
            this.openButton = new System.Windows.Forms.ToolStripMenuItem();
            this.openRecentButton = new System.Windows.Forms.ToolStripMenuItem();
            this.save = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.closeButton = new System.Windows.Forms.ToolStripMenuItem();
            this.editButton = new System.Windows.Forms.ToolStripMenuItem();
            this.copyButton = new System.Windows.Forms.ToolStripMenuItem();
            this.cutButton = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteButton = new System.Windows.Forms.ToolStripMenuItem();
            this.undoButton = new System.Windows.Forms.ToolStripMenuItem();
            this.redoButton = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.gatePicBox)).BeginInit();
            this.menuStrip.SuspendLayout();
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
            this.GateMenu.Name = "contextMenuStrip1";
            this.GateMenu.Size = new System.Drawing.Size(61, 4);
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
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileButton,
            this.editButton});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 8;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileButton
            // 
            this.fileButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newButton,
            this.openButton,
            this.openRecentButton,
            this.save,
            this.saveAsButton,
            this.closeButton});
            this.fileButton.Name = "fileButton";
            this.fileButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.fileButton.Size = new System.Drawing.Size(37, 20);
            this.fileButton.Text = "File";
            // 
            // newButton
            // 
            this.newButton.Name = "newButton";
            this.newButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newButton.Size = new System.Drawing.Size(186, 22);
            this.newButton.Text = "New";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // openButton
            // 
            this.openButton.Name = "openButton";
            this.openButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openButton.Size = new System.Drawing.Size(186, 22);
            this.openButton.Text = "Open";
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // openRecentButton
            // 
            this.openRecentButton.Name = "openRecentButton";
            this.openRecentButton.Size = new System.Drawing.Size(186, 22);
            this.openRecentButton.Text = "Open Recent";
            // 
            // save
            // 
            this.save.Name = "save";
            this.save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.save.Size = new System.Drawing.Size(186, 22);
            this.save.Text = "Save";
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // saveAsButton
            // 
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsButton.Size = new System.Drawing.Size(186, 22);
            this.saveAsButton.Text = "Save As";
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Name = "closeButton";
            this.closeButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.closeButton.Size = new System.Drawing.Size(186, 22);
            this.closeButton.Text = "Close";
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // editButton
            // 
            this.editButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyButton,
            this.cutButton,
            this.pasteButton,
            this.undoButton,
            this.redoButton});
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(39, 20);
            this.editButton.Text = "Edit";
            // 
            // copyButton
            // 
            this.copyButton.Name = "copyButton";
            this.copyButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyButton.Size = new System.Drawing.Size(180, 22);
            this.copyButton.Text = "Copy";
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // cutButton
            // 
            this.cutButton.Name = "cutButton";
            this.cutButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutButton.Size = new System.Drawing.Size(180, 22);
            this.cutButton.Text = "Cut";
            this.cutButton.Click += new System.EventHandler(this.cutButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.pasteButton.Size = new System.Drawing.Size(180, 22);
            this.pasteButton.Text = "Paste";
            this.pasteButton.Click += new System.EventHandler(this.pasteButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Name = "undoButton";
            this.undoButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoButton.Size = new System.Drawing.Size(180, 22);
            this.undoButton.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // redoButton
            // 
            this.redoButton.Name = "redoButton";
            this.redoButton.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
            this.redoButton.Size = new System.Drawing.Size(180, 22);
            this.redoButton.Text = "Redo";
            this.redoButton.Click += new System.EventHandler(this.redoButton_Click);
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
            this.Controls.Add(this.menuStrip);
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
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox toolBox;
        private System.Windows.Forms.ComboBox gateBox;
        private System.Windows.Forms.PictureBox gatePicBox;
        private System.Windows.Forms.ContextMenuStrip GateMenu;
        private System.Windows.Forms.ListBox SelectionSettings;
        private System.Windows.Forms.ComboBox SelectionSettingValue;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileButton;
        private System.Windows.Forms.ToolStripMenuItem newButton;
        private System.Windows.Forms.ToolStripMenuItem openButton;
        private System.Windows.Forms.ToolStripMenuItem editButton;
        private System.Windows.Forms.ToolStripMenuItem undoButton;
        private System.Windows.Forms.ToolStripMenuItem redoButton;
        private System.Windows.Forms.ToolStripMenuItem openRecentButton;
        private System.Windows.Forms.ToolStripMenuItem save;
        private System.Windows.Forms.ToolStripMenuItem saveAsButton;
        private System.Windows.Forms.ToolStripMenuItem closeButton;
        private System.Windows.Forms.ToolStripMenuItem copyButton;
        private System.Windows.Forms.ToolStripMenuItem cutButton;
        private System.Windows.Forms.ToolStripMenuItem pasteButton;
    }
}

