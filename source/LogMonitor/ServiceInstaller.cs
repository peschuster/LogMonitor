using System.ComponentModel;
using System.Configuration.Install;

namespace LogMonitor
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : Installer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceInstaller" /> class.
        /// </summary>
        public ServiceInstaller()
        {
            this.InitializeComponent();
        }
    }
}
