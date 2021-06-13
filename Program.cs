using LookSin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;

namespace Recive_data_from_ADC
{
    class Program
    {
        static bool exitSystem = false;
        static TOP_file tOP_File_2;
        static TOP_file tOP_File;



        #region Trap application termination
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            Console.WriteLine("Exiting system due to external CTRL-C, or process kill, or shutdown");





            ////do your cleanup here
            //System.Threading.Thread.Sleep(5000); //simulate some cleanup delay

            //Console.WriteLine("Cleanup complete");

            ////allow main to run off
            //exitSystem = true;

            ////shutdown right away so there are no lingering threads
            Environment.Exit(-1);

            return true;
        }
        #endregion


        static void Main(string[] args)
        {
            // Some biolerplate to react to close window event, CTRL-C, kill, etc
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);



            byte[] ip = {192,168,0,57};
            IPAddress iPAddres = new IPAddress(ip);
            tOP_File = new TOP_file($@"C:\Users\Nikita\Desktop\LOL", iPAddres, 1400);
            Console.WriteLine("Запись с порта 1400 начата");
            tOP_File.StartRecive();


            tOP_File_2 = new TOP_file($@"C:\Users\Nikita\Desktop\LOL", iPAddres, 2500);
            tOP_File_2.StartRecive();
            Console.WriteLine("Запись с порта 2500 начата");
            
        }

    }
}
