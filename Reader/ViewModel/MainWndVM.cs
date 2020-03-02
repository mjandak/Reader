using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading;
using Reader.DAL;

namespace Reader
{
    public class MainWndVM : INotifyPropertyChanged
    {
        private ObservableCollection<FeedVM> _feeds;

        public ObservableCollection<FeedVM> Feeds
        {
            get
            {
                return _feeds;
            }
            private set
            {
                _feeds = value;
            }
        }

        public List<ItemVM> ItemsStream
        {
            get
            {
                var tmp = Feeds.Where(f => f.ExclusivelyInStream);
                if (tmp.Count() == 0) tmp = Feeds;
                return new List<ItemVM>(tmp.SelectMany(f => f.Items).OrderByDescending(i => i.DatePublished));
            }
        }

        public bool MultipleFeedsSelectioON { get; set; }

        public MainWndVM()
        {
            MsgBus<FeedVM.NewFeedSavedEvent>.Instance.MessageRecieved += newFeedSaved;
            MsgBus<FeedVM.FeedRefreshedEvent>.Instance.MessageRecieved += feedRefreshed;
            MsgBus<FeedVM.FeedDeletedEvent>.Instance.MessageRecieved += feedDeleted;
            MsgBus<FeedVM.FeedExclusiveSwitchedEvent>.Instance.MessageRecieved += feedExclusive;
            RefreshFeeds = new DelegateCommand(refreshFeeds, x => _feeds.Count > 0);
            Feeds = new ObservableCollection<FeedVM>();
            Feeds.CollectionChanged += (s, e) => NotifyPropertyChanged(nameof(ItemsStream));
            IEnumerable<Feed> feeds = Repository.Instance.GetAllFeeds();
            foreach (Feed item in feeds)
            {
                var fvm = new FeedVM(item);
                Feeds.Add(fvm);
            }
        }

        private void feedExclusive(FeedVM.FeedExclusiveSwitchedEvent e)
        {
            if (!e.Feed.ExclusivelyInStream)
            {
                NotifyPropertyChanged(nameof(ItemsStream));
                return;
            }

            if (!MultipleFeedsSelectioON)
            {
                foreach (FeedVM item in Feeds)
                {
                    item.ExclusivelyInStream = (item == e.Feed);
                }
            }
            NotifyPropertyChanged(nameof(ItemsStream));
        }

        private void feedRefreshed(FeedVM.FeedRefreshedEvent e)
        {
            NotifyPropertyChanged(nameof(ItemsStream));
        }

        private void feedDeleted(FeedVM.FeedDeletedEvent e)
        {
            Feeds.Remove(e.Feed);
            NotifyPropertyChanged(nameof(ItemsStream));
        }

        private void newFeedSaved(FeedVM.NewFeedSavedEvent e)
        {
            Feeds.Add(new FeedVM(e.Feed));
            //Feeds = new ObservableCollection<FeedVM>();
            //var feeds = Repository.Instance.GetAllFeeds();
            //foreach (DAL.Feed item in feeds)
            //{
            //    var fvm = new FeedVM(item);
            //    Feeds.Add(fvm);
            //}
            //NotifyPropertyChanged(nameof(Feeds));
        }

        private void refreshFeeds()
        {
            foreach (FeedVM item in _feeds)
            {
                //FeedDownloader fd = new FeedDownloader(item.Feed);
                //fd.DownloadFinished += FeedDownloadFinished;
                //ThreadPool.QueueUserWorkItem(o => fd.Begin());
                //fd.Begin();
                item.Refresh();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public ICommand RefreshFeeds { get; set; }
    }
}
