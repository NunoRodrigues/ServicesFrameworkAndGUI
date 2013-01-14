using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework.Agendamento
{
    [DataContract]
    public class Agenda
    {
        [DataMember]
        public TimeSpan LocalToTargetDiference = TimeSpan.Zero;

        [DataMember]
        public List<AgendaItem> List = new List<AgendaItem>();

        public TimeSpan NextCall()
        {
            DateTime now = DateTime.Now.Add(LocalToTargetDiference);

            TimeSpan nextCall = TimeSpan.MaxValue;

            if (List.Count > 0)
            {
                nextCall = List.Min(i => i.GetNextInterval(now));
            }

            return nextCall;
        }
    }
}
