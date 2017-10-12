using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Egee.Wcf.Service.Interface
{
    [ServiceContract]
    public interface ISappelService
    {
        [OperationContract]
        void Init();

        [OperationContract]
        string GetVersion();
    }
}
