using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace LogMonitor
{
    public partial class ServiceInstaller
    {
        private IContainer components = null;

        private global::System.ServiceProcess.ServiceInstaller serviceInstaller;

        private ServiceProcessInstaller serviceProcessInstaller;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.serviceInstaller = new global::System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstaller = new ServiceProcessInstaller();
            
            //
            // serviceInstaller
            // 
            this.serviceInstaller.Description = "LogMonitor feeding data from IIS logs into Graphite and StatsD.";
            this.serviceInstaller.DisplayName = "LogMonitor";
            this.serviceInstaller.ServiceName = "LogMonitor";
            this.serviceInstaller.StartType = ServiceStartMode.Automatic;
            this.serviceInstaller.DelayedAutoStart = true;
            
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            
            // 
            // ServiceInstaller
            // 
            this.Installers.AddRange(new Installer[] 
            {
                this.serviceInstaller, 
                this.serviceProcessInstaller
            });
        }
    }
}