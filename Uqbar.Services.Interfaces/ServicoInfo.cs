using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework
{
    [DataContract]
    public class ServicoInfo
    {
        public enum EstadosEnum
        {
            Indisponivel = 1,
            Parado = 2,
            Correr = 3,
            Agendado = 4
        }

        [DataMember]
        public long NextCall { get; set; }

        [DataMember]
        public int TimerInterval { get; set; }

        [DataMember]
        public DateTime ActionStart { get; set; }

        [DataMember]
        public DateTime ActionEnd { get; set; }

        [DataMember]
        public EstadosEnum Estado { get; set; }
    }
}
