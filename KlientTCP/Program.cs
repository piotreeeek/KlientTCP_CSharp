using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KlientTCP
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TcpClient tcpClient = new TcpClient();
            int port = 7;
            bool flag = true;
            while (flag)
            {
                try
                {
                    Console.WriteLine("Propszę podaj domenę lub adress ip");
                    string host = Console.ReadLine();
                    Console.WriteLine("Propszę podaj numer portu.");
                    int intTemp = Convert.ToInt32(Console.ReadLine());
                    port = intTemp;
                    tcpClient = new TcpClient(host, port);
                    Console.WriteLine("Udało się nawiązać połaczenie z serwerem");
                    flag = false;
                }
                catch (SocketException)
                {
                    Console.WriteLine("Niestety serwer nie odpowiada. Spróbuj jeszcze raz.");
                }
                catch (FormatException)
                {
                    Console.WriteLine("Podany numer portu nie jest prawidlowy.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Podany numer portu nie jest prawidlowy.");
                }
            }
            

            

            NetworkStream networkStream = tcpClient.GetStream();
            bool flag_2 = true;


            while (flag_2)
            {
                try
                {
                    Console.WriteLine("Podaj wiadomośc do wysłania:");
                    string sendData = Console.ReadLine();

                    if (networkStream.CanWrite)
                    {
                        Byte[] sendBytes = Encoding.UTF8.GetBytes(sendData);
                        tcpClient.ReceiveBufferSize = sendBytes.Length;
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        Console.WriteLine("wysano do serwera" + sendData);
                    }
                    if (networkStream.CanRead)
                    {
                        byte[] bytes = new byte[(int)tcpClient.ReceiveBufferSize];

                        networkStream.Read(bytes, 0, (int)tcpClient.ReceiveBufferSize);

                        string returnData = Encoding.UTF8.GetString(bytes);
                        Console.WriteLine("odebrano od serwera" + returnData);
                    }

                    Console.WriteLine("Jeśli chcesz znowu coś nadac wciśnij klawisz Y. W przeciwnym wypadku wciśnij dowolny klawisz");
                    string condition = Console.ReadLine();
                    if (condition.ToLower() != "y")
                    {
                        flag_2 = false;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Serwer nieodpowiada. Kończe aplikacje");
                    Thread.Sleep(3000);
                    flag_2 = false;
                }
            }
            networkStream.Close();
            tcpClient.Close();
        }
    }
}
