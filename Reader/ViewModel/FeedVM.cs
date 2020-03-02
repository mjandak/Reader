using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using Reader.DAL;

namespace Reader
{
    public class FeedVM : ViewModelBase
    {
        private const string urlRegex = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)";
        private DAL.Feed _feed;
        private List<ItemVM> _items;

        public DAL.Feed Feed
        {
            get { return _feed; }
        }

        [Required(ErrorMessage = "This field is required.")]
        public string Name
        {
            get { return _feed.Name; }
            set
            {
                _feed.Name = value;
                //PropToValidateChanged(nameof(Name));
                ValidateProperty(value);
                PropChanged();
            }
        }

        [Required(ErrorMessage = "This field is required.")]
        [RegularExpression(urlRegex)]
        public string Url
        {
            get { return _feed.Url; }
            set
            {
                _feed.Url = value;
                //PropToValidateChanged(nameof(Url));
                ValidateProperty(value);
                PropChanged();
            }
        }

        private bool exclusivelyInStream;
        public bool ExclusivelyInStream
        {
            get => exclusivelyInStream;
            set
            {
                exclusivelyInStream = value;
                PropChanged(nameof(ExclusivelyInStream));
            }
        }

        public List<ItemVM> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                PropChanged(nameof(Items));
            }
        }

        public ICommand RefreshCmd
        {
            get;
            private set;
        }

        public ICommand SaveCmd
        {
            get;
            private set;
        }

        public ICommand EditCmd
        {
            get;
            private set;
        }

        public ICommand DeleteCmd
        {
            get;
            private set;
        }

        public ICommand ExclusiveSwitchedCmd
        {
            get;
            private set;
        }

        public override bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return false;
                if (!Regex.IsMatch(Url ?? "", urlRegex)) return false;
                return true;
            }
        }

        bool _refreshing;
        public bool Refreshing
        {
            get
            {
                return _refreshing;
            }
            private set
            {
                _refreshing = value;
                PropChanged(nameof(Refreshing));
            }
        }

        public FeedVM(DAL.Feed feed)
        {
            _feed = feed.ThrowIfNull(nameof(feed));
            RefreshCmd = new DelegateCommand(Refresh);
            SaveCmd = new DelegateCommand(Save);
            EditCmd = new DelegateCommand(Edit);
            DeleteCmd = new DelegateCommand(Delete);
            ExclusiveSwitchedCmd = new DelegateCommand(ExclusiveSwitched);

            _items = new List<ItemVM>();
            foreach (DAL.Item item in _feed.Items)
            {
                Items.Add(new ItemVM(item));
            }
            _feed.PropertyChanged += _feed_PropertyChanged;
            //Validate(false);
        }

        /// <summary>
        /// Underlying feed has changed handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _feed_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DAL.Feed.Items))
            {
                var tmp = new List<ItemVM>();
                foreach (DAL.Item item in _feed.Items)
                {
                    tmp.Add(new ItemVM(item));
                }
                Items = tmp;
            }
        }

        public void Refresh()
        {
            var fd = new FeedDownloader(_feed);
            fd.DownloadFinished += fd_DownloadFinished;
            ThreadPool.QueueUserWorkItem(o => fd.Begin());
            Refreshing = true;
        }

        public void Save()
        {
            Validate();
            if (HasErrors) return;
            if (_feed.Id == null)
            {
                DAL.Repository.Instance.AddFeed(_feed);
                MsgBus<NewFeedSavedEvent>.Instance.SendMessage(new NewFeedSavedEvent(_feed));
                return;
            }
            DAL.Repository.Instance.UpdateFeed(_feed);
        }

        public void Edit()
        {
            MsgBus<FeedEditEvent>.Instance.SendMessage(new FeedEditEvent(this));
        }

        private void Delete()
        {
            Repository.Instance.DeleteFeed(_feed.Id.Value);
            MsgBus<FeedDeletedEvent>.Instance.SendMessage(new FeedDeletedEvent(this));
        }

        private void ExclusiveSwitched()
        {
            MsgBus<FeedExclusiveSwitchedEvent>.Instance.SendMessage(new FeedExclusiveSwitchedEvent(this));
        }

        private void fd_DownloadFinished(DAL.Feed feed)
        {
            DAL.Repository.Instance.UpdateFeed(feed);
            Refreshing = false;
            MsgBus<FeedRefreshedEvent>.Instance.SendMessage(new FeedRefreshedEvent(this));
        }

        public class NewFeedSavedEvent
        {
            public NewFeedSavedEvent(DAL.Feed feed)
            {
                Feed = feed;
            }

            public DAL.Feed Feed { get; }
        }

        public class FeedEditEvent
        {
            public FeedEditEvent(FeedVM feed)
            {
                Feed = feed;
            }

            public FeedVM Feed { get; }
        }

        public class FeedRefreshedEvent
        {
            public FeedRefreshedEvent(FeedVM feed)
            {
                Feed = feed;
            }
            public FeedVM Feed { get; }
        }

        public class FeedDeletedEvent
        {
            public FeedDeletedEvent(FeedVM feed)
            {
                Feed = feed;
            }
            public FeedVM Feed { get; }
        }

        public class FeedExclusiveSwitchedEvent
        {
            public FeedExclusiveSwitchedEvent(FeedVM feed)
            {
                Feed = feed;
            }
            public FeedVM Feed { get; }
        }
    }
}
