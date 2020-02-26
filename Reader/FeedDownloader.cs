using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Reader
{
	public class FeedDownloader
    {
        const int _bufferSize = 4 * 1024;
		DAL.Feed _feed;
        WebResponse _resp;
        Stream _stream;
        byte[] _buffer = new byte[_bufferSize];

        private MemoryStream _receivedData = new MemoryStream();
		internal MemoryStream ReceivedData
		{
			get { return _receivedData; }
			private set { _receivedData = value; }
		}

        int _numberOfBytesRead = 0;

		/// <summary>
		/// Gets called when feed's xml has been downloaded, with feed's items rebuild.
		/// </summary>
        public event Finished DownloadFinished;

        /// <summary>
        /// Create instance of FeedDownloader.
        /// </summary>
        /// <param name="feed">Feed which will be filled with downloaded items.</param>
		public FeedDownloader(DAL.Feed feed)
		{
			_feed = feed;
		}

		/// <summary>
		/// Make web request and start reading data asynchronously from response stream.
        /// There is a little chance that this method blocks until all data is read.
		/// </summary>
        public void Begin()
        {
            try
            {
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_feed.Url);
				request.Method = "GET";
				request.Proxy = null;
				_resp = request.GetResponse();
                _stream = _resp.GetResponseStream();
                Read();
            }
            catch (Exception ex)
			{
				ProcessException(ex);
			}
        }

		/// <summary>
		/// Read next chunk of incoming data from stream asynchronously.
        /// There is a little chance that this method blocks until all data is read.
		/// </summary>
        private void Read()
        {
            while (true)
            {
                IAsyncResult r = _stream.BeginRead(_buffer, 0, _buffer.Length, ReadCallback, null);

                // BeginXXX can finish the entire task synchronously — providing it never blocked and never caused another thread to do the same.
                if (!r.CompletedSynchronously)
                    // This will nearly always return here. Read data will be handled by callback.
                    return;
                //Completed synchrously
                if (!SaveReceivedData(r)) break;
            }
            _receivedData.Position = 0;
            Finished();
        }

		/// <summary>
		/// Gets called when pending I/O operation completed.
		/// </summary>
		/// <param name="r"></param>
        private void ReadCallback(IAsyncResult r)
        {
            try
            {
                //When CompletedSynchronously returns true, the callback will still be fired—but possibly on the same thread as that which called BeginXXX.
                //Failing to consider this possibility can sometimes result in unintended recursion, leading to a stack overflow.
                if (r.CompletedSynchronously) return;
                if (SaveReceivedData(r))
                {
					// More data to read!
                    Read();
                    return;
                }
                _receivedData.Position = 0;
                Finished();
            }
            catch (Exception ex)
			{
				ProcessException(ex);
			}
        }

        /// <summary>
        /// If there are nomore data to read, returns false. Otherwise saves the recieved chunk of data and returns true.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        private bool SaveReceivedData(IAsyncResult r)
        {
            //Call EndRead to determine how many bytes were read from the stream.
            //EndRead can be called once on every IAsyncResult from BeginRead.
            //This method blocks until the I / O operation has completed.
            try
            {
                int chunkSize = _stream.EndRead(r);
                if (chunkSize == 0)
                {
                    return false;
                }
                _numberOfBytesRead += chunkSize;
                _receivedData.Write(_buffer, 0, chunkSize);
                return true;
            }
            catch (Exception e)
            {
                ProcessException(e);
                throw;
            }
        }

        void ProcessException(Exception ex)
        {
            Cleanup();
            Console.WriteLine("Error: " + ex.Message);
        }

        void Cleanup()
        {
            if (_stream != null) _stream.Close();
        }

		private void Finished()
		{
			string xml = new StreamReader(_receivedData).ReadToEnd();
            _stream.Close();
			_feed.Xml = xml;
			_feed.SetItems();
            DownloadFinished?.Invoke(_feed);
        }
    }

	public delegate void Finished(DAL.Feed feed);
}
