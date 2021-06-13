using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LookSin
{
    class UDHHandler
    {
        private bool FlagStorResive = false;

        /// <summary>
        /// Делегат для события возвращаюего полученные данные из UDP
        /// </summary>
        /// <param name="vs"></param>
        public delegate void UDHHandDelegat(byte[] vs);

        /// <summary>
        /// Делегат для создания события, которое возвращает исключения
        /// </summary>
        public event UDHHandDelegat DataRecive;


        /// <summary>
        /// Событие возвращающее исключение
        /// </summary>
        /// <param name="str"></param>
        public delegate void UDPHandlerExeption(string str);

        /// <summary>
        /// Событие возвращающее принятые данные
        /// </summary>
        public event UDPHandlerExeption Exep;

        
        /// <summary>
        /// Номер порта прослушивания
        /// </summary>
        public UInt16 NumPort { get; private set; }
        

        /// <summary>
        /// ip адрес отправителя
        /// </summary>
        public IPAddress iPAddres { get; private set; }


        private UdpClient udpClient;


        private IPEndPoint RemoteIpEndPoint = null;


        /// <summary>
        /// Путь к файлу с катологом
        /// </summary>
        public string path_to_catolog_for_file { get; private set; }


        /// <summary>
        /// Начальное имя файл, изменяет в тот момент когда в предыдущем файле размер превысил 1 Гбайт
        /// </summary>
        public string Start_Name_file { get; private set; }



        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="NumPort">Принимает прослушиваемый порт</param>
        /// <param name="iPAddres">ip адрес отправителя</param>
        public UDHHandler(uint NumPort, IPAddress iPAddres, string path_to_catolog_for_file) 
        {
            this.iPAddres = iPAddres;
            this.NumPort = (ushort)NumPort;
            udpClient = new UdpClient((ushort)NumPort);
            RemoteIpEndPoint = new IPEndPoint(iPAddres, (ushort)NumPort);
            this.path_to_catolog_for_file = path_to_catolog_for_file;
        }

        
        /// <summary>
        /// Метод проверяющий, есть ли данная директория, если она существует, то там создается файл
        /// Если данной директории не существует, выдается исключение
        /// </summary>
        public void Creat_Dir_or_File_in_Dir()
        {
            if(path_to_catolog_for_file != null)
            {
                //Создаем экземпляр класса для проверки наличия данной директории
                System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(path_to_catolog_for_file);

                //Проверяем существует ли такая директория 
                //Если нет, то создаем ее, если она существует создаем файл согодняшних измерений
                if (!dirInfo.Exists)
                {
                    //TODO:НЕобходимо создать событие, говорящее о том, что директория не существует
                    Console.WriteLine("Директория не существует, и будет создана");
                    try
                    {
                        //Создание директории
                        dirInfo.Create();
                        //TODO:НЕобходимо создать событие, говорящее о том, что директория слздана
                        Console.WriteLine("Директория cоздана");
                    }
                    catch (Exception e)
                    {
                        //TODO:НЕобходимо создать событие, говорящее о том, что позникло исключение
                        Console.WriteLine($"Возникло исключение вида {e}");
                    }
                }

                ///Создаем начальное имя файла из даты, времени и года плюс порт просушиввания
                ///Имя файла будет изменяться со врменем когда размер файла будет превышать 1Гбайт, будет 
                ///создаваться новый файл с дополнительным префиксом _1,_2...

                string dateTime = DateTime.Now.ToString();
                dateTime = dateTime.Replace(":", ".");
                Start_Name_file = dateTime + $" Порт прослушивания {NumPort}";

                //Создаем папку в директории для записи измерений производимых сегодня.
                try
                {
                    using (File.Create($@"{path_to_catolog_for_file}\{Start_Name_file}.txt")) { }
                    //TODO:НЕобходимо создать событие, говорящее о том, что файл создан
                    Console.WriteLine("Файл создан");
                    //Message?.Invoke("Файл создан");
                }
                catch (Exception e)
                {
                    //TODO:НЕобходимо создать событие, говорящее о том, что возникл исключение
                    Console.WriteLine($"Возникло исключение вида {e}");
                    //Exep_?.Invoke($"Возникло исключение вида {e}"); 
                }
            }
            
        }







        public void UDPRecive() 
        {
            try
            {
                while (FlagStorResive == false)
                {
                    byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    Console.WriteLine(receiveBytes);
                    WriterInFILE(receiveBytes);
                    //Console.WriteLine(receiveBytes.ToString());
                    //DataRecive?.Invoke(receiveBytes);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Произошло исключение в блоке UDPRecive, вида {e} ");
                //Exep?.Invoke($"Произошло исключение в блоке UDPRecive, вида {e} \r\n" +
                //    $"----> Метод приема закрыт, для возобновления необходимо вызвать заново метод UDPRecive();");
                return;
            }
        }

        /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Эксперемент
        /// 

        /// <summary>
        /// Счетчик файлов 
        /// </summary>
        private static int count;
        public static int Count_file { get { return count; } private set { count = 1; } }
        public void WriterInFILE(byte[] vs)
        {
            try
            {
                //Проверяем длину файла
                //Запись в файл будем осуществлять по 1 Гбайту
                FileInfo fileInfo = new FileInfo($@"{path_to_catolog_for_file}\{Start_Name_file}.txt");
                if (fileInfo.Length < 30000)
                {
                    //1073741824
                    //Если файл размером меньше чем 1 Гбайт, то записываем данные
                    using (FileStream fileStream = new FileStream($@"{path_to_catolog_for_file}\{Start_Name_file}.txt", FileMode.Append))
                    {
                        fileStream.Write(vs);
                    }
                }
                else
                {
                    count++;
                    //Если длина файла превысила размер 1 Гбайт, то создаем новый файл с префиксом 2
                    //Переписываем переменную Start_Name_file и начинаем запись 
                    string tresh = Start_Name_file + count;
                    using (File.Create($@"{path_to_catolog_for_file}\{Start_Name_file}_{count}.txt")) { }
                        Start_Name_file = @$"{Start_Name_file}_{count}";
                    using (FileStream fileStream = new FileStream($@"{path_to_catolog_for_file}\{Start_Name_file}.txt", FileMode.Append))
                    {
                        fileStream.Write(vs);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"В потоке {Thread.CurrentThread.Name} возникло исключение вида {e}\r\n");
                //Exep_?.Invoke($"Возникло исключение вида {e}\r\n" +
                //$"Возможные ошибки:\r\n" +
                //$"-Файл был удален\r\n" +
                //$"-Запись в файл невозможна\r\n" +
                //$"-Возникло исключение при создании файла"); 
            }

        }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public void UDPClose() 
        {
            FlagStorResive = true;
            udpClient.Close();
        }




    }
}
