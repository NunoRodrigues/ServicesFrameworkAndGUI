using System;
using System.Collections.Generic;
using System.ServiceModel;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.Agendamento;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.WCF;

namespace Uqbar.Services.WindowsService.WCF
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
            return ModulesService.Instance.Agenda;
        }

        public bool SetAgenda(Agenda agenda)
        {
            Configuration.Set(agenda);

            ModulesService.Instance.Agenda = agenda;

            ModulesService.Instance.TimmerTick(null);

            return true;
        }

        public ServicoInfo GetInfo()
        {
            return ModulesService.Instance.Info;
        }

        public Mensagem[] GetMensagens(int start, int length)
        {
            Mensagem[] result = ModulesService.Instance.Mensagens.PeekRange(start, length);
            return result;
        }
    }
}
