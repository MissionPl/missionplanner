﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Comms;
using System.Threading;

namespace MissionPlanner.Controls
{
    public partial class UDPConfig : UserControl
    {
        public Label Title;
        private TextBox textBoxPort;
        private CheckBox checkBoxAutoReconnect;
        private Label label2;

        public UDPConfig()
        {
            InitializeComponent();
        }

        public string Port
        {
            get { return textBoxPort.Text; }
            set { textBoxPort.Text = value; }
        }

        public CheckState AutoReconnect
        {
            get { return checkBoxAutoReconnect.CheckState; }
            set { checkBoxAutoReconnect.CheckState = value; }
        }

        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.Title = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.checkBoxAutoReconnect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(10, 40);
            this.label2.Margin = new System.Windows.Forms.Padding(10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Enter local port";
            // 
            // Title
            // 
            this.Title.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Title.AutoSize = true;
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.ForeColor = System.Drawing.SystemColors.Control;
            this.Title.Location = new System.Drawing.Point(10, 10);
            this.Title.Margin = new System.Windows.Forms.Padding(20);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(30, 13);
            this.Title.TabIndex = 2;
            this.Title.Tag = "UDP";
            this.Title.Text = "UDP";
            // 
            // textBoxPort
            // 
            this.textBoxPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPort.Location = new System.Drawing.Point(13, 60);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(277, 20);
            this.textBoxPort.TabIndex = 4;
            // 
            // checkBoxAutoReconnect
            // 
            this.checkBoxAutoReconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxAutoReconnect.AutoSize = true;
            this.checkBoxAutoReconnect.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxAutoReconnect.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.checkBoxAutoReconnect.Checked = true;
            this.checkBoxAutoReconnect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoReconnect.ForeColor = System.Drawing.SystemColors.Control;
            this.checkBoxAutoReconnect.Location = new System.Drawing.Point(13, 170);
            this.checkBoxAutoReconnect.Name = "checkBoxAutoReconnect";
            this.checkBoxAutoReconnect.Size = new System.Drawing.Size(99, 17);
            this.checkBoxAutoReconnect.TabIndex = 8;
            this.checkBoxAutoReconnect.Text = "Auto reconnect";
            this.checkBoxAutoReconnect.UseVisualStyleBackColor = false;
            // 
            // UDPConfig
            // 
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.checkBoxAutoReconnect);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(10);
            this.Name = "UDPConfig";
            this.Size = new System.Drawing.Size(300, 200);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}