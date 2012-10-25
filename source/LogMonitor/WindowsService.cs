using System;
using System.Diagnostics;
using System.ServiceProcess;

// Note: Windows-Dienst (De-)Installation with installutil.exe (/u) <Assemblyname>
namespace LogMonitor
{
    public partial class WindowsService : ServiceBase
    {
        private ConfigurationAppStarter starter;

        private Kernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsService" /> class.
        /// </summary>
        public WindowsService()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Called when [start].
        /// </summary>
        /// <param name="args">The args.</param>
        protected override void OnStart(string[] args)
        {
#if DEBUGGER
            Debugger.Launch();
#endif

            try
            {
                this.starter = new ConfigurationAppStarter();
                this.kernel = this.starter.Start();
            }
            catch (Exception exception)
            {
                this.applicationEventLog.WriteEntry(exception.ToString(), EventLogEntryType.Error);

                // Don't start, if initialization wasn't successfull.
                throw;
            }
        }

        protected override void OnStop()
        {
            if (this.kernel != null)
            {
                this.kernel.Dispose();
                this.kernel = null;
            }

            if (this.starter != null)
            {
                this.starter.Dispose();
                this.starter = null;
            }
        }
    }
}