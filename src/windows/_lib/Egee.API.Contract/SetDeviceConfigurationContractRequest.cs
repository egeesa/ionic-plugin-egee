using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egee.API.Contract
{
    public class SetDeviceConfigurationContractRequest
    {
        public string PortComEntrant { get; set; }
        public string PathScript { get; set; }
        public List<ConfigParam> ConfigParams { get; set; }
    }
}
