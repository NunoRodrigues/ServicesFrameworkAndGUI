using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.Mensagens;

namespace Uqbar.Services.Framework
{
    public class Logger
    {
        private EventLog _log = null;

        public Logger()
        {
            _log = new EventLog();
            _log.Source = "Uqbar.Services.WindowsService";
            _log.Log = "Application";

            ((System.ComponentModel.ISupportInitialize)(_log)).BeginInit();
            if (!EventLog.SourceExists(_log.Source))
            {
                EventLog.CreateEventSource(_log.Source, _log.Log);
            }
            ((System.ComponentModel.ISupportInitialize)(_log)).EndInit();
        }

        private EventLogEntryType GetType(Mensagem.MessageType type)
        {
            EventLogEntryType result = EventLogEntryType.Information;

            switch (type)
            {
                case Mensagem.MessageType.Warning:
                    result = EventLogEntryType.Warning;
                    break;
                case Mensagem.MessageType.Error:
                    result = EventLogEntryType.Error;
                    break;
            }

            return result;
        }

        public void Write(Mensagem msg)
        {
            string message = "::: " + msg.Type + " : " + msg.Body;

            if (string.IsNullOrEmpty(msg.ExtraInfo) == false)
            {
                message += "\n\n" + msg.ExtraInfo;
            }

            Write(message, GetType(msg.Type));
        }

        public void Write(string msg)
        {
            Write(msg, EventLogEntryType.Information);
        }

        public void Write(string msg, EventLogEntryType type)
        {
            Console.WriteLine(msg);

            _log.WriteEntry(msg, type);
        }
    }
}
