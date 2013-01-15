using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Uqbar.Services.Framework.Agendamento;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.WCF;

namespace Uqbar.Services.Framework.WindowsService
{
    public abstract class WindowsServiceBase : ServiceBase
    {
        public static WindowsServiceBase Instance { get; private set; }

        public MessageCallbackListeners Listeners = new MessageCallbackListeners();

        public Timers Timers = new Timers();

        public Logger Log = new Logger();

        public ServicoInfo Info = null;

        public Agenda Agenda = null;

        public ConcurrentQueue<Mensagem> Mensagens = null;

        private Service _wcf = null;

        private int _timersPollingInterval = 60000;

        private Type _wcfEndPointType = null;

        public WindowsServiceBase()
        {
            if (WindowsServiceBase.Instance == null)
            {
                WindowsServiceBase.Instance = this;
            }
        }

        public void ServiceStart(int timerPollingInterval, string wcfBaseAddress)
        {
            ServiceStart(timerPollingInterval, wcfBaseAddress, typeof(ServiceEndPoint));
        }

        public void ServiceStart(int timerPollingInterval, string wcfBaseAddress, Type wcfEndPoint)
        {
            Log.Write("SERVICE - START");

            this._timersPollingInterval = timerPollingInterval;
            this._wcfEndPointType = wcfEndPoint;

            // Info
            Instance.Info = new ServicoInfo();
            Instance.Info.Estado = ServicoInfo.EstadosEnum.Parado;
            Instance.Info.TimerInterval = this._timersPollingInterval;

            // Mensagens
            Instance.Mensagens = new ConcurrentQueue<Mensagem>();

            // Agenda
            Instance.Agenda = Configuration.Get<Agenda>();
            if (Instance.Agenda == null) { Instance.Agenda = new Agenda(); }

            // WCF
            Instance._wcf = new Service(new Uri(wcfBaseAddress));
            Instance._wcf.Start(this._wcfEndPointType, "/");
            Log.Write(wcfBaseAddress);

            //Action
            ActionStart();
        }

        public void ServiceStop()
        {
            // WCF
            _wcf.Stop(this._wcfEndPointType);

            //Action
            ActionStop();

            Log.Write("SERVICE - STOP");
        }

        public void ActionStart()
        {
            // Timer
            Instance.Timers.Start(0, this._timersPollingInterval, Instance.TimmerTick);
        }

        public void ActionStop()
        {
            // Timer
            Instance.Timers.Stop(Instance.TimmerTick);
        }

        public abstract void ActionLaunch();

        public void TimmerTick(Object stateInfo)
        {
            TimeSpan nextCall = Agenda.NextCall();

            // Tem Agendamento
            if (nextCall.TotalMilliseconds > 0 && nextCall < TimeSpan.MaxValue)
            {
                if (Instance.Info.Estado != ServicoInfo.EstadosEnum.Correr)
                {
                    Instance.Info.Estado = ServicoInfo.EstadosEnum.Agendado;

                    if (nextCall.TotalMilliseconds <= this._timersPollingInterval)
                    {
                        Instance.Info.Estado = ServicoInfo.EstadosEnum.Correr;

                        this.Info.ActionStart = DateTime.Now;
                        this.Info.ActionEnd = this.Info.ActionStart;

                        ActionLaunch();

                        nextCall = Agenda.NextCall();
                        this.Info.ActionEnd = DateTime.Now;

                        Instance.Info.Estado = ServicoInfo.EstadosEnum.Agendado;
                    }
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

            OnNewMessage(new Mensagem() { Body = tick, Type = Mensagem.MessageType.Info, Time = DateTime.Now });
        }

        public void OnNewMessage(Mensagem msg)
        {
            Log.Write(msg);

            // Queue - Max Size
            //Instance.Mensagens.FixLength(Properties.Settings.Default.NumberMaxMessages);

            // Queue - Add New
            Instance.Mensagens.Enqueue(msg);

            Listeners.Callback(msg);
        }
    }
}
