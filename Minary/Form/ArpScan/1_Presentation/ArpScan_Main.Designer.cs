﻿namespace Minary.Form.ArpScan.Presentation
{
  public partial class ArpScan
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArpScan));
      this.bt_Close = new System.Windows.Forms.Button();
      this.dgv_Targets = new System.Windows.Forms.DataGridView();
      this.gb_Range = new System.Windows.Forms.GroupBox();
      this.rb_Netrange = new System.Windows.Forms.RadioButton();
      this.rb_Subnet = new System.Windows.Forms.RadioButton();
      this.tb_Netrange2 = new System.Windows.Forms.TextBox();
      this.tb_Netrange1 = new System.Windows.Forms.TextBox();
      this.tb_Subnet2 = new System.Windows.Forms.TextBox();
      this.tb_Subnet1 = new System.Windows.Forms.TextBox();
      this.cms_ManageTargets = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.unselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.fingerprintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.thisSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.unscanedSystemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bt_Scan = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.dgv_Targets)).BeginInit();
      this.gb_Range.SuspendLayout();
      this.cms_ManageTargets.SuspendLayout();
      this.SuspendLayout();
      // 
      // bt_Close
      // 
      this.bt_Close.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.bt_Close.Location = new System.Drawing.Point(647, 294);
      this.bt_Close.Name = "bt_Close";
      this.bt_Close.Size = new System.Drawing.Size(75, 23);
      this.bt_Close.TabIndex = 8;
      this.bt_Close.Text = "Close";
      this.bt_Close.UseVisualStyleBackColor = true;
      this.bt_Close.Click += new System.EventHandler(this.Bt_Close_Click);
      // 
      // dgv_Targets
      // 
      this.dgv_Targets.AllowUserToAddRows = false;
      this.dgv_Targets.AllowUserToDeleteRows = false;
      this.dgv_Targets.AllowUserToResizeColumns = false;
      this.dgv_Targets.AllowUserToResizeRows = false;
      this.dgv_Targets.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.dgv_Targets.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
      this.dgv_Targets.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dgv_Targets.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dgv_Targets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv_Targets.Location = new System.Drawing.Point(7, 77);
      this.dgv_Targets.MultiSelect = false;
      this.dgv_Targets.Name = "dgv_Targets";
      this.dgv_Targets.ReadOnly = true;
      this.dgv_Targets.RowHeadersVisible = false;
      this.dgv_Targets.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
      this.dgv_Targets.RowTemplate.Height = 24;
      this.dgv_Targets.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.dgv_Targets.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.dgv_Targets.Size = new System.Drawing.Size(731, 210);
      this.dgv_Targets.TabIndex = 0;
      this.dgv_Targets.TabStop = false;
      this.dgv_Targets.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Dgv_Targets_MouseDown);
      this.dgv_Targets.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Dgv_Targets_MouseUp);
      // 
      // gb_Range
      // 
      this.gb_Range.Controls.Add(this.rb_Netrange);
      this.gb_Range.Controls.Add(this.rb_Subnet);
      this.gb_Range.Controls.Add(this.tb_Netrange2);
      this.gb_Range.Controls.Add(this.tb_Netrange1);
      this.gb_Range.Controls.Add(this.tb_Subnet2);
      this.gb_Range.Controls.Add(this.tb_Subnet1);
      this.gb_Range.Location = new System.Drawing.Point(7, 8);
      this.gb_Range.Name = "gb_Range";
      this.gb_Range.Size = new System.Drawing.Size(731, 56);
      this.gb_Range.TabIndex = 0;
      this.gb_Range.TabStop = false;
      this.gb_Range.Text = "Target range";
      // 
      // rb_Netrange
      // 
      this.rb_Netrange.AutoSize = true;
      this.rb_Netrange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.rb_Netrange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rb_Netrange.Location = new System.Drawing.Point(398, 23);
      this.rb_Netrange.Name = "rb_Netrange";
      this.rb_Netrange.Size = new System.Drawing.Size(80, 17);
      this.rb_Netrange.TabIndex = 4;
      this.rb_Netrange.TabStop = true;
      this.rb_Netrange.Text = "Net range";
      this.rb_Netrange.UseVisualStyleBackColor = true;
      this.rb_Netrange.CheckedChanged += new System.EventHandler(this.Rb_Netrange_CheckedChanged);
      // 
      // rb_Subnet
      // 
      this.rb_Subnet.AutoSize = true;
      this.rb_Subnet.Checked = true;
      this.rb_Subnet.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.rb_Subnet.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rb_Subnet.Location = new System.Drawing.Point(14, 23);
      this.rb_Subnet.Name = "rb_Subnet";
      this.rb_Subnet.Size = new System.Drawing.Size(64, 17);
      this.rb_Subnet.TabIndex = 1;
      this.rb_Subnet.TabStop = true;
      this.rb_Subnet.Text = "Subnet";
      this.rb_Subnet.UseVisualStyleBackColor = true;
      this.rb_Subnet.CheckedChanged += new System.EventHandler(this.Rb_Subnet_CheckedChanged);
      // 
      // tb_Netrange2
      // 
      this.tb_Netrange2.Location = new System.Drawing.Point(612, 23);
      this.tb_Netrange2.Name = "tb_Netrange2";
      this.tb_Netrange2.Size = new System.Drawing.Size(100, 20);
      this.tb_Netrange2.TabIndex = 6;
      this.tb_Netrange2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Tb_Netrange2_KeyUp);
      // 
      // tb_Netrange1
      // 
      this.tb_Netrange1.Location = new System.Drawing.Point(490, 23);
      this.tb_Netrange1.Name = "tb_Netrange1";
      this.tb_Netrange1.Size = new System.Drawing.Size(102, 20);
      this.tb_Netrange1.TabIndex = 5;
      this.tb_Netrange1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Tb_Netrange1_KeyUp);
      // 
      // tb_Subnet2
      // 
      this.tb_Subnet2.Location = new System.Drawing.Point(211, 22);
      this.tb_Subnet2.Name = "tb_Subnet2";
      this.tb_Subnet2.ReadOnly = true;
      this.tb_Subnet2.Size = new System.Drawing.Size(100, 20);
      this.tb_Subnet2.TabIndex = 3;
      // 
      // tb_Subnet1
      // 
      this.tb_Subnet1.Location = new System.Drawing.Point(88, 22);
      this.tb_Subnet1.Name = "tb_Subnet1";
      this.tb_Subnet1.ReadOnly = true;
      this.tb_Subnet1.Size = new System.Drawing.Size(102, 20);
      this.tb_Subnet1.TabIndex = 2;
      // 
      // cms_ManageTargets
      // 
      this.cms_ManageTargets.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.unselectAllToolStripMenuItem,
            this.fingerprintToolStripMenuItem});
      this.cms_ManageTargets.Name = "cms_ManageTargets";
      this.cms_ManageTargets.Size = new System.Drawing.Size(135, 70);
      // 
      // selectAllToolStripMenuItem
      // 
      this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
      this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.selectAllToolStripMenuItem.Text = "Select all";
      this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
      // 
      // unselectAllToolStripMenuItem
      // 
      this.unselectAllToolStripMenuItem.Name = "unselectAllToolStripMenuItem";
      this.unselectAllToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.unselectAllToolStripMenuItem.Text = "Unselect all";
      this.unselectAllToolStripMenuItem.Click += new System.EventHandler(this.UnselectAllToolStripMenuItem_Click);
      // 
      // fingerprintToolStripMenuItem
      // 
      this.fingerprintToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thisSystemToolStripMenuItem,
            this.unscanedSystemsToolStripMenuItem,
            this.allToolStripMenuItem});
      this.fingerprintToolStripMenuItem.Name = "fingerprintToolStripMenuItem";
      this.fingerprintToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
      this.fingerprintToolStripMenuItem.Text = "Fingerprint";
      // 
      // thisSystemToolStripMenuItem
      // 
      this.thisSystemToolStripMenuItem.Name = "thisSystemToolStripMenuItem";
      this.thisSystemToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
      this.thisSystemToolStripMenuItem.Text = "This system";
      this.thisSystemToolStripMenuItem.Click += new System.EventHandler(this.ThisSystemToolStripMenuItem_Click);
      // 
      // unscanedSystemsToolStripMenuItem
      // 
      this.unscanedSystemsToolStripMenuItem.Name = "unscanedSystemsToolStripMenuItem";
      this.unscanedSystemsToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
      this.unscanedSystemsToolStripMenuItem.Text = "Unscaned systems";
      this.unscanedSystemsToolStripMenuItem.Click += new System.EventHandler(this.UnscanedSystemsToolStripMenuItem_Click);
      // 
      // allToolStripMenuItem
      // 
      this.allToolStripMenuItem.Name = "allToolStripMenuItem";
      this.allToolStripMenuItem.Size = new System.Drawing.Size(171, 22);
      this.allToolStripMenuItem.Text = "All systems";
      this.allToolStripMenuItem.Click += new System.EventHandler(this.AllToolStripMenuItem_Click);
      // 
      // bt_Scan
      // 
      this.bt_Scan.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.bt_Scan.Location = new System.Drawing.Point(538, 293);
      this.bt_Scan.Name = "bt_Scan";
      this.bt_Scan.Size = new System.Drawing.Size(75, 23);
      this.bt_Scan.TabIndex = 7;
      this.bt_Scan.Text = "Scan";
      this.bt_Scan.UseVisualStyleBackColor = true;
      this.bt_Scan.Click += new System.EventHandler(this.Bt_Scan_Click);
      // 
      // ArpScan
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(750, 329);
      this.Controls.Add(this.bt_Scan);
      this.Controls.Add(this.gb_Range);
      this.Controls.Add(this.dgv_Targets);
      this.Controls.Add(this.bt_Close);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ArpScan";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.Text = " Target systems ...";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ArpScan_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.dgv_Targets)).EndInit();
      this.gb_Range.ResumeLayout(false);
      this.gb_Range.PerformLayout();
      this.cms_ManageTargets.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button bt_Close;
    private System.Windows.Forms.DataGridView dgv_Targets;
    private System.Windows.Forms.GroupBox gb_Range;
    private System.Windows.Forms.TextBox tb_Subnet1;
    private System.Windows.Forms.TextBox tb_Subnet2;
    private System.Windows.Forms.TextBox tb_Netrange2;
    private System.Windows.Forms.TextBox tb_Netrange1;
    private System.Windows.Forms.RadioButton rb_Subnet;
    private System.Windows.Forms.RadioButton rb_Netrange;
    private System.Windows.Forms.ContextMenuStrip cms_ManageTargets;
    private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem unselectAllToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem fingerprintToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem unscanedSystemsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem thisSystemToolStripMenuItem;
    private System.Windows.Forms.Button bt_Scan;
  }
}