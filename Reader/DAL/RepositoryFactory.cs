using System;
using System.Configuration;
using System.Linq;

namespace Reader.DAL
{
    public class RepositoryFactory
    {
        private const string repositoryConfigKey = "RepositoryType";

        public static IRepository GetRepositoryInstance()
        {
            try
            {
                string repositoryTypeName = ConfigurationManager.AppSettings[repositoryConfigKey];
                Type repositoryType = Type.GetType(repositoryTypeName);
                if (repositoryType == null) throw new Exception($"Couldn't find {repositoryTypeName} type.");
                Type IRepositoryType = typeof(IRepository);
                if (repositoryType.GetInterfaces().FirstOrDefault(t => t == IRepositoryType) == null)
                    throw new Exception($"{repositoryTypeName} has to implement {nameof(IRepository)} interface.");

                return (IRepository)Activator.CreateInstance(repositoryType);
            }
            catch (Exception e)
            {
                throw new Exception("Error creating RepositoryType instance.", e);
            }
        }
    }
}
