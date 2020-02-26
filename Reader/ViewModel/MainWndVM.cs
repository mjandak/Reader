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
                return new List<ItemVM>(Feeds.SelectMany(f => f.Items).OrderByDescending(i => i.DatePublished));
            }
        }

        public MainWndVM()
        {
            MessageBus<FeedVM.NewFeedSavedEvent>.Instance.MessageRecieved += newFeedSaved;
            MessageBus<FeedVM.FeedRefreshedEvent>.Instance.MessageRecieved += feedRefreshed;
            MessageBus<FeedVM.FeedDeletedEvent>.Instance.MessageRecieved += feedDeleted;
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

        private void feedRefreshed(FeedVM.FeedRefreshedEvent obj)
        {
            NotifyPropertyChanged(nameof(ItemsStream));
        }

        private void feedDeleted(FeedVM.FeedDeletedEvent obj)
        {
            Feeds.Remove(obj.Feed);
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
