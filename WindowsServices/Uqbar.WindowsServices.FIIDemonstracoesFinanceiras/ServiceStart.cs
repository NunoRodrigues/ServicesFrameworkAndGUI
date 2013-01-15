using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Uqbar.WindowsServices.FIIDemonstracoesFinanceiras
{
    public static class ServiceStart
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main()
        {
            //System.Diagnostics.Debugger.Launch();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new ModulesService() 
            };

            // RUN (Service and Command Line)
            if (Environment.UserInteractive)
            {
                ServiceStart.RunInteractive(ServicesToRun);
            }
            else
            {
                ServiceBase.Run(ServicesToRun);
            }
        }

        public static void RunInteractive(ServiceBase[] servicesToRun)
        {
            ModulesService.Instance.Log.Write("Services running in interactive mode.");

            MethodInfo onStartMethod = typeof(ServiceBase).GetMethod("OnStart",
                BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (ServiceBase service in servicesToRun)
            {
                Console.Write("Starting {0}...", service.ServiceName);
                onStartMethod.Invoke(service, new object[] { new string[] { } });
                Console.Write("Started");
            }
            ModulesService.Instance.Log.Write("Press any key to stop the services and end the process...");
            Console.ReadKey();

            MethodInfo onStopMethod = typeof(ServiceBase).GetMethod("OnStop",
                BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (ServiceBase service in servicesToRun)
            {
                ModulesService.Instance.Log.Write("Stopping " + service.ServiceName + "...");
                onStopMethod.Invoke(service, null);
                ModulesService.Instance.Log.Write("Stopped");
            }

            ModulesService.Instance.Log.Write("All services stopped.");

            // Keep the console alive for a second to allow the user to see the message.
            Thread.Sleep(10000);
        }

        [Conditional("DEBUG")]
        static void DebugMode()
        {
            if (!Debugger.IsAttached)
                Debugger.Launch();

            Debugger.Break();
        }
    }
}
