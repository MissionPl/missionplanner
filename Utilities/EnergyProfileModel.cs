﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Data;


namespace MissionPlanner.Utilities
{
    /// <summary>
    /// static methods for calculate the specific velocity, current and consumption
    /// </summary>
    public static class EnergyProfileModel
    {
        private static bool _enabled;
        private static double _percentDev;
        
        /// <summary>
        /// populate/initialize energyprofile
        /// </summary>
        public static void Initialize()
        {
            // Init currentmodel for hover
            // ===========================
            CurrentHover = new CurrentModel(0f, 0f);

            // Init currentset
            // ===============

            // old pattern for init a flexible dev
            //CurrentSet.Add(1, new CurrentModel(-90f, 16.22f, 2.7f));
            //CurrentSet.Add(2, new CurrentModel(-72f, 17.91f, 0.67f));
            //CurrentSet.Add(3, new CurrentModel(-54f, 16.89f, 1.35f));
            //CurrentSet.Add(4, new CurrentModel(-36f, 17.23f, 1.35f));
            //CurrentSet.Add(5, new CurrentModel(-18f, 16.22f, 2.36f));
            //CurrentSet.Add(6, new CurrentModel(0.00f, 16.89f, 2.03f));
            //CurrentSet.Add(7, new CurrentModel(18f, 18.24f, 4.06f));
            //CurrentSet.Add(8, new CurrentModel(36f, 19.59f, 2.71f));
            //CurrentSet.Add(9, new CurrentModel(54f, 20.95f, 1.35f));
            //CurrentSet.Add(10, new CurrentModel(72f, 20.27f, 2.03f));
            //CurrentSet.Add(11, new CurrentModel(90f, 21.62f, 1.35f));

            // new pattern without flex dev
            //CurrentSet.Add(1, new CurrentModel(-90f, 16.22f));
            //CurrentSet.Add(2, new CurrentModel(-72f, 17.91f));
            //CurrentSet.Add(3, new CurrentModel(-54f, 16.89f));
            //CurrentSet.Add(4, new CurrentModel(-36f, 17.23f));
            //CurrentSet.Add(5, new CurrentModel(-18f, 16.22f));
            //CurrentSet.Add(6, new CurrentModel(0.00f, 16.89f));
            //CurrentSet.Add(7, new CurrentModel(18f, 18.24f));
            //CurrentSet.Add(8, new CurrentModel(36f, 19.59f));
            //CurrentSet.Add(9, new CurrentModel(54f, 20.95f));
            //CurrentSet.Add(10, new CurrentModel(72f, 20.27f));
            //CurrentSet.Add(11, new CurrentModel(90f, 21.62f));

            // for release
            CurrentSet.Add(1, new CurrentModel(-90f, 16.22f));
            CurrentSet.Add(2, new CurrentModel(-72f, 17.91f));
            CurrentSet.Add(3, new CurrentModel(-54f, 16.89f));
            CurrentSet.Add(4, new CurrentModel(-36f, 17.23f));
            CurrentSet.Add(5, new CurrentModel(-18f, 16.22f));
            CurrentSet.Add(6, new CurrentModel(0.00f, 16.89f));
            CurrentSet.Add(7, new CurrentModel(18f, 18.24f));
            CurrentSet.Add(8, new CurrentModel(36f, 19.59f));
            CurrentSet.Add(9, new CurrentModel(54f, 20.95f));
            CurrentSet.Add(10, new CurrentModel(72f, 20.27f));
            CurrentSet.Add(11, new CurrentModel(90f, 21.62f));
        }

        // Getter & Setter


        /// <summary>
        /// enable-flag
        /// </summary>
        public static bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value == false)
                {
                    ClearProperties();
                }
                else
                {
                    Initialize();
                }
                _enabled = value;
            }
        }

        /// <summary>
        /// Clearing the static fields
        /// </summary>
        private static void ClearProperties()
        {
            CurrentSet.Clear();
            CurrentHover = null;
            Current.Clear();
            Velocity.Clear();
            MinCurrentSplinePoints.Clear();
            AverageCurrentSplinePoints.Clear();
            MaxCurrentSplinePoints.Clear();
            PercentDev = 0.0f;
        }

        /// <summary>
        /// static dictionary for current values
        /// </summary>
        public static Dictionary<string, double> Current { get; } = new Dictionary<string, double>();

        public static CurrentModel CurrentHover { get; set; }
        /// <summary>
        /// static dictionary for current values
        /// </summary>
        public static Dictionary<int, CurrentModel> CurrentSet { get; set; } = new Dictionary<int, CurrentModel>();

        /// <summary>
        /// static dictionary for velocity values
        /// </summary>
        public static Dictionary<string, double> Velocity { get; } = new Dictionary<string, double>();

        //Lists of SplinePoints
        public static List<PointF> MinCurrentSplinePoints { get; set; } = new List<PointF>();
        public static List<PointF> AverageCurrentSplinePoints { get; set; } = new List<PointF>();
        public static List<PointF> MaxCurrentSplinePoints { get; set; } = new List<PointF>();

        //gives the deviation from percent
        public static double PercentDev
        {
            get { return _percentDev; }
            set
            {
                _percentDev = Math.Round(value, 2);
            }
        }

        // list of items for dropdown in configview
        public static List<int> DeviationInPercentList { get;  } = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8 , 9, 10 , 15, 20, 25};
    }

    // set datamodel for current
    public class CurrentModel
    {
        private double _angle;
        private double _averageCurrent;
        private double _deviation;
        private bool _percentDevFlag;

        public double PercentDev { get; set; } = 0.0f;

        public double Angle
        {
            get { return Math.Round(_angle, 0); }
            set { _angle = Math.Round(value, 0); }
        }

        public double AverageCurrent
        {
            get { return Math.Round(_averageCurrent, 2); }
            set { _averageCurrent = Math.Round(value, 2); }
        }

        public double Deviation
        {
            get
            {
                // CustomMessageBox.Show("get dev currentmodel--> " + _deviation);
                if (!_percentDevFlag)
                {
                    return Math.Round(_deviation, 2);
                }
                if (Math.Round(AverageCurrent * EnergyProfileModel.PercentDev, 2) == Math.Round(_deviation, 2))
                {
                    return _deviation;
                }
                _deviation = Math.Round(AverageCurrent * EnergyProfileModel.PercentDev, 2);
                return _deviation;
            }
            set
            {
                if (!_percentDevFlag)
                    _deviation = Math.Round(value, 2);
            }
        }

        public double MaxCurrent => Math.Round(AverageCurrent + _deviation, 2);

        public double MinCurrent => Math.Round(AverageCurrent - _deviation, 2);

        public CurrentModel(double angle, double averageCurrent, double deviation)
        {
            _angle = angle;
            _averageCurrent = averageCurrent;
            _deviation = deviation;
        }
        
        public CurrentModel(double angle, double averageCurrent)
        {
            _angle = angle;
            _averageCurrent = averageCurrent;
            _percentDevFlag = true; // set for a fix dev
        }
    }
}
