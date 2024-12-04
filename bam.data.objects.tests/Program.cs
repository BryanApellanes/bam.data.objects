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
            BamConsoleContext.StaticMain(args);
        }
    }
}