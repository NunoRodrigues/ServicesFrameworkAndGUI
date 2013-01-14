using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.Agendamento;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.WCF;
using Uqbar.Services.WindowsService.WCF;

namespace Uqbar.Services.WindowsService
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

        private DummyAction _action = null;

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
            Log.Write("START");

            // Info
            Instance.Info = new ServicoInfo();
            Instance.Info.Estado = ServicoInfo.EstadosEnum.Parado;
            Instance.Info.TimerInterval = Properties.Settings.Default.TimerPollingInterval;

            // Mensagens
            Instance.Mensagens = new ConcurrentQueue<Mensagem>();

            // Action
            _action = new DummyAction();
            _action.NewMessage += this.OnNewMessage;

            // Agenda
            Instance.Agenda = Configuration.Get<Agenda>();
            if (Instance.Agenda == null) { Instance.Agenda = new Agenda(); }

            // WCF
            Instance._wcf = new Service(new Uri(Properties.Settings.Default.WCFBaseAddress));
            Instance._wcf.Start(typeof(ServiceEndPoint), "/");
            Log.Write(Properties.Settings.Default.WCFBaseAddress);

            //Action
            ActionStart();
        }

        protected override void OnStop()
        {
            Log.Write("STOP");

            // WCF
            _wcf.Stop(typeof(ServiceEndPoint));

            //Action
            ActionStop();

           
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
            _action.Perform();
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
                    Instance.Info.Estado = ServicoInfo.EstadosEnum.Correr;

                    ActionLaunch();

                    nextCall = Agenda.NextCall();
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
