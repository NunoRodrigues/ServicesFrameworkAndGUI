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
using Uqbar.Services.WinSrv.CVM.Scrappers;
using Uqbar.Services.WinSrv.CVM.WCF;

namespace Uqbar.Services.WinSrv.CVM
{
    public partial class ModulesService : ServiceBase
    {
        public static ModulesService Instance { get; private set; }

        public MessageCallbackListeners Listeners = new MessageCallbackListeners();

        public Timers Timers = new Timers();

        public Logger Log = new Logger();

        public ServicoInfo Info = null;

        public Agenda Agenda = null;

        public ConcurrentQueue<Mensagem> Mensagens = null;

        private Service _wcf = null;

        public ModulesService()
        {
            InitializeComponent();

            if (ModulesService.Instance == null)
            {
                ModulesService.Instance = this;
            }
        }

        protected override void OnStart(string[] args)
        {
            Log.Write("SERVICE - START");


            // Info
            Instance.Info = new ServicoInfo();
            Instance.Info.Estado = ServicoInfo.EstadosEnum.Parado;
            Instance.Info.TimerInterval = Properties.Settings.Default.TimerPollingInterval;

            // Mensagens
            Instance.Mensagens = new ConcurrentQueue<Mensagem>();
            
            // Agenda
            Instance.Agenda = Configuration.Get<Agenda>();
            if (Instance.Agenda == null) { Instance.Agenda = new Agenda(); }

            // WCF
            Instance._wcf = new Service(new Uri(Properties.Settings.Default.WCFBaseAddress));
            Instance._wcf.Start(typeof(ServiceEndPoint), "/");
            Log.Write(Properties.Settings.Default.WCFBaseAddress);

            //Action
            ActionStart();

            // TODO :REMOVE
            ActionLaunch();
        }

        protected override void OnStop()
        {
            // WCF
            _wcf.Stop(typeof(ServiceEndPoint));

            //Action
            ActionStop();

            Log.Write("SERVICE - STOP");
        }

        public void ActionStart()
        {
            // Timer
            Instance.Timers.Start(0, Properties.Settings.Default.TimerPollingInterval, Instance.TimmerTick);
        }

        public void ActionStop()
        {
            // Timer
            Instance.Timers.Stop(Instance.TimmerTick);
        }

        public void ActionLaunch()
        {

            Uqbar.Services.Framework.Providers.Web provider = new Uqbar.Services.Framework.Providers.Web();

            FIIDadosCadastrais fiiDados = new FIIDadosCadastrais() { URL = new Uri("http://www.cvm.gov.br/asp/cvmwww/cadastro/CadListPartic.asp?Fisic_Juridic=&Tipo_Partic=67&Cpfcgc_Partic=&DtReg_Partic=&ContSocio="), DataProvider = provider };

            fiiDados.NewMessage += this.OnNewMessage;

            fiiDados.Perform();

            /*
            Providers.Web provider = new Providers.Web();

            FIIDemonstraçõesFinanceiras fiiDados = new FIIDemonstraçõesFinanceiras()
            {
                URL = new Uri(@"http://cvmweb.cvm.gov.br/SWB/Sistemas/SCW/CPublica/ResultListaPartic.aspx?TPConsulta=10"),
                DataProvider = provider
            };
            
            fiiDados.NewMessage += this.OnNewMessage;

            fiiDados.Perform();
            */
        }

        public void OnNewMessage(Mensagem msg)
        {
            Log.Write(msg);

            // Queue - Max Size
            //Instance.Mensagens.FixLength(Properties.Settings.Default.NumberMaxMessages);

            // Queue - Add New
            Instance.Mensagens.Enqueue(msg);


            /*
            // Email
            if (msg.Type == Mensagem.MessageType.Error && string.IsNullOrEmpty(Properties.Settings.Default.ErrorsEmailAddress) == false)
            {
                Email.SendEmail(Properties.Settings.Default.ErrorsEmailAddress, "Uqbar.ServerMonitor : Error : " + msg.Title, msg.Title + "\n\n" + ((string.IsNullOrEmpty(msg.ExceptionData)) ? "" : msg.ExceptionData));
            }
            if (msg.Type == Mensagem.MessageType.Warning && string.IsNullOrEmpty(Properties.Settings.Default.WarningsEmailAddress) == false)
            {
                Email.SendEmail(Properties.Settings.Default.WarningsEmailAddress, "Uqbar.ServerMonitor : Warning : " + msg.Title, msg.Title);
            }
            */

            Listeners.Callback(msg);
        }

        public void TimmerTick(Object stateInfo)
        {
            TimeSpan nextCall = Agenda.NextCall();

            if (nextCall.TotalMilliseconds > 0 && nextCall < TimeSpan.MaxValue)
            {
                Instance.Info.Estado = ServicoInfo.EstadosEnum.Agendado;

                if (nextCall.TotalMilliseconds <= Properties.Settings.Default.TimerPollingInterval)
                {
                    this.Info.ActionStart = DateTime.UtcNow;
                    Instance.Info.Estado = ServicoInfo.EstadosEnum.Correr;

                    ActionLaunch();

                    nextCall = Agenda.NextCall();
                    this.Info.ActionEnd = DateTime.UtcNow;
                }
            }
            else
            {
                Instance.Info.Estado = ServicoInfo.EstadosEnum.Parado;
            }

            Instance.Info.NextCall = (long)(nextCall.TotalMilliseconds);

            // TODO: REMOVE
            string tick = "Tick : " + Instance.Info.Estado + " : ";
            if (nextCall != TimeSpan.MaxValue)
            {
                DateTime next = DateTime.Now.AddTicks(nextCall.Ticks);
                tick += next.ToString();
            }
            else
            {
                tick += "NA";
            }
            OnNewMessage(new Mensagem() { Body = tick, Type = Mensagem.MessageType.Info, Time = DateTime.Now});
        }

        public event Mensagem.MensagemDelegate NewMessage;
    }
}
