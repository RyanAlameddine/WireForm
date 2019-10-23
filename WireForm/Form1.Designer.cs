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
            this.debugger1 = new System.Windows.Forms.TextBox();
            this.debugger2 = new System.Windows.Forms.TextBox();
            this.gatePicBox = new System.Windows.Forms.PictureBox();
            this.GateMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.alskjdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sdfsdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolBox.Location = new System.Drawing.Point(1000, 18);
            this.toolBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.toolBox.Name = "toolBox";
            this.toolBox.Size = new System.Drawing.Size(180, 28);
            this.toolBox.TabIndex = 0;
            this.toolBox.SelectedIndexChanged += new System.EventHandler(this.toolBox_SelectedIndexChanged);
            // 
            // gateBox
            // 
            this.gateBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gateBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gateBox.FormattingEnabled = true;
            this.gateBox.Location = new System.Drawing.Point(1000, 60);
            this.gateBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gateBox.Name = "gateBox";
            this.gateBox.Size = new System.Drawing.Size(180, 28);
            this.gateBox.TabIndex = 1;
            this.gateBox.Visible = false;
            this.gateBox.SelectedIndexChanged += new System.EventHandler(this.GateBox_SelectedIndexChanged);
            // 
            // debugger1
            // 
            this.debugger1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.debugger1.Location = new System.Drawing.Point(1032, 589);
            this.debugger1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.debugger1.Name = "debugger1";
            this.debugger1.Size = new System.Drawing.Size(148, 26);
            this.debugger1.TabIndex = 2;
            this.debugger1.Text = "0";
            this.debugger1.TextChanged += new System.EventHandler(this.debugger1_TextChanged);
            // 
            // debugger2
            // 
            this.debugger2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.debugger2.Location = new System.Drawing.Point(1032, 629);
            this.debugger2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.debugger2.Name = "debugger2";
            this.debugger2.Size = new System.Drawing.Size(148, 26);
            this.debugger2.TabIndex = 3;
            this.debugger2.Text = "0";
            this.debugger2.TextChanged += new System.EventHandler(this.debugger2_TextChanged);
            // 
            // gatePicBox
            // 
            this.gatePicBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gatePicBox.BackColor = System.Drawing.Color.Transparent;
            this.gatePicBox.Location = new System.Drawing.Point(1000, 95);
            this.gatePicBox.Name = "gatePicBox";
            this.gatePicBox.Size = new System.Drawing.Size(180, 128);
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
            this.GateMenu.Size = new System.Drawing.Size(136, 68);
            this.GateMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.GateMenu_Closed);
            // 
            // alskjdfToolStripMenuItem
            // 
            this.alskjdfToolStripMenuItem.Name = "alskjdfToolStripMenuItem";
            this.alskjdfToolStripMenuItem.Size = new System.Drawing.Size(135, 32);
            this.alskjdfToolStripMenuItem.Text = "alskjdf";
            // 
            // sdfsdfToolStripMenuItem
            // 
            this.sdfsdfToolStripMenuItem.Name = "sdfsdfToolStripMenuItem";
            this.sdfsdfToolStripMenuItem.Size = new System.Drawing.Size(135, 32);
            this.sdfsdfToolStripMenuItem.Text = "sdfsdf";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 692);
            this.Controls.Add(this.gatePicBox);
            this.Controls.Add(this.debugger2);
            this.Controls.Add(this.debugger1);
            this.Controls.Add(this.gateBox);
            this.Controls.Add(this.toolBox);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox toolBox;
        private System.Windows.Forms.ComboBox gateBox;
        private System.Windows.Forms.TextBox debugger1;
        private System.Windows.Forms.TextBox debugger2;
        private System.Windows.Forms.PictureBox gatePicBox;
        private System.Windows.Forms.ContextMenuStrip GateMenu;
        private System.Windows.Forms.ToolStripMenuItem alskjdfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sdfsdfToolStripMenuItem;
    }
}

