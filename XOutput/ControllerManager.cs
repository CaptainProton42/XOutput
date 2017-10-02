﻿using SlimDX.DirectInput;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;


namespace XOutput
{

    class ControllerManager : ScpDevice
    {
        private class ContData
        {
            public byte[] parsedData = new byte[28];
            public byte[] output = new byte[8];
        }

        private DirectInput directInput;
        private List<ControllerDevice> devices;
        public int deviceCount = 0;
        public int pluggedDevices = 0;
        public bool running = false;
        private List<Thread> workers = new List<Thread>();
        public const String BUS_CLASS_GUID = "{F679F562-3164-42CE-A4DB-E7DDBE723909}";
        private List<ContData> processingData = new List<ContData>();
        private Control handle;
        public bool isExclusive = false;

        private List<object> ds4locks = new List<object>();

        public ControllerManager(Control _handle)
            : base(BUS_CLASS_GUID)
        {
            directInput = new DirectInput();
            devices = new List<ControllerDevice>();
            handle = _handle;
            ds4locks = new List<object>();
        }

        #region Utility Functions

        public ControllerDevice getController(int n)
        {
            return devices[n];

        }

        public void setControllerEnable(int i, bool b)
        {
            devices[i].enabled = b;
        }

        private Int32 Scale(Int32 Value, Boolean Flip)  //returns the actual value to be sent to the device
        {
            Value -= 0x80;

            if (Value == -128) Value = -127;
            if (Flip) Value *= -1;

            return (Int32)((float)Value * 258.00787401574803149606299212599f);
        }

        #endregion

        public override Boolean Open(int Instance = 0)
        {
            return base.Open(Instance);
        }

        public override Boolean Open(String DevicePath)
        {
            m_Path = DevicePath;
            m_WinUsbHandle = (IntPtr)INVALID_HANDLE_VALUE;

            if (GetDeviceHandle(m_Path))
            {

                m_IsActive = true;

            }
            return true;
        }

        public override bool Start()
        {
            Open();
            detectControllers();

            for (int i = 0; i < deviceCount; i++)
            {
                if (devices[i] != null && devices[i].enabled)
                {
                    Resize(ref processingData, i + 1);
                    Resize(ref ds4locks, i + 1);
                    Resize(ref workers, i + 1);

                    processingData[i] = new ContData();
                    ds4locks[i] = new object();
                    Plugin(i + 1);
                    Console.WriteLine("Plugged in device {0} at slot {1}. ", devices[i].name, i);
                    int t = i;
                    workers[i] = new Thread(() =>
                    { ProcessData(t); });
                    workers[i].Start();

                    if (isExclusive)
                    {
                        devices[i].joystick.Unacquire();
                        devices[i].joystick.SetCooperativeLevel(handle, CooperativeLevel.Exclusive | CooperativeLevel.Background);
                        devices[i].joystick.Acquire();
                        Console.WriteLine("Device {0}'s cooperative level set to exclusive.", devices[i].joystick.Information.InstanceName);
                    }
                }
            }

            running = true;
            return running;
        }

        public void Resize<T>(ref List<T> list, int size)
        {
            int cur = list.Count;
            if (size < cur) list.RemoveRange(size, cur - size);
            else if (size > cur)
            {
                list.Capacity = size;
                list.AddRange(Enumerable.Repeat(default(T), size - cur));
            }
        }

        public List<ControllerDevice> detectControllers()
        {
            deviceCount = getDeviceCount()[0];
            int allDeviceCount = getDeviceCount()[1];
            Console.WriteLine("Detected {0} attached controllers out of {1} controllers in total.", deviceCount, allDeviceCount);

            for (int i = 0; i < devices.Count(); i++) //Remove disconnected controllers
            {
                if (devices[i] != null && !directInput.IsDeviceAttached(devices[i].joystick.Information.InstanceGuid))
                {
                    Console.WriteLine("{0} removed.", devices[i].joystick.Properties.InstanceName);
                    devices[i] = null;
                    if (i < workers.Count())
                    {
                        workers[i].Abort();
                        workers[i] = null;
                        Unplug(i + 1);
                    }
                }
            }

            Resize(ref devices, deviceCount);

            int skip = 0;

            foreach (var deviceInstance in directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly))
            {
                Joystick joystick = new Joystick(directInput, deviceInstance.InstanceGuid);

                if (joystick.Information.ProductGuid.ToString() == "028e045e-0000-0000-0000-504944564944") //If its an emulated controller skip it
                {
                    skip++;
                    continue;
                }

                if (joystick.Capabilities.ButtonCount < 1 && joystick.Capabilities.AxesCount < 1) //Skip if it doesn't have any button and axes
                {
                    skip++;
                    continue;
                }

                int spot = -1;
                for (int i = 0; i < deviceCount; i++)
                {
                    if (devices[i] == null)
                    {
                        if (spot == -1)
                        {
                            spot = i;
                            Console.WriteLine("Slot {0} is empty.", i);
                        }
                    }
                    else if (devices[i] != null && devices[i].joystick.Information.InstanceGuid == deviceInstance.InstanceGuid) //If the device is already initialized skip it
                    {
                        Console.WriteLine("Device {0} in Slot {1} already aquired. ", deviceInstance.InstanceName, i);
                        spot = -1;
                        break;
                    }
                }

                if (spot == -1)
                    continue;

                devices[spot] = new ControllerDevice(joystick, spot + 1);
                Console.WriteLine("Device {0} assigned to slot {1}.", devices[spot].name, spot);

                joystick.Properties.BufferSize = 128;
                joystick.Acquire();

                /*if (IsActive)       // seems unnecessary
                {
                    processingData[spot] = new ContData();
                    Console.WriteLine("Plug " + spot);
                    Plugin(spot + 1);
                    int t = spot;
                    workers[spot] = new Thread(() =>
                    { ProcessData(t); });
                    workers[spot].Start();
                }*/
            }
            Console.WriteLine("Skipped {0} device(s).", skip);
            return devices;
        }

        public int[] getDeviceCount()
        {
            int[] i = new int[2];
            i[0] = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly).Count;
            i[1] = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AllDevices).Count;
            return i;
        }

        public override bool Stop()
        {
            if (running)
            {
                running = false;
                for (int i = 0; i < workers.Count; i++)
                {
                    if (workers[i] != null)
                    {
                        workers[i].Abort();
                        Unplug(i + 1);
                        Console.WriteLine("Unplugged device {0} at slot {1}. ", devices[i].name, i);

                        devices[i].joystick.Unacquire();
                        devices[i].joystick.SetCooperativeLevel(handle, CooperativeLevel.Nonexclusive | CooperativeLevel.Background);
                        devices[i].joystick.Acquire();
                        Console.WriteLine("Device {0}'s cooperative level set to non-exclusive.", devices[i].joystick.Information.InstanceName);
                    }
                }
                workers.Clear();
                workers.Capacity = 0;
            }
            deviceCount = getDeviceCount()[0];
            return base.Stop();
        }

        public Boolean Plugin(Int32 Serial)
        {
            if (IsActive)
            {
                Int32 Transfered = 0;
                Byte[] Buffer = new Byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                Buffer[4] = (Byte)((Serial >> 0) & 0xFF);
                Buffer[5] = (Byte)((Serial >> 8) & 0xFF);
                Buffer[6] = (Byte)((Serial >> 16) & 0xFF);
                Buffer[7] = (Byte)((Serial >> 24) & 0xFF);

                pluggedDevices++;

                return DeviceIoControl(m_FileHandle, 0x2A4000, Buffer, Buffer.Length, null, 0, ref Transfered, IntPtr.Zero);
            }

            return false;
        }

        public Boolean Unplug(Int32 Serial)
        {
            if (IsActive)
            {
                Int32 Transfered = 0;
                Byte[] Buffer = new Byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                Buffer[4] = (Byte)((Serial >> 0) & 0xFF);
                Buffer[5] = (Byte)((Serial >> 8) & 0xFF);
                Buffer[6] = (Byte)((Serial >> 16) & 0xFF);
                Buffer[7] = (Byte)((Serial >> 24) & 0xFF);

                pluggedDevices--;

                return DeviceIoControl(m_FileHandle, 0x2A4004, Buffer, Buffer.Length, null, 0, ref Transfered, IntPtr.Zero);
            }
            return false;
        }

        private void ProcessData(int n) //process input and send to XInput device
        {
            while (IsActive)
            {
                lock (ds4locks[n])
                {
                    if (devices[n] == null)
                    {
                        //Console.WriteLine("die" + n.ToString());
                        //continue;
                    }
                    byte[] data = devices[n].getoutput();
                    if (data != null && devices[n].enabled)
                    {

                        data[0] = (byte)n;
                        Parse(data, processingData[n].parsedData);  //input to output
                        Report(processingData[n].parsedData, processingData[n].output);
                    }
                    Thread.Sleep(1);    //basically the polling rate
                }
            }
        }

        public Boolean Report(Byte[] Input, Byte[] Output)
        {
            if (IsActive)
            {
                Int32 Transfered = 0;


                bool result = DeviceIoControl(m_FileHandle, 0x2A400C, Input, Input.Length, Output, Output.Length, ref Transfered, IntPtr.Zero) && Transfered > 0;   //send the actual data to the XInput device
                int deviceInd = Input[4] - 1;
                return true;

            }
            return false;
        }

        public void Parse(Byte[] Input, Byte[] Output)
        {
            Byte Serial = (Byte)(Input[0] + 1);

            for (Int32 Index = 0; Index < 28; Index++) Output[Index] = 0x00;

            Output[0] = 0x1C;
            Output[4] = (Byte)(Input[0] + 1);
            Output[9] = 0x14;

            if (true)//Input[1] == 0x02) // Pad is active
            {

                UInt32 Buttons = (UInt32)((Input[10] << 0) | (Input[11] << 8) | (Input[12] << 16) | (Input[13] << 24));

                if ((Buttons & (0x1 << 0)) > 0) Output[10] |= (Byte)(1 << 5); // Back
                if ((Buttons & (0x1 << 1)) > 0) Output[10] |= (Byte)(1 << 6); // Left  Thumb
                if ((Buttons & (0x1 << 2)) > 0) Output[10] |= (Byte)(1 << 7); // Right Thumb
                if ((Buttons & (0x1 << 3)) > 0) Output[10] |= (Byte)(1 << 4); // Start

                if ((Buttons & (0x1 << 4)) > 0) Output[10] |= (Byte)(1 << 0); // Up
                if ((Buttons & (0x1 << 5)) > 0) Output[10] |= (Byte)(1 << 3); // Down
                if ((Buttons & (0x1 << 6)) > 0) Output[10] |= (Byte)(1 << 1); // Right
                if ((Buttons & (0x1 << 7)) > 0) Output[10] |= (Byte)(1 << 2); // Left

                if ((Buttons & (0x1 << 10)) > 0) Output[11] |= (Byte)(1 << 0); // Left  Shoulder
                if ((Buttons & (0x1 << 11)) > 0) Output[11] |= (Byte)(1 << 1); // Right Shoulder

                if ((Buttons & (0x1 << 12)) > 0) Output[11] |= (Byte)(1 << 7); // Y
                if ((Buttons & (0x1 << 13)) > 0) Output[11] |= (Byte)(1 << 5); // B
                if ((Buttons & (0x1 << 14)) > 0) Output[11] |= (Byte)(1 << 4); // A
                if ((Buttons & (0x1 << 15)) > 0) Output[11] |= (Byte)(1 << 6); // X

                if ((Buttons & (0x1 << 16)) > 16) Output[11] |= (Byte)(1 << 2); // Guide     

                Output[12] = Input[26]; // Left Trigger
                Output[13] = Input[27]; // Right Trigger

                Int32 ThumbLX = Scale(Input[14], false);
                Int32 ThumbLY = -Scale(Input[15], false);
                Int32 ThumbRX = Scale(Input[16], false);
                Int32 ThumbRY = -Scale(Input[17], false);

                Output[14] = (Byte)((ThumbLX >> 0) & 0xFF); // LX
                Output[15] = (Byte)((ThumbLX >> 8) & 0xFF);

                Output[16] = (Byte)((ThumbLY >> 0) & 0xFF); // LY
                Output[17] = (Byte)((ThumbLY >> 8) & 0xFF);

                Output[18] = (Byte)((ThumbRX >> 0) & 0xFF); // RX
                Output[19] = (Byte)((ThumbRX >> 8) & 0xFF);

                Output[20] = (Byte)((ThumbRY >> 0) & 0xFF); // RY
                Output[21] = (Byte)((ThumbRY >> 8) & 0xFF);
            }
        }

    }
}