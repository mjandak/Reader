using System;

namespace Reader.DAL
{
	class Repository
	{
		private static IRepository _instance;
		public static IRepository Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}
				else
				{
					throw new Exception("Repository not set.");
				}
			}
			set
			{
				_instance = value;
			}
		}

	}
}
