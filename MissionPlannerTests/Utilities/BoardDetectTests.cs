﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities.BoardDetection;

namespace MissionPlanner.Utilities.Tests
{
    [TestClass()]
    public class BoardDetectTests
    {
        [TestMethod()]
        public void DetectBoardTest()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                        { board = "fmuv2", description = "", hardwareid = @"USB\VID_0483&PID_5740", name = "" },

                });

            if (ans != Boards.px4v2)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTestMany()
        {
            Boards ans;
            /*
            var ans = BoardDetect.DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                    {
                        board = "PX4 BL FMU v1.x", description = "", hardwareid = @"USB\VID_26AC&PID_0010", name = ""
                    },
                });
            if(ans == Boards.none)
                Assert.Fail();*/
            ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                    {
                        board = "PX4 BL FMU v2.x", description = "", hardwareid = @"USB\VID_26AC&PID_0011", name = ""
                    },
                }); if (ans == Boards.none)
                Assert.Fail();
            ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                    {
                        board = "PX4 BL FMU v3.x", description = "", hardwareid = @"USB\VID_26AC&PID_0011", name = ""
                    },
                }); if (ans == Boards.none)
                Assert.Fail();
            ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                    {
                        board = "PX4 BL FMU v4.x", description = "", hardwareid = @"USB\VID_26AC&PID_0012", name = ""
                    },
                }); if (ans == Boards.none)
                Assert.Fail();
            ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                    {
                        board = "PX4 BL FMU v4.x PRO", description = "", hardwareid = @"USB\VID_26AC&PID_0013",
                        name = ""
                    },
                }); if (ans == Boards.none)
                Assert.Fail();
            ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                    {
                        board = "PX4 BL FMU v5.x", description = "", hardwareid = @"USB\VID_26AC&PID_0032", name = ""
                    },
                }); if (ans == Boards.none)
                Assert.Fail();
            ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {
                    new DeviceInfo()
                    {
                        board = "ProfiCNC CUBE F4 BL", description = "", hardwareid = @"USB\VID_2DAE&PID_0001",
                        name = ""
                    },
                });
        }


        [TestMethod()]
        public void DetectBoardTest1()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo()
                        { board = "fmuv2-bl", description = "", hardwareid = @"USB\VID_1209&PID_5740", name = "" },

                });

            if (ans != Boards.px4v2)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest2()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo()
                        { board = "CubeBlack", description = "", hardwareid = @"USB\VID_2DAE&PID_0016", name = "" },

                });

            if (ans != Boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest3()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo() { board = "PX4 BL FMU v2.x", description = "", hardwareid = @"USB\VID_2341&PID_0010", name = "" },
                    new DeviceInfo() { board = "PX4 FMU v2.x", description = "", hardwareid = @"USB\VID_2341&PID_0010", name = "" },

                });

            if (ans != Boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest4()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo() { board = "PX4 BL FMU v2.x", description = "", hardwareid = @"USB\VID_26AC&PID_0010", name = "" },
                    new DeviceInfo() { board = "PX4 FMU v2.x", description = "", hardwareid = @"USB\VID_26AC&PID_0010", name = "" },

                });

            if (ans != Boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest5()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {

                    new DeviceInfo() { board = "PX4 BL FMU v5.x", description = "", hardwareid = @"USB\VID_26AC&PID_0032", name = "" },  
                    new DeviceInfo() { board = "PX4 FMU v5.x", description = "", hardwareid = @"USB\VID_26AC&PID_0032", name = "" },

                });
            if (ans != Boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest6()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {


                    new DeviceInfo() { board = "PX4 BL FMU v3.x", description = "", hardwareid = @"USB\VID_26AC&PID_0011", name = "" },

                    new DeviceInfo() { board = "PX4 FMU v3.x", description = "", hardwareid = @"USB\VID_26AC&PID_0011", name = "" },


                });

            if (ans != Boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest7()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {



                    new DeviceInfo()
                    {
                        board = "", description = "chibios or normal px4", hardwareid = @"USB\VID_0483&PID_5740",
                        name = ""
                    },

                });

            if (ans != Boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest8()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {




                    new DeviceInfo()
                    {
                        board = "PX4 BL FMU v2.x", description = "chibios or normal px4", hardwareid = @"USB\VID_26AC&PID_0011",
                        name = ""
                    },      new DeviceInfo()
                    {
                        board = "PX4 FMU v2.x", description = "chibios or normal px4", hardwareid = @"USB\VID_26AC&PID_0011",
                        name = ""
                    },
                });

            if (ans != Boards.chbootloader)
                Assert.Fail();
        }

        [TestMethod()]
        public void DetectBoardTest9()
        {
            var ans = BoardDetector.GetInstance().DetectBoard("com1",
                new List<DeviceInfo>()
                {


                    new DeviceInfo()
                    {
                        board = "", description = "chibios or normal px4", hardwareid = @"USB\VID_1209&PID_5740",
                        name = ""
                    },
                });

            if (ans != Boards.chbootloader)
                Assert.Fail();
        }
    }
}