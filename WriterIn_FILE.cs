using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Recive_data_from_ADC
{
    /// <summary>
    /// Класс для записи данных в бинарный файл 
    /// </summary>
    public class WriterIn_FILE
    {
        /// <summary>
        /// Делегат для события которое говорит об возникших проблемах или исключении
        /// </summary>
        /// <param name="str"></param>
        public delegate void DelegateForString(string str);

        /// <summary>
        /// Событие возвращающее сообщение
        /// </summary>
        public event DelegateForString Message;

        /// <summary>
        /// Событие возвращающее возникшее исклчение
        /// </summary>
        public static event DelegateForString Exep_;



        /// <summary>
        /// Начальное имя файл, изменяет в тот момент когда в предыдущем файле размер превысил 1 Гбайт
        /// </summary>
        public string Start_Name_file { get; private set; }

        /// <summary>
        /// Приемный порт, в данном классе выступает в роле идентефикатора для файла, в который пишутся данные с определенного порта
        /// По желанию можно любое число от 0 до 4294967295
        /// </summary>
        public uint receiving_port { get; private set; }
        
        /// <summary>
        /// Путь к катологу где лежит файл
        /// </summary>
        public string path_to_catolog_for_file { get; private set; }




        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="receiving_port">Приемный порт, в данном классе выступает в роле идентефикатора для файла, в который пишутся данные с определенного порта
        /// По желанию можно любое число от 0 до 4294967295</param>
        /// <param name="path_to_catolog_for_file">Путь к катологу где лежит файл</param>
        public WriterIn_FILE(uint receiving_port, string path_to_catolog_for_file)
        {
            //Создаем экземпляр класса для проверки наличия данной директории
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(path_to_catolog_for_file);
            
            //Проверяем существует ли такая директория 
            //Если нет, то создаем ее, если она существует создаем файл согодняшних измерений
            if (!dirInfo.Exists)
            {
                Console.WriteLine("Директория не существует, и будет создана");
                //Message?.Invoke("Директория не существует, и будет создана");
                try
                {
                    dirInfo.Create();
                    Console.WriteLine("Директория cоздана");
                    //Message?.Invoke("Директория cоздана");
                }
                catch (Exception e) 
                {
                    Console.WriteLine($"Возникло исключение вида {e}");
                    //Exep_?.Invoke($"Возникло исключение вида {e}"); 
                }
            }

            ///Создаем начальное имя файла из даты, времени и года плюс порт просушиввания
            ///Имя файла будет изменяться со врменем когда размер файла будет превышать 1Гбайт, будет 
            ///создаваться новый файл с дополнительным префиксом _1,_2...

            string dateTime = DateTime.Now.ToString();
            dateTime = dateTime.Replace(":", ".");
            Start_Name_file = dateTime + $" Порт прослушивания {receiving_port}.txt";

            this.receiving_port = receiving_port;

            this.path_to_catolog_for_file = path_to_catolog_for_file;

            //Создаем папку в директории для записи измерений производимых сегодня.
            try
            {
                using (File.Create($@"{path_to_catolog_for_file}\{Start_Name_file}")) { }
                Console.WriteLine("Файл создан");
                //Message?.Invoke("Файл создан");
            }
            catch(Exception e) 
            {
                Console.WriteLine($"Возникло исключение вида {e}");
                //Exep_?.Invoke($"Возникло исключение вида {e}"); 
            }

        }

        /// <summary>
        /// Счетчик файлов 
        /// </summary>
        private static int count;
        public static int Count_file { get { return count; } private set { count = 1; } }


        /// <summary>
        /// Метод для записи данных в файл, также производит проверку файла и если рамер файла превышает 1гбайт, создает новый файл с префиксом _2
        /// </summary>
        /// <param name="date">Данные, типо object был выдран для того, чтобы можно было передавать данные в поток</param>
        public void WriterInFILE(object date)
        {
            byte[] vs = (byte[])date;
            try
            {
                //Проверяем длину файла
                //Запись в файл будем осуществлять по 1 Гбайту
                FileInfo fileInfo = new FileInfo($@"{path_to_catolog_for_file}\{Start_Name_file}");
                if(fileInfo.Length != 1073741824) 
                {
                    //Если файл размером меньше чем 1 Гбайт, то записываем данные
                    using(FileStream fileStream = new FileStream($@"{path_to_catolog_for_file}\{Start_Name_file}", FileMode.OpenOrCreate))
                    {
                        fileStream.Write(vs);
                    }
                }
                else
                {
                    count++;
                    //Если длина файла превысила размер 1 Гбайт, то создаем новый файл с префиксом 2
                    //Переписываем переменную Start_Name_file и начинаем запись 
                    File.Create($@"{path_to_catolog_for_file}\{Start_Name_file}_{count}");
                    Start_Name_file = @$"{Start_Name_file}_{count}";
                    using (FileStream fileStream = new FileStream($@"{path_to_catolog_for_file}\{Start_Name_file}", FileMode.OpenOrCreate))
                    {
                        fileStream.Write(vs);
                    }
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine($"Возникло исключение вида {e}\r\n");
                //Exep_?.Invoke($"Возникло исключение вида {e}\r\n" +
                //$"Возможные ошибки:\r\n" +
                //$"-Файл был удален\r\n" +
                //$"-Запись в файл невозможна\r\n" +
                //$"-Возникло исключение при создании файла"); 
            }

        }
    }
}
