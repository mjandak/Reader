using System;
using System.Collections.Generic;

namespace Reader.SQLite.ORM
{
    public partial class Feed
    {
        public long Id { get; set; }
        public string Xml { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
