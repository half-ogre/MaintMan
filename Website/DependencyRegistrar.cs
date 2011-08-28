using AnglicanGeek.Mvc;

namespace MaintMan
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void RegisterDependencies(IDependencyRegistry dependencyRegistry)
        {
            var configuration = new Configuration();
            dependencyRegistry.RegisterCreator<IConfiguration>(() => configuration);

            dependencyRegistry.RegisterBinding<IHttpRequestExecutor, HttpRequestExecutor>();
            dependencyRegistry.RegisterBinding<IBuildExecutor, BuildExecutor>();
            dependencyRegistry.RegisterBinding<IMaintenanceTarballCreator, MaintenanceTarballCreator>();
        }
    }
}