//Description:      Code to test controlling a Prior Scientific NanoScan NPC-D-6330 in C# by importing the native C library
//                  Not all DLL functions are implemented: Init(), OpenSession(), GetChannels(), DoCommand(), GetResult(), CloseSession() and Uninit()
//                  are included as a demonstration of how. 
//                  The full DLL function set can be found in "NPC-D Series Digital Controller Interface Library.pdf" from Prior
//                  Commands to control the PZTs with DoCommand() can be found in "NPC-D-6xx0_NanoMechanism_Controller_Interface_Command_Set_And_Control_System_6.6.31.pdf"
//                  An example of requesting motion status using DoCommand() and stage-moving.get is included below but different commands will require the response handling differently.
//Authors:          James Williamson, Wenbin Zhong
//                  University of Huddersfield
//Date:             July 2023

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace prior_controller_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string comPort = "COM3"; // Change this based on how your controller enumerates when plugged in. Open device manager and look at com ports

            var handle = Init();
            if(handle == IntPtr.Zero)
            {
                Console.WriteLine("Init failed");
                return;
            }

            Console.WriteLine("Init successful");
            int result = OpenSession(handle, comPort);
            Console.WriteLine(result.ToString());

            if(result > 0)
            {
                Console.WriteLine("Successfully connected to device.");
            }
            else
            {
                Console.WriteLine("Couldn't connect.");
                Uninit(handle);
                return;
            }

            int noChannels = GetChannels(handle);
            Console.WriteLine(noChannels.ToString() + " channels detected.");

            result = DoCommand(handle, "stage.status.stage-moving.get 1");
            Console.WriteLine("Result from DoCommand was " + result.ToString());

            if (result < 0) // command not known, no controller connection, or comms error
            {
                CloseSession(handle);
                Uninit(handle);
                return;
            }
            else if(result == 1)
            {
                var buffer = new byte[1]; //stage.status.stage-moving.get returns a single ascii character, 48 (ASCII 0) or 49 (ASCII 1)
                result = GetResult(handle, 0, buffer, buffer.Length);
                string answer = BitConverter.ToString(buffer);
                var movingStatus = Convert.ToUInt32(answer);
                if(movingStatus == 0)
                {
                    Console.WriteLine("Axis not moving");
                }
                else if(movingStatus == 1)
                {
                    Console.WriteLine("Axis moving");
                }
                else
                {
                    Console.WriteLine("???");
                }
            }
            else
            {
                Console.WriteLine("More than one result received should be using getresultnamed()");
            }


            CloseSession(handle);
            Uninit(handle);
        }

        [DllImport("controller_interface64.dll", EntryPoint = "OpenSession")]

        private static extern int OpenSession(IntPtr handle, string device);

        [DllImport("controller_interface64.dll", EntryPoint = "Init")]
        private static extern IntPtr Init();

        [DllImport("controller_interface64.dll", EntryPoint = "Uninit")]
        private static extern void Uninit(IntPtr handle);

        [DllImport("controller_interface64.dll", EntryPoint = "CloseSession")]
        private static extern void CloseSession(IntPtr handle);

        [DllImport("controller_interface64.dll", EntryPoint = "GetChannels")]
        private static extern int GetChannels(IntPtr handle);


        [DllImport("controller_interface64.dll", EntryPoint = "DoCommand")]
        private static extern int DoCommand(IntPtr handle, string command);

        [DllImport("controller_interface64.dll", EntryPoint = "GetResult")]
        private static extern int GetResult(IntPtr handle, int index, byte[] buffer, int bufferLen);

    }
}
