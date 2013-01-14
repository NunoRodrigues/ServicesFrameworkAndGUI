using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.WCF;

namespace Uqbar.Services.WinSrv.CVM
{
    public class MessageCallbackListeners
    {
        private List<OperationContext> _list = new List<OperationContext>();

        public MessageCallbackListeners()
        {
            
        }

        public void Add(OperationContext context)
        {
            if(_list.FirstOrDefault(l => l.SessionId == context.SessionId) == null)
            {
                _list.Add(context);

                ModulesService.Instance.Log.Write("New Listener ::: Added : Session ID : " + context.SessionId);
            }
        }

        public void Remove(OperationContext context)
        {
            OperationContext currContext = _list.FirstOrDefault(l => l.SessionId == context.SessionId);

            if (currContext != null)
            {
                _list.Remove(currContext);

                ModulesService.Instance.Log.Write("Listener ::: Removed : Session ID : " + context.SessionId);
            }
        }

        public void Callback(Mensagem msg)
        {
            // Callback the Listeners
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                try
                {
                    if (_list[i].Channel.State != System.ServiceModel.CommunicationState.Faulted)
                    {
                        _list[i].GetCallbackChannel<IServiceCallback>().OnNewMessage(msg);
                    }
                    else
                    {
                        _list.Remove(_list[i]);
                    }
                }
                catch
                {
                    _list.Remove(_list[i]);
                }
            }
        }
    }
}
