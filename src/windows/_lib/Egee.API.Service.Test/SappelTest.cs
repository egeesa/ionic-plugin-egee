using Egee.API.Contract;
using Egee.API.Service.Host;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Egee.API.Service.Test
{
    [TestFixture]
    public class SappelTest
    {
        SappelController SappelController { get; set; }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.UTF8.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }


        [OneTimeSetUp]
        public void Init()
        {
            SappelController = new SappelController();
        }

        [Test]
        [Category("SappelTest")]
        public void GetVersion()
        {
            var result = SappelController.GetVersion();
            string version = result.ToString();
        }

        //[Test]
        //[Category("SappelTest")]
        //public void ReadAllTelegram()
        //{
        //    List<FrameResponse> data = new List<FrameResponse>();
        //    data = SappelController.ReadAllTelegram("6", "0012f18e03e");
        //}

        [Test]
        [Category("SappelTest")]
        public void GetTelegrams()
        {
            string data = "";
            var result = SappelController.GetTelegrams("5", "0012f18e03e","5000");
            if (result != null)
                data = result.ToString();
        }

        [Test]
        [Category("SappelTest")]
        public void GetTelegramCompteur()
        {
            string data = "";
            var result = SappelController.GetTelegramCompteur("5", "0012f18e03e","C16BA789456","5000");
            if (result != null)
                data = result.ToString();
        }

        [Test]
        [Category("SappelTest")]
        public void GetDeviceConfiguration()
        {
            //QzpcVGVtcFxJWkFSQENTSQ==
            string path = EncodeTo64(@"C:\Temp\IZAR@CSI");
            var result = SappelController.GetDeviceConfiguration("5", path);
        }

        [Test]
        [Category("SappelTest")]
        public void SetDeviceConfiguration()
        {
            SetDeviceConfigurationContractRequest request = new SetDeviceConfigurationContractRequest();
            List<ConfigParam> configParamList = new List<ConfigParam>();
            ConfigParam configParam;


            //
            configParam = new ConfigParam
            {
                Name = "Manufacturer",
                Value = "SAP"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "RadioAddress",
                Value = "C06AB12345"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "Medium",
                Value = "9"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "ProductionNumber",
                Value = "40600228"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "RadioSendingInterval",
                Value = "8"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "CurrentDate",
                Value = "20180419"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "CurrentTime",
                Value = "1745"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "RFKey",
                Value = "39BC8A10E66D83F8"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "PulseWeightNumerator",
                Value = "1"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "PulseWeightDenominator",
                Value = "1"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "VIF",
                Value = "19"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "CIField",
                Value = "161"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "IndexE",
                Value = "18350"
            };
            configParamList.Add(configParam);
            //
            //configParam = new ConfigParam();
            //configParam.Name = "IndexESecond";
            //configParam.Value = "0";
            //configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "DueDate",
                Value = "20180101"
            };
            configParamList.Add(configParam);

            //
            configParam = new ConfigParam
            {
                Name = "AlarmsHoldTime",
                Value = "378"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmMeterBlocked",
                Value = "56"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmReverseFlow",
                Value = "15"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmLeakDetectWindow",
                Value = "9"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmLeakDetectInterval",
                Value = "30"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmUnderFlowRate",
                Value = "250"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmUnderFlowTime",
                Value = "56"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmExcessFlowRate",
                Value = "3031"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmExcessFlowTime",
                Value = "60"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "HistoricFrame",
                Value = "0"
            };
            configParamList.Add(configParam);

            //Port COM
            request.PortComEntrant = "7";

            //Path script
            request.PathScript = EncodeTo64(@"C:\Temp\IZAR@CSI");

            //liste des paramètres
            request.ConfigParams = configParamList;


            bool result = SappelController.SetDeviceConfiguration(request);

        }

        [Test]
        [Category("SappelTest")]
        public void GetConfiguration()
        {
            //QzpcVGVtcFxJWkFSQENTSQ==
            string path = EncodeTo64(@"C:\Temp\IZAR@CSI");
            var result = SappelController.GetConfiguration("5", path);
    
        }


        [Test]
        [Category("SappelTest")]
        public void SetConfiguration()
        {
            SetDeviceConfigurationContractRequest request = new SetDeviceConfigurationContractRequest();
            List<ConfigParam> configParamList = new List<ConfigParam>();
            ConfigParam configParam;
            

            //
            configParam = new ConfigParam
            {
                Name = "Manufacturer",
                Value = "SAP"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "RadioAddress",
                Value = "C06AB123456"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "Medium",
                Value = "9"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "ProductionNumber",
                Value = "40600228"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "RadioSendingInterval",
                Value = "8"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "CurrentDate",
                Value = "20180419"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "CurrentTime",
                Value = "1045"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "RFKey",
                Value = "39BC8A10E66D83F8"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "PulseWeightNumerator",
                Value = "1"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "PulseWeightDenominator",
                Value = "1"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "VIF",
                Value = "19"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "CIField",
                Value = "161"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "IndexE",
                Value = "52364"
            };
            configParamList.Add(configParam);
            //
            //configParam = new ConfigParam();
            //configParam.Name = "IndexESecond";
            //configParam.Value = "0";
            //configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "DueDate",
                Value = "20180101"
            };
            configParamList.Add(configParam);

            //
            configParam = new ConfigParam
            {
                Name = "AlarmsHoldTime",
                Value = "378"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmMeterBlocked",
                Value = "56"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmReverseFlow",
                Value = "15"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmLeakDetectWindow",
                Value = "9"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmLeakDetectInterval",
                Value = "30"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmUnderFlowRate",
                Value = "250"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmUnderFlowTime",
                Value = "56"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmExcessFlowRate",
                Value = "3031"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "AlarmExcessFlowTime",
                Value = "60"
            };
            configParamList.Add(configParam);
            //
            configParam = new ConfigParam
            {
                Name = "HistoricFrame",
                Value = "0"
            };
            configParamList.Add(configParam);

            //Port COM
            request.PortComEntrant = "6";

            //Path script
            request.PathScript = EncodeTo64(@"C:\Temp\IZAR@CSI");

            //liste des paramètres
            request.ConfigParams = configParamList;


            bool result = SappelController.SetConfiguration(request);
            
        }

    }
}
