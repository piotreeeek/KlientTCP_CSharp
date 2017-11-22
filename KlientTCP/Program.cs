using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        Console.WriteLine("wysano do serwera: " + sendData);
                    }
                    if (networkStream.CanRead)
                    {
                        if (sendData.Split(' ').First().ToUpper() == "GET")
                        {
                            byte[] bytes = new byte[1024];

                            int length = networkStream.Read(bytes, 0, bytes.Length);

                            string returnData = Encoding.UTF8.GetString(new List<byte>(bytes).GetRange(0, length).ToArray());
                            if (returnData.ToUpper() == "NO_FILE")
                            {
                                Console.WriteLine("odebrano od serwera: Brak takiego pliku.");
                            }
                            else
                            {
                                string[] splitted = returnData.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                                Dictionary<string, string> headers = new Dictionary<string, string>();
                                foreach (string str in splitted)
                                {
                                    if (str.Contains(":"))
                                    {
                                        headers.Add(str.Substring(0, str.IndexOf(":")), str.Substring(str.IndexOf(":") + 1));
                                    }

                                }
                                //Get filesize from header
                                int filesize = Convert.ToInt32(headers["Content-length"]);
                                //Get filename from header
                                string filename = headers["Filename"];
                                int bufferCount = Convert.ToInt32(Math.Ceiling((double)filesize / (double)1024));
                                FileStream fs = new FileStream("D:\\politechnika\\semestr_7\\sieciowe\\przykladowe\\klient\\" + filename, FileMode.OpenOrCreate);

                                while (filesize > 0)
                                {
                                    byte[] buffer = new byte[1024];

                                    int size = networkStream.Read(buffer, 0, 1024);

                                    fs.Write(buffer, 0, size);

                                    filesize -= size;
                                }


                                fs.Close();
                            }
                        }
                        else
                        {
                            byte[] bytes = new byte[1024];

                            int length = networkStream.Read(bytes, 0, bytes.Length);

                            string returnData = Encoding.UTF8.GetString(new List<byte>(bytes).GetRange(0, length).ToArray());
                            Console.WriteLine("odebrano od serwera: " + returnData);
                        }
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
