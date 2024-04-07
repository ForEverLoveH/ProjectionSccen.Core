using System.Net.Sockets;
using Google.Protobuf;
using ProjectionSccen.Core.Summer.Messages;
using ProjectionSccen.Core.Summer.Protos;

namespace ProjectionSccen.Core.Summer ;

public class Connection
{
    /// <summary>
    /// 
    /// </summary>
    private Socket netSocket;
    /// <summary>
    /// 
    /// </summary>
    public Socket M_netSocket
    {
        get => netSocket;
    }
    private MessageRouter _messageRouter;
    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    public delegate void DisconnectConnectionCallBack(Connection sender);
    /// <summary>
    /// 
    /// </summary>
    public event DisconnectConnectionCallBack disconnectCallBack;
    /// <summary>
    /// 
    /// </summary>
    public delegate void DataRecieveCallBack(Connection sender, IMessage message);
    /// <summary>
    /// 
    /// </summary>
    public event DataRecieveCallBack dataRecieveCallBack;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public delegate void SendDataCallBack(string message);
    /// <summary>
    /// 
    /// </summary>
    public event SendDataCallBack sendDataCallBack;
    /// <summary>
    /// 
    /// </summary>
    public delegate void LogDataCallBack(string message);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="netSocket"></param>
    public Connection(Socket netSocket)
    {
        this.netSocket = netSocket;
        _messageRouter= MessageRouter.GetInstance();
     
        //创建解码器
        LengthFieldDecoder lfd = new LengthFieldDecoder(netSocket, 64 * 1024, 0, 4, 0, 4);
        lfd.DataReceived += HandleClientDataRecieved;     
        lfd.DisConnection += () => disconnectCallBack?.Invoke(this);
        lfd.Start();

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleClientDataRecieved(object sender, byte[] e)
    {
        var code = GetUshort(e, 0);
        var msg = ProtoExtension.ParseFrom(code, e, 2, e.Length - 2);
        if (msg != null)
        {
            if (_messageRouter.Running)
            {
                _messageRouter.AddMessageDataToQueue(this,msg);
            }
        }

    
    }
     /// <summary>
 ///
 /// </summary>
 /// <param name="data">大端数据</param>
 /// <param name="offset"></param>
 /// <returns></returns>
 public ushort GetUshort(byte[] data, int offset)
 {
     if (BitConverter.IsLittleEndian)
         return (ushort)((data[offset] << 8) | data[offset + 1]);
     else
         return (ushort)((data[offset + 1] << 8) | data[offset]);
 }
 /// <summary>
 /// 发送消息往客户端
 /// </summary>
 /// <param name="data"></param>
 /// <param name="offset"></param>
 /// <param name="length"></param>
 public void SocketSendDataToClient(byte[] data, int offset, int length)
 {
     lock (this)
     {
         if (netSocket.Connected)
         {
             netSocket.BeginSend(data, offset, data.Length, SocketFlags.None, new AsyncCallback(SendDataCallBacks), netSocket);
         }
     }
 }
 /// <summary>
 /// 
 /// </summary>
 /// <param name="message"></param>
 public void SendDataToClient(IMessage message)
 {
     using (var ds = DataStream.Allocate())
     {
         int code = ProtoExtension.SeqCode(message.GetType());
         ds.WriteInt(message.CalculateSize() + 2);
         ds.WriteUShort((ushort)code);
         message.WriteTo(ds);
         this.SendDataToClient(ds.ToArray());
     }
 }
 /// <summary>
 ///
 /// </summary>
 /// <param name="data"></param>
 /// <param name="offset"></param>
 /// <param name="length"></param>
 public void SendDataToClient(byte[] data, int offset, int length)
 {
     lock (this)
     {
         if (netSocket.Connected)
         {
             sendDataCallBack?.Invoke(string.Format("发送消息：len={0}", data.Length));
             byte[] buffer = new byte[4 + length];
             byte[] lenb = BitConverter.GetBytes(length);
             if (BitConverter.IsLittleEndian) Array.Reverse(lenb);
             Array.Copy(lenb, 0, buffer, 0, 4);
             Array.Copy(data, offset, buffer, 4, length);
             netSocket.BeginSend(buffer, offset, buffer.Length, SocketFlags.None, new AsyncCallback(SendDataCallBacks), netSocket);
         }
     }
 }

 /// <summary>
 ///
 /// </summary>
 /// <param name="data"></param>
 private void SendDataToClient(byte[] data)
 {
     this.SocketSendDataToClient(data, 0, data.Length);
 }

 /// <summary>
 /// 发送消息是否成功
 /// </summary>
 /// <param name="ar"></param>
 private void SendDataCallBacks(IAsyncResult ar)
 {
     Socket socket = ar.AsyncState as Socket;
     int len = netSocket.EndSend(ar);
 }
    /// <summary>
    /// 
    /// </summary>
 public void CloseNetConnection()
 {
     netSocket.Close();
    
 }
}