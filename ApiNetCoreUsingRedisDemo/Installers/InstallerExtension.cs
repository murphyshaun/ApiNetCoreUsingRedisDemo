namespace ApiNetCoreUsingRedisDemo.Installers
{
    public static class InstallerExtension
    {
        public static void InstallerServiceInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            var installers = typeof(Program).Assembly.ExportedTypes.Where(x => typeof(IInstaller).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(Activator.CreateInstance).Cast<IInstaller>();

            foreach (var item in installers)
            {
                item.InstallService(services, configuration);
            }
        }
    }
}
