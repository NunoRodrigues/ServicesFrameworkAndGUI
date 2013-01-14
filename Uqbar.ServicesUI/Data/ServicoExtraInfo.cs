using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using Uqbar.Services.Framework;
using Uqbar.Services.Framework.Agendamento;
using Uqbar.Services.Framework.Mensagens;
using Uqbar.Services.Framework.WCF;
using Uqbar.ServicesUI.ViewModels;

namespace Uqbar.ServicesUI.Data
{
    public class ServicoExtraInfo : ServicoInfo, IServiceCallback, ISettings
    {
        #region Propriedades
        
        public DateTime Updated { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        private string _safeID = null;
        public string SafeID
        {
            get
            {
                if (string.IsNullOrEmpty(_safeID))
                {
                    _safeID = "";
                    MatchCollection col = Regex.Matches(this.URL, @"[A-Za-z0-9]*", RegexOptions.IgnoreCase);

                    foreach (Match item in col)
                    {
                        if (item.Length > 0)
                        {
                            _safeID += item.ToString();
                        }
                    }
                }

                return _safeID;
            }
        }

        #endregion

        #region Comunications

        private bool channelOpen = false;
        private IService channel = null;
        private DuplexChannelFactory<IService> channelFactory = null;

        private void channelFactory_Closed(object sender, EventArgs e)
        {
            channelOpen = false;
            channel = null;
        }

        private void channelFactory_Faulted(object sender, EventArgs e)
        {
            channelOpen = false;
            channel = null;
        }

        private void openChannel()
        {
            if (channelOpen == false || channelFactory.State != CommunicationState.Opened)
            {
                // WCF
                WSDualHttpBinding binding = new WSDualHttpBinding();
                binding.MaxReceivedMessageSize = int.MaxValue;
                EndpointAddress endpoint = new EndpointAddress(this.URL);
                channelFactory = new DuplexChannelFactory<IService>(this, binding, endpoint);
                channelFactory.Faulted += channelFactory_Faulted;
                channelFactory.Closed += channelFactory_Closed;

                channel = channelFactory.CreateChannel();

                channelOpen = false;
            }
        }

        private void closeChannel()
        {
            channelOpen = false;
            if (channelFactory != null && channelFactory.State == CommunicationState.Opened)
            {
                channelFactory.Close();
            }
            channel = null;

            this.Estado = EstadosEnum.Indisponivel;
            this.Updated = DateTime.UtcNow;
        }

        #endregion

        public void RefreshInfo()
        {
            // RefreshInfo Data
            try
            {
                openChannel();

                ServicoInfo remoteInfo = channel.GetInfo();
                CopyFrom(remoteInfo);
            }
            catch (Exception ex)
            {
                closeChannel();
            }
        }

        public Agenda GetAgenda()
        {
            Agenda agenda;

            try
            {
                openChannel();

                agenda = channel.GetAgenda();
            }
            catch (Exception ex)
            {
                closeChannel();
                agenda = new Agenda();
            }

            return agenda;
        }

        public bool SetAgenda(Agenda agenda)
        {
            try
            {
                openChannel();

                return channel.SetAgenda(agenda);
            }
            catch (Exception ex)
            {
                closeChannel();
                agenda = new Agenda();
            }

            return false;
        }

        public Mensagem[] GetMensagens(int start, int length)
        {
            Mensagem[] result;

            try
            {
                openChannel();

                result = channel.GetMensagens(start, length);
            }
            catch (Exception ex)
            {
                closeChannel();

                result = null;
            }

            return result;
        }

        public void OnInfoChanged(ServicoInfo newInfo)
        {
            CopyFrom(newInfo);
        }

        public void OnNewMessage(Mensagem newMsg)
        {

        }

        public void CopyFrom(ServicoInfo newInfo)
        {
            this.Estado = newInfo.Estado;
            this.NextCall = newInfo.NextCall;
            this.TimerInterval = newInfo.TimerInterval;
            this.ActionStart = newInfo.ActionStart;
            this.ActionEnd = newInfo.ActionEnd;

            this.Updated = DateTime.UtcNow;
        }

        public void CopyFrom(ServicoExtraInfo newInfo)
        {
            this.CopyFrom(newInfo as ServicoInfo);

            this.URL = newInfo.URL;
        }
    }
}