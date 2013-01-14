using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework.Mensagens
{
    public interface IMessagemProvider
    {
        event Mensagem.MensagemDelegate NewMessage;
    }
}
