using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.WindowsService;

namespace Uqbar.Services.Framework.WCF
{
    public interface IServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnInfoChanged(ServicoInfo newInfo);

        [OperationContract(IsOneWay = true)]
        void OnNewMessage(Mensagem newMsg);
    }
}
