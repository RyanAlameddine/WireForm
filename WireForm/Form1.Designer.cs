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
            this.toolBox = new System.Windows.Forms.ComboBox();
            this.gateBox = new System.Windows.Forms.ComboBox();
            this.debugger1 = new System.Windows.Forms.TextBox();
            this.debugger2 = new System.Windows.Forms.TextBox();
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
            // 
            // debugger1
            // 
            this.debugger1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.debugger1.Location = new System.Drawing.Point(688, 383);
            this.debugger1.Name = "debugger1";
            this.debugger1.Size = new System.Drawing.Size(100, 20);
            this.debugger1.TabIndex = 2;
            this.debugger1.Text = "0";
            this.debugger1.TextChanged += new System.EventHandler(this.debugger1_TextChanged);
            // 
            // debugger2
            // 
            this.debugger2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.debugger2.Location = new System.Drawing.Point(688, 409);
            this.debugger2.Name = "debugger2";
            this.debugger2.Size = new System.Drawing.Size(100, 20);
            this.debugger2.TabIndex = 3;
            this.debugger2.Text = "0";
            this.debugger2.TextChanged += new System.EventHandler(this.debugger2_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.debugger2);
            this.Controls.Add(this.debugger1);
            this.Controls.Add(this.gateBox);
            this.Controls.Add(this.toolBox);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseWheel);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox toolBox;
        private System.Windows.Forms.ComboBox gateBox;
        private System.Windows.Forms.TextBox debugger1;
        private System.Windows.Forms.TextBox debugger2;
    }
}

