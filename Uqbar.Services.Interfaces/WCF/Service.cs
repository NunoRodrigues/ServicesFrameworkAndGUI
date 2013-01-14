using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.Services.Framework.WCF
{
    public class Service
    {
        private Dictionary<string, ServiceHost> _services = new Dictionary<string, ServiceHost>();

        private Uri _baseAddress;

        public Service(Uri baseAddress)
        {
            this._baseAddress = baseAddress;
        }

        public ServiceHost Start(Type serviceType, string EndPointName)
        {
            Uri finalAddress = new Uri(_baseAddress, EndPointName);

            // Create the ServiceHost.
            ServiceHost host = new ServiceHost(serviceType, finalAddress);
            System.ServiceModel.Description.ServiceEndpoint endpoint = host.Description.Endpoints[0];
            WSDualHttpBinding binding = (WSDualHttpBinding) endpoint.Binding;
            binding.MaxReceivedMessageSize = int.MaxValue;

            //host.Description.Endpoints[0].Binding.
            /*
            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            host.Description.Behaviors.Add(smb);
            */

            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each serviceType contract implemented
            // by the serviceType.
            host.Open();

            _services.Add(serviceType.ToString(), host);

            return host;
        }

        public void Stop(Type service)
        {
            if (_services.ContainsKey(service.ToString()))
            {
                ServiceHost host = _services[service.ToString()];

                // Close the ServiceHost.
                host.Close();

                _services.Remove(service.ToString());
            }
        }
    }
}
