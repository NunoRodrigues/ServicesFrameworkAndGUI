using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework.Mensagens
{
    public class Mensagem
    {
        public delegate void MensagemDelegate(Mensagem msg);

        public enum MessageType
        {
            Trace,
            Info,
            Warning,
            Error
        }

        public MessageType Type { get; set; }

        public DateTime Time { get; set; }
        
        public string Body { get; set; }
        
        public string ExtraInfo { get; set; }
    }

    public static class MessageExtensions
    {
        public static void Raise(this Mensagem.MensagemDelegate sender, Mensagem.MessageType type, string body)
        {
            sender.Raise(new Mensagem() { Type = type, Body = body, Time = DateTime.Now });
        }

        public static void Raise(this Mensagem.MensagemDelegate sender, string source, Mensagem.MessageType type, string body, Exception exception)
        {
            sender.Raise(new Mensagem() { Type = type, Body = body, ExtraInfo = exception.ToString() });
        }

        public static void Raise(this Mensagem.MensagemDelegate sender, Mensagem msg)
        {
            sender(msg);
        }
    }
}
