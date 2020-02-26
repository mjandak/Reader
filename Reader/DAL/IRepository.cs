using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reader.DAL
{
	public interface IRepository
	{
		IEnumerable<Feed> GetAllFeeds();
		void AddFeed(Feed feed);
		void DeleteFeed(long id);
		void UpdateFeed(Feed feed);
	}
}
