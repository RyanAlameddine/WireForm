namespace WinformsWireform
{
    partial class WireformForm
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
            this.GateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CircuitPropertyBox = new System.Windows.Forms.ListBox();
            this.CircuitPropertyValueBox = new System.Windows.Forms.ComboBox();
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
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DrawingPanel = new System.Windows.Forms.Panel();
            this.CircuitPropertyValueTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip.SuspendLayout();
            this.DrawingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBox
            // 
            this.toolBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolBox.FormattingEnabled = true;
            this.toolBox.Items.AddRange(new object[] {
            "Gate Controller (Ctrl-G)",
            "Wire Painter (Ctrl-W)",
            "Text Tool (Ctrl-E)"});
            this.toolBox.Location = new System.Drawing.Point(872, 14);
            this.toolBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.toolBox.Name = "toolBox";
            this.toolBox.Size = new System.Drawing.Size(177, 24);
            this.toolBox.TabIndex = 0;
            this.toolBox.SelectedIndexChanged += new System.EventHandler(this.ToolBox_SelectedIndexChanged);
            // 
            // GateMenu
            // 
            this.GateMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.GateMenu.Name = "contextMenuStrip1";
            this.GateMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // CircuitPropertyBox
            // 
            this.CircuitPropertyBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CircuitPropertyBox.FormattingEnabled = true;
            this.CircuitPropertyBox.ItemHeight = 16;
            this.CircuitPropertyBox.Location = new System.Drawing.Point(872, 17);
            this.CircuitPropertyBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CircuitPropertyBox.Name = "CircuitPropertyBox";
            this.CircuitPropertyBox.Size = new System.Drawing.Size(177, 276);
            this.CircuitPropertyBox.TabIndex = 6;
            this.CircuitPropertyBox.SelectedIndexChanged += new System.EventHandler(this.CircuitPropertyBox_SelectedIndexChanged);
            // 
            // CircuitPropertyValueBox
            // 
            this.CircuitPropertyValueBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CircuitPropertyValueBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CircuitPropertyValueBox.FormattingEnabled = true;
            this.CircuitPropertyValueBox.Location = new System.Drawing.Point(889, 300);
            this.CircuitPropertyValueBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CircuitPropertyValueBox.Name = "CircuitPropertyValueBox";
            this.CircuitPropertyValueBox.Size = new System.Drawing.Size(160, 24);
            this.CircuitPropertyValueBox.TabIndex = 7;
            this.CircuitPropertyValueBox.SelectedIndexChanged += new System.EventHandler(this.CircuitPropertyValueBox_SelectedIndexChanged);
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileButton,
            this.editButton,
            this.createToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1067, 28);
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
            this.fileButton.Size = new System.Drawing.Size(46, 24);
            this.fileButton.Text = "File";
            // 
            // newButton
            // 
            this.newButton.Name = "newButton";
            this.newButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newButton.Size = new System.Drawing.Size(233, 26);
            this.newButton.Text = "New";
            this.newButton.Click += new System.EventHandler(this.NewButton_Click);
            // 
            // openButton
            // 
            this.openButton.Name = "openButton";
            this.openButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openButton.Size = new System.Drawing.Size(233, 26);
            this.openButton.Text = "Open";
            this.openButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // openRecentButton
            // 
            this.openRecentButton.Name = "openRecentButton";
            this.openRecentButton.Size = new System.Drawing.Size(233, 26);
            this.openRecentButton.Text = "Open Recent";
            // 
            // save
            // 
            this.save.Name = "save";
            this.save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.save.Size = new System.Drawing.Size(233, 26);
            this.save.Text = "Save";
            this.save.Click += new System.EventHandler(this.Save_Click);
            // 
            // saveAsButton
            // 
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveAsButton.Size = new System.Drawing.Size(233, 26);
            this.saveAsButton.Text = "Save As";
            this.saveAsButton.Click += new System.EventHandler(this.SaveAs_Click);
            // 
            // closeButton
            // 
            this.closeButton.Name = "closeButton";
            this.closeButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.closeButton.Size = new System.Drawing.Size(233, 26);
            this.closeButton.Text = "Close";
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
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
            this.editButton.Size = new System.Drawing.Size(49, 24);
            this.editButton.Text = "Edit";
            // 
            // copyButton
            // 
            this.copyButton.Name = "copyButton";
            this.copyButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyButton.Size = new System.Drawing.Size(218, 26);
            this.copyButton.Text = "Copy";
            this.copyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // cutButton
            // 
            this.cutButton.Name = "cutButton";
            this.cutButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.cutButton.Size = new System.Drawing.Size(218, 26);
            this.cutButton.Text = "Cut";
            this.cutButton.Click += new System.EventHandler(this.CutButton_Click);
            // 
            // pasteButton
            // 
            this.pasteButton.Name = "pasteButton";
            this.pasteButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteButton.Size = new System.Drawing.Size(218, 26);
            this.pasteButton.Text = "Paste";
            this.pasteButton.Click += new System.EventHandler(this.PasteButton_Click);
            // 
            // undoButton
            // 
            this.undoButton.Name = "undoButton";
            this.undoButton.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoButton.Size = new System.Drawing.Size(218, 26);
            this.undoButton.Text = "Undo";
            this.undoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // redoButton
            // 
            this.redoButton.Name = "redoButton";
            this.redoButton.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Z)));
            this.redoButton.Size = new System.Drawing.Size(218, 26);
            this.redoButton.Text = "Redo";
            this.redoButton.Click += new System.EventHandler(this.RedoButton_Click);
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.createToolStripMenuItem.Text = "Create";
            // 
            // DrawingPanel
            // 
            this.DrawingPanel.Controls.Add(this.CircuitPropertyValueTextBox);
            this.DrawingPanel.Controls.Add(this.CircuitPropertyBox);
            this.DrawingPanel.Controls.Add(this.CircuitPropertyValueBox);
            this.DrawingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DrawingPanel.Location = new System.Drawing.Point(0, 28);
            this.DrawingPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DrawingPanel.Name = "DrawingPanel";
            this.DrawingPanel.Size = new System.Drawing.Size(1067, 526);
            this.DrawingPanel.TabIndex = 9;
            this.DrawingPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawingPanel_Paint);
            this.DrawingPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawingPanel_MouseDown);
            this.DrawingPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Panel_MouseMove);
            this.DrawingPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DrawingPanel_MouseUp);
            // 
            // CircuitPropertyValueTextBox
            // 
            this.CircuitPropertyValueTextBox.Location = new System.Drawing.Point(872, 301);
            this.CircuitPropertyValueTextBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CircuitPropertyValueTextBox.Name = "CircuitPropertyValueTextBox";
            this.CircuitPropertyValueTextBox.Size = new System.Drawing.Size(177, 22);
            this.CircuitPropertyValueTextBox.TabIndex = 8;
            this.CircuitPropertyValueTextBox.Visible = false;
            this.CircuitPropertyValueTextBox.TextChanged += new System.EventHandler(this.CircuitPropertyValueTextBox_TextChanged);
            this.CircuitPropertyValueTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CircuitPropertyValueTextBox_KeyDown);
            this.CircuitPropertyValueTextBox.Validated += new System.EventHandler(this.CircuitPropertyValueTextBox_Validated);
            // 
            // WireformForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.toolBox);
            this.Controls.Add(this.DrawingPanel);
            this.Controls.Add(this.menuStrip);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "WireformForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseWheel);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.DrawingPanel.ResumeLayout(false);
            this.DrawingPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox toolBox;
        internal System.Windows.Forms.ContextMenuStrip GateMenu;
        private System.Windows.Forms.ListBox CircuitPropertyBox;
        private System.Windows.Forms.ComboBox CircuitPropertyValueBox;
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
        internal System.Windows.Forms.Panel DrawingPanel;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.TextBox CircuitPropertyValueTextBox;
    }
}

