using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Egee.Wcf.Service.Host
{
    [RunInstaller(true)]
    public partial class SappelInstaller : System.Configuration.Install.Installer
    {
        public SappelInstaller()
        {
            //InitializeComponent();
            serviceProcessInstaller1 = new ServiceProcessInstaller();
            serviceProcessInstaller1.Account = ServiceAccount.LocalSystem;
            serviceInstaller1 = new ServiceInstaller();
            serviceInstaller1.ServiceName = "WinSvcHostedSappelWCF";
            serviceInstaller1.DisplayName = "WinSvcHostedSappelWCF";
            serviceInstaller1.Description = "Service e-GEE WCF du Wrapper Sappel";
            serviceInstaller1.StartType = ServiceStartMode.Automatic;
            Installers.Add(serviceProcessInstaller1);
            Installers.Add(serviceInstaller1);
        }
    }
}
