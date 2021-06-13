using LookSin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Recive_data_from_ADC
{
    class TOP_file
    {
        /// <summary>
        /// Порт для прослушивания
        /// </summary>
        private static uint NumPort;
        public static uint numPort { get { return NumPort; } private set { } }




        /// <summary>
        /// Путь к катологу для прослушивания
        /// </summary>
        private static string path_to_catolog_for_file;
        public static string Path_to_catolog_for_file { get { return path_to_catolog_for_file; } private set { } }



        /// <summary>
        /// ip адрес отправителя
        /// </summary>
        private static IPAddress iPAddress;
        public static IPAddress IPAdd_ { get { return iPAddress; } private set { } }

        public static UDHHandler UDPhandler;

        public static WriterIn_FILE WR_File;


        Thread thread_WR;
        Thread thread_UDP;

        public TOP_file(string path_to_catolog_for_file, IPAddress iPAddress, uint NumPort)
        {

            ////////////////////////////////////////////////////////////////
            UDPhandler = new UDHHandler(NumPort, iPAddress);
            UDPhandler.DataRecive += UDPhandler_DataRecive;

            ////////////////////////////////////////////////////////////////
            WR_File = new WriterIn_FILE(NumPort, path_to_catolog_for_file);
            TOP_file.NumPort = NumPort;
            TOP_file.path_to_catolog_for_file = path_to_catolog_for_file;
            TOP_file.iPAddress = iPAddress;

            ///Создаем поток для работы с записью в файл
            thread_WR = new Thread(new ParameterizedThreadStart(WR_File.WriterInFILE));

            ///Создаем поток для работы с UDP
            thread_UDP = new Thread(new ThreadStart(UDPhandler.UDPRecive));

        }

        private void UDPhandler_DataRecive(byte[] vs)
        {
            try
            {
               if(thread_WR.IsAlive) thread_WR.Join();
               else thread_WR.Start((object)vs);
            }
            catch(Exception e) { Console.WriteLine($"Исключение вида {e}"); }
        }

        public void StartRecive()
        {
            thread_UDP.Start();
        }

        /// <summary>
        /// Метод полностью завершает выполнение потоков приема и записи данных
        /// </summary>
        public void Stop()
        {
            UDPhandler.UDPClose();
            thread_WR.Abort();
            thread_WR.Join();
            thread_UDP.Abort();
            thread_UDP.Join();
        }
    }
}
