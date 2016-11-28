﻿using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using ZedGraph;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigCompassMot : UserControl, IActivate, IDeactivate
    {
        private readonly LineItem current = new LineItem("Current");
        private readonly PointPairList currentlist = new PointPairList();
        private readonly LineItem interference = new LineItem("Interference");
        private readonly PointPairList interferencelist = new PointPairList();
        private bool incompassmot;

        public ConfigCompassMot()
        {
            InitializeComponent();
            Utilities.ThemeManager.ApplyThemeTo(this);
            setupgraph();
        }

        public void Activate()
        {
            BUT_compassmot.Text = lbl_start.Text;
        }

        public void Deactivate()
        {
            // make sure we are stopped
            try
            {
                if (MainV2.comPort.BaseStream.IsOpen)
                    MainV2.comPort.SendAck();
            }
            catch
            {
            }

            timer1.Stop();
        }

        private void DoCompassMot()
        {
            if (incompassmot)
            {
                MainV2.comPort.SendAck();
                incompassmot = false;
                BUT_compassmot.Text = lbl_start.Text;
            }
            else
            {
                // compassmot
                MainV2.comPort.MAV.cs.messages.Clear();
                interference.Clear();
                current.Clear();
                try
                {
                    MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 0, 1, 0);
                }
                catch
                {
                    CustomMessageBox.Show("Compassmot requires AC 3.2+", Strings.ERROR);
                }
                incompassmot = true;
            }
        }

        private void BUT_compassmot_Click(object sender, EventArgs e)
        {
            BUT_compassmot.Text = lbl_finish.Text;
            DoCompassMot();
            timer1.Start();
        }

        private void setupgraph()
        {
            zedGraphControl1.GraphPane.YAxis.Title.IsVisible = true;
            zedGraphControl1.GraphPane.YAxis.Title.Text = "Interference %";
            zedGraphControl1.GraphPane.Title.IsVisible = true;
            zedGraphControl1.GraphPane.Title.Text = "Compass Motor Calibration";
            zedGraphControl1.GraphPane.XAxis.Title.Text = "Throttle %";

            zedGraphControl1.GraphPane.Y2Axis.Title.IsVisible = true;
            zedGraphControl1.GraphPane.Y2Axis.Title.Text = "Amps";
            zedGraphControl1.GraphPane.Y2Axis.IsVisible = true;


            current.IsY2Axis = true;
            current.YAxisIndex = 0;
            current.Symbol.IsVisible = false;
            //current.Bar.Fill.Color = Color.Green;

            //interference.Bar.Fill.Color = Color.Red;

            current.Line.Color = Color.Green;


            interference.Line.Color = Color.Red;
            interference.Symbol.IsVisible = false;

            zedGraphControl1.GraphPane.CurveList.Add(interference);
            zedGraphControl1.GraphPane.CurveList.Add(current);

            zedGraphControl1.GraphPane.XAxis.Scale.Min = 0;
            zedGraphControl1.GraphPane.XAxis.Scale.Max = 100;
            zedGraphControl1.GraphPane.XAxis.Scale.MinorStep = 5;
            zedGraphControl1.GraphPane.XAxis.Scale.MajorStep = 10;

            zedGraphControl1.GraphPane.YAxis.Scale.Min = 0;
            zedGraphControl1.GraphPane.YAxis.Scale.Max = 100;
            zedGraphControl1.GraphPane.YAxis.Scale.MinorStep = 5;
            zedGraphControl1.GraphPane.YAxis.Scale.MajorStep = 10;

            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var message = new StringBuilder();

            MainV2.comPort.MAV.cs.messages.ForEach(x => { message.AppendLine(x); });

            txt_status.Text = message.ToString();
            txt_status.SelectionStart = txt_status.Text.Length;
            txt_status.ScrollToCaret();

            var bytearray = MainV2.comPort.MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.COMPASSMOT_STATUS);

            if (bytearray != null)
            {
                var status = bytearray.ToStructure<MAVLink.mavlink_compassmot_status_t>();

                lbl_status.Text = "Current: " + status.current.ToString("0.00") + "\nx,y,z " +
                                  status.CompensationX.ToString("0.00") + "," + status.CompensationY.ToString("0.00") +
                                  "," + status.CompensationZ.ToString("0.00") + "\nThrottle: " + (status.throttle/10.0) +
                                  "\nInterference: " + status.interference;

                interferencelist.Add(status.throttle/10.0, status.interference);
                currentlist.Add(status.throttle/10.0, status.current);

                interferencelist.Sort();
                currentlist.Sort();

                interference.Points = interferencelist;
                current.Points = currentlist;

                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
            }
        }
    }
}