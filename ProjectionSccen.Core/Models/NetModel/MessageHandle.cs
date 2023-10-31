using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ProjectionSccen.Core.Manager.NetControllerManager;

namespace ProjectionSccen.Core.Models.NetModel
{
    public class MessageHandle : Singleton<MessageHandle>
    {

        /// <summary>
        /// 协调多个线程的通信
        /// </summary>
        private AutoResetEvent AutoResetEvent = new AutoResetEvent(true);

        bool _isRunning = false;

        /// <summary>
        /// 
        /// </summary>
        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }
        /// <summary>
        /// 
        /// </summary>
        private int threadCount = 1;//默认工作线程数目
        /// <summary>
        /// 
        /// </summary>
        private int workCount = 0;//当前线程数

        private Queue<ClientMessage> messagesQueue = new Queue<ClientMessage>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public delegate void HandleRecieveDataCallBack(ClientMessage data);
        /// <summary>
        /// 
        /// </summary>
        public event HandleRecieveDataCallBack _HandleRecieveDataCallBack;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void AddDataToMessage(ClientMessage message)
        {

            lock (messagesQueue)
            {
                messagesQueue.Enqueue(message);

            }
            if (messagesQueue.Count > 0)
            {
                AutoResetEvent.Set();//唤醒1等待线程
            }
        }
        /// <summary>
        /// 
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void MessageWork(object? state)
        {
            try
            {
                workCount = Interlocked.Increment(ref workCount);
                ///一直处理
                while (_isRunning)
                {
                    if (messagesQueue.Count > 0)
                    {
                        ClientMessage messageData;
                        lock (messagesQueue)
                        {
                            //存在可能有多个线程，但是只有一个消息
                            if (messagesQueue.Count == 0) continue;
                            messageData = messagesQueue.Dequeue();
                        }
                        if (messageData != null)
                        {
                            _HandleRecieveDataCallBack?.Invoke(messageData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return;
            }
            finally
            {
                workCount = Interlocked.Decrement(ref workCount);
            }
        }
        /// <summary>
        /// 关闭消息分发器
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            messagesQueue.Clear();
            while (workCount > 0)
            {
                AutoResetEvent.Set();
            }
            Thread.Sleep(50);//考虑多线程，数据不一定同步
        }
    } 
}
