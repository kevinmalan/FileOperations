using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileOperations.FileWatcher.Services;
using Topshelf;

namespace FileOperations.FileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(hostConfig =>
            {
                hostConfig.Service<FileListenService>(serviceConfig =>
                {
                    serviceConfig.ConstructUsing(service => new FileListenService());
                    serviceConfig.WhenStarted(service => service.StartService());
                    serviceConfig.WhenStopped(service => service.StopService());
                });
                hostConfig.RunAsLocalSystem();

                hostConfig.SetDisplayName("File Watcher Service");
                hostConfig.SetDescription("Listens for File changes");
                hostConfig.SetServiceName("File Watcher Service");
            });

            Console.Read();
        }
    }
}
