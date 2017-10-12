using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Egee.Wcf.Service.Host
{
    public partial class SappelService : ServiceBase
    {
        //https://www.codeproject.com/Articles/653493/WCF-Hosting-with-Windows-Service
        //https://stackoverflow.com/questions/1195478/how-to-make-a-net-windows-service-start-right-after-the-installation/1195621#1195621
        private ServiceHost m_svcHost = null;
        public SappelService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (m_svcHost != null) m_svcHost.Close();

            string strAdrHTTP = "http://localhost:9001/SappelService";
            string strAdrTCP = "net.tcp://localhost:9002/SappelService";

            Uri[] adrbase = { new Uri(strAdrHTTP), new Uri(strAdrTCP) };
            m_svcHost = new ServiceHost(typeof(Egee.Wcf.Service.SappelService), adrbase);

            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            m_svcHost.Description.Behaviors.Add(mBehave);

            BasicHttpBinding httpb = new BasicHttpBinding();
            m_svcHost.AddServiceEndpoint(typeof(Egee.Wcf.Service.Interface.ISappelService), httpb, strAdrHTTP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

            NetTcpBinding tcpb = new NetTcpBinding();
            m_svcHost.AddServiceEndpoint(typeof(Egee.Wcf.Service.Interface.ISappelService), tcpb, strAdrTCP);
            m_svcHost.AddServiceEndpoint(typeof(IMetadataExchange),
            MetadataExchangeBindings.CreateMexTcpBinding(), "mex");

            m_svcHost.Open();
        }

        protected override void OnStop()
        {
            if (m_svcHost != null)
            {
                m_svcHost.Close();
                m_svcHost = null;
            }
        }
    }
}
