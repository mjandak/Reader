using System;
using System.Collections.Generic;
using System.Text;

namespace Reader
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Type of message parameter.</typeparam>
    public class MessageBus<T>
    {
        private static MessageBus<T> _instance = null;
        private static readonly object _lock = new object();

        protected MessageBus()
        {
        }

        public static MessageBus<T> Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MessageBus<T>();
                    }
                    return _instance;
                }
            }
        }

        public event Action<T> MessageRecieved;

        public void SendMessage(T Message)
        {
            MessageRecieved?.Invoke(Message);
        }
    }
}
