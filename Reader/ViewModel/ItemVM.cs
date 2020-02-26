using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Reader
{
    public class ItemVM : ViewModelBase
    {
        private DAL.Item _item;

        public static string OpenEventId = "open";

        public string Title
        {
            get { return _item.Title; }
            set { _item.Title = value; }
        }

        public string Link
        {
            get { return _item.Link; }
            set { _item.Link = value; }
        }

        public string Description
        {
            get { return _item.Descritpion; }
        }

        public DateTime? DatePublished 
        { 
            get { return _item.DatePublished; }
        }

        public string FeedName
        { 
            get { return _item.Feed.Name; } 
        }

        public ItemVM(DAL.Item item)
        {
            OpenCmd = new DelegateCommand(() => MessageBus<ItemOpenEvent>.Instance.SendMessage(new ItemOpenEvent(Link)));
            _item = item;
        }

        public ICommand OpenCmd { get; set; }
    }

    /// <summary>
    /// Feed item needs to be displayed.
    /// </summary>
    public class ItemOpenEvent
    {
        public string Link { get; set; }

        public ItemOpenEvent(string link)
        {
            Link = link;
        }
    }
}
