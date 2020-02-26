using System;
namespace Reader.DAL
{
	public class Item : ModelBase
	{
		public string Title
		{
			get;
			set;
		}

		public string Link
		{
			get;
			set;
		}

        public string Descritpion
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public DateTime? DatePublished 
        { 
            get; 
            set; 
        }

        public Feed Feed { get; set; }

        public Item(string title, string link, string desc = null, string content = null)
		{
			Title = title;
			Link = link;
            Descritpion = desc;
            Content = content;
		}
	}
}
