namespace PSDcreator
{
    partial class uipKrahnTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(uipKrahnTools));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.gbxInstructions = new System.Windows.Forms.GroupBox();
            this.rtxtInstructions = new System.Windows.Forms.RichTextBox();
            this.gbx_Placement = new System.Windows.Forms.GroupBox();
            this.rdo_Struct = new System.Windows.Forms.RadioButton();
            this.rdo_nonStruct = new System.Windows.Forms.RadioButton();
            this.gbxInstructions.SuspendLayout();
            this.gbx_Placement.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(873, 482);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(792, 482);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // gbxInstructions
            // 
            this.gbxInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxInstructions.Controls.Add(this.rtxtInstructions);
            this.gbxInstructions.Location = new System.Drawing.Point(12, 12);
            this.gbxInstructions.Name = "gbxInstructions";
            this.gbxInstructions.Size = new System.Drawing.Size(936, 350);
            this.gbxInstructions.TabIndex = 2;
            this.gbxInstructions.TabStop = false;
            this.gbxInstructions.Text = "Instructions";
            // 
            // rtxtInstructions
            // 
            this.rtxtInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxtInstructions.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtInstructions.Location = new System.Drawing.Point(6, 19);
            this.rtxtInstructions.Name = "rtxtInstructions";
            this.rtxtInstructions.ReadOnly = true;
            this.rtxtInstructions.Size = new System.Drawing.Size(924, 325);
            this.rtxtInstructions.TabIndex = 0;
            this.rtxtInstructions.Text = "";
            // 
            // gbx_Placement
            // 
            this.gbx_Placement.Controls.Add(this.rdo_nonStruct);
            this.gbx_Placement.Controls.Add(this.rdo_Struct);
            this.gbx_Placement.Location = new System.Drawing.Point(18, 368);
            this.gbx_Placement.Name = "gbx_Placement";
            this.gbx_Placement.Size = new System.Drawing.Size(930, 100);
            this.gbx_Placement.TabIndex = 3;
            this.gbx_Placement.TabStop = false;
            this.gbx_Placement.Text = "Side To Place Section Mark:";
            // 
            // rdo_Struct
            // 
            this.rdo_Struct.AutoSize = true;
            this.rdo_Struct.Checked = true;
            this.rdo_Struct.Location = new System.Drawing.Point(7, 20);
            this.rdo_Struct.Name = "rdo_Struct";
            this.rdo_Struct.Size = new System.Drawing.Size(70, 17);
            this.rdo_Struct.TabIndex = 0;
            this.rdo_Struct.TabStop = true;
            this.rdo_Struct.Text = "Structural";
            this.rdo_Struct.UseVisualStyleBackColor = true;
            this.rdo_Struct.CheckedChanged += new System.EventHandler(this.rdo_Struct_CheckedChanged);
            // 
            // rdo_nonStruct
            // 
            this.rdo_nonStruct.AutoSize = true;
            this.rdo_nonStruct.Location = new System.Drawing.Point(7, 44);
            this.rdo_nonStruct.Name = "rdo_nonStruct";
            this.rdo_nonStruct.Size = new System.Drawing.Size(93, 17);
            this.rdo_nonStruct.TabIndex = 1;
            this.rdo_nonStruct.Text = "Non-Structural";
            this.rdo_nonStruct.UseVisualStyleBackColor = true;
            this.rdo_nonStruct.CheckedChanged += new System.EventHandler(this.rdo_nonStruct_CheckedChanged);
            // 
            // uipKrahnTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(960, 517);
            this.Controls.Add(this.gbx_Placement);
            this.Controls.Add(this.gbxInstructions);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "uipKrahnTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Krahn Tools";
            this.gbxInstructions.ResumeLayout(false);
            this.gbx_Placement.ResumeLayout(false);
            this.gbx_Placement.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.GroupBox gbxInstructions;
        private System.Windows.Forms.RichTextBox rtxtInstructions;
        private System.Windows.Forms.GroupBox gbx_Placement;
        private System.Windows.Forms.RadioButton rdo_nonStruct;
        private System.Windows.Forms.RadioButton rdo_Struct;
    }
}