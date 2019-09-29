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
            this.SuspendLayout();
            // 
            // toolBox
            // 
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
            this.gateBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.gateBox.FormattingEnabled = true;
            this.gateBox.Location = new System.Drawing.Point(667, 39);
            this.gateBox.Name = "gateBox";
            this.gateBox.Size = new System.Drawing.Size(121, 21);
            this.gateBox.TabIndex = 1;
            this.gateBox.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gateBox);
            this.Controls.Add(this.toolBox);
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

        }

        #endregion

        private System.Windows.Forms.ComboBox toolBox;
        private System.Windows.Forms.ComboBox gateBox;
    }
}

