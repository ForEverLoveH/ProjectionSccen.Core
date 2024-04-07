using System.Reflection;
using Google.Protobuf;

namespace ProjectionSccen.Core.Summer.Messages;

public class MessageRouter : Singleton<MessageRouter>
{
    private int threadCount = 1;
  
    private int workCount = 0;//当前线程数
    /// <summary>
    /// 
    /// </summary>
    private AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
    /// <summary>
    /// 
    /// </summary>
    private Queue<ClientMessageData> _messageDatas = new Queue<ClientMessageData>();
    /// <summary>
    /// 消息处理器
    /// </summary>
    /// <param name="netConnection"></param>
    /// <param name="messageData"></param>
    public delegate void MessageHandler<T>(Connection netConnection, T messageData);

    /// <summary>
    /// 频道字典(订阅记录)
    /// </summary>
    private Dictionary<string, Delegate> Handler = new Dictionary<string, Delegate>();
    

    /// <summary>
    /// 将消息传入到消息对列中
    /// </summary>
    /// <param name="netConnection"></param>
    /// <param name="messageData"></param>
    public void AddMessageDataToQueue(Connection netConnection, IMessage message)
    {
        lock (_messageDatas)
        {
            _messageDatas.Enqueue(new ClientMessageData()
            {
                connetion = netConnection,
                message = message
            });
        }
        if (_messageDatas.Count > 0)
        {
            _autoResetEvent.Set();//唤醒1等待线程
        }
    }
    public void OnMessage<T>(MessageHandler<T> handler) where T : IMessage
    {
        try
        {
            string key = typeof(T).FullName;
            if (!Handler.ContainsKey(key))
            {
                Handler[key] = null;
            }
            Handler[key] = (Handler[key] as MessageHandler<T>) + handler;
            Console.Write(Handler[key].GetInvocationList().Length);
        }
        catch (Exception ex)
        {
         
            return;
        }
    }

    

    #region 退订

    public void OffMessage<T>(MessageHandler<T> handler) where T : IMessage
    {
        try
        {
            string key = typeof(T).FullName;
            if (!Handler.ContainsKey(key))
            {
                Handler[key] = null;
            }
            Handler[key] = Handler[key] as MessageHandler<T> - handler;
        }
        catch (Exception ex)
        {
         
            return;
        }
    }

    #endregion 退订
    
    /// <summary>
    /// 消息分发器是否在运行
    /// </summary>
    private bool _isRunning = false;
    /// <summary>
    /// 
    /// </summary>
    public bool Running
    { get { return _isRunning; } }
    /// <summary>
    ///线程池
    /// </summary>
    /// <param name="threadCount"></param>
    public void Start(int threadCount)
    {
        if (_isRunning) return;
        _isRunning = true;
        this.threadCount = threadCount;
        this.threadCount = Math.Min(Math.Max(threadCount, 1), 200);
        for (int i = 0; i < this.threadCount; i++)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(MessageWork));
        }
        while (workCount < this.threadCount)
        {
            Thread.Sleep(100);
        }
    }

    // a =>C  b=>D   C D

    /// <summary>
    /// 线程工作 线程死锁
    /// </summary>
    /// <param name="state"></param>
    private void MessageWork(object state)
    {
        try
        {
            workCount = Interlocked.Increment(ref workCount);
            ///一直处理
            while (_isRunning)
            {
                if (_messageDatas.Count > 0)
                {
                    ClientMessageData messageData;
                    lock (_messageDatas)
                    {
                        //存在可能有多个线程，但是只有一个消息
                        if (_messageDatas.Count == 0) continue;
                        messageData = _messageDatas.Dequeue();
                    }

                    if (messageData != null)
                    {
                        var packMessage = messageData.message;
                        if (packMessage != null)
                        {
                            ExcuteLoopMessage(packMessage, messageData.connetion);
                        }
                    }
                }
                else
                {
                    _autoResetEvent.WaitOne(); ///休眠等待，可使用set唤醒 存在前一个线程已经拿走了你当前的消息数据(也就是当前线程要俩消息数）
                    continue;
                }
            }
        }
        
        finally
        {
            workCount = Interlocked.Decrement(ref workCount);
        }
    }
    /// <summary>
    /// 递归处理消息(分发)
    /// </summary>
    /// <param name="message"></param>
    public void ExcuteLoopMessage(IMessage message, Connection connection)
    {
        //触发订阅
        var fireMethod = this.GetType().GetMethod("FireMessageData", BindingFlags.NonPublic | BindingFlags.Instance);
        var met = fireMethod.MakeGenericMethod(message.GetType());
        met.Invoke(this, new object[] { connection, message });
        var t = message.GetType();
        foreach (var p in t.GetProperties())
        {
            // Log.Information($"{p.Name}");
            if (p.Name == "Parser" || p.Name == "Descriptor")
                continue;
            //只要发现消息就可以订阅 递归思路实现
            var value = p.GetValue(message);
            if (value != null)
            {
                //发现消息是否需要进一步递归 触发订阅
                if (typeof(IMessage).IsAssignableFrom(value.GetType()))
                {
                    //发现消息是否需要进一步递归 触发订阅
                    //继续递归
                    ExcuteLoopMessage((IMessage)value, connection);
                }
            }
        }
        
    }
    /// <summary>
    /// 触发消息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="netConnection"></param>
    /// <param name="messageData"></param>
    private void FireMessageData<T>(Connection netConnection, T messageData)
    {
        string type = typeof(T).FullName;
        if (Handler.ContainsKey(type))
        {
            MessageHandler<T> handler = (MessageHandler<T>)Handler[type];
            try
            {
                handler?.Invoke(netConnection, messageData);
            }
            catch (Exception ex)
            {
             
                //打印错误日志
                 Console.WriteLine("messageRouter is error" + ex.Message);
            }
        }
    }
    /// <summary>
    /// 关闭消息分发器
    /// </summary>
    public void Stop()
    {
        _isRunning = false;
       _messageDatas.Clear();
        while (workCount > 0)
        {
            _autoResetEvent.Set();
        }
        Thread.Sleep(50);//考虑多线程，数据不一定同步
    }

} 