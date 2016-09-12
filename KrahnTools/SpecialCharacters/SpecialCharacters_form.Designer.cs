namespace KrahnTools.SpecialCharacters
{
    partial class SpecialCharacters_form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecialCharacters_form));
            this.SpecialChars_lb = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Preview_lbl = new System.Windows.Forms.Label();
            this.Paste_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SpecialChars_lb
            // 
            this.SpecialChars_lb.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpecialChars_lb.FormattingEnabled = true;
            this.SpecialChars_lb.ItemHeight = 20;
            this.SpecialChars_lb.Location = new System.Drawing.Point(25, 46);
            this.SpecialChars_lb.Name = "SpecialChars_lb";
            this.SpecialChars_lb.Size = new System.Drawing.Size(188, 144);
            this.SpecialChars_lb.TabIndex = 0;
            this.SpecialChars_lb.SelectedIndexChanged += new System.EventHandler(this.SpecialChars_lb_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select Special Character From List Below";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(222, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Character Preview";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Preview_lbl
            // 
            this.Preview_lbl.AutoSize = true;
            this.Preview_lbl.BackColor = System.Drawing.SystemColors.MenuBar;
            this.Preview_lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Preview_lbl.Location = new System.Drawing.Point(219, 70);
            this.Preview_lbl.Margin = new System.Windows.Forms.Padding(3);
            this.Preview_lbl.MaximumSize = new System.Drawing.Size(100, 100);
            this.Preview_lbl.MinimumSize = new System.Drawing.Size(100, 100);
            this.Preview_lbl.Name = "Preview_lbl";
            this.Preview_lbl.Size = new System.Drawing.Size(100, 100);
            this.Preview_lbl.TabIndex = 1;
            this.Preview_lbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Paste_btn
            // 
            this.Paste_btn.Location = new System.Drawing.Point(235, 176);
            this.Paste_btn.Name = "Paste_btn";
            this.Paste_btn.Size = new System.Drawing.Size(75, 23);
            this.Paste_btn.TabIndex = 2;
            this.Paste_btn.Text = "Copy";
            this.Paste_btn.UseVisualStyleBackColor = true;
            this.Paste_btn.Click += new System.EventHandler(this.Paste_btn_Click);
            // 
            // SpecialCharacters_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 206);
            this.Controls.Add(this.Paste_btn);
            this.Controls.Add(this.Preview_lbl);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SpecialChars_lb);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(345, 245);
            this.MinimumSize = new System.Drawing.Size(345, 245);
            this.Name = "SpecialCharacters_form";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Special Characters";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox SpecialChars_lb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label Preview_lbl;
        private System.Windows.Forms.Button Paste_btn;
    }
}