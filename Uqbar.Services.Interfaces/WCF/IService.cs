using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Uqbar.Services.Framework.Agendamento;
using Uqbar.Services.Framework.Mensagens;

namespace Uqbar.Services.Framework.WCF
{
    [ServiceContract(CallbackContract = typeof(IServiceCallback))]
    public interface IService
    {
        [OperationContract]
        void Start(bool runThisInstant);

        [OperationContract]
        void Stop();

        [OperationContract]
        ServicoInfo GetInfo();

        [OperationContract]
        Agenda GetAgenda();

        [OperationContract]
        bool SetAgenda(Agenda agenda);

        [OperationContract]
        Mensagem[] GetMensagens(int start, int length);
    }
}
