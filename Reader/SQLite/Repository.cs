using System;
using System.Collections.Generic;
using System.Linq;

namespace Reader.SQLite
{
    public class Repository : DAL.IRepository
    {
        public IEnumerable<DAL.Feed> GetAllFeeds()
        {
            using var context = new ORM.Entities();
            List<DAL.Feed> l = new List<DAL.Feed>(context.Feeds.Count());
            foreach (ORM.Feed item in context.Feeds)
            {
                DAL.Feed f = new DAL.Feed()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Url = item.Url,
                    Xml = item.Xml
                };
                f.SetItems();
                l.Add(f);
            }
            return l;
        }

        public void AddFeed(DAL.Feed feed)
        {
            //The AUTOINCREMENT keyword imposes extra CPU, memory, disk space, and disk I/O overhead and should be avoided if not strictly needed.
            //It is usually not needed.
            //On an INSERT, if the ROWID or INTEGER PRIMARY KEY column is not explicitly given a value, then it will be filled automatically with an unused integer, usually one more than the largest ROWID currently in use. 
            //This is true regardless of whether or not the AUTOINCREMENT keyword is used.

            //In Entity Framework, the SaveChanges() method internally creates a transaction and wraps all INSERT, UPDATE and DELETE operations under it. 
            //Multiple SaveChanges() calls, create separate transactions, perform CRUD operations and then commit each transaction.

            using (var context = new ORM.Entities())
            {
                //context.Database.Log = Console.Write;

                long maxid = context.Feeds.Max(f => f.Id);

                ORM.Feed f = new ORM.Feed()
                {
                    Id = maxid + 1,
                    Name = feed.Name,
                    Url = feed.Url,
                    Xml = feed.Xml
                };
                context.Feeds.Add(f);
                context.SaveChanges();

                feed.Id = f.Id;
            }
        }

        public void DeleteFeed(long id)
        {
            using (var context = new ORM.Entities())
            {
                ORM.Feed dbFeed = context.Feeds.FirstOrDefault(f => f.Id == id);
                if (dbFeed == null) throw new Exception($"Feed {id} not found in repository.");
                context.Feeds.Remove(dbFeed);
                context.SaveChanges();
            }
        }


        public void UpdateFeed(DAL.Feed feed)
        {
            //var f = new ORM.Feed() { Id = feed.Id, Name = feed.Name, Url = feed.Url };
            //using (var context = new ORM.Entities())
            //{
            //    context.Feeds.Attach(f);
            //    context.Entry(f).Property("Name").IsModified = true;
            //    context.Entry(f).Property("Url").IsModified = true;
            //    context.SaveChanges();
            //}

            using (var context = new ORM.Entities())
            {
                ORM.Feed dbFeed = context.Feeds.FirstOrDefault(f => f.Id == feed.Id);
                if (dbFeed == null) throw new Exception($"Feed {feed.Id} not found in repository.");
                dbFeed.Name = feed.Name;
                dbFeed.Url = feed.Url;
                dbFeed.Xml = feed.Xml;
                context.SaveChanges();
            }
        }
    }
}
