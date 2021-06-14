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
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // addressBox
            // 
            this.addressBox.Location = new System.Drawing.Point(12, 39);
            this.addressBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.addressBox.Name = "addressBox";
            this.addressBox.Size = new System.Drawing.Size(289, 22);
            this.addressBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server address :";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // OKBt
            // 
            this.OKBt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBt.Location = new System.Drawing.Point(15, 96);
            this.OKBt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.OKBt.Name = "OKBt";
            this.OKBt.Size = new System.Drawing.Size(75, 23);
            this.OKBt.TabIndex = 2;
            this.OKBt.Text = "OK";
            this.OKBt.UseVisualStyleBackColor = true;
            // 
            // CancelBt
            // 
            this.CancelBt.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBt.Location = new System.Drawing.Point(227, 96);
            this.CancelBt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.CancelBt.Name = "CancelBt";
            this.CancelBt.Size = new System.Drawing.Size(75, 23);
            this.CancelBt.TabIndex = 3;
            this.CancelBt.Text = "Cancel";
            this.CancelBt.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(237, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Let empty to connect on local server";
            // 
            // ConnectDialog
            // 
            this.AcceptButton = this.OKBt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBt;
            this.ClientSize = new System.Drawing.Size(312, 146);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CancelBt);
            this.Controls.Add(this.OKBt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.addressBox);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
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
        private System.Windows.Forms.Label label2;
    }
}