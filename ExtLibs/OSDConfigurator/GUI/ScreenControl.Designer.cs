﻿namespace OSDConfigurator.GUI
{
    partial class ScreenControl
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
            this.panelItemList = new System.Windows.Forms.FlowLayoutPanel();
            this.groupScreenOptions = new System.Windows.Forms.GroupBox();
            this.tableRoot = new System.Windows.Forms.TableLayoutPanel();
            this.tableRight = new System.Windows.Forms.TableLayoutPanel();
            this.groupOptions = new System.Windows.Forms.GroupBox();
            this.grEditorOptions = new System.Windows.Forms.GroupBox();
            this.cbUseNameCaptions = new System.Windows.Forms.CheckBox();
            this.btnClearAll = new System.Windows.Forms.Button();
            this.btnPaste = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.cbReducedView = new System.Windows.Forms.CheckBox();
            this.tableLeft = new System.Windows.Forms.TableLayoutPanel();
            this.layoutControl = new OSDConfigurator.GUI.LayoutControl();
            this.cbHighDefView = new System.Windows.Forms.CheckBox();
            this.tableRoot.SuspendLayout();
            this.tableRight.SuspendLayout();
            this.grEditorOptions.SuspendLayout();
            this.tableLeft.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelItemList
            // 
            this.panelItemList.AutoScroll = true;
            this.panelItemList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelItemList.Location = new System.Drawing.Point(4, 196);
            this.panelItemList.MinimumSize = new System.Drawing.Size(50, 50);
            this.panelItemList.Name = "panelItemList";
            this.panelItemList.Size = new System.Drawing.Size(405, 183);
            this.panelItemList.TabIndex = 1;
            // 
            // groupScreenOptions
            // 
            this.groupScreenOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupScreenOptions.Location = new System.Drawing.Point(3, 123);
            this.groupScreenOptions.Name = "groupScreenOptions";
            this.groupScreenOptions.Size = new System.Drawing.Size(344, 124);
            this.groupScreenOptions.TabIndex = 1;
            this.groupScreenOptions.TabStop = false;
            this.groupScreenOptions.Text = "Screen Options";
            // 
            // tableRoot
            // 
            this.tableRoot.ColumnCount = 2;
            this.tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableRoot.Controls.Add(this.tableRight, 1, 0);
            this.tableRoot.Controls.Add(this.tableLeft, 0, 0);
            this.tableRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableRoot.Location = new System.Drawing.Point(0, 0);
            this.tableRoot.Name = "tableRoot";
            this.tableRoot.RowCount = 1;
            this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableRoot.Size = new System.Drawing.Size(773, 387);
            this.tableRoot.TabIndex = 5;
            // 
            // tableRight
            // 
            this.tableRight.ColumnCount = 1;
            this.tableRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableRight.Controls.Add(this.groupOptions, 0, 2);
            this.tableRight.Controls.Add(this.groupScreenOptions, 0, 1);
            this.tableRight.Controls.Add(this.grEditorOptions, 0, 0);
            this.tableRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableRight.Location = new System.Drawing.Point(420, 3);
            this.tableRight.MaximumSize = new System.Drawing.Size(400, 0);
            this.tableRight.Name = "tableRight";
            this.tableRight.RowCount = 3;
            this.tableRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableRight.Size = new System.Drawing.Size(350, 381);
            this.tableRight.TabIndex = 0;
            // 
            // groupOptions
            // 
            this.groupOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupOptions.Location = new System.Drawing.Point(3, 253);
            this.groupOptions.Name = "groupOptions";
            this.groupOptions.Size = new System.Drawing.Size(344, 125);
            this.groupOptions.TabIndex = 0;
            this.groupOptions.TabStop = false;
            this.groupOptions.Text = "Item Options";
            // 
            // grEditorOptions
            // 
            this.grEditorOptions.Controls.Add(this.cbHighDefView);
            this.grEditorOptions.Controls.Add(this.cbUseNameCaptions);
            this.grEditorOptions.Controls.Add(this.btnClearAll);
            this.grEditorOptions.Controls.Add(this.btnPaste);
            this.grEditorOptions.Controls.Add(this.btnCopy);
            this.grEditorOptions.Controls.Add(this.cbReducedView);
            this.grEditorOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grEditorOptions.Location = new System.Drawing.Point(3, 3);
            this.grEditorOptions.Name = "grEditorOptions";
            this.grEditorOptions.Size = new System.Drawing.Size(344, 114);
            this.grEditorOptions.TabIndex = 2;
            this.grEditorOptions.TabStop = false;
            this.grEditorOptions.Text = "Editor Options";
            // 
            // cbUseNameCaptions
            // 
            this.cbUseNameCaptions.AutoSize = true;
            this.cbUseNameCaptions.Location = new System.Drawing.Point(23, 58);
            this.cbUseNameCaptions.Name = "cbUseNameCaptions";
            this.cbUseNameCaptions.Size = new System.Drawing.Size(103, 19);
            this.cbUseNameCaptions.TabIndex = 4;
            this.cbUseNameCaptions.Text = "Show Names";
            this.cbUseNameCaptions.UseVisualStyleBackColor = true;
            // 
            // btnClearAll
            // 
            this.btnClearAll.Location = new System.Drawing.Point(127, 58);
            this.btnClearAll.Name = "btnClearAll";
            this.btnClearAll.Size = new System.Drawing.Size(93, 33);
            this.btnClearAll.TabIndex = 3;
            this.btnClearAll.Text = "Clear All";
            this.btnClearAll.UseVisualStyleBackColor = true;
            // 
            // btnPaste
            // 
            this.btnPaste.Location = new System.Drawing.Point(226, 19);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(93, 33);
            this.btnPaste.TabIndex = 2;
            this.btnPaste.Text = "Paste Layout";
            this.btnPaste.UseVisualStyleBackColor = true;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(127, 19);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(93, 33);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy Layout";
            this.btnCopy.UseVisualStyleBackColor = true;
            // 
            // cbReducedView
            // 
            this.cbReducedView.AutoSize = true;
            this.cbReducedView.Location = new System.Drawing.Point(23, 28);
            this.cbReducedView.Name = "cbReducedView";
            this.cbReducedView.Size = new System.Drawing.Size(82, 19);
            this.cbReducedView.TabIndex = 0;
            this.cbReducedView.Text = "Decrease";
            this.cbReducedView.UseVisualStyleBackColor = true;
            // 
            // tableLeft
            // 
            this.tableLeft.AutoScroll = true;
            this.tableLeft.AutoSize = true;
            this.tableLeft.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLeft.ColumnCount = 1;
            this.tableLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLeft.Controls.Add(this.layoutControl, 0, 0);
            this.tableLeft.Controls.Add(this.panelItemList, 0, 1);
            this.tableLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLeft.Location = new System.Drawing.Point(3, 3);
            this.tableLeft.Name = "tableLeft";
            this.tableLeft.RowCount = 2;
            this.tableLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLeft.Size = new System.Drawing.Size(411, 381);
            this.tableLeft.TabIndex = 1;
            // 
            // layoutControl
            // 
            this.layoutControl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.layoutControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.layoutControl.Location = new System.Drawing.Point(80, 4);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.Padding = new System.Windows.Forms.Padding(10);
            this.layoutControl.ScreenControl = null;
            this.layoutControl.ScreenSize = new System.Drawing.Size(253, 186);
            this.layoutControl.Size = new System.Drawing.Size(253, 186);
            this.layoutControl.TabIndex = 2;
            this.layoutControl.Visualizer = null;
            // 
            // cbHighDefView
            // 
            this.cbHighDefView.AutoSize = true;
            this.cbHighDefView.Location = new System.Drawing.Point(23, 88);
            this.cbHighDefView.Name = "cbHighDefView";
            this.cbHighDefView.Size = new System.Drawing.Size(108, 24);
            this.cbHighDefView.TabIndex = 5;
            this.cbHighDefView.Text = "HD Layout";
            this.cbHighDefView.UseVisualStyleBackColor = true;
            // 
            // ScreenControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableRoot);
            this.Name = "ScreenControl";
            this.Size = new System.Drawing.Size(773, 387);
            this.tableRoot.ResumeLayout(false);
            this.tableRoot.PerformLayout();
            this.tableRight.ResumeLayout(false);
            this.grEditorOptions.ResumeLayout(false);
            this.grEditorOptions.PerformLayout();
            this.tableLeft.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel panelItemList;
        private System.Windows.Forms.GroupBox groupScreenOptions;
        private System.Windows.Forms.TableLayoutPanel tableRoot;
        private System.Windows.Forms.TableLayoutPanel tableRight;
        private System.Windows.Forms.TableLayoutPanel tableLeft;
        private System.Windows.Forms.GroupBox groupOptions;
        private System.Windows.Forms.GroupBox grEditorOptions;
        private System.Windows.Forms.CheckBox cbReducedView;
        private LayoutControl layoutControl;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.CheckBox cbUseNameCaptions;
        private System.Windows.Forms.CheckBox cbHighDefView;
    }
}
