using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServidorChat
{
    public class Program
    {
        public static int contagem = 0;
        private static byte[] _buffer = new byte[1024];
        private static List<Socket> _clientSockets = new List<Socket>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            Console.Title = "Servidor";
            StartListening();
            Console.ReadLine();
        }
    
        public static void StartListening()
        {

            IPAddress enderecoIP = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(enderecoIP, 11000);


            _serverSocket.Bind(localEndPoint);
            _serverSocket.Listen(10);

            Console.WriteLine("Esperando por conexão de clientes ... ");
            _serverSocket.BeginAccept(new AsyncCallback(AccepCallback), null);
        }

        private static void AccepCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _clientSockets.Add(socket);
            contagem++;
            Console.WriteLine(contagem + " - Cliente Conectado(s)");
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AccepCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            int received = socket.EndReceive(ar);
            byte[] dataBuf = new byte[received];

            Array.Copy(_buffer, dataBuf, received);

            string text = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("text received: " + text);

            

        }

        private static void SendCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        public void User(Socket client)
        {
            while (true)
            {
                byte[] msg = new byte[1024];
                int size = client.Receive(msg);
                client.Send(msg, 0, size, SocketFlags.None);
            }
        }
    }
}
