using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace vayfrem.services
{
    public class MultipleUserService
    {
        public bool IsHost { get; set; } = true;
        public bool IsActive { get; set; } = false;

        TcpListener? listener;
        TcpClient? client;

        // temp password
        public string Password { get; set; } = "123456";

        public string? Host { get; set; }


        // makes available for multiple usage
        public void Activate()
        {
            IsActive = true;

            int port = 9090;

            IPAddress localIpAddress = IPAddress.Parse("192.168.56.1");
            listener = new TcpListener(localIpAddress, port);
            listener.Start();

            Debug.WriteLine("Listening");

            Task.Run(() =>
            {
                Debug.WriteLine($"Listening {localIpAddress}");
                Host = localIpAddress.ToString();
                Listen();
            });
        }

        private void Listen()
        {
            while (true)
            {
                Debug.WriteLine("Listening");

                TcpClient client = listener!.AcceptTcpClient();

                NetworkStream stream = client.GetStream();

                byte[] buffer = new byte[256];

                int bytes = stream.Read(buffer, 0, buffer.Length);

                string receivedPassword = Encoding.ASCII.GetString(buffer, 0, bytes);

                if(true)
                {
                    Debug.WriteLine("Connection is succeed...");

                    while (true)
                    {
                        Debug.WriteLine("Waiting...");

                        Thread.Sleep(2000);
                    }
                }
                else
                {
                    Debug.WriteLine("Password is invalid!");

                    stream.Close();
                    client.Close();
                }
            }
        }

        public void Stop()
        {
            if (listener == null) return;
            IsActive = false;
            listener!.Stop();
        }

        public void Connect(string ip, int port)
        {
            IsHost = false;
            IsActive = true;

            client = new TcpClient(ip, port);

            Task.Run(() =>
            {
                try
                {
                    ListenHost();
                    Send("Connected");
                }
                catch (Exception ex) 
                {
                    Debug.WriteLine(ex.Message);   
                }
                finally
                {
                    if (client != null) client.Close();
                }
            });
        }

        public void Send(string message)
        {
            if (client == null || !client.Connected)
            {
                Debug.WriteLine("No connection");
                return;
            }

            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.WriteLine("Sent Message: " + message);
        }

        private void ListenHost()
        {
            if(client!.Connected)
            {
                NetworkStream stream = client.GetStream();
                byte[] data = new byte[256];

                while (true)
                {
                    int bytes = stream.Read(data, 0, data.Length);
                    string responseData = Encoding.ASCII.GetString(data, 0, bytes);
                    Debug.WriteLine("Response from server:" + responseData);
                }
            }
            else
            {
                Debug.WriteLine("Connection is failed");
            }
        }

        public void Disconnect()
        {
            client!.Close();
            client.Dispose();

            Debug.WriteLine("Connection is closed");
        }
    }
}
