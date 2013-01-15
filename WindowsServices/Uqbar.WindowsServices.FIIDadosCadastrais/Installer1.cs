using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Uqbar.ServerMonitorService
{
    [RunInstaller(true)]
    public partial class Installer1 : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller processInstaller;

        private ServiceInstaller monitorInstaller;

        public Installer1()
        {
            InitializeComponent();

            EventLogInstaller installer = FindInstaller(this.Installers);
            if (installer != null)
            {
                installer.Log = "UqbarServerMonitor"; // enter your event log name here
            }

            processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalService;

            monitorInstaller = new ServiceInstaller();
            monitorInstaller.StartType = ServiceStartMode.Automatic;
            monitorInstaller.ServiceName = "Uqbar Server Monitor";

            Installers.Add(monitorInstaller);
            Installers.Add(processInstaller);

        }

        private EventLogInstaller FindInstaller(InstallerCollection installers)
        {
            foreach (Installer installer in installers)
            {
                if (installer is EventLogInstaller)
                {
                    return (EventLogInstaller)installer;
                }

                EventLogInstaller eventLogInstaller = FindInstaller(installer.Installers);
                if (eventLogInstaller != null)
                {
                    return eventLogInstaller;
                }
            }
            return null;
        }
    }
}
