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
            System.ServiceModel.Description.ServiceEndpoint endpoint = host.Description.Endpoints[0]; // Unsafe, mas de proposito. quero q expluda caso haja esquecimentos
            WSDualHttpBinding binding = (WSDualHttpBinding) endpoint.Binding;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ClientBaseAddress = GetCallbackAddress(finalAddress);

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

        public static Uri GetCallbackAddress(string baseAddress)
        {
            return GetCallbackAddress(new Uri(baseAddress));
        }

        public static Uri GetCallbackAddress(Uri baseAddress)
        {
            return new Uri(baseAddress, "/Callback");
        }
    }
}
