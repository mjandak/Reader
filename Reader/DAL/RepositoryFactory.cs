using System;
using System.Configuration;
using System.Linq;

namespace Reader.DAL
{
    public class RepositoryFactory
    {
        public static IRepository GetRepositoryInstance()
        {
            try
            {
                string typeName = ConfigurationManager.AppSettings["RepositoryType"];
                Type type = Type.GetType(typeName);
                if (type == null) throw new Exception("Could not resolve RepositoryType in AppSettinggs.");
                Type IRepositoryType = typeof(IRepository);
                if (type.GetInterfaces().FirstOrDefault(t => t == IRepositoryType) == null)
                    throw new Exception("RepositoryType must implement IRepository");

                return (IRepository)Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating RepositoryType instance.", e);
            }
        }
    }
}
