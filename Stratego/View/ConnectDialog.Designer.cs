namespace Stratego.View
{
    partial class ConnectDialog
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
            this.addressBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.OKBt = new System.Windows.Forms.Button();
            this.CancelBt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // addressBox
            // 
            this.addressBox.Location = new System.Drawing.Point(9, 32);
            this.addressBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.addressBox.Name = "addressBox";
            this.addressBox.Size = new System.Drawing.Size(218, 20);
            this.addressBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server address :";
            // 
            // OKButton
            // 
            this.OKBt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBt.Location = new System.Drawing.Point(11, 78);
            this.OKBt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.OKBt.Name = "OKButton";
            this.OKBt.Size = new System.Drawing.Size(56, 19);
            this.OKBt.TabIndex = 2;
            this.OKBt.Text = "OK";
            this.OKBt.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelBt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBt.Location = new System.Drawing.Point(170, 78);
            this.CancelBt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CancelBt.Name = "CancelButton";
            this.CancelBt.Size = new System.Drawing.Size(56, 19);
            this.CancelBt.TabIndex = 3;
            this.CancelBt.Text = "Cancel";
            this.CancelBt.UseVisualStyleBackColor = true;
            // 
            // ConnectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(234, 119);
            this.Controls.Add(this.CancelBt);
            this.Controls.Add(this.OKBt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.addressBox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ConnectDialog";
            this.Text = "ConnectDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox addressBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button OKBt;
        private System.Windows.Forms.Button CancelBt;
    }
}