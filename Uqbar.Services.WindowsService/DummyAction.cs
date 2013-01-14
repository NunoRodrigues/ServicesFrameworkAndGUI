using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uqbar.Services.Framework.Mensagens;

namespace Uqbar.Services.WindowsService
{
    public class DummyAction : IMessagemProvider
    {
        public event Mensagem.MensagemDelegate NewMessage;

        public void Perform()
        {
            this.NewMessage.Raise(Mensagem.MessageType.Warning, "!!!!! DUMMY ACTION PERFORMED !!!!!");
        }
    }
}
