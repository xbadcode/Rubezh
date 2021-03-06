﻿namespace ResursTerminal.Forms
{
	partial class FormMain
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
			this._menuStripMain = new System.Windows.Forms.MenuStrip();
			this._statusStripMain = new System.Windows.Forms.StatusStrip();
			this._toolStripMain = new System.Windows.Forms.ToolStrip();
			this._splitContainerMain = new System.Windows.Forms.SplitContainer();
			this._treeViewSystem = new System.Windows.Forms.TreeView();
			this.panel1 = new System.Windows.Forms.Panel();
			this._comboBoxCommands = new System.Windows.Forms.ComboBox();
			this._propertyGrid = new System.Windows.Forms.PropertyGrid();
			this._contextMenuStripSystem = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._toolStripMenuItemAddNetwork = new System.Windows.Forms.ToolStripMenuItem();
			this._toolStripMenuItemRemoveNetwork = new System.Windows.Forms.ToolStripMenuItem();
			this._toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this._toolStripMenuItemAddDevice = new System.Windows.Forms.ToolStripMenuItem();
			this._toolStripMenuItemRemoveDevice = new System.Windows.Forms.ToolStripMenuItem();
			this._buttonExecute = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this._splitContainerMain)).BeginInit();
			this._splitContainerMain.Panel1.SuspendLayout();
			this._splitContainerMain.Panel2.SuspendLayout();
			this._splitContainerMain.SuspendLayout();
			this.panel1.SuspendLayout();
			this._contextMenuStripSystem.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _menuStripMain
			// 
			this._menuStripMain.Location = new System.Drawing.Point(0, 0);
			this._menuStripMain.Name = "_menuStripMain";
			this._menuStripMain.Size = new System.Drawing.Size(875, 24);
			this._menuStripMain.TabIndex = 0;
			this._menuStripMain.Text = "_menuStripMain";
			// 
			// _statusStripMain
			// 
			this._statusStripMain.Location = new System.Drawing.Point(0, 674);
			this._statusStripMain.Name = "_statusStripMain";
			this._statusStripMain.Size = new System.Drawing.Size(875, 22);
			this._statusStripMain.TabIndex = 1;
			this._statusStripMain.Text = "_statusStripMain";
			// 
			// _toolStripMain
			// 
			this._toolStripMain.Location = new System.Drawing.Point(0, 24);
			this._toolStripMain.Name = "_toolStripMain";
			this._toolStripMain.Size = new System.Drawing.Size(875, 25);
			this._toolStripMain.TabIndex = 2;
			this._toolStripMain.Text = "_toolStripMain";
			// 
			// _splitContainerMain
			// 
			this._splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainerMain.Location = new System.Drawing.Point(0, 49);
			this._splitContainerMain.Name = "_splitContainerMain";
			// 
			// _splitContainerMain.Panel1
			// 
			this._splitContainerMain.Panel1.Controls.Add(this._treeViewSystem);
			// 
			// _splitContainerMain.Panel2
			// 
			this._splitContainerMain.Panel2.Controls.Add(this.panel1);
			this._splitContainerMain.Panel2.Controls.Add(this._propertyGrid);
			this._splitContainerMain.Size = new System.Drawing.Size(875, 625);
			this._splitContainerMain.SplitterDistance = 291;
			this._splitContainerMain.TabIndex = 3;
			// 
			// _treeViewSystem
			// 
			this._treeViewSystem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeViewSystem.Location = new System.Drawing.Point(0, 0);
			this._treeViewSystem.Name = "_treeViewSystem";
			this._treeViewSystem.Size = new System.Drawing.Size(291, 625);
			this._treeViewSystem.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 398);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(580, 227);
			this.panel1.TabIndex = 1;
			// 
			// _comboBoxCommands
			// 
			this._comboBoxCommands.FormattingEnabled = true;
			this._comboBoxCommands.Location = new System.Drawing.Point(20, 35);
			this._comboBoxCommands.Name = "_comboBoxCommands";
			this._comboBoxCommands.Size = new System.Drawing.Size(352, 24);
			this._comboBoxCommands.TabIndex = 0;
			// 
			// _propertyGrid
			// 
			this._propertyGrid.Dock = System.Windows.Forms.DockStyle.Top;
			this._propertyGrid.Location = new System.Drawing.Point(0, 0);
			this._propertyGrid.Name = "_propertyGrid";
			this._propertyGrid.Size = new System.Drawing.Size(580, 398);
			this._propertyGrid.TabIndex = 0;
			// 
			// _contextMenuStripSystem
			// 
			this._contextMenuStripSystem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripMenuItemAddNetwork,
            this._toolStripMenuItemRemoveNetwork,
            this._toolStripSeparator,
            this._toolStripMenuItemAddDevice,
            this._toolStripMenuItemRemoveDevice});
			this._contextMenuStripSystem.Name = "_contextMenuStripSystem";
			this._contextMenuStripSystem.Size = new System.Drawing.Size(227, 106);
			// 
			// _toolStripMenuItemAddNetwork
			// 
			this._toolStripMenuItemAddNetwork.Enabled = false;
			this._toolStripMenuItemAddNetwork.Name = "_toolStripMenuItemAddNetwork";
			this._toolStripMenuItemAddNetwork.Size = new System.Drawing.Size(226, 24);
			this._toolStripMenuItemAddNetwork.Text = "Добавить сеть";
			// 
			// _toolStripMenuItemRemoveNetwork
			// 
			this._toolStripMenuItemRemoveNetwork.Enabled = false;
			this._toolStripMenuItemRemoveNetwork.Name = "_toolStripMenuItemRemoveNetwork";
			this._toolStripMenuItemRemoveNetwork.Size = new System.Drawing.Size(226, 24);
			this._toolStripMenuItemRemoveNetwork.Text = "Удалить сеть";
			// 
			// _toolStripSeparator
			// 
			this._toolStripSeparator.Name = "_toolStripSeparator";
			this._toolStripSeparator.Size = new System.Drawing.Size(223, 6);
			// 
			// _toolStripMenuItemAddDevice
			// 
			this._toolStripMenuItemAddDevice.Enabled = false;
			this._toolStripMenuItemAddDevice.Name = "_toolStripMenuItemAddDevice";
			this._toolStripMenuItemAddDevice.Size = new System.Drawing.Size(226, 24);
			this._toolStripMenuItemAddDevice.Text = "Добавить устройтсво";
			// 
			// _toolStripMenuItemRemoveDevice
			// 
			this._toolStripMenuItemRemoveDevice.Enabled = false;
			this._toolStripMenuItemRemoveDevice.Name = "_toolStripMenuItemRemoveDevice";
			this._toolStripMenuItemRemoveDevice.Size = new System.Drawing.Size(226, 24);
			this._toolStripMenuItemRemoveDevice.Text = "Удалить устройство";
			// 
			// _buttonExecute
			// 
			this._buttonExecute.Location = new System.Drawing.Point(378, 36);
			this._buttonExecute.Name = "_buttonExecute";
			this._buttonExecute.Size = new System.Drawing.Size(105, 23);
			this._buttonExecute.TabIndex = 1;
			this._buttonExecute.Text = "Выполнить";
			this._buttonExecute.UseVisualStyleBackColor = true;
			this._buttonExecute.Click += new System.EventHandler(this.EventHandler_buttonExecute_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._comboBoxCommands);
			this.groupBox1.Controls.Add(this._buttonExecute);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(580, 76);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "groupBox1";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(875, 696);
			this.Controls.Add(this._splitContainerMain);
			this.Controls.Add(this._toolStripMain);
			this.Controls.Add(this._statusStripMain);
			this.Controls.Add(this._menuStripMain);
			this.MainMenuStrip = this._menuStripMain;
			this.Name = "FormMain";
			this.Text = "Form1";
			this._splitContainerMain.Panel1.ResumeLayout(false);
			this._splitContainerMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this._splitContainerMain)).EndInit();
			this._splitContainerMain.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this._contextMenuStripSystem.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip _menuStripMain;
		private System.Windows.Forms.StatusStrip _statusStripMain;
		private System.Windows.Forms.ToolStrip _toolStripMain;
		private System.Windows.Forms.SplitContainer _splitContainerMain;
		private System.Windows.Forms.TreeView _treeViewSystem;
		private System.Windows.Forms.ContextMenuStrip _contextMenuStripSystem;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemAddNetwork;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemRemoveNetwork;
		private System.Windows.Forms.ToolStripSeparator _toolStripSeparator;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemAddDevice;
		private System.Windows.Forms.ToolStripMenuItem _toolStripMenuItemRemoveDevice;
		private System.Windows.Forms.PropertyGrid _propertyGrid;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ComboBox _comboBoxCommands;
		private System.Windows.Forms.Button _buttonExecute;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}

