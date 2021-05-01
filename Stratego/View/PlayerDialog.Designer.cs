
namespace Stratego.View
{
    partial class PlayerDialog
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.NameLb = new System.Windows.Forms.Label();
            this.ColorLb = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.OkBt = new System.Windows.Forms.Button();
            this.playerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // NameLb
            // 
            this.NameLb.AutoSize = true;
            this.NameLb.Location = new System.Drawing.Point(12, 9);
            this.NameLb.Name = "NameLb";
            this.NameLb.Size = new System.Drawing.Size(45, 17);
            this.NameLb.TabIndex = 0;
            this.NameLb.Text = "Name";
            // 
            // ColorLb
            // 
            this.ColorLb.AutoSize = true;
            this.ColorLb.Location = new System.Drawing.Point(12, 45);
            this.ColorLb.Name = "ColorLb";
            this.ColorLb.Size = new System.Drawing.Size(41, 17);
            this.ColorLb.TabIndex = 1;
            this.ColorLb.Text = "Color";
            // 
            // textBox1
            // 
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "Color", true));
            this.textBox1.Location = new System.Drawing.Point(107, 45);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 22);
            this.textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.playerBindingSource, "Name", true));
            this.textBox2.Location = new System.Drawing.Point(107, 9);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 22);
            this.textBox2.TabIndex = 3;
            // 
            // OkBt
            // 
            this.OkBt.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBt.Location = new System.Drawing.Point(15, 91);
            this.OkBt.Name = "OkBt";
            this.OkBt.Size = new System.Drawing.Size(192, 23);
            this.OkBt.TabIndex = 4;
            this.OkBt.Text = "OK";
            this.OkBt.UseVisualStyleBackColor = true;
            // 
            // playerBindingSource
            // 
            this.playerBindingSource.DataSource = typeof(Stratego.Model.Player);
            // 
            // PlayerDialog
            // 
            this.AcceptButton = this.OkBt;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(226, 126);
            this.Controls.Add(this.OkBt);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.NameLb);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.ColorLb);
            this.Name = "PlayerDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.playerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label NameLb;
        private System.Windows.Forms.Label ColorLb;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.BindingSource playerBindingSource;
        private System.Windows.Forms.Button OkBt;
    }
}
