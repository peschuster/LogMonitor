using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Tail
{
    partial class Service : ServiceBase
    {
        public Service()
        {
            this.InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Hier Code hinzufügen, um den Dienst zu starten.
        }

        protected override void OnStop()
        {
            // TODO: Hier Code zum Ausführen erforderlicher Löschvorgänge zum Anhalten des Dienstes hinzufügen.
        }
    }
}
