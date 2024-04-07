using System.Net;
using System.Net.Sockets;
using Google.Protobuf;

namespace ProjectionSccen.Core.Summer;

public class TcpServer
{
    /// <summary>
    /// 
    /// </summary>
    private IPEndPoint _endPoint;
    /// <summary>
    /// 
    /// </summary>
    private Socket serverSocket;
    /// <summary>
    /// 
    /// </summary>
    private int balckLog = 100;
    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    public delegate void NewConnectionedCallBack(Connection connection);
    /// <summary>
    /// 
    /// </summary>
    public event NewConnectionedCallBack _NewConnectionedCallBack;
    /// <summary>
    /// 
    /// </summary>
    public delegate void DisConnectionCallBack(Connection connection);
    /// <summary>
    /// 
    /// </summary>
    public event DisConnectionCallBack _disConnectionCallBack;
    /// <summary>
    /// 
    /// </summary>
    public delegate void LogDataCallBack(string message);
    /// <summary>
    /// 
    /// </summary>
    public event LogDataCallBack _logDataCallBack;
    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="data"></param>
    public delegate void DataReceviedCallback(Connection connection,  IMessage data);
    /// <summary>
    /// 
    /// </summary>
    public event DataReceviedCallback _DataReceviedCallback;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public delegate void SendDataCallBack(string message);
    /// <summary>
    /// 
    /// </summary>
    public event SendDataCallBack _sendDataCallBack;
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<Socket> _socketConnected;
    /// <summary>
    /// 
    /// </summary>
    public bool IsRunning
    {
        get => serverSocket == null;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipAddress"></param>
    /// <param name="port"></param>
    public TcpServer(string ipAddress, int port)
    {
        _endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipaddress"></param>
    /// <param name="port"></param>
    /// <param name="block"></param>
    public TcpServer(string ipaddress, int port, int block) : this(ipaddress, port)
    {
        this.balckLog = block;
    }
    /// <summary>
    /// 
    /// </summary>
    public void StartService()
    {
        if (!IsRunning)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(_endPoint);
            serverSocket.Listen(balckLog);
            _logDataCallBack?.Invoke("开始监听端口："+_endPoint.Port);
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += HandleConnection;
            serverSocket.AcceptAsync(args);

        }
        else
        {
            _logDataCallBack?.Invoke("tcp Service 已连接");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleConnection(object sender, SocketAsyncEventArgs e)
    {
        Socket client = e.AcceptSocket as Socket;
        e.AcceptSocket = null;
        serverSocket.AcceptAsync(e);
        if (e.SocketError == SocketError.Success)
        {
            if(client!=null)HandleConnection(client);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="socket"></param>
    private void HandleConnection(Socket socket)
    {
        _socketConnected?.Invoke(this, socket);
        Connection connection = new Connection(socket);
        connection.dataRecieveCallBack +=  (sender, message) => _DataReceviedCallback?.Invoke(sender, message); ;
        connection.disconnectCallBack +=   con => _disConnectionCallBack?.Invoke(con);
        connection.sendDataCallBack += message => _sendDataCallBack?.Invoke(message);
        _NewConnectionedCallBack?.Invoke(connection);
    }
    /// <summary>
    /// 
    /// </summary>
    public void StopService()
    {
        if(serverSocket==null)return;
        serverSocket.Close();
        serverSocket.Dispose();
    }     
}