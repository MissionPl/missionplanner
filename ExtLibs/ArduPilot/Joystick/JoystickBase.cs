﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using static alglib;


namespace MissionPlanner.Joystick
{
    public abstract class JoystickBase: IDisposable
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected IMyJoystickState state;
        public bool enabled = false;
        bool[] buttonpressed = new bool[128];
        public string name;
        public bool elevons = false;
        public bool scriptcontrolenabled=false;

        public int camtype = 0;
        public static PlatformID pid = Environment.OSVersion.Platform;

        public bool manual_control = false;

        string joystickconfigbutton = "joystickbuttons.xml";
        string joystickconfigaxis = "joystickaxis.xml";

        // set to default midpoint
        protected int hat1 = 65535/2;
        protected int hat2 = 65535/2;
        int custom0 = 65535/2;
        int custom1 = 65535/2;


        //no need for finalizer...
        //~Joystick()
        //{
        //    Dispose(false);
        //}

        private Func<MAVLinkInterface> _Interface;

        protected SynchronizationContext _context;

        protected MAVLinkInterface Interface
        {
            get { return _Interface(); }
        }
        //-----------------------camera control--------------------
        // # 30x camera
        static ushort[] wCRC_Table = new ushort[] { 0x0000, 0xCC01, 0xD801, 0x1400, 0xF001, 0x3C00, 0x2800, 0xE401, 0xA001, 0x6C00, 0x7800, 0xB401, 0x5000, 0x9C01, 0x8801, 0x4400 };
        static byte[] Ctrstr = Array.Empty<byte>();
        static byte[] head_up = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x04, 0x00, 0x00, 0xf4, 0x20, 0x00, 0x00 };
        static byte[] head_down = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x04, 0x00, 0x00, 0xf4, 0x50, 0x00, 0x00 };
        static byte[] head_left = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x04, 0x0C, 0x1E, 0x00, 0x00, 0x00, 0x00 };
        static byte[] head_right = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x04, 0x0C, 0x8E, 0x00, 0x00, 0x00, 0x00 };
        static byte[] zoom = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x12, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00 };
        static byte[] zoom_sub = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x12, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00 };
        static byte[] zoom_stop = new byte[] { 0xEB, 0x90, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x40, 0x88, 0x0F, 0x12, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00 };

        // trip 2
        private void observationmode()
        {
            //MainV2.comPort.trip2doCommand((byte)MainV2.comPort.sysidcurrent,
            //            (byte)MainV2.comPort.compidcurrent,
            //            MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
            //           0, 3, 0, 0, 0, 0,
            //           0);
            Interface.trip2doCommand((byte)Interface.sysidcurrent, (byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, 0, 3, 0, 0, 0, 0, 0, false);
        }
        private void gimbletrip2mavlin(float roll, float pitch)
        {
            Interface.trip2doCommand((byte)Interface.sysidcurrent, (byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL, 6, roll, pitch, 0, 0, 0,0, false);
            //MainV2.comPort.trip2doCommand((byte)MainV2.comPort.sysidcurrent,
            //            (byte)MainV2.comPort.compidcurrent,
            //            MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
            //            6, roll, pitch, 0, 0, 0,
            //           0);
        }
        private void sethome()
        {
            //MainV2.comPort.trip2doCommand((byte)MainV2.comPort.sysidcurrent,
            //            (byte)MainV2.comPort.compidcurrent,
            //            MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
            //            0, 13, 0, 0, 0, 0,
            //           0);
            Interface.trip2doCommand((byte)Interface.sysidcurrent, (byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
                        0, 13, 0, 0, 0, 0,
                       0);
        }
        private void trip2zoomin()
        {
            Interface.trip2doCommand((byte)Interface.sysidcurrent, (byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
                        6, 0, 0, 1, 0, 0,
                       0, false);
            Thread.Sleep(100);
            Interface.trip2doCommand((byte)Interface.sysidcurrent, (byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
                        6, 0, 0, 0, 0, 0,
                       0, false);

        }

        private void trip2zoomout()
        {
            Interface.trip2doCommand((byte)Interface.sysidcurrent, (byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
                        6, 0, 0, 2, 0, 0,
                       0, false);
            Thread.Sleep(100);
            Interface.trip2doCommand((byte)Interface.sysidcurrent, (byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
                        6, 0, 0, 0, 0, 0,
                       0, false);

        }

        private void trip2zoomstop()
        {
            Interface.trip2doCommand((byte)Interface.sysidcurrent, (byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_DIGICAM_CONTROL,
                        6, 0, 0, 0, 0, 0,
                       0, false);
        }

        private Boolean camrolllstop = true;
        private Boolean campitchstop = true;
        private void trip2gimble(int type)
        {
            //0 --> all angle 0
            // 1 --> left 10 deg move
            // 2 --> right
            // 3 --> up
            // 4 --> down
            // 5 --> stop
            if (type == 0)
            {
                sethome();
                Thread.Sleep(500);
                observationmode();

            }
            else if (type == 1)
            {
                gimbletrip2mavlin(0.5f, 0);
            }
            else if (type == 2)
            {
                gimbletrip2mavlin(-.5f, 0);
            }
            else if (type == 3)
            {
                gimbletrip2mavlin(0, 0.5f);

            }
            else if (type == 4)
            {

                gimbletrip2mavlin(0, -.5f);
            }
            else if(type == 5)
            {
                gimbletrip2mavlin(0, 0);
            }
        }

        static void crc_fly16()
        {
            int length = Ctrstr.Length;
            int lens = length;
            ushort crcTmp = 0xFFFF;
            while (length > 0)
            {
                length--;
                ushort tmp = (ushort)(wCRC_Table[(Ctrstr[lens - length - 1] ^ crcTmp) & 15] ^ (crcTmp >> 4));
                crcTmp = (ushort)(wCRC_Table[((Ctrstr[lens - length - 1] >> 4) ^ tmp) & 15] ^ (tmp >> 4));
            }
            byte crc1 = (byte)(0x00FF & crcTmp);
            byte crc2 = (byte)(0xFF & (crcTmp >> 8));

            Ctrstr = Ctrstr.Concat(new byte[] { crc1, crc2 }).ToArray();
        }

        protected void TCP_CMD()
        {
           

            try
            {
                
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                if (camtype == 2)
                {
                    crc_fly16();
                    s.Connect("192.168.42.200", 2000);
                    byte[] data = Ctrstr.ToArray();
                    s.Send(data);
                    Console.WriteLine("send data TCP Control CMD");
                    s.Close();
                    Ctrstr = Array.Empty<byte>();
                }
                else if(camtype == 3)
                {

                }

                
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        // run for up
        public void upCMD()
        {
            Ctrstr = head_up.ToArray();
            TCP_CMD();
        }

        // run for down
        public void downCMD()
        {
            Ctrstr = head_down.ToArray();
            TCP_CMD();
        }
        // run for left
        public void leftCMD()
        {
            Ctrstr = head_left.ToArray();
            TCP_CMD();
        }
        // run for right
        public void rightCMD()
        {
            Ctrstr = head_right.ToArray();
            TCP_CMD();
        }
        // ------zoom stop ------
        // run for stop zoom
        public void stopzoomCMD()
        {
            Ctrstr = zoom_stop.ToArray();
            TCP_CMD();
        }
        //------------------ run for right ----------------------
        public void zoomCMD()
        {
            Ctrstr = zoom.ToArray();
            TCP_CMD();
           
        }
        //-------------- run for right -------------------------
        public void subzoomCMD()
        {
            Ctrstr = zoom_sub.ToArray();
            TCP_CMD();
            
        }
        //-------------------------- end camera control------------
        public JoystickBase(Func<MAVLinkInterface> currentInterface)
        {
            this._Interface = currentInterface;

            this._context = SynchronizationContext.Current;
            if (_context == null)
            {
                _context = new SynchronizationContext();
            }

            for (int a = 0; a < JoyButtons.Length; a++)
                JoyButtons[a].buttonno = -1;

            if (currentInterface() == null)
                return;

            if (Interface.MAV.cs.firmware == Firmwares.ArduPlane)
            {
                loadconfig("joystickbuttons" + Interface.MAV.cs.firmware + ".xml",
                    "joystickaxis" + Interface.MAV.cs.firmware + ".xml");
            }
            else if (Interface.MAV.cs.firmware == Firmwares.ArduCopter2)
            {
                loadconfig("joystickbuttons" + Interface.MAV.cs.firmware + ".xml",
                    "joystickaxis" + Interface.MAV.cs.firmware + ".xml");
            }
            else if (Interface.MAV.cs.firmware == Firmwares.ArduRover)
            {
                loadconfig("joystickbuttons" + Interface.MAV.cs.firmware + ".xml",
                    "joystickaxis" + Interface.MAV.cs.firmware + ".xml");
            }
            else
            {
                loadconfig();
            }
        }

        public void loadconfig(string joystickconfigbuttonin = "joystickbuttons.xml",
            string joystickconfigaxisin = "joystickaxis.xml")
        {
            log.Info("Loading joystick config files " + joystickconfigbuttonin + " " + joystickconfigaxisin);

            // save for later
            if (File.Exists(joystickconfigaxisin))
            {
                this.joystickconfigbutton = joystickconfigbuttonin;
                this.joystickconfigaxis = joystickconfigaxisin;
            }
            else
            {
                this.joystickconfigbutton = Settings.GetUserDataDirectory() + joystickconfigbuttonin;
                this.joystickconfigaxis = Settings.GetUserDataDirectory() + joystickconfigaxisin;
            }
            
            // load config
            if (File.Exists(this.joystickconfigbutton) && File.Exists(this.joystickconfigaxis))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof (JoyButton[]), new Type[] {typeof (JoyButton)});

                    using (StreamReader sr = new StreamReader(this.joystickconfigbutton))
                    {
                        JoyButtons = (JoyButton[]) reader.Deserialize(sr);
                    }
                }
                catch
                {
                }

                try
                {
                    System.Xml.Serialization.XmlSerializer reader =
                        new System.Xml.Serialization.XmlSerializer(typeof (JoyChannel[]),
                            new Type[] {typeof (JoyChannel)});

                    using (StreamReader sr = new StreamReader(this.joystickconfigaxis))
                    {
                        JoyChannels = (JoyChannel[]) reader.Deserialize(sr);
                    }
                }
                catch
                {
                }
            }

            Array.Resize(ref JoyChannels, 20);
        }

        public void saveconfig()
        {
            log.Info("Saving joystick config files " + joystickconfigbutton + " " + joystickconfigaxis);

            // save config
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof (JoyButton[]), new Type[] {typeof (JoyButton)});

            using (StreamWriter sw = new StreamWriter(joystickconfigbutton))
            {
                writer.Serialize(sw, JoyButtons);
            }

            writer = new System.Xml.Serialization.XmlSerializer(typeof (JoyChannel[]), new Type[] {typeof (JoyChannel)});

            using (StreamWriter sw = new StreamWriter(joystickconfigaxis))
            {
                writer.Serialize(sw, JoyChannels);
            }
        }

        protected JoyChannel[] JoyChannels = new JoyChannel[20]; // we are base 1
        JoyButton[] JoyButtons = new JoyButton[128]; // base 0

        public abstract bool AcquireJoystick(string name);

        public static int getPressedButton(string name)
        {
            var joystick = getJoyStickByName(name);

            if (joystick == null)
                return -1;

            joystick.GetCurrentState();

            System.Threading.Thread.Sleep(500);

            var obj = joystick.GetCurrentState();

            var buttonsbefore = obj.GetButtons();

            CustomMessageBox.Show(
                "Please press the joystick button you want assigned to this function after clicking ok");

            DateTime start = DateTime.Now;

            while (start.AddSeconds(10) > DateTime.Now)
            {
                var nextstate = joystick.GetCurrentState();

                var buttons = nextstate.GetButtons();

                for (int a = 0; a < joystick.getNumButtons(); a++)
                {
                    if (buttons[a] != buttonsbefore[a])
                        return a;
                }
            }

            CustomMessageBox.Show("No valid option was detected");

            return -1;
        }

        public void setReverse(int channel, bool reverse)
        {
            JoyChannels[channel].reverse = reverse;
        }

        public void setAxis(int channel, joystickaxis axis)
        {
            JoyChannels[channel].axis = axis;
        }

        public void setChannel(int channel, joystickaxis axis, bool reverse, int expo)
        {
            JoyChannel joy = new JoyChannel();
            joy.axis = axis;
            joy.channel = channel;
            joy.expo = expo;
            joy.reverse = reverse;

            JoyChannels[channel] = joy;
        }

        public void setChannel(JoyChannel chan)
        {
            JoyChannels[chan.channel] = chan;
        }

        public JoyChannel getChannel(int channel)
        {
            return JoyChannels[channel];
        }

        public void setButton(int arrayoffset, JoyButton buttonconfig)
        {
            JoyButtons[arrayoffset] = buttonconfig;
        }

        public JoyButton getButton(int arrayoffset)
        {
            return JoyButtons[arrayoffset];
        }

        public void changeButton(int buttonid, int newid)
        {
            JoyButtons[buttonid].buttonno = newid;
        }

        public int getHatSwitchDirection()
        {
            return (state.GetPointOfView())[0];
        }

        public abstract int getNumberPOV();

        protected int BOOL_TO_SIGN(bool input)
        {
            if (input == true)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public void clearRCOverride()
        {
            // disable it, before continuing
            this.enabled = false;

            MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();

            rc.target_component = Interface.MAV.compid;
            rc.target_system = Interface.MAV.sysid;

            rc.chan1_raw = 0;
            rc.chan2_raw = 0;
            rc.chan3_raw = 0;
            rc.chan4_raw = 0;
            rc.chan5_raw = 0;
            rc.chan6_raw = 0;
            rc.chan7_raw = 0;
            rc.chan8_raw = 0;
            rc.chan9_raw = 0;
            rc.chan10_raw = 0;
            rc.chan11_raw = 0;
            rc.chan12_raw = 0;
            rc.chan13_raw = 0;
            rc.chan14_raw = 0;
            rc.chan15_raw = 0;
            rc.chan16_raw = 0;
            rc.chan17_raw = 0;
            rc.chan18_raw = 0;

            Interface.MAV.cs.rcoverridech1 = 0;
            Interface.MAV.cs.rcoverridech2 = 0;
            Interface.MAV.cs.rcoverridech3 = 0;
            Interface.MAV.cs.rcoverridech4 = 0;
            Interface.MAV.cs.rcoverridech5 = 0;
            Interface.MAV.cs.rcoverridech6 = 0;
            Interface.MAV.cs.rcoverridech7 = 0;
            Interface.MAV.cs.rcoverridech8 = 0;
            Interface.MAV.cs.rcoverridech9 = 0;
            Interface.MAV.cs.rcoverridech10 = 0;
            Interface.MAV.cs.rcoverridech11 = 0;
            Interface.MAV.cs.rcoverridech12 = 0;
            Interface.MAV.cs.rcoverridech13 = 0;
            Interface.MAV.cs.rcoverridech14 = 0;
            Interface.MAV.cs.rcoverridech15 = 0;
            Interface.MAV.cs.rcoverridech16 = 0;
            Interface.MAV.cs.rcoverridech17 = 0;
            Interface.MAV.cs.rcoverridech18 = 0;

            try
            {
                Interface.sendPacket(rc, rc.target_system, rc.target_component);
                System.Threading.Thread.Sleep(20);
                Interface.sendPacket(rc, rc.target_system, rc.target_component);
                System.Threading.Thread.Sleep(20);
                Interface.sendPacket(rc, rc.target_system, rc.target_component);
                System.Threading.Thread.Sleep(20);
                Interface.sendPacket(rc, rc.target_system, rc.target_component);
                System.Threading.Thread.Sleep(20);
                Interface.sendPacket(rc, rc.target_system, rc.target_component);
                System.Threading.Thread.Sleep(20);
                Interface.sendPacket(rc, rc.target_system, rc.target_component);

                Interface.sendPacket(rc, rc.target_system, rc.target_component);
                Interface.sendPacket(rc, rc.target_system, rc.target_component);
                Interface.sendPacket(rc, rc.target_system, rc.target_component);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void DoJoystickButtonFunction()
        {
            foreach (JoyButton but in JoyButtons)
            {
                if (but.buttonno != -1)
                {
                    getButtonState(but, but.buttonno);
                }
            }
        }

        void ProcessButtonEvent(JoyButton but, bool buttondown)
        {
            if (but.buttonno != -1)
            {
                // only do_set_relay and Button_axis0-1 uses the button up option
                if (buttondown == false)
                {
                    if (but.function != buttonfunction.Do_Set_Relay &&
                        but.function != buttonfunction.Button_axis0 &&
                        but.function != buttonfunction.Button_axis1)
                    {
                        return;
                    }
                }

                switch (but.function)
                {
                    case buttonfunction.ChangeMode:
                        string mode = but.mode;
                        if (mode != null)
                        {
                            _context.Send(delegate
                            {
                                try
                                {
                                    Interface.setMode((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,mode);
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Failed to change Modes");
                                }
                            }, null);
                        }
                        break;
                    case buttonfunction.Mount_Mode:
                        _context.Send( delegate
                        {
                            try
                            {
                                Interface.setParam((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,"MNT_MODE", but.p1);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to change mount mode");
                            }
                        }, null);

                        break;

                    case buttonfunction.Arm:
                        _context.Send( delegate
                        {
                            try
                            {
                                Interface.doARM((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,true);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Arm");
                            }
                        }, null);
                        break;
                    case buttonfunction.TakeOff:
                        _context.Send( delegate
                        {
                            try
                            {
                                Interface.setMode("Guided");
                                if (Interface.MAV.cs.firmware == Firmwares.ArduCopter2)
                                {
                                    Interface.doCommand((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 2);
                                }
                                else
                                {
                                    Interface.doCommand((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 20);
                                }
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to takeoff");
                            }
                        }, null);
                        break;
                    case buttonfunction.Disarm:
                        _context.Send( delegate
                        {
                            try
                            {
                                Interface.doARM((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,false);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Disarm");
                            }
                        }, null);
                        break;
                    case buttonfunction.Do_Set_Relay:
                        _context.Send( delegate
                        {
                            try
                            {
                                int number = (int) but.p1;
                                int state = buttondown == true ? 1 : 0;
                                Interface.doCommand((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,MAVLink.MAV_CMD.DO_SET_RELAY, number, state, 0, 0, 0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_SET_RELAY");
                            }
                        }, null);
                        break;
                    case buttonfunction.Digicam_Control:
                        Interface.setDigicamControl(true);
                        break;
                    case buttonfunction.Do_Repeat_Relay:
                        _context.Send( delegate
                        {
                            try
                            {
                                int relaynumber = (int) but.p1;
                                int repeat = (int) but.p2;
                                int time = (int) but.p3;
                                Interface.doCommand((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,MAVLink.MAV_CMD.DO_REPEAT_RELAY, relaynumber, repeat, time, 0,
                                    0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_REPEAT_RELAY");
                            }
                        }, null);
                        break;
                    case buttonfunction.Do_Set_Servo:
                        _context.Send( delegate
                        {
                            try
                            {
                                int channel = (int) but.p1;
                                int pwm = (int) but.p2;
                                Interface.doCommand((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,MAVLink.MAV_CMD.DO_SET_SERVO, channel, pwm, 0, 0, 0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_SET_SERVO");
                            }
                        }, null);
                        break;
                    case buttonfunction.Do_Repeat_Servo:
                        _context.Send( delegate
                        {
                            try
                            {
                                int channelno = (int) but.p1;
                                int pwmvalue = (int) but.p2;
                                int repeattime = (int) but.p3;
                                int delay_ms = (int) but.p4;
                                Interface.doCommand((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,MAVLink.MAV_CMD.DO_REPEAT_SERVO, channelno, pwmvalue,
                                    repeattime, delay_ms, 0, 0, 0);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to DO_REPEAT_SERVO");
                            }
                        }, null);
                        break;
                    case buttonfunction.Toggle_Pan_Stab:
                        _context.Send( delegate
                        {
                            try
                            {
                                float current = (float) Interface.MAV.param["MNT_STAB_PAN"];
                                float newvalue = (current > 0) ? 0 : 1;
                                Interface.setParam((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,"MNT_STAB_PAN", newvalue);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Toggle_Pan_Stab");
                            }
                        }, null);
                        break;
                    case buttonfunction.Gimbal_pnt_track:
                        _context.Send( delegate
                        {
                            try
                            {
                                Interface.doCommandInt((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent, MAVLink.MAV_CMD.DO_SET_ROI, 0, 0, 0, 0,
                                    (int) (Interface.MAV.cs.gimballat * 1e7), (int) (Interface.MAV.cs.gimballng * 1e7),
                                    (float) Interface.MAV.cs.GimbalPoint.Alt);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Gimbal_pnt_track");
                            }
                        }, null);
                        break;
                    case buttonfunction.Mount_Control_0:
                        _context.Send( delegate
                        {
                            try
                            {
                                Interface.setMountControl((byte)Interface.sysidcurrent,(byte)Interface.compidcurrent,0, 0, 0, false);
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Mount_Control_0");
                            }
                        }, null);
                        break;
                    case buttonfunction.Button_axis0:
                        _context.Send( delegate
                        {
                            try
                            {
                                int pwmmin = (int) but.p1;
                                int pwmmax = (int) but.p2;

                                if (buttondown)
                                    custom0 = pwmmax;
                                else
                                    custom0 = pwmmin;
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Button_axis0");
                            }
                        }, null);
                        break;
                    case buttonfunction.Button_axis1:
                        _context.Send( delegate
                        {
                            try
                            {
                                int pwmmin = (int) but.p1;
                                int pwmmax = (int) but.p2;

                                if (buttondown)
                                    custom1 = pwmmax;
                                else
                                    custom1 = pwmmin;
                            }
                            catch
                            {
                                CustomMessageBox.Show("Failed to Button_axis1");
                            }
                        }, null);
                        break;
                }
            }
        }

        const int RESXu = 1024;
        const int RESXul = 1024;
        const int RESXl = 1024;
        const int RESKul = 100;
        /*

        ushort expou(ushort x, ushort k)
        {
          // k*x*x*x + (1-k)*x
          return ((ulong)x*x*x/0x10000*k/(RESXul*RESXul/0x10000) + (RESKul-k)*x+RESKul/2)/RESKul;
        }
        // expo-funktion:
        // ---------------
        // kmplot
        // f(x,k)=exp(ln(x)*k/10) ;P[0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20]
        // f(x,k)=x*x*x*k/10 + x*(1-k/10) ;P[0,1,2,3,4,5,6,7,8,9,10]
        // f(x,k)=x*x*k/10 + x*(1-k/10) ;P[0,1,2,3,4,5,6,7,8,9,10]
        // f(x,k)=1+(x-1)*(x-1)*(x-1)*k/10 + (x-1)*(1-k/10) ;P[0,1,2,3,4,5,6,7,8,9,10]

        short expo(short x, short k)
        {
            if (k == 0) return x;
            short y;
            bool neg = x < 0;
            if (neg) x = -x;
            if (k < 0)
            {
                y = RESXu - expou((ushort)(RESXu - x), (ushort)-k);
            }
            else
            {
                y = expou((ushort)x, (ushort)k);
            }
            return neg ? -y : y;
        }

        */



        public abstract void UnAcquireJoyStick();

        /// <summary>
        /// Button press check with debounce
        /// </summary>
        /// <param name="buttonno"></param>
        /// <returns></returns>
        bool getButtonState(JoyButton but, int buttonno)
        {
            var buts = state.GetButtons();

            // button down
            bool ans = buts[buttonno] && !buttonpressed[buttonno]; // press check + debounce
            if (ans)
                ButtonDown(but);

            // button up
            ans = !buts[buttonno] && buttonpressed[buttonno];
            if (ans)
                ButtonUp(but);

            buttonpressed[buttonno] = buts[buttonno]; // set only this button

            return ans;
        }

        void ButtonDown(JoyButton but)
        {
            ProcessButtonEvent(but, true);
        }

        void ButtonUp(JoyButton but)
        {
            ProcessButtonEvent(but, false);
        }

        public abstract int getNumButtons();

        public joystickaxis getJoystickAxis(int channel)
        {
            try
            {
                
                return JoyChannels[channel].axis;
                
            }
            catch
            {
                return joystickaxis.None;
            }
        }

        public bool isButtonPressed(int buttonno)
        {
            if (state == null)
                return false;

            var buts = state.GetButtons();

            if (buts == null || JoyButtons[buttonno].buttonno < 0)
                return false;

            return buts[JoyButtons[buttonno].buttonno];
        }
        private Boolean isZ = false;
        protected short pickchannel(int chan, joystickaxis axis, bool rev, int expo)
        {
            int min, max, trim = 0;

            if (Interface.MAV.param.Count > 0)
            {
                try
                {
                    if (Interface.MAV.param.ContainsKey("RC" + chan + "_MIN"))
                    {
                        min = (int) (float) (Interface.MAV.param["RC" + chan + "_MIN"]);
                        max = (int) (float) (Interface.MAV.param["RC" + chan + "_MAX"]);
                        trim = (int) (float) (Interface.MAV.param["RC" + chan + "_TRIM"]);
                    }
                    else
                    {
                        min = 1000;
                        max = 2000;
                        trim = 1500;
                    }
                }
                catch
                {
                    min = 1000;
                    max = 2000;
                    trim = 1500;
                }
            }
            else
            {
                min = 1000;
                max = 2000;
                trim = 1500;
            }

            if (manual_control)
            {
                min = -1000;
                max = 1000;
                trim = 0;
            }

            if (chan == 3)
            {
                trim = (min + max)/2;
                //                trim = min; // throttle
            }

            int range = Math.Abs(max - min);

            int working = 0;

            switch (axis)
            {
                case joystickaxis.None:
                    working = ushort.MaxValue/2;
                    break;
                case joystickaxis.Pass:
                    working = (int) (((float) (trim - min)/range)*ushort.MaxValue);
                    break;
                case joystickaxis.ARx:
                    working = state.ARx;
                    break;

                case joystickaxis.ARy:
                    working = state.ARy;
                    break;

                case joystickaxis.ARz:
                    working = state.ARz;
                    break;

                case joystickaxis.AX:
                    working = state.AX;
                    break;

                case joystickaxis.AY:
                    working = state.AY;
                    break;

                case joystickaxis.AZ:
                    working = state.AZ;
                    break;

                case joystickaxis.FRx:
                    working = state.FRx;
                    break;

                case joystickaxis.FRy:
                    working = state.FRy;
                    break;

                case joystickaxis.FRz:
                    working = state.FRz;
                    break;

                case joystickaxis.FX:
                    working = state.FX;
                    break;

                case joystickaxis.FY:
                    working = state.FY;
                    break;

                case joystickaxis.FZ:
                    working = state.FZ;
                    break;

                case joystickaxis.Rx:
                    working = state.Rx;
                    break;

                case joystickaxis.Ry:
                    working = state.Ry;
                    break;

                case joystickaxis.Rz:
                    working = state.Rz;
                    break;

                case joystickaxis.VRx:
                    working = state.VRx;
                    break;

                case joystickaxis.VRy:
                    working = state.VRy;
                    break;

                case joystickaxis.VRz:
                    working = state.VRz;
                    break;

                case joystickaxis.VX:
                    working = state.VX;
                    break;

                case joystickaxis.VY:
                    working = state.VY;
                    break;

                case joystickaxis.VZ:
                    working = state.VZ;
                    break;

                case joystickaxis.X:
                    working = state.X;
                    break;

                case joystickaxis.Y:
                    working = state.Y;
                    break;

                case joystickaxis.Z:
                    working = state.Z;
                    break;

                case joystickaxis.Slider1:
                    int[] slider = state.GetSlider();
                    working = slider[0];
                    break;

                case joystickaxis.Slider2:
                    int[] slider1 = state.GetSlider();
                    working = slider1[1];
                    break;

                case joystickaxis.Hatud1:
                    hat1 = (int) Constrain(hat1, 0, 65535);
                    working = hat1;
                    break;

                case joystickaxis.Hatlr2:
                    hat2 = (int) Constrain(hat2, 0, 65535);
                    working = hat2;
                    break;

                case joystickaxis.Custom1:
                    working = (int)(((float)(custom0 - min) / range) * ushort.MaxValue);
                    working = (int)Constrain(working, 0, 65535);
                    break;

                case joystickaxis.Custom2:
                    working = (int)(((float)(custom1 - min) / range) * ushort.MaxValue);
                    working = (int)Constrain(working, 0, 65535);
                    break;
                case joystickaxis.UINT16_MAX:
                    return (short)-1;
            }
            // between 0 and 65535 - convert to int -500 to 500
            working = (int)map(working, 0, 65535, -500, 500);

            if (rev)
                working *= -1;

            // save for later
            int raw = working;

            working = (int) Expo(working, expo, min, max, trim);

            //add limits to movement
            working = Math.Max(min, working);
            working = Math.Min(max, working);
            
            //---------------camera control -------------------
            if (scriptcontrolenabled)
            {
                if (working > 1600 && joystickaxis.Y == axis)
                {
                    if (camtype == 2)
                    {
                        downCMD();
                    }
                    else if (camtype == 3)
                    {
                        campitchstop = false;
                        trip2gimble(3);
                    }
                    
                }
                else if (working < 1400 && joystickaxis.Y == axis)
                {
                    if (camtype == 2)
                    {
                        upCMD();
                    }
                    else if (camtype == 3)
                    {
                        campitchstop = false;
                        trip2gimble(4);
                    }
                    
                }
                else if(joystickaxis.Y == axis)
                {
                    if (camtype == 2)
                    {
                        //upCMD();
                    }
                    else if (camtype == 3 && !campitchstop)
                    {
                        campitchstop = true;
                        trip2gimble(5);
                    }
                }
                if (working > 1600 && joystickaxis.X == axis)
                {
                    
                    if (camtype == 2)
                    {

                        rightCMD();
                    }
                    else if (camtype == 3)
                    {

                        camrolllstop = false;
                        trip2gimble(2);
                    }
                }
                else if (working < 1400 && joystickaxis.X == axis)
                {
                    if (camtype == 2)
                    {
                        leftCMD();
                    }
                    else if (camtype == 3)
                    {
                        camrolllstop = false;
                        trip2gimble(1);
                    }
                    
                }
                else if(joystickaxis.X == axis)
                {
                    if (camtype == 2)
                    {
                        //upCMD();
                    }
                    else if (camtype == 3 && !camrolllstop)
                    {
                        camrolllstop = true;
                        trip2gimble(5);
                    }
                }

                if (working > 1600 && joystickaxis.Z == axis)
                {
                    
                    if (camtype == 2)
                    {
                        zoomCMD();
                    }
                    else if (camtype == 3)
                    {
                        if (!isZ)
                        {
                            trip2zoomin();
                        }
                        
                    }
                    isZ = true;
                    
                }
                else if (working < 1400 && joystickaxis.Z == axis)
                {
                    isZ = true;
                    if (camtype == 2)
                    {
                        subzoomCMD();
                    }
                    else if (camtype == 3)
                    {
                        if (!isZ)
                        {
                            trip2zoomout();
                        }
                        
                    }
                    
                }
                else if (isZ)
                {
                    isZ = false;
                    if (camtype == 2)
                    {
                        stopzoomCMD();
                    }
                    else if (camtype == 3)
                    {
                        trip2zoomstop();
                    }
                    
                }

            }
            
            return (short) working;
        }

        public static double Expo(double input, double expo, double min, double max, double mid)
        {
            // input range -500 to 500

            double expomult = expo/100.0;
            
            if (input >= 0)
            {
                // linear scale
                double linearpwm = map(input, 0, 500, mid, max);

                double expomid = (max - mid)/2;

                double factor = 0;

                // over half way though input
                if (input > 250)
                {
                    factor = 250 - (input - 250);
                }
                else
                {
                    factor = input;
                }

                return linearpwm - (factor*expomult);
            }
            else
            {
                double linearpwm = map(input, -500, 0, min, mid);

                double expomid = (mid - min)/2;

                double factor = 0;

                // over half way though input
                if (input < -250)
                {
                    factor = -250 - (input + 250);
                }
                else
                {
                    factor = input;
                }

                return linearpwm - (factor*expomult);
            }
        }

        static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min)*(out_max - out_min)/(in_max - in_min) + out_min;
        }

        double Constrain(double value, double min, double max)
        {
            if (value > max)
                return max;
            if (value < min)
                return min;
            return value;
        }

        public virtual void Dispose()
        {
        }

        public virtual bool start(string name)
        {
            this.name = name;

            if (AcquireJoystick(name) == false)
            {
                return false;
            }

            enabled = true;

            System.Threading.Thread t11 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                Name = "Joystick loop",
                Priority = System.Threading.ThreadPriority.AboveNormal,
                IsBackground = true
            };
            t11.Start();

            return true;
        }

        public abstract bool IsJoystickValid();
        public abstract IMyJoystickState GetCurrentState();

        /// <summary>
        /// Updates the rcoverride values and controls the mode changes
        /// </summary>
        protected virtual void mainloop()
        {
            while (enabled && IsJoystickValid())
            {
                try
                {
                    System.Threading.Thread.Sleep(50);
                    //joystick stuff
                    state = GetCurrentState();
                    

                    //Console.WriteLine(state);

                    if (getNumberPOV() > 0)
                    {
                        int pov = getHatSwitchDirection();

                        if (pov != -1)
                        {
                            int angle = pov/100;

                            //0 = down = 18000
                            //0 = up = 0

                            // 0
                            if (angle > 270 || angle < 90)
                                hat1 += 500;
                            // 180
                            if (angle > 90 && angle < 270)
                                hat1 -= 500;
                            // 90
                            if (angle > 0 && angle < 180)
                                hat2 += 500;
                            // 270
                            if (angle > 180 && angle < 360)
                                hat2 -= 500;
                        }
                    }

                    if (elevons)
                    {
                        //g.channel_roll.set_pwm(BOOL_TO_SIGN(g.reverse_elevons) * (BOOL_TO_SIGN(g.reverse_ch2_elevon) * int(ch2_temp - elevon2_trim) - BOOL_TO_SIGN(g.reverse_ch1_elevon) * int(ch1_temp - elevon1_trim)) / 2 + 1500);
                        //g.channel_pitch.set_pwm(                                 (BOOL_TO_SIGN(g.reverse_ch2_elevon) * int(ch2_temp - elevon2_trim) + BOOL_TO_SIGN(g.reverse_ch1_elevon) * int(ch1_temp - elevon1_trim)) / 2 + 1500);
                        short roll = pickchannel(1, JoyChannels[1].axis, false, JoyChannels[1].expo);
                        short pitch = pickchannel(2, JoyChannels[2].axis, false, JoyChannels[2].expo);

                        if (getJoystickAxis(1) != joystickaxis.None)
                            Interface.MAV.cs.rcoverridech1 =
                                (short)
                                (BOOL_TO_SIGN(JoyChannels[1].reverse)*((int) (pitch - 1500) - (int) (roll - 1500))/2 +
                                 1500);
                        if (getJoystickAxis(2) != joystickaxis.None)
                            Interface.MAV.cs.rcoverridech2 =
                                (short)
                                (BOOL_TO_SIGN(JoyChannels[2].reverse)*((int) (pitch - 1500) + (int) (roll - 1500))/2 +
                                 1500);
                    }
                    else
                    {
                        if (getJoystickAxis(1) != joystickaxis.None)
                            Interface.MAV.cs.rcoverridech1 = pickchannel(1, JoyChannels[1].axis,
                                JoyChannels[1].reverse, JoyChannels[1].expo);
                        //(ushort)(((int)state.Rz / 65.535) + 1000);
                        if (getJoystickAxis(2) != joystickaxis.None)
                            Interface.MAV.cs.rcoverridech2 = pickchannel(2, JoyChannels[2].axis,
                                JoyChannels[2].reverse, JoyChannels[2].expo);
                        //(ushort)(((int)state.Y / 65.535) + 1000);
                    }
                    if (getJoystickAxis(3) != joystickaxis.None)
                        Interface.MAV.cs.rcoverridech3 = pickchannel(3, JoyChannels[3].axis, JoyChannels[3].reverse,
                            JoyChannels[3].expo); //(ushort)(1000 - ((int)slider[0] / 65.535) + 1000);
                    if (getJoystickAxis(4) != joystickaxis.None)
                        Interface.MAV.cs.rcoverridech4 = pickchannel(4, JoyChannels[4].axis, JoyChannels[4].reverse,
                            JoyChannels[4].expo); //(ushort)(((int)state.X / 65.535) + 1000);

                    if (getJoystickAxis(5) != joystickaxis.None)
                        Interface.MAV.cs.rcoverridech5 = pickchannel(5, JoyChannels[5].axis, JoyChannels[5].reverse,
                            JoyChannels[5].expo);
                    if (getJoystickAxis(6) != joystickaxis.None)
                        Interface.MAV.cs.rcoverridech6 = pickchannel(6, JoyChannels[6].axis, JoyChannels[6].reverse,
                            JoyChannels[6].expo);
                    if (getJoystickAxis(7) != joystickaxis.None)
                        Interface.MAV.cs.rcoverridech7 = pickchannel(7, JoyChannels[7].axis, JoyChannels[7].reverse,
                            JoyChannels[7].expo);
                    if (getJoystickAxis(8) != joystickaxis.None)
                        Interface.MAV.cs.rcoverridech8 = pickchannel(8, JoyChannels[8].axis, JoyChannels[8].reverse,
                            JoyChannels[8].expo);

                    if (getJoystickAxis(9) != joystickaxis.None) Interface.MAV.cs.rcoverridech9 = pickchannel(9, JoyChannels[9].axis, JoyChannels[9].reverse, JoyChannels[9].expo);
                    if (getJoystickAxis(10) != joystickaxis.None) Interface.MAV.cs.rcoverridech10 = pickchannel(10, JoyChannels[10].axis, JoyChannels[10].reverse, JoyChannels[10].expo);
                    if (getJoystickAxis(11) != joystickaxis.None) Interface.MAV.cs.rcoverridech11 = pickchannel(11, JoyChannels[11].axis, JoyChannels[11].reverse, JoyChannels[11].expo);
                    if (getJoystickAxis(12) != joystickaxis.None) Interface.MAV.cs.rcoverridech12 = pickchannel(12, JoyChannels[12].axis, JoyChannels[12].reverse, JoyChannels[12].expo);
                    if (getJoystickAxis(13) != joystickaxis.None) Interface.MAV.cs.rcoverridech13 = pickchannel(13, JoyChannels[13].axis, JoyChannels[13].reverse, JoyChannels[13].expo);
                    if (getJoystickAxis(14) != joystickaxis.None) Interface.MAV.cs.rcoverridech14 = pickchannel(14, JoyChannels[14].axis, JoyChannels[14].reverse, JoyChannels[14].expo);
                    if (getJoystickAxis(15) != joystickaxis.None) Interface.MAV.cs.rcoverridech15 = pickchannel(15, JoyChannels[15].axis, JoyChannels[15].reverse, JoyChannels[15].expo);
                    if (getJoystickAxis(16) != joystickaxis.None) Interface.MAV.cs.rcoverridech16 = pickchannel(16, JoyChannels[16].axis, JoyChannels[16].reverse, JoyChannels[16].expo);
                    if (getJoystickAxis(17) != joystickaxis.None) Interface.MAV.cs.rcoverridech17 = pickchannel(17, JoyChannels[17].axis, JoyChannels[17].reverse, JoyChannels[17].expo);
                    if (getJoystickAxis(18) != joystickaxis.None) Interface.MAV.cs.rcoverridech18 = pickchannel(18, JoyChannels[18].axis, JoyChannels[18].reverse, JoyChannels[18].expo);


                    // disable button actions when not connected.
                    if (Interface.BaseStream.IsOpen)
                        DoJoystickButtonFunction();

                    //Console.WriteLine("{0} {1} {2} {3}", Interface.MAV.cs.rcoverridech1, Interface.MAV.cs.rcoverridech2, Interface.MAV.cs.rcoverridech3, Interface.MAV.cs.rcoverridech4);
                }
                catch (SharpDX.SharpDXException ex)
                {
                    log.Error(ex);
                    clearRCOverride();
                    _context.Send(
                        delegate { CustomMessageBox.Show("Lost Joystick", "Lost Joystick"); }, null);
                    return;
                }
                catch (Exception ex)
                {
                    log.Info("Joystick thread error " + ex.ToString());
                } // so we cant fall out
            }
        }

        public virtual short getValueForChannel(int channel)
        {
            if (!IsJoystickValid())
                return 0;

            state = GetCurrentState();

            short ans = pickchannel(channel, JoyChannels[channel].axis, JoyChannels[channel].reverse,
                JoyChannels[channel].expo);
            log.DebugFormat("{0} = {1} = {2}", channel, ans, state.X);
            return ans;
        }

        public virtual short getRawValueForChannel(int channel)
        {
            if (!IsJoystickValid())
                return 0;

            state = GetCurrentState();

            short ans = pickchannel(channel, JoyChannels[channel].axis, false, 0);
            log.DebugFormat("{0} = {1} = {2}", channel, ans, state.X);
            return ans;
        }

        public static joystickaxis getMovingAxis(string name, int threshold)
        {
            var js = getJoyStickByName(name);

            if (js == null)
                return joystickaxis.ARx;

            js.GetCurrentState();

            System.Threading.Thread.Sleep(300);
            
            var obj = js.GetCurrentState();
            Hashtable values = new Hashtable();

            // get the state of the joystick before.
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                values[property.Name] = int.Parse(property.GetValue(obj, null).ToString());
            }
            values["Slider1"] = obj.GetSlider()[0];
            values["Slider2"] = obj.GetSlider()[1];
            values["Hatud1"] = obj.GetPointOfView()[0];
            values["Hatlr2"] = obj.GetPointOfView()[0];
            values["Custom1"] = 0;
            values["Custom2"] = 0;

            CustomMessageBox.Show("Please move the joystick axis you want assigned to this function after clicking ok");

            DateTime start = DateTime.Now;

            while (start.AddSeconds(10) > DateTime.Now)
            {
                System.Threading.Thread.Sleep(50);
                var nextstate = js.GetCurrentState();

                int[] slider = nextstate.GetSlider();

                int[] hat1 = nextstate.GetPointOfView();

                type = nextstate.GetType();
                properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    //Console.WriteLine("Name: " + property.Name + ", Value: " + property.GetValue(obj, null));

                    log.InfoFormat("test name {0} old {1} new {2} ", property.Name, values[property.Name],
                        int.Parse(property.GetValue(nextstate, null).ToString()));
                    log.InfoFormat("{0}  {1} {2}", property.Name, (int) values[property.Name],
                        (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold));
                    if ((int) values[property.Name] >
                        (int.Parse(property.GetValue(nextstate, null).ToString()) + threshold) ||
                        (int) values[property.Name] <
                        (int.Parse(property.GetValue(nextstate, null).ToString()) - threshold))
                    {
                        log.Info(property.Name);
                        js.UnAcquireJoyStick();
                        return (joystickaxis) Enum.Parse(typeof (joystickaxis), property.Name);
                    }
                }

                // slider1
                if ((int) values["Slider1"] > (slider[0] + threshold) ||
                    (int) values["Slider1"] < (slider[0] - threshold))
                {
                    js.UnAcquireJoyStick();
                    return joystickaxis.Slider1;
                }

                // slider2
                if ((int) values["Slider2"] > (slider[1] + threshold) ||
                    (int) values["Slider2"] < (slider[1] - threshold))
                {
                    js.UnAcquireJoyStick();
                    return joystickaxis.Slider2;
                }

                // Hatud1
                if ((int) values["Hatud1"] != (hat1[0]))
                {
                    js.UnAcquireJoyStick();
                    return joystickaxis.Hatud1;
                }

                // Hatlr2
                if ((int) values["Hatlr2"] != (hat1[0]))
                {
                    js.UnAcquireJoyStick();
                    return joystickaxis.Hatlr2;
                }
            }

            CustomMessageBox.Show("No valid option was detected");

            return joystickaxis.None;
        }

        public static IList<string> getDevices()
        {
            if (pid == PlatformID.Unix)
            {
                return JoystickLinux.getDevices();
            }
            else
            {
                return JoystickWindows.getDevices().Select(a => a.ProductName.TrimUnPrintable()).ToList();
            }
        }

        public static JoystickBase getJoyStickByName(string name)
        {
            if (pid == PlatformID.Unix)
            {
                return JoystickLinux.getJoyStickByName(name);
            }
            else
            {
                return JoystickWindows.getJoyStickByName(name);
            }
        }

        public static JoystickBase Create(Func<MAVLinkInterface> func)
        {
            if (pid == PlatformID.Unix)
            {
                return new JoystickLinux(func);
            }
            else
            {
                return new JoystickWindows(func);
            }
        }
    }
}