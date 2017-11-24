using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Egee.API.Contract
{
    
    public class FrameResponse
    {
        public string deviceId { get; set; }
        public string manuString { get; set; }
        public int ciField { get; set; }
        //Volume
        public string volumeCode { get; set; }
        public string volumeUnite { get; set; }
        public double volumeIndex { get; set; }
        public int volumeValue { get; set; }
        public string volumeFormat { get; set; }
        public int volumeExponent { get; set; }
        //Energy
        public string energyCode { get; set; }
        public string energyUnite { get; set; }
        public double energyIndex { get; set; }
        public int energyValue { get; set; }
        public string energyFormat { get; set; }
        public int energyExponent { get; set; }

        //Alarme
        public string alarmeCode { get; set; }
        public int alarmeFormated { get; set; }

        //Battery
        public string batteryCode { get; set; }
        public string batteryUnite { get; set; }
        public int batteryFormated { get; set; }
        //date
        public string date { get; set; }
    }

}
