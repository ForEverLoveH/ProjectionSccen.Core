using ProjectionSccen.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProjectionSccen.Core.Manager
{
    public class NetControllerManager
    {
        /// <summary>
        ///
        /// </summary>
        private int port = 9996;

        /// <summary>
        ///
        /// </summary>
        private string IpAddress;

        /// <summary>
        ///
        /// </summary>
        private Socket mSocket;

        /// <summary>
        ///
        /// </summary>
        private bool isConnect;

        /// <summary>
        ///
        /// </summary>
        public bool IsConnection
        {
            get => isConnect; set => isConnect = value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        public delegate void EventDataCallBack(string data);

        /// <summary>
        ///
        /// </summary>
        public event EventDataCallBack DataCallBack;

        /// <summary>
        ///
        /// </summary>
        /// <param name="data"></param>
        public delegate void HandleRecieveDataCallBack(string data);

        /// <summary>
        ///
        /// </summary>
        public event HandleRecieveDataCallBack _HandleRecieveDataCallBack;

        /// <summary>
        ///
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public NetControllerManager(string ipAddress, int port = 9996)
        {
            this.port = port;
            this.IpAddress = ipAddress;
        }

        /// <summary>
        /// 链接服务器
        /// </summary>
        public void ConnectionServer()
        {
            try
            {
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(IpAddress), port);
                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mSocket.Connect(iPEndPoint);
                isConnect = true;
                DataCallBack?.Invoke("链接到服务器");
            }
            catch (Exception ex)
            {
                return;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void RecieveDataFromServer()
        {
            if (isConnect)
            {
                while (true)
                {
                    string result = "";
                    int count = mSocket.Available;
                    if (count > 0)
                    {
                        byte[] buffer = new byte[count];
                        mSocket.Receive(buffer);

                        string res = Encoding.UTF8.GetString(buffer);
                        int len = res.Length;
                        if (res.Length > 0)
                        {
                            if (res.Contains('#'))
                            {
                                var data = res.Split('#');
                                if (data.Length > 0)
                                {
                                    
                                    for (int i = 0; i < data.Length - 1; i++)
                                    {
                                        var ds = data[i].Split(' ');
                                        if (ds.Length > 0)
                                        {
                                            result = ds[0];
                                        }
                                        if (result.Length > 0)
                                        {

                                            DataCallBack?.Invoke($"接收到服务端数据:{result}" +
                                                                      DateTime.Now.ToString("yyyy-mmddMM hh:ss"));
                                            _HandleRecieveDataCallBack?.Invoke(result);


                                            
                                        }
                                    }
                                }
                            }
                        };
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void SendDataToServer(ClientMessage message)
        {
            if (isConnect)
            {
                string data = JsonDataManager.Instance.SerializeObject<ClientMessage>(message);
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                mSocket.Send(buffer, buffer.Length, 0);
            }
        }
    }
}