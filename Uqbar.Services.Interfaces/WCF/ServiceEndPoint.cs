using System;
using System.Collections.Generic;
using System.ServiceModel;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.Agendamento;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.WCF;
using Uqbar.Services.Framework.WindowsService;

namespace Uqbar.Services.Framework.WCF
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)] // InstanceContextMode = InstanceContextMode.Single
    public class ServiceEndPoint : IService
    {
        public ServiceEndPoint()
        {

        }

        public void Start(bool runThisInstant)
        {
            
        }

        public void Stop()
        {
            
        }

        public Agenda GetAgenda()
        {
            return WindowsServiceBase.Instance.Agenda;
        }

        public bool SetAgenda(Agenda agenda)
        {
            Configuration.Set(agenda);

            WindowsServiceBase.Instance.Agenda = agenda;

            WindowsServiceBase.Instance.TimmerTick(null);

            return true;
        }

        public ServicoInfo GetInfo()
        {
            return WindowsServiceBase.Instance.Info;
        }

        public Mensagem[] GetMensagens(int start, int length)
        {
            Mensagem[] result = WindowsServiceBase.Instance.Mensagens.PeekRange(start, length);
            return result;
        }
    }
}
