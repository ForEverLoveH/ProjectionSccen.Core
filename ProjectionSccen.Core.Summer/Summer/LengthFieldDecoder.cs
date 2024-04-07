using System.Net.Sockets;

namespace ProjectionSccen.Core.Summer ;

public class LengthFieldDecoder
{
    public bool isStart = false;
    private Socket m_socket;

    /// <summary>
    /// 第几个是body消息长度 长度字段的位置下标
    /// </summary>
    private int lengthFieldOffset = 8;

    /// <summary>
    /// 长度字段所占字节数 低端本身长度
    /// </summary>
    private int lengthFieldLength = 4;

    /// <summary>
    /// 长度字段和内容之间距离多少字节 偏移位
    /// 负数表示往前偏移 body长度需要减去这个绝对值
    /// </summary>
    private int lengthAdjustment = 0;

    /// <summary>
    /// 结果数据多少不需要的字节数目
    /// 得到了整个数据包之后，舍弃的前面的内容（几个字节)
    /// </summary>
    private int initialBytesToStrip = 0;

    /// <summary>
    /// 缓冲区间
    /// </summary>
    private byte[] mBuffer;

    /// <summary>
    /// 读取位置
    /// </summary>
    private int moffset = 0;

    /// <summary>
    /// 一次性接收数据的最大字节数 默认64k
    /// </summary>
    private int mSize = 64 * 1024;

    /// <summary>
    /// 接收数据成功后的委托
    /// </summary>
    public event EventHandler<byte[]> DataReceived;
    /// <summary>
    /// 连接断开的委托
    /// </summary>
    public delegate void DisConnectionEventHandle();
    /// <summary>
    ///
    /// </summary>
    public event DisConnectionEventHandle DisConnection;
    public delegate void LogDataCallBack(string message);
    public event LogDataCallBack logDataCallBack;

    /// <summary>
    ///
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="lenFieldOffer"></param>
    /// <param name="lenfieldLen"></param>
    public LengthFieldDecoder(Socket socket, int lenFieldOffset, int lenfieldLen)
    {
        m_socket = socket;
        this.lengthFieldOffset = lenFieldOffset;
        this.lengthFieldLength = lenfieldLen;
        mBuffer = new byte[mSize];
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="socket"></param>
    /// <param name="maxBufferLen"></param>
    /// <param name="lengthOffset"></param>
    /// <param name="lenFieldLen"></param>
    /// <param name="lenthAdjustMent"></param>
    /// <param name="initalBytesToStrip">最后结果</param>
    public LengthFieldDecoder(Socket socket, int maxBufferLen, int lengthOffset, int lenFieldLen, int lenthAdjustMent, int initalBytesToStrip)
    {
        m_socket = socket;
        mSize = maxBufferLen;
        this.lengthFieldOffset = lengthOffset;
        this.lengthFieldLength = lenFieldLen;
        this.lengthAdjustment = lenthAdjustMent;
        this.initialBytesToStrip = initalBytesToStrip;
        mBuffer = new byte[mSize];
    }

    /// <summary>
    /// 启动解码器
    /// </summary>
    public void Start()
    {
        if (!isStart && m_socket != null)
        {
            isStart = true;
            BeginAsyncReceiveData();
        }
    }

    /// <summary>
    /// 开始异步接收
    /// </summary>
    public void BeginAsyncReceiveData()
    {
        m_socket.BeginReceive(mBuffer, moffset, mSize - moffset, SocketFlags.None, new AsyncCallback(RecieveData), null);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="ar"></param>
    private void RecieveData(IAsyncResult ar)
    {
        try
        {
            int res = m_socket.EndReceive(ar);
            if (res == 0)
            {
                if (DisConnection != null)
                {
                    DisConnection();
                }
                return;
            }
            HandleRecieveData(res);
            BeginAsyncReceiveData();
        }
        catch (SocketException ex)
        {
            logDataCallBack?.Invoke(ex.Message);
            
            DoDisConnection();
        }
        catch (ObjectDisposedException)
        {
            DoDisConnection();
        }
    }

    /// <summary>
    ///
    /// </summary>
    private void DoDisConnection()
    {
        try
        {
            DisConnection?.Invoke();
            m_socket?.Shutdown(SocketShutdown.Both);
        }
        catch { }
        m_socket?.Close();
        m_socket?.Dispose();
        m_socket = null;
    }

    /// <summary>
    /// 拆包
    /// </summary>
    /// <param name="res"></param>
    /// <exception cref="Exception"></exception>
    private void HandleRecieveData(int res)
    {
        int headlen = lengthFieldOffset + lengthFieldLength;
        int adj = lengthAdjustment;
        int size = res;
        if (moffset > 0)
        {
            size += moffset;
            moffset = 0;
        } while (true)
        {
            //剩下的长度
            int remain = size - moffset;
            if (remain > mSize)
                throw new Exception();
            if (remain < headlen)
            {
                ///接收不够一个数据包时，继续接收
                Array.Copy(mBuffer, moffset, mBuffer, 0, remain);
                moffset = remain;
                return;
            }
            ///包长度
            // int po = BitConverter.ToInt32(mBuffer,moffset +lengthFieldOffset);// 小端
            int po = GetInt32EE(mBuffer, moffset + lengthFieldOffset);

            if (remain < headlen + adj + po)
            {
                Array.Copy(mBuffer, moffset, mBuffer, 0, remain);
                moffset = remain;
                return;
            }
            ///BODY 的读取位置
            int bodyStart = moffset + Math.Max(headlen, headlen + adj);
            //包的真正的长度
            int bodyCount = Math.Min(headlen, bodyStart + adj);
            //获取包
            int total = headlen + adj + po; //包的长度
            int count = total - initialBytesToStrip;
            byte[] data = new byte[count];
            Array.Copy(mBuffer, moffset + initialBytesToStrip, data, 0, count);
            moffset += total;
            
            DataReceived?.Invoke(this, data);
        }
    }

    /// <summary>
    /// 获取大端模式字节代码
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private int GetInt32EE(Byte[] data, int index)
    {
        return (data[index] << 0x18) | (data[index + 1] << 0x10) | (data[index + 2] << 8) | (data[index + 3]);
    }
}