using Stratego.Model;

namespace Stratego.View
{
    partial class Map
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
            this.content = new System.Windows.Forms.TableLayoutPanel();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playersToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.playersToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PlayerPropertyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.netwerkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.waitForPlayersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addressesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.chatBox = new System.Windows.Forms.ToolStripTextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // content
            // 
            this.content.ColumnCount = 2;
            this.content.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.625F));
            this.content.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.375F));
            this.content.Dock = System.Windows.Forms.DockStyle.Fill;
            this.content.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.content.Location = new System.Drawing.Point(0, 31);
            this.content.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.content.Name = "content";
            this.content.RowCount = 1;
            this.content.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 422F));
            this.content.Size = new System.Drawing.Size(800, 419);
            this.content.TabIndex = 2;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 27);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playersToolStripMenuItem,
            this.playersToolStripMenuItem1,
            this.playersToolStripMenuItem2,
            this.customToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.newToolStripMenuItem.Text = "New";
            // 
            // playersToolStripMenuItem
            // 
            this.playersToolStripMenuItem.Name = "playersToolStripMenuItem";
            this.playersToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.playersToolStripMenuItem.Text = "2 players";
            // 
            // playersToolStripMenuItem1
            // 
            this.playersToolStripMenuItem1.Name = "playersToolStripMenuItem1";
            this.playersToolStripMenuItem1.Size = new System.Drawing.Size(151, 26);
            this.playersToolStripMenuItem1.Text = "3 players";
            // 
            // playersToolStripMenuItem2
            // 
            this.playersToolStripMenuItem2.Name = "playersToolStripMenuItem2";
            this.playersToolStripMenuItem2.Size = new System.Drawing.Size(151, 26);
            this.playersToolStripMenuItem2.Text = "4 players";
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(151, 26);
            this.customToolStripMenuItem.Text = "Custom";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapEditorToolStripMenuItem,
            this.PlayerPropertyToolStripMenuItem,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(49, 27);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // mapEditorToolStripMenuItem
            // 
            this.mapEditorToolStripMenuItem.Name = "mapEditorToolStripMenuItem";
            this.mapEditorToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.mapEditorToolStripMenuItem.Text = "Set up Army";
            this.mapEditorToolStripMenuItem.Click += new System.EventHandler(this.EditorModeClick);
            // 
            // PlayerPropertyToolStripMenuItem
            // 
            this.PlayerPropertyToolStripMenuItem.Name = "PlayerPropertiesToolStripMenuItem";
            this.PlayerPropertyToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.PlayerPropertyToolStripMenuItem.Text = "Player Properties";
            this.PlayerPropertyToolStripMenuItem.Click += new System.EventHandler(this.PlayerPropertiesToolStripMenuItem_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.startToolStripMenuItem.Text = "Start";
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.stopToolStripMenuItem.Text = "Stop";
            // 
            // netwerkToolStripMenuItem
            // 
            this.netwerkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.waitForPlayersToolStripMenuItem,
            this.jointToolStripMenuItem,
            this.addressesToolStripMenuItem});
            this.netwerkToolStripMenuItem.Name = "netwerkToolStripMenuItem";
            this.netwerkToolStripMenuItem.Size = new System.Drawing.Size(79, 27);
            this.netwerkToolStripMenuItem.Text = "Network";
            // 
            // waitForPlayersToolStripMenuItem
            // 
            this.waitForPlayersToolStripMenuItem.Name = "waitForPlayersToolStripMenuItem";
            this.waitForPlayersToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.waitForPlayersToolStripMenuItem.Text = "Wait for players";
            this.waitForPlayersToolStripMenuItem.Click += new System.EventHandler(this.WaitForPlayersToolStripMenuItem_Click);
            // 
            // jointToolStripMenuItem
            // 
            this.jointToolStripMenuItem.Name = "jointToolStripMenuItem";
            this.jointToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.jointToolStripMenuItem.Text = "Joint";
            this.jointToolStripMenuItem.Click += new System.EventHandler(this.JointToolStripMenuItem_Click);
            // 
            // addressesToolStripMenuItem
            // 
            this.addressesToolStripMenuItem.Name = "addressesToolStripMenuItem";
            this.addressesToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.addressesToolStripMenuItem.Text = "Addresses";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.netwerkToolStripMenuItem,
            this.chatBox});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(800, 31);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // chatBox
            // 
            this.chatBox.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.chatBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chatBox.Name = "chatBox";
            this.chatBox.Size = new System.Drawing.Size(200, 27);
            // 
            // Map
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.content);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Map";
            this.Text = "Map";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel content;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mapEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PlayerPropertyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem netwerkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem waitForPlayersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addressesToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripTextBox chatBox;
        private System.Windows.Forms.ToolStripMenuItem playersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playersToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem playersToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
    }
}