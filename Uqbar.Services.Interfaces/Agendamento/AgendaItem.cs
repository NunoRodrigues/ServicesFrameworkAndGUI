using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Uqbar.Services.Framework.Agendamento
{
    [DataContract]
    public class AgendaItem
    {
        public enum Periodicidade
        {
            Pontual,
            Diario,
            Semanal,
            Mensal
        }

        public static Dictionary<AgendaItem.Periodicidade, string> PeriodicidadeLabels = new Dictionary<AgendaItem.Periodicidade, string>()
                                                   {
                                                       {AgendaItem.Periodicidade.Pontual, "Pontual"},
                                                       {AgendaItem.Periodicidade.Diario, "Diário"},
                                                       {AgendaItem.Periodicidade.Semanal, "Semanal"},
                                                       {AgendaItem.Periodicidade.Mensal, "Mensal"}
                                                   };

        public static Dictionary<DayOfWeek, string> DiasSemanaLabels = new Dictionary<DayOfWeek, string>()
                                                   {
                                                       {DayOfWeek.Sunday, "Domingo"},
                                                       {DayOfWeek.Monday, "Segunda"},
                                                       {DayOfWeek.Tuesday, "Terça"},
                                                       {DayOfWeek.Wednesday, "Quarta"},
                                                       {DayOfWeek.Thursday, "Quinta"},
                                                       {DayOfWeek.Friday, "Sexta"},
                                                       {DayOfWeek.Saturday, "Sábado"},
                                                   };

        internal AgendaItem()
        {
            
        }

        public static AgendaItem New()
        {
            return new AgendaItem() { Id = Guid.NewGuid() };
        }

        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public Periodicidade Tipo { get; set; }

        [DataMember]
        public DateTime? Data { get; set; }

        [DataMember]
        public int Hora { get; set; }

        [DataMember]
        public int Minutos { get; set; }

        [DataMember]
        public int Dia { get; set; }

        [DataMember]
        public List<DayOfWeek> DiasSemana { get; set; }

        public virtual TimeSpan GetNextInterval(DateTime now)
        {
            switch (Tipo)
            {
                case Periodicidade.Pontual:
                    return GetPontual(now);

                case Periodicidade.Diario:
                    return GetDiario(now);

                case Periodicidade.Semanal:
                    return GetSemanal(now);

                case Periodicidade.Mensal:
                    return GetMensal(now);
            }

            return TimeSpan.MaxValue;
        }

        private TimeSpan GetPontual(DateTime now)
        {
            if (Data != null)
            {
                DateTime currData = (DateTime) Data;
                if (currData.Hour == 0 && currData.Minute == 0)
                {
                    currData = new DateTime(currData.Year, currData.Month, currData.Day, Hora, Minutos, 0);
                }

                TimeSpan dif = currData - now;

                if (dif.TotalSeconds > 0)
                {
                    return dif;
                }
            }

            return TimeSpan.MaxValue;
        }

        private TimeSpan GetDiario(DateTime now)
        {
            TimeSpan currHours = now - new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            TimeSpan diff = (new TimeSpan(0, Hora, Minutos, 0) - currHours);

            if (diff.TotalSeconds > 0)
            {
                return diff;
            }
            else
            {
                return (new TimeSpan(TimeSpan.TicksPerDay) + diff);
            }
        }

        private TimeSpan GetSemanal(DateTime now)
        {
            if (DiasSemana != null && DiasSemana.Count > 0)
            {
                DateTime call = new DateTime(now.Year, now.Month, now.Day, Hora, Minutos, 0);

                if (DiasSemana.Contains(now.DayOfWeek) == false || (call - now).TotalMilliseconds <= 0)
                {
                    DayOfWeek forward = DiasSemana.FirstOrDefault(d => (int)now.DayOfWeek < (int)d);
                    if ((int) now.DayOfWeek < (int) forward)
                    {
                        // next day is forward from the current day
                        call = call.AddDays((int) forward - (int) now.DayOfWeek);
                    }
                    else
                    {
                        int backwards = 0;
                        if (DiasSemana.Contains(forward) == false)
                        {
                            backwards = DiasSemana.Min(d => (int) d);
                        }

                        call = call.AddDays((7 - (int)now.DayOfWeek) + backwards);
                    }
                }

                return call - now;
            }

            return TimeSpan.MaxValue;
        }

        private TimeSpan GetMensal(DateTime now)
        {
            DateTime maxDay = new DateTime(now.Year, now.Month + 1, 1, Hora, Minutos, 0).AddDays(-1);
            DateTime nextCall = (Dia < maxDay.Day) ? new DateTime(now.Year, now.Month, Dia, Hora, Minutos, 0) : maxDay;

            return nextCall - now;
        }
    }
}
