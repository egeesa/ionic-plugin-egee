using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egee.API.Contract
{
    public class ConfigurationData
    {
        public string Manufacturer { get; set; }
        public string RadioAddress { get; set; }
        public string Generation { get; set; }
        public string Medium { get; set; }
        public string ProductionNumber { get; set; }
        public string ProductionVersion { get; set; }
        public string RadioSendingInterval { get; set; }
        public string RemainingBatteryLifeTime { get; set; }
        public string CurrentDate { get; set; }
        public string CurrentTime { get; set; }
        public string RFKey { get; set; }
        public string PulseWeightNumerator { get; set; }
        public string PulseWeightDenominator { get; set; }
        public string VIF { get; set; }
        public string CIField { get; set; }
        public int IndexE { get; set; }
        public int IndexESecond { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public string FlagExcessFlow { get; set; }
        public string FlagLeakCurrent { get; set; }
        public string FlagLeakHistoric { get; set; }
        public string FlagMagFraudCurrent { get; set; }
        public string FlagMagFraudHistoric { get; set; }
        public string FlagMechFraudCurrent { get; set; }
        public string FlagMechFraudHistoric { get; set; }
        public string FlagMeterBlocked { get; set; }
        public string FlagReverseFlow { get; set; }
        public string FlagUnderFlow { get; set; }
        public string AlarmsHoldTime { get; set; }
        public string AlarmMeterBlocked { get; set; }
        public string AlarmReverseFlow { get; set; }
        public string AlarmLeakDetectWindow { get; set; }
        public string AlarmLeakDetectInterval { get; set; }
        public string AlarmUnderFlowRate { get; set; }
        public string AlarmUnderFlowTime { get; set; }
        public string AlarmExcessFlowRate { get; set; }
        public string AlarmExcessFlowTime { get; set; }
        public string HistoricFrame { get; set; }
    }
}
