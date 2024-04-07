using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ProjectionSccen.Core.Models;
using ProjectionSccen.Core.Summer;
using ProjectionSccen.Core.Summer.Messages;

namespace ProjectionSccen.Core.Service.NetService;
/// <summary>
/// 
/// </summary>
public class NetService
{
    /// <summary>
    /// 
    /// </summary>
    private Connection _connection;
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
    /// <param name="num"></param>
    public delegate void HandleExamNumberCallBack(int num);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="studentDatas"></param>
    public delegate  void HandleCurrentStudentData(List<StudentData> studentData,string examTime);
    /// <summary>
    /// 
    /// </summary>
    public event HandleCurrentStudentData _HandleCurrentStudentData;
    /// <summary>
    /// 
    /// </summary>
    public event HandleExamNumberCallBack _HandleExamNumber;
    
    /// <summary>
    /// 
    /// </summary>
    private bool isConnection = false;
    /// <summary>
    /// 
    /// </summary>
    private int lastMachineNum = 0;
    /// <summary>
    /// 
    /// </summary>
    private int currentMachineNumber = 0;
    /// <summary>
    /// 
    /// </summary>
    public bool IsConnection
    {
        get => isConnection;
    }
    /// <summary>
    /// 
    /// </summary>
    private MessageRouter _messageRouter;
    /// <summary>
    /// 
    /// </summary>
    private string ipaddress;
    /// <summary>
    /// 
    /// </summary>
    private int port;
    /// <summary>
    /// 
    /// </summary>
    private Socket clientSocket;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ipaddress"></param>
    /// <param name="port"></param>
    public NetService(string ipaddress, int port)
    {
        this.ipaddress = ipaddress;
        this.port = port;
         this. _messageRouter = MessageRouter.GetInstance();
    }
    /// <summary>
    /// 
    /// </summary>
    public void StartNetService()
    {
        try
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipaddress), port);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipEndPoint);
            _logDataCallBack?.Invoke("连接到服务器");
            _connection = new Connection(clientSocket);
            _connection.disconnectCallBack += DisConnectServer;
            _messageRouter.Start(4);
            isConnection=true;
        }
        catch (Exception e)
        {
            _logDataCallBack?.Invoke(e.Message);
            return;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void ListenMessage( )
    {
        _messageRouter.OnMessage<RspExamNumber>(_RspExamNumber);
        _messageRouter.OnMessage<RspExamMessage>(_RspExamMessage);
     
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="netConnection"></param>
    /// <param name="messageData"></param>
    private void _RspExamMessage(Connection netConnection, RspExamMessage messageData)
    {
        List<StudentData>studentDatas= messageData.Students.ToList();
        string examTime= messageData.ExamTime;  
        _HandleCurrentStudentData?.Invoke(studentDatas,examTime);
       
    }
   

    /// <summary>
    /// 
    /// </summary>
    /// <param name="netConnection"></param>
    /// <param name="messageData"></param>
    private void _RspExamNumber(Connection netConnection, RspExamNumber messageData)
    {
        int num = messageData.Num;
        if(num > 0) _HandleExamNumber?.Invoke(num);
        else
        {
            return;
        }
        
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    private   void DisConnectServer(Connection sender)
    {
        _logDataCallBack?.Invoke("与服务器失去连接");
        isConnection = true;
    }
    /// <summary>
    /// 
    /// </summary>
    public void CloseNetService()
    {
        _connection?.CloseNetConnection();
        if (_messageRouter.Running)
        {
            _messageRouter.Stop();
        }
    }
}