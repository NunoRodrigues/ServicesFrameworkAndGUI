using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using HtmlAgilityPack;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.Agendamento;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.WCF;
using Uqbar.Services.Framework.WindowsService;

namespace Uqbar.WindowsServices.FIIDadosCadastrais
{
    public partial class ModulesService : WindowsServiceBase
    {
        public ModulesService() : base()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.ServiceStart(Properties.Settings.Default.TimerPollingInterval, Properties.Settings.Default.WCFBaseAddress);
        }

        protected override void OnStop()
        {
            this.ServiceStop();
        }

        public override void ActionLaunch()
        {

            Uqbar.Services.Framework.Providers.Web provider = new Uqbar.Services.Framework.Providers.Web();

            FIIDadosCadastrais fiiDados = new FIIDadosCadastrais() { URL = new Uri("http://www.cvm.gov.br/asp/cvmwww/cadastro/CadListPartic.asp?Fisic_Juridic=&Tipo_Partic=67&Cpfcgc_Partic=&DtReg_Partic=&ContSocio="), DataProvider = provider };

            fiiDados.NewMessage += this.OnNewMessage;

            fiiDados.Perform(this.Info);
        }
    }
}
