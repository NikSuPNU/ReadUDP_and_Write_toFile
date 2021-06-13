using System;
using System.Collections.Generic;
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
        /// Конструктор класса
        /// </summary>
        /// <param name="NumPort">Принимает прослушиваемый порт</param>
        /// <param name="iPAddres">ip адрес отправителя</param>
        public UDHHandler(uint NumPort, IPAddress iPAddres) 
        {
            this.iPAddres = iPAddres;
            this.NumPort = (ushort)NumPort;
            udpClient = new UdpClient((ushort)NumPort);
            RemoteIpEndPoint = new IPEndPoint(iPAddres, (ushort)NumPort);
        }



        public void UDPRecive() 
        {
            try
            {
                while (FlagStorResive == false)
                {
                    byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                    Console.WriteLine(receiveBytes.ToString());
                    DataRecive?.Invoke(receiveBytes);
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

        public void UDPClose() 
        {
            FlagStorResive = true;
            udpClient.Close();
        }




    }
}
