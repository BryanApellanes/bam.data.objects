using Bam.Console;
using Bam.CoreServices;
using Bam.Shell;

namespace Bam.Application
{
    [Serializable]
    class Program 
    {
        static void Main(string[] args)
        {
            ServiceRegistry serviceRegistry = BamConsoleContext.GetDefaultServiceRegistry();
            serviceRegistry.For<IMenuItemRunner>().UseSingleton(new ConsoleMenuItemRunner(serviceRegistry, new MenuInputMethodArgumentProvider(new ServiceRegistryTypedArgumentProvider(serviceRegistry))));
            IMenuOptions options = serviceRegistry.Get<MenuOptions>();

            BamConsoleContext.Current = new BamConsoleContext(serviceRegistry)
            {
                MenuManager = MenuManager.FromOptions(options)
            };
            BamConsoleContext.Current.Main(args);
            
        }
    }
}