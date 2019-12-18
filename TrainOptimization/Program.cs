using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration.Install;
using System.Reflection;

namespace TrainOptimization
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
#if DEBUG
            if (System.Environment.UserInteractive)
            {
                if (args.Length > 0)
                {
                    string lvParam = args[0].ToLowerInvariant().Trim();
                    DebugLog.Logar("lvParam = " + lvParam);
                }
            }

            TrainOptimizer lvTrainOpt = new TrainOptimizer();
            lvTrainOpt.onDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            if (System.Environment.UserInteractive && (args.Length > 0))
            {
                if (args.Length > 0)
                {
                    string lvParam = args[0].ToLowerInvariant().Trim();
                    DebugLog.Logar("lvParam = " + lvParam);

                    switch (lvParam)
                    {
                        case "--install":
                            ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                            DebugLog.Logar("Instalado !");
                            break;
                        case "--uninstall":
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                            DebugLog.Logar("Desinstalado !");
                            break;
                    }
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new TrainOptimizer() 
                };
                ServiceBase.Run(ServicesToRun);
            }
#endif
        }
    }
}
