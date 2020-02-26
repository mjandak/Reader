using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace Reader.DAL
{
    public class Feed : ModelBase
    {
        private string _url;
        private string _xml;

        public long? Id { get; set; } = null;

        public string Xml
        {
            get => _xml;
            set => _xml = value?.Trim();
        }

        private List<Item> _items = new List<Item>();
        public List<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropChanged(nameof(Items));
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropChanged(nameof(Name));
            }
        }

        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                OnPropChanged(nameof(Url));
            }
        }

        public bool IsEmpty
        {
            get => string.IsNullOrEmpty(Xml);
        }

        /// <summary>
        /// Read feed's xml and fill Items collection.
        /// </summary>
        public void SetItems()
        {
            if (IsEmpty) return;

            IEnumerable<XElement> xmlItems;
            XDocument xdoc = XDocument.Parse(Xml);
            XNamespace dn = "";
            XNamespace cn = "http://purl.org/rss/1.0/modules/content/";

            if (xdoc.Root.Name == "rss") //rss 2.0
            {
                xmlItems = xdoc.Root.Descendants("item");
            }
            else if (xdoc.Root.Name.LocalName == "RDF") //rss 1.0
            {
                dn = "http://purl.org/rss/1.0/";
                xmlItems = xdoc.Root.Descendants(dn + "item");
            }
            else
            {
                Items.Add(new Item("Unknown RSS format.", ""));
                return;
            }

            var items = new List<Item>();

            foreach (XElement item in xmlItems)
            {
                XElement descritption = item.Element(dn + "description");
                XElement content = item.Element(cn + "encoded");
                XElement pubDate = item.Element(dn + "pubDate");
                if (pubDate == null)
                {

                }
                items.Add(
                    new Item(
                        item.Element(dn + "title").Value,
                        item.Element(dn + "link").Value,
                        descritption?.Value,
                        content?.Value
                        )
                    {
                        DatePublished = pubDate == null ? DateTime.MaxValue : DateTime.Parse(pubDate.Value),
                        Feed = this
                    }
                    );
            }

            Items = items;
        }
    }
}
