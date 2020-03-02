using System;
using System.Collections.Generic;
using System.Text;

namespace Reader
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    public class MsgBus<T>
    {
        private static MsgBus<T> _instance = null;
        private static readonly object _lock = new object();

        protected MsgBus()
        {
        }

        public static MsgBus<T> Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MsgBus<T>();
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
