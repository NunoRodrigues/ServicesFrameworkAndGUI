using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Uqbar.ServerMonitorService.WCF
{
    /*
    public class EndPoints
    {
        private Dictionary<string, ServiceHost> _services = new Dictionary<string, ServiceHost>();

        private Uri _baseAddress;

        public EndPoints(Uri baseAddress)
        {
            this._baseAddress = baseAddress;
        }

        public ServiceHost Start(Type serviceType, string MethodName)
        {
            Uri finalAddress = new Uri(_baseAddress, MethodName);

            // Create the ServiceHost.
            ServiceHost host = new ServiceHost(serviceType, finalAddress);

            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            host.Description.Behaviors.Add(smb);

            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each serviceType contract implemented
            // by the serviceType.
            host.Open();

            Console.WriteLine("The serviceType is ready at {0}", finalAddress);

            _services.Add(serviceType.ToString(), host);

            return host;
        }

        public void Stop(Type service)
        {
            ServiceHost host = _services[service.ToString()];

            // Close the ServiceHost.
            host.Close();

            _services.Remove(service.ToString());
        }
    }
    */
}
