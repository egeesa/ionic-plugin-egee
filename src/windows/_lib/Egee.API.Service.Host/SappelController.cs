using Egee.API.Contract;
using IZAR_CSIXLib;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;


namespace Egee.API.Service.Host
{
    [RoutePrefix("sappel")]
    public class SappelController : ApiController
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();


        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        #region INITIALISATION 

            [HttpGet]
            [Route("init")]
            public void Init()
            {
                HyScript hyScript = new HyScript();
                string initResponse = hyScript.call("init");
                string licenseResponse = hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return License.check('EGEEEGEE','I1ARCQI0')");
            }

            [HttpGet]
            [Route("getversion")]
            public string GetVersion()
            {
                try
                {
                    HyScript hyScript = new HyScript();

                    var version = hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return Environment.getVersion()");
                    if (version == null)
                        return "";

                    return version;
                }
                catch (Exception ex)
                {
                    _logger.Info($"Echec get version : {ex}");
                    return null;
                }
            }

        #endregion

        #region LECTURE DES MODULES 

        [HttpGet]
        [Route("telegrams/{portcom}/{macadresseprt}/{nombreappel}")]
        public List<FrameResponse> GetTelegrams(string portcom, string macadresseprt, string nombreappel)
        {
            try
            {
                List<FrameResponse> myFrameList = new List<FrameResponse>();

                HyScript hyScript = new HyScript();
                string cheminLog = @"C:\CSILogfile.log";
                int appel = Convert.ToInt32(nombreappel); // 1000 = 1 seconde
                int port = Convert.ToInt32(portcom);
                string comPort = "com://" + port;
                string response = "false";
                string dataJSON = "";
                string rawDataInterpretJSON = "";
                string cmdClear = "if(dc~=nil) then dc:close(); end cl=nil pl=nil dc=nil tsf=nil";
                string cmdLicence = "return License.check('EGEEEGEE','I1ARCQI0')";

                string cmdLecture = " setLogLevel(-1); setLogFileName('" + cheminLog.Replace(@"\", @"\\") + "');" +
                                   " dc = DataConcentrator.new();" +
                                   " ds = BtReceiverDataSource.new();" +
                                   " ds:setDataConcentrator(ref(dc));" +
                                   " pl = IPhysicalLayer.new('" + comPort + "');" +
                                   " cl = ICommunicationLayer.new('hybtoh://', ref(pl));" +
                                   " ds:setCommunicationLayer(ref(cl));" +
                                   " local ri = RadioInterpreter.new();" +
                                   " ri:setProcessLevel();" +
                                   " dc:setRadioInterpreter(ri);" +
                                   " ri:enableDecryption(false);" +
                                   " tsf=ToStringFormater.new();" +
                                   " tsf:setAddSPDEAddress(true);" +
                                   " setFormater(tsf);" +
                                   " ds:setSourceId('" + macadresseprt + "');" +
                                   " ds:config('timeoutNoAnswer', 150);" +
                                   " ds:config('timeoutNoDataFollows', 2);" +
                                   " ds:config('timeoutAfterTelegram', 10); return true";


                string cmdInitStructure = "IDeviceID.setDefaultAddressInterpretationMode(0);" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(1, 'LLDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(2, 'ddDDDDDDLLL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(3, 'DDDDDDDDL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(4, 'DDLLLDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(5, 'DDDDDDdd');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(6, 'DDLLDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(7, 'DDDDDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(8, 'LDDDDDDDDL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(9, 'LDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(10, 'XXXXXXXXXXXX');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(11, 'DDDDDD');"+
                                     "IDeviceID.setMeterNumberAddressInterpretation(12, 'LDDLLDDDDD');";

                //Check Licence
                string licenceOK = hyScript.call(cmdLicence);
                _logger.Info("Vérification Licence : " + licenceOK);

                if (licenceOK != "true")
                {
                    _logger.Info("Licence invalide.");
                    return null;
                }
                else
                {
                    //clear
                    hyScript.call(cmdClear);

                    //Init structure
                    hyScript.call(cmdInitStructure);

                    //Init lecture
                    response = hyScript.call(cmdLecture);

                    if (response != "true")
                    {
                        _logger.Info("Echec initialisation de la lecture.");
                        return null;
                    }
                    else
                    {
                        //Timer
                        if (appel == 0)
                            appel = 8000;
                        //start async
                        _logger.Info("Début de la synchronisation des signaux. startASync()");
                        hyScript.call("ds:startASync()");

                        Thread.Sleep(appel);

                        //stop async
                        _logger.Info("Fin de la synchronisation des signaux. stopASync()");
                        hyScript.call("ds:stopASync()");

                        _logger.Info("Nombre de télégrammes traités: " + hyScript.call("return dc:getNumberOfProcessedTelegrams()"));
                        _logger.Info("Nombre de télégrammes collectés: " + hyScript.call("return dc:getNumberOfCollectedTelegrams()"));
                        _logger.Info("Nombre de faux télégrammes : " + hyScript.call("return dc:getNumberOfFalseTelegrams()"));


                        string foundedDevice = hyScript.call("return dc:getNumberOfFoundedDevices()");
                        _logger.Info("Nombre de modules trouvés : " + foundedDevice);
                        
                        //
                        if (Convert.ToInt32(foundedDevice) == 0)
                        {
                            _logger.Info("Pas de télégrammes reçus.");
                            return null;
                        }
                        else
                        {
                            //Récupération et traitement des télégrammes 
                            dataJSON = hyScript.call(" dl = dc:getDeviceList() f = ToStringFormater.new() f:setAddSPDEAddress(true) return dl:__tostring(dl:getBeginDeviceDescriptionIterator(true))");

                            FrameResponse dataFrame;
                            SappelResponseContract sappelResponseContract = JsonConvert.DeserializeObject<SappelResponseContract>(dataJSON);

                            List<Entry> entries = new List<Entry>();

                            entries = sappelResponseContract.entries.ToList();

                            if (entries.Any())
                            {
                                foreach (var entry in entries)
                                {
                                    List<Telegram> telegrams = new List<Telegram>();
                                    telegrams = entry.value.meteringPoint.telegrams.ToList();

                                    foreach (var telegram in telegrams)
                                    {
                                        dataFrame = new FrameResponse();

                                        if (telegram.mBusData.deviceId != null)
                                        {
                                            if (telegram.mBusData.deviceId.spdeid != null)
                                            {
                                                dataFrame.deviceId = telegram.mBusData.deviceId.spdeid.spde;
                                                dataFrame.structure = telegram.mBusData.deviceId.spdeid.format;
                                            }

                                            if (telegram.mBusData.deviceId.manuString != null)
                                                dataFrame.manuString = telegram.mBusData.deviceId.manuString;

                                            if (telegram.mBusData.deviceId.idAsString != null)
                                            {
                                                string idAds = telegram.mBusData.deviceId.idAsString;
                                                dataFrame.deviceId = ExtractSPDEString(idAds);
                                            }
                                        }

                                        dataFrame.ciField = telegram.mBusData.ciField;

                                        dataFrame.rssi = telegram.telegramTypeSpecifica.qualityIndicator.rssi;

                                        //Date
                                        if(telegram.mBusData.timestamp != null)
                                        {
                                            if(telegram.mBusData.timestamp.valid == true)
                                            {
                                                string dateString = telegram.mBusData.timestamp.day.ToString() + "/" + telegram.mBusData.timestamp.month.ToString() + "/" + telegram.mBusData.timestamp.year.ToString();
                                                string heureString = telegram.mBusData.timestamp.hour.ToString() + ":" + telegram.mBusData.timestamp.minute.ToString();
                                                dataFrame.date = dateString+" "+ heureString;
                                            }
                                             
                                        }

                                        //Alarmes
                                        if (telegram.mBusData.alarmField != null)
                                        {
                                            if (telegram.mBusData.alarmField.data != "00" && telegram.mBusData.alarmField.data != "00 00")
                                            {
                                                dataFrame.alarmeCode = TelegramAlarmFormat(telegram.mBusData.alarmField.data);

                                            }
                                        }

                                        //raw data
                                        string rawData = TelegramFormat(telegram.mBusData.rawData.data);
                                        rawDataInterpretJSON = Interpreter(rawData);

                                        Telegram telegramResponse = JsonConvert.DeserializeObject<Telegram>(rawDataInterpretJSON);
                                        List<MBusValue> mBusValues = telegramResponse.mBusData.mBusValues.ToList();

                                        if (mBusValues.Any())
                                        {
                                            foreach (var mBusValue in mBusValues)
                                            {

                                                if ((mBusValue.valid == true))
                                                {
                                                    if (mBusValue.dimension != null)
                                                    {
                                                        if (mBusValue.dimension.stringId == "VOLUME")
                                                        {
                                                            //volume
                                                            if (mBusValue.storageNumber == 0 && mBusValue.tariffNumber == 0 && mBusValue.subUnitNumber == 0)
                                                            {
                                                                dataFrame.volumeValue = Convert.ToInt32(mBusValue.formated);
                                                                dataFrame.volumeCode = mBusValue.dimension.stringId;
                                                                dataFrame.volumeUnite = mBusValue.unit.stringId;
                                                                dataFrame.volumeExponent = Convert.ToInt32(mBusValue.exponent);
                                                                dataFrame.volumeIndex = Math.Truncate(CalculIndex(Convert.ToInt32(mBusValue.formated), Convert.ToInt32(mBusValue.exponent)));
                                                                dataFrame.volumeFormat = dataFrame.volumeCode + "  " + dataFrame.volumeIndex + "  m3";
                                                            }
                                                        }
                                                        if (mBusValue.dimension.stringId == "ENERGY")
                                                        {
                                                            //Energy
                                                            if (mBusValue.storageNumber == 0 && mBusValue.tariffNumber == 0 && mBusValue.subUnitNumber == 0)
                                                            {
                                                                dataFrame.energyValue = Convert.ToInt32(mBusValue.formated);
                                                                dataFrame.energyCode = mBusValue.dimension.stringId;
                                                                dataFrame.energyUnite = mBusValue.unit.stringId;
                                                                dataFrame.energyExponent = Convert.ToInt32(mBusValue.exponent);
                                                                dataFrame.energyIndex = Math.Truncate(CalculIndex(Convert.ToInt32(mBusValue.formated), Convert.ToInt32(mBusValue.exponent)));
                                                                dataFrame.energyFormat = dataFrame.energyCode + "  " + dataFrame.energyIndex + " KWh";
                                                            }
                                                        }

                                                        if (mBusValue.dimension.stringId == "OPERATIONTIMEBATTERY")
                                                        {
                                                            if (mBusValue.unit.stringId == "YEAR")
                                                            {
                                                                dataFrame.batteryFormated = Convert.ToInt32(mBusValue.formated) / 12;
                                                            }
                                                            if (mBusValue.unit.stringId == "DAY")
                                                            {
                                                                dataFrame.batteryFormated = Convert.ToInt32(mBusValue.formated) / 365;
                                                            }

                                                            dataFrame.batteryCode = mBusValue.dimension.stringId;
                                                            dataFrame.batteryUnite = mBusValue.unit.stringId;
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Info("Pas de mBusValues reçus.");
                                            return null;
                                        }

                                        if (!myFrameList.Where(x => x.deviceId == dataFrame.deviceId).Any())
                                        {
                                            if ((dataFrame.volumeCode != null || dataFrame.energyCode != null) && dataFrame.ciField != 0 && dataFrame.deviceId != null)
                                                myFrameList.Add(dataFrame);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                _logger.Info("Pas de télégrammes reçus.");
                                return null;
                            }
                            
                        }
                    }
                }
                if (myFrameList == null)
                    return null;
                //cleanup
                _logger.Info("Cleanup");
                hyScript.call("dc:getDeviceList():clear()");

                _logger.Info("Resultat: " + JsonConvert.SerializeObject(myFrameList));
                return myFrameList;
            }
            catch (Exception ex)
            {
                _logger.Info($"Echec get telegrams : {ex}");
                return null;
            }
        }

        [HttpGet]
        [Route("telegramcompteur/{portcom}/{macadresseprt}/{numerocompteur}/{nombreappel}")]
        public List<FrameResponse> GetTelegramCompteur(string portcom, string macadresseprt, string numerocompteur, string nombreappel)
        {
            try
            {
                List<FrameResponse> myFrameList = new List<FrameResponse>();

                HyScript hyScript = new HyScript();
                string cheminLog = @"C:\CSILogfile.log";
                int appel = Convert.ToInt32(nombreappel); // 1000 = 1 seconde
                string compteur = "PSAP0" + numerocompteur.ToUpper() + "000";
                int port = Convert.ToInt32(portcom);
                string comPort = "com://" + port;
                string response = "false";
                string dataJSON = "";
                string rawDataInterpretJSON = "";
                string cmdClear = "if(dc~=nil) then dc:close(); end cl=nil pl=nil dc=nil tsf=nil";
                string cmdLicence = "return License.check('EGEEEGEE','I1ARCQI0')";

                string cmdLecture = " setLogLevel(-1); setLogFileName('" + cheminLog.Replace(@"\", @"\\") + "');" +
                                   " dc = DataConcentrator.new();" +
                                   " ds = BtReceiverDataSource.new();" +
                                   " ds:setDataConcentrator(ref(dc));" +
                                   " pl = IPhysicalLayer.new('" + comPort + "');" +
                                   " cl = ICommunicationLayer.new('hybtoh://', ref(pl));" +
                                   " ds:setCommunicationLayer(ref(cl));" +
                                   " local ri = RadioInterpreter.new();" +
                                   " ri:setProcessLevel();" +
                                   " dc:setRadioInterpreter(ri);" +
                                   " ri:enableDecryption(false);" +
                                   " tsf=ToStringFormater.new();" +
                                   " tsf:setAddSPDEAddress(true);" +
                                   " setFormater(tsf);" +
                                   " ds:setSourceId('" + macadresseprt + "');" +
                                   " ds:config('timeoutNoAnswer', 150);" +
                                   " ds:config('timeoutNoDataFollows', 2);" +
                                   " ds:config('timeoutAfterTelegram', 10); return true";


                string cmdInitStructure = "IDeviceID.setDefaultAddressInterpretationMode(0);" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(1, 'LLDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(2, 'ddDDDDDDLLL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(3, 'DDDDDDDDL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(4, 'DDLLLDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(5, 'DDDDDDdd');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(6, 'DDLLDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(7, 'DDDDDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(8, 'LDDDDDDDDL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(9, 'LDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(10, 'XXXXXXXXXXXX');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(11, 'DDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(12, 'LDDLLDDDDD');";

                //Check Licence
                string licenceOK = hyScript.call(cmdLicence);
                _logger.Info("Vérification Licence : " + licenceOK);

                if (licenceOK != "true")
                {
                    _logger.Info("Licence invalide.");
                    return null;
                }
                else
                {
                    //clear
                    hyScript.call(cmdClear);

                    //Init structure
                    hyScript.call(cmdInitStructure);

                    //Init lecture
                    response = hyScript.call(cmdLecture);

                    if (response != "true")
                    {
                        _logger.Info("Echec initialisation de la lecture.");
                        return null;
                    }
                    else
                    {
                        //Timer
                        if (appel == 0)
                            appel = 8000;

                        //start async
                        _logger.Info("Début réception des signaux par le PRT. startASync()");
                        hyScript.call("ds:startASync()");

                        Thread.Sleep(appel);

                        //stop async
                        _logger.Info("Fin réception des signaux par le PRT. stopASync()");
                        hyScript.call("ds:stopASync()");

                        _logger.Info("Nombre de télégrammes traités: " + hyScript.call("return dc:getNumberOfProcessedTelegrams()"));
                        _logger.Info("Nombre de télégrammes collectés: " + hyScript.call("return dc:getNumberOfCollectedTelegrams()"));
                        _logger.Info("Nombre de faux télégrammes : " + hyScript.call("return dc:getNumberOfFalseTelegrams()"));


                        string foundedDevice = hyScript.call("return dc:getNumberOfFoundedDevices()");
                        _logger.Info("Nombre de modules trouvés : " + foundedDevice);

                        //
                        if (Convert.ToInt32(foundedDevice) == 0)
                        {
                            _logger.Info("Pas de télégrammes reçus.");
                            return null;
                        }
                        else
                        {
                            //Récupération et traitement des télégrammes 
                            dataJSON = hyScript.call(" dl = dc:getDeviceList() f = ToStringFormater.new() f:setAddSPDEAddress(true) return dl:__tostring(dl:getBeginDeviceDescriptionIterator(true))");

                            FrameResponse dataFrame;
                            SappelResponseContract sappelResponseContract = JsonConvert.DeserializeObject<SappelResponseContract>(dataJSON);

                            List<Entry> entries = new List<Entry>();

                            entries = sappelResponseContract.entries.Where(e => e.key.ToUpper() == compteur).ToList();

                            if (entries.Any())
                            {
                                foreach (var entry in entries)
                                {
                                    List<Telegram> telegrams = new List<Telegram>();
                                    telegrams = entry.value.meteringPoint.telegrams.ToList();

                                    foreach (var telegram in telegrams)
                                    {
                                        dataFrame = new FrameResponse();

                                        if (telegram.mBusData.deviceId != null)
                                        {
                                            if (telegram.mBusData.deviceId.spdeid != null)
                                            {
                                                dataFrame.deviceId = telegram.mBusData.deviceId.spdeid.spde;
                                                dataFrame.structure = telegram.mBusData.deviceId.spdeid.format;
                                            }

                                            if (telegram.mBusData.deviceId.manuString != null)
                                                dataFrame.manuString = telegram.mBusData.deviceId.manuString;

                                            if (telegram.mBusData.deviceId.idAsString != null)
                                            {
                                                string idAds = telegram.mBusData.deviceId.idAsString;
                                                dataFrame.deviceId = ExtractSPDEString(idAds);
                                            }
                                        }

                                        dataFrame.ciField = telegram.mBusData.ciField;

                                        dataFrame.rssi = telegram.telegramTypeSpecifica.qualityIndicator.rssi;

                                        //Date
                                        if (telegram.mBusData.timestamp != null)
                                        {
                                            if (telegram.mBusData.timestamp.valid == true)
                                            {
                                                string dateString = telegram.mBusData.timestamp.day.ToString() + "/" + telegram.mBusData.timestamp.month.ToString() + "/" + telegram.mBusData.timestamp.year.ToString();
                                                string heureString = telegram.mBusData.timestamp.hour.ToString() + ":" + telegram.mBusData.timestamp.minute.ToString();
                                                dataFrame.date = dateString + " " + heureString;
                                            }

                                        }

                                        //Alarmes
                                        if (telegram.mBusData.alarmField != null)
                                        {
                                            if (telegram.mBusData.alarmField.data != "00" && telegram.mBusData.alarmField.data != "00 00")
                                            {
                                                dataFrame.alarmeCode = TelegramAlarmFormat(telegram.mBusData.alarmField.data);

                                            }
                                        }

                                        //raw data
                                        string rawData = TelegramFormat(telegram.mBusData.rawData.data);
                                        rawDataInterpretJSON = Interpreter(rawData);

                                        Telegram telegramResponse = JsonConvert.DeserializeObject<Telegram>(rawDataInterpretJSON);
                                        List<MBusValue> mBusValues = telegramResponse.mBusData.mBusValues.ToList();

                                        if (mBusValues.Any())
                                        {
                                            foreach (var mBusValue in mBusValues)
                                            {

                                                if ((mBusValue.valid == true))
                                                {
                                                    if (mBusValue.dimension != null)
                                                    {
                                                        if (mBusValue.dimension.stringId == "VOLUME")
                                                        {
                                                            //volume
                                                            if (mBusValue.storageNumber == 0 && mBusValue.tariffNumber == 0 && mBusValue.subUnitNumber == 0)
                                                            {
                                                                dataFrame.volumeValue = Convert.ToInt32(mBusValue.formated);
                                                                dataFrame.volumeCode = mBusValue.dimension.stringId;
                                                                dataFrame.volumeUnite = mBusValue.unit.stringId;
                                                                dataFrame.volumeExponent = Convert.ToInt32(mBusValue.exponent);
                                                                dataFrame.volumeIndex = Math.Truncate(CalculIndex(Convert.ToInt32(mBusValue.formated), Convert.ToInt32(mBusValue.exponent)));
                                                                dataFrame.volumeFormat = dataFrame.volumeCode + "  " + dataFrame.volumeIndex + "  m3";
                                                            }
                                                        }
                                                        if (mBusValue.dimension.stringId == "ENERGY")
                                                        {
                                                            //Energy
                                                            if (mBusValue.storageNumber == 0 && mBusValue.tariffNumber == 0 && mBusValue.subUnitNumber == 0)
                                                            {
                                                                dataFrame.energyValue = Convert.ToInt32(mBusValue.formated);
                                                                dataFrame.energyCode = mBusValue.dimension.stringId;
                                                                dataFrame.energyUnite = mBusValue.unit.stringId;
                                                                dataFrame.energyExponent = Convert.ToInt32(mBusValue.exponent);
                                                                dataFrame.energyIndex = Math.Truncate(CalculIndex(Convert.ToInt32(mBusValue.formated), Convert.ToInt32(mBusValue.exponent)));
                                                                dataFrame.energyFormat = dataFrame.energyCode + "  " + dataFrame.energyIndex + " KWh";
                                                            }
                                                        }

                                                        if (mBusValue.dimension.stringId == "OPERATIONTIMEBATTERY")
                                                        {
                                                            if (mBusValue.unit.stringId == "YEAR")
                                                            {
                                                                dataFrame.batteryFormated = Convert.ToInt32(mBusValue.formated) / 12;
                                                            }
                                                            if (mBusValue.unit.stringId == "DAY")
                                                            {
                                                                dataFrame.batteryFormated = Convert.ToInt32(mBusValue.formated) / 365;
                                                            }

                                                            dataFrame.batteryCode = mBusValue.dimension.stringId;
                                                            dataFrame.batteryUnite = mBusValue.unit.stringId;
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        else
                                        {
                                            _logger.Info("Pas de mBusValues reçus.");
                                            return null;
                                        }

                                        if (!myFrameList.Where(x => x.deviceId == dataFrame.deviceId).Any())
                                        {
                                            if ((dataFrame.volumeCode != null || dataFrame.energyCode != null) && dataFrame.ciField != 0 && dataFrame.deviceId != null)
                                                myFrameList.Add(dataFrame);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                _logger.Info("Pas de télégrammes reçus pour le compteur N° "+ numerocompteur.ToUpper() + ".");
                                return null;
                            }

                        }
                    }
                }
                if (myFrameList == null)
                    return null;
                //cleanup
                _logger.Info("Cleanup");
                hyScript.call("dc:getDeviceList():clear()");

                _logger.Info("Resultat: " + JsonConvert.SerializeObject(myFrameList));
                return myFrameList;
            }
            catch (Exception ex)
            {
                _logger.Info($"Echec get telegrams : {ex}");
                return null;
            }
        }


        #endregion

        #region PROGRAMMATION DES MODULES
        //-------------------PROGRAMMATION------------------------------------------
        [HttpGet]
        [Route("getdeviceconfiguration/{portcom}/{pathconfig}")]
        public List<ConfigParam> GetDeviceConfiguration(string portcom, string pathconfig)
        {
            try
            {
                //@"C:\Temp\IZAR@CSI";
                HyScript hyScript = new HyScript();
                string path = DecodeFrom64(pathconfig);

                string comPort = "com://" + Convert.ToInt32(portcom);
                string sBTHead = "hybtoh://";
                string sScript = "";
                string sRep = "";
                string sList = "";
                string deviceModule = path + @"\devicemodule";
                string sS1 = deviceModule.Replace(@"\", @"\\");
                string sS2 = "";
                string sS3 = "";
                string response = "";

                List<ConfigParam> configParamList = new List<ConfigParam>();
                ConfigParam configParam;
                string luaScriptPath = path + @"\script\script.lua";
                sScript = luaScriptPath.Replace(@"\", @"\\");


                _logger.Info("--------GET MODULE CONFIGURATION ------------------");

                //Check Licence
                string licenceOK = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
                _logger.Info("Vérification Licence : " + licenceOK);

                if (licenceOK != "true")
                {
                    _logger.Info("Licence invalide.");
                    return null;
                }
                else
                {
                    //clear last exception
                    hyScript.call("clearLastException()");

                    //Chargement du script de configuration
                    _logger.Info("--- Load configuration script : " + sScript);
                    sRep = hyScript.call("return exec('" + sScript + "')");

                    _logger.Info("Result: " + sRep);

                    if (sRep != luaScriptPath)
                    {
                        string exception = hyScript.call("return getLastException();");
                        _logger.Info("Last Exception: " + exception);
                        _logger.Info("Echec chargement script de configuration.");
                        return null;
                    }
                    else
                    {
                        //clear last exception
                        hyScript.call("clearLastException()");

                        //clear
                        _logger.Info("---Clear data configuration.");
                        sRep = hyScript.call("clear();");

                        //Version CSI
                        sRep = hyScript.call("return Environment.getVersion();");
                        _logger.Info("IZAR@CSI Version: " + sRep);

                        // Start identification of a product
                        _logger.Info("---  Start identification of a product : " + "return indentifyProduct('" + comPort + "', '" + sBTHead + "', '" + sS1 + "', '" + sS2 + "', '" + sS3 + "')");
                        sRep = hyScript.call("return indentifyProduct('" + comPort + "', '" + sBTHead + "', '" + sS1 + "', '" + sS2 + "', '" + sS3 + "')");
                        _logger.Info("Identification result: " + sRep);

                        if (sRep != "true")
                        {
                            string exception = hyScript.call("return getLastException();");
                            _logger.Info("Last Exception: " + exception);
                            _logger.Info("Echec identification du module.");
                            sRep = hyScript.call("clear();");
                            return null;
                        }
                        else
                        {
                            //Description: Nom du produit
                            response = hyScript.call("return getDescription()");
                            _logger.Info("--- Description: " + response);
                            configParam = new ConfigParam();
                            configParam.Name = "Description";
                            configParam.Value = response;
                            configParamList.Add(configParam);


                            //Nom du script de config en cours d'emploi
                            response = hyScript.call("return getDMName()");
                            _logger.Info("--- DM Nom: " + response);
                            configParam = new ConfigParam();
                            configParam.Name = "DMName";
                            configParam.Value = response;
                            configParamList.Add(configParam);

                            //Version du script de config en cours d'emploi
                            response = hyScript.call("return getDMVersion()");
                            _logger.Info("--- DM Version: " + response);
                            configParam = new ConfigParam();
                            configParam.Name = "DMVersion";
                            configParam.Value = response;
                            configParamList.Add(configParam);

                            //----- Get Paramater list                 
                            sRep = hyScript.call("return getParamNameList()");
                            sList = sRep;
                            while (sRep != "")
                            {
                                sRep = hyScript.call("return retMobile(0)");
                                sList = sList + sRep;
                            }

                            //Parse parameter list to get the value of parameters ---char(9)=Tab=\t
                            string[] tabNames = sList.Split((char)9);

                            if (tabNames == null)
                            {
                                string exception = hyScript.call("return getLastException();");
                                _logger.Info("Last Exception: " + exception);
                                _logger.Info("Echec chargement liste des paramètres.");
                                return null;
                            }
                            else
                            {

                                foreach (var nomParam in tabNames)
                                {
                                    string result = hyScript.call("return getValue('" + nomParam + "')");
                                    configParam = new ConfigParam();
                                    configParam.Name = nomParam;
                                    configParam.Value = result;
                                    configParamList.Add(configParam);
                                    _logger.Info(configParam.Name + " ====> " + configParam.Value);
                                }
                            }
                        }
                    }
                }
                if (configParamList == null)
                    return null;
                sRep = hyScript.call("clear();");
                _logger.Info("Resultat: " + JsonConvert.SerializeObject(configParamList));

                return configParamList;
            }
            catch (Exception ex)
            {
                _logger.Info($"Echec get configuration : {ex}");
                return null;
            }
        }

        [HttpPost]
        [Route("setdeviceconfiguration")]
        public bool SetDeviceConfiguration(SetDeviceConfigurationContractRequest request)
        {
            try
            {
                HyScript hyScript = new HyScript();

                string comPort = "com://" + Convert.ToInt32(request.PortComEntrant);
                string sBTHead = "hybtoh://";
                string sScript = "";
                string sRep = "";
                string result = "";
                string deviceModule = DecodeFrom64(request.PathScript) + @"\devicemodule";
                string sS1 = deviceModule.Replace(@"\", @"\\");
                string sS2 = "";
                string sS3 = "";
                string luaScriptPath = DecodeFrom64(request.PathScript) + @"\script\script.lua";
                sScript = luaScriptPath.Replace(@"\", @"\\");


                _logger.Info("--------WRITE BACK ------------------");

                //Check Licence
                string licenceOK = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
                _logger.Info("Vérification Licence : " + licenceOK);

                if (licenceOK != "true")
                {
                    _logger.Info("Licence invalide.");
                    return false;
                }
                else
                {
                    //clear last exception
                    hyScript.call("clearLastException()");

                    //clear
                    _logger.Info("---Clear data configuration.");
                    sRep = hyScript.call("clear();");

                    //Version CSI
                    sRep = hyScript.call("return Environment.getVersion();");
                    _logger.Info("IZAR@CSI Version: " + sRep);

                    //Chargement du script de configuration
                    _logger.Info("--- Load configuration script : " + sScript);
                    sRep = hyScript.call("return exec('" + sScript + "')");

                    if (sRep != luaScriptPath)
                    {
                        string exception = hyScript.call("return getLastException();");
                        _logger.Info("Last Exception: " + exception);
                        _logger.Info("Echec chargement script de configuration.");
                        return false;
                    }
                    else
                    {
                        //clear last exception
                        hyScript.call("clearLastException()");

                        // Start identification of a product
                        _logger.Info("---  Start identification of a product : " + "return indentifyProduct('" + comPort + "', '" + sBTHead + "', '" + sS1 + "', '" + sS2 + "', '" + sS3 + "')");
                        sRep = hyScript.call("return indentifyProduct('" + comPort + "', '" + sBTHead + "', '" + sS1 + "', '" + sS2 + "', '" + sS3 + "')");

                        if (sRep != "true")
                        {
                            string exception = hyScript.call("return getLastException();");
                            _logger.Info("Last Exception: " + exception);
                            _logger.Info("Echec identification du module.");
                            return false;
                        }
                        else
                        {
                            ConfigParam configParam;
                            string newMeterAddress = "";
                            string newRadioAddress = "";

                            //Vérifier que le module connecté est égale au module en cours de programmation
                            string productNumberConnect = hyScript.call("return dc:getValue('ProductionNumber')");
                            configParam = request.ConfigParams.Where(c => c.Name == "ProductionNumber").FirstOrDefault();

                            if (configParam.Value != productNumberConnect)
                            {
                                _logger.Info("Le module à configurer est différent du module connecté. Module connecté: " + productNumberConnect + " || Module à configurer: " + configParam.Value);
                                return false;
                            }
                            else
                            {
                                string structureRadioAddress = "";
                                StructureCompteur structureCompteur = new StructureCompteur();

                                //Contôle code fabricant
                                configParam = request.ConfigParams.Where(c => c.Name == "Manufacturer").FirstOrDefault();
                                string manufacturer = configParam.Value;

                                //Code fabricannt = "SAP"
                                if (manufacturer != null && manufacturer == "SAP")
                                {
                                    //Liste des structures
                                    List<StructureCompteur> structureList = new List<Contract.StructureCompteur>();
                                    structureList = InitialiserListeStructure();

                                    configParam = new ConfigParam();
                                    configParam = request.ConfigParams.Where(c => c.Name == "RadioAddress").FirstOrDefault();
                                    if (configParam != null)
                                        newRadioAddress = configParam.Value;

                                    configParam = new ConfigParam();
                                    configParam = request.ConfigParams.Where(c => c.Name == "MeterAddress").FirstOrDefault();
                                    if (configParam != null)
                                        newMeterAddress = configParam.Value;

                                    //Contôle structure
                                    if (newRadioAddress != "" || newRadioAddress != null)
                                    {
                                        structureRadioAddress = StructureCompteur(newRadioAddress);
                                        structureCompteur = structureList.Where(s => s.Format == structureRadioAddress).FirstOrDefault();
                                        if(structureCompteur == null)
                                        {
                                            structureCompteur = new StructureCompteur
                                            {
                                                Index = 10,
                                                Format = "XXXXXXXXXXXX"
                                            };
                                        }
                                    }
                                    else
                                    {
                                        if (newMeterAddress != "" || newMeterAddress != null)
                                        {
                                            structureRadioAddress = StructureCompteur(newMeterAddress);
                                            structureCompteur = structureList.Where(s => s.Format == structureRadioAddress).FirstOrDefault();

                                            if (structureCompteur == null)
                                            {
                                                structureCompteur = new StructureCompteur
                                                {
                                                    Index = 10,
                                                    Format = "XXXXXXXXXXXX"
                                                };
                                            }
                                        }
                                    }


                                    if (structureCompteur != null)
                                    {
                                        //Traitement structure  de compteur standard
                                        if (structureCompteur.Format == "LDDLLDDDDDD")
                                        {
                                            if (newRadioAddress != "" || newRadioAddress != null)
                                            {
                                                sRep = hyScript.call("return dc:setValue('RadioAddress', '" + newRadioAddress + "')");
                                                _logger.Info("Parameter RadioAddress set status: " + sRep);
                                            }
                                            else
                                            {
                                                if (newMeterAddress != "" || newMeterAddress != null)
                                                {
                                                    sRep = hyScript.call("return dc:setValue('MeterAddress', '" + newMeterAddress + "')");
                                                    _logger.Info("Parameter MeterAddress set status: " + sRep);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Traitemenet structure spécifique GPMM

                                            if (newRadioAddress != "" || newRadioAddress != null)
                                            {
                                               
                                                string cmdSetAdress = "return setAddress('RadioAddressObj',"+ structureCompteur.Index+ ",'" + newRadioAddress + "')";
                                                sRep = hyScript.call(cmdSetAdress);


                                                if (sRep != "true")
                                                {
                                                    string exception = hyScript.call("return getLastException();");
                                                    _logger.Info("Last Exception: " + exception);
                                                }
                                                else
                                                {
                                                    _logger.Info("Parameter RadioAddressObj set status: " + sRep);
                                                }
                                            }
                                            else
                                            {
                                                if (newMeterAddress != "" || newMeterAddress != null)
                                                {


                                                    //
                                                    string cmdSetAdress = "return setAddress('MeterAddressObj'," + structureCompteur.Index + ",'" + newRadioAddress + "')";
                                                    sRep = hyScript.call(cmdSetAdress);

                                                    if (sRep != "true")
                                                    {
                                                        string exception = hyScript.call("return getLastException();");
                                                        _logger.Info("Last Exception: " + exception);
                                                        _logger.Info("Echec set MeterAddressObj.");

                                                    }
                                                    else
                                                    {
                                                        _logger.Info("Parameter MeterAddressObj set status: " + sRep);
                                                    }
                                                }
                                            }


                                        }
                                    }
                                    else
                                    {
                                        _logger.Info("Structure de compteur non prise en compte: " + structureRadioAddress);
                                    }
                                }
                                else
                                {
                                    //Code fabricannt = "HYP" ou "DME"
                                    sRep = hyScript.call("return dc:setValue('RadioAddress', '" + productNumberConnect + "')");
                                    _logger.Info("Parameter RadioAddress/MeterAddress set status: " + sRep);
                                }


                                foreach (var item in request.ConfigParams)
                                {
                                    configParam = (ConfigParam)item;

                                    if (configParam.Name != "Description" && configParam.Name != "DMName" && configParam.Name != "ProductionNumber" && configParam.Name != "RadioAddress" && configParam.Name != "MeterAddress")
                                    {
                                        sRep = hyScript.call("return dc:setValue('" + configParam.Name + "', '" + configParam.Value + "')");
                                        _logger.Info("Parameter " + configParam.Name + " set status: " + sRep);
                                    }
                                }

                                result = hyScript.call("return writeback()");
                            }
                        }
                    }
                }
                if (result != "true")
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.Info($"Echec set configuration : {ex}");
                return false;
            }
        }

        [HttpGet]
        [Route("getconfiguration/{portcom}/{pathconfig}")]
        public List<ConfigParam> GetConfiguration(string portcom, string pathconfig)
        {
            try
            {

                //@"C:\Temp\IZAR@CSI";
                HyScript hyScript = new HyScript();
                string path = DecodeFrom64(pathconfig);

                List<ConfigParam> configParamList;
                ConfigParam configParam = new ConfigParam();
                var configParams = new List<string> {
                    "Manufacturer",
                    "Generation",
                    "Medium",
                    "ProductionNumber",
                    "VIF",
                    "CIField",
                    "IndexE",
                    "Status",
                    "Generation",
                    "IndexESecond",
                    "CIField",
                    "RadioState",
                    "CurrentDate",
                    "CurrentTime",
                    "RadioSendingInterval",
                    "RemainingBatteryLifetime",
                    "AlarmsHoldTime",
                    "AlarmMeterBlocked",
                    "AlarmReverseFlow",
                    "AlarmLeakDetectWindow",
                    "AlarmLeakDetectInterval",
                    "AlarmUnderFlowRate",
                    "AlarmUnderFlowTime",
                    "AlarmExcessFlowRate",
                    "AlarmExcessFlowTime",
                    "HistoricFrame"
                };
                string cheminLog = @"C:\logfileLecture.log";
                string comPort = "com://" + Convert.ToInt32(portcom);
                string sBTHead = "hybtoh://";
                string response = "";
                string cmdClear = "if(dc~=nil) then dc:close(); end cl=nil pl=nil dc=nil tsf=nil";
                string cmdLicence = "return License.check('EGEEEGEE','I1ARCQI0')";
                string deviceModulePath = path + @"\devicemodule";
                string deviceModule = deviceModulePath.Replace(@"\", @"\\");

                string cmdConfigOpen = "setLogLevel(-1); setLogFileName('" + cheminLog.Replace(@"\", @"\\") + "');"+
                           "dc = DeviceConfigurator.new();" +
                           " fs = LocalFileSystem.new('" + deviceModule + "');" +
                           " dc:load(fs);" +
                           " pl = IPhysicalLayer.new('" + comPort + "');" +
                           " cl = ICommunicationLayer.new('" + sBTHead + "', ref(pl));" +
                           " dc:setCommunicationLayer(ref(cl));" +
                           " tsf=ToStringFormater.new();" +
                           " tsf:setAddSPDEAddress(true);" +
                           " setFormater(tsf);" +
                           " return dc:open(1);";

                string cmdInitStructure = "IDeviceID.setAddressInterpretationMode(0);" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(1, 'LLDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(2, 'ddDDDDDDLLL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(3, 'DDDDDDDDL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(4, 'DDLLLDDDDDd');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(5, 'DDDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(6, 'DDLLDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(7, 'DDDDDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(8, 'LDDDDDDDDL');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(9, 'LDDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(10, 'XXXXXXXXXXXX');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(11, 'DDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(12, 'LDDLLDDDDD');";

                //Check Licence
                string licenceOK = hyScript.call(cmdLicence);
                _logger.Info("Vérification Licence : " + licenceOK);

                if (licenceOK != "true")
                {
                    _logger.Info("Licence invalide.");
                    return null;
                }
                else
                {
                    //clear
                    _logger.Info("Clear : " + cmdClear);
                    hyScript.call(cmdClear);

                    //clear last exception
                    hyScript.call("clearLastException()");

                    //Set structure 
                    _logger.Info("Set structure ");
                    hyScript.call(cmdInitStructure);

                    //initialisation lecture
                    _logger.Info("Initialisation : " + cmdConfigOpen);
                    response = hyScript.call(cmdConfigOpen);

                    if (response != "true")
                    {
                        string exception = hyScript.call("return getLastException();");
                        _logger.Info("Last Exception: " + exception);
                        _logger.Info("Echec connexion Opto Irda.");
                        hyScript.call(cmdClear);
                        return null;
                    }
                    else
                    {

                        configParamList = new List<ConfigParam>();
                        //Description: Nom du produit
                        response = hyScript.call("return dc:getDescription()");
                        _logger.Info("--- Description: " + response);
                        configParam = new ConfigParam
                        {
                            Name = "Description",
                            Value = response
                        };
                        configParamList.Add(configParam);
                        _logger.Info(configParam.Name + " ====> " + configParam.Value);

                        //Nom du script de config en cours d'emploi
                        response = hyScript.call("return dc:getDMName()");
                        _logger.Info("--- DM Nom: " + response);
                        configParam = new ConfigParam
                        {
                            Name = "DMName",
                            Value = response
                        };

                        configParamList.Add(configParam);
                        _logger.Info(configParam.Name + " ====> " + configParam.Value);

                        //Version du script de config en cours d'emploi
                        response = hyScript.call("return dc:getDMVersion()");
                        _logger.Info("--- DM Version: " + response);
                        configParam = new ConfigParam
                        {
                            Name = "DMVersion",
                            Value = response
                        };
                        configParamList.Add(configParam);
                        _logger.Info(configParam.Name + " ====> " + configParam.Value);

                        //Init structure()
                        hyScript.call(cmdInitStructure);

                        //get RadioAddressObj
                        string jsonRadio = hyScript.call("local ret = dc:getValue('RadioAddressObj') if (istype(ret, 'SPDEDeviceID')) then ret = SPDEDeviceID.new(ret:getBytes()) end return ret ");
                        if(jsonRadio != null && jsonRadio != "")
                        {
                            RadioAddressObj radioAddressObj = JsonConvert.DeserializeObject<RadioAddressObj>(jsonRadio);
                            configParam = new ConfigParam
                            {
                                Name = "RadioAddress",
                                Value = radioAddressObj.spde,
                            };
                        
                            configParamList.Add(configParam);
                            _logger.Info("--- RadioAddressObj: " + jsonRadio);
                        }
                       
                        // get MeterAddressObj 
                        string jsonMeter = hyScript.call("local ret = dc:getValue('MeterAddressObj') if (istype(ret, 'SPDEDeviceID')) then ret = SPDEDeviceID.new(ret:getBytes()) end return ret ");
                        if (jsonMeter != null && jsonMeter != "")
                        {
                            RadioAddressObj meterAddressObj = JsonConvert.DeserializeObject<RadioAddressObj>(jsonMeter);
                            configParam = new ConfigParam
                            {
                                Name = "MeterAddress",
                                Value = meterAddressObj.spde,
                            };

                            configParamList.Add(configParam);
                            _logger.Info("--- MeterAddressObj: " + jsonMeter);
                        }
                       


                        //   
                        foreach (var paramName in configParams)
                        {
                            if(paramName != "RadioAddress" || paramName != "MeterAddress")
                            {
                                var value = hyScript.call("return dc:getValue('" + paramName + "');");

                                if (value != null)
                                {
                                    configParam = new ConfigParam
                                    {
                                        Name = paramName,
                                        Value = value
                                    };

                                    configParamList.Add(configParam);
                                    _logger.Info(configParam.Name + " ====> " + configParam.Value);
                                }
                            }
                            
                        }
                    }
                }
                if (configParamList == null)
                    return null;

                hyScript.call("dc:close()");
                _logger.Info("Resultat: " + JsonConvert.SerializeObject(configParamList));

                return configParamList;
            }
            catch (Exception ex)
            {
                _logger.Info($"Echec identification module : {ex}");
                return null;
            }
        }


        [HttpPost]
        [Route("setconfiguration")]
        public bool SetConfiguration(SetDeviceConfigurationContractRequest request)
        {
            try
            {
                _logger.Info($"Objet recu : " + JsonConvert.SerializeObject(request));

                //@"C:\Temp\IZAR@CSI";
                HyScript hyScript = new HyScript();
                string cheminLog = @"C:\logfileConfiguration.log";
                string path = DecodeFrom64(request.PathScript);
                string sRep = "";
                string result = "";
                string comPort = "com://" + Convert.ToInt32(request.PortComEntrant);
                string sBTHead = "hybtoh://";
                string response = "";
                string cmdClear = "if(dc~=nil) then dc:close(); end cl=nil pl=nil dc=nil tsf=nil";
                string cmdLicence = "setLogLevel(-1); setLogFileName('" + cheminLog.Replace(@"\", @"\\") + "'); return License.check('EGEEEGEE','I1ARCQI0')";
                string deviceModulePath = path + @"\devicemodule";
                string deviceModule = deviceModulePath.Replace(@"\", @"\\");

                string cmdConfigOpen = "dc = DeviceConfigurator.new();" +
                           " fs = LocalFileSystem.new('" + deviceModule + "');" +
                           " dc:load(fs);" +
                           " pl = IPhysicalLayer.new('" + comPort + "');" +
                           " cl = ICommunicationLayer.new('" + sBTHead + "', ref(pl));" +
                           " dc:setCommunicationLayer(ref(cl));" +
                           " tsf=ToStringFormater.new();" +
                           " tsf:setAddSPDEAddress(true);" +
                           " setFormater(tsf);" +
                           " return dc:open(1);";

                string cmdInitStructure = "IDeviceID.setAddressInterpretationMode(0);" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(1, 'LLDDDDDD');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(2, 'ddDDDDDDLLL');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(3, 'DDDDDDDDL');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(4, 'DDLLLDDDDD');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(5, 'DDDDDDDD');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(6, 'DDLLDDDDDD');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(7, 'DDDDDDDDDD');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(8, 'LDDDDDDDDL');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(9, 'LDDDDDD');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(10, 'XXXXXXXXXXXX');" +
                                      "IDeviceID.setMeterNumberAddressInterpretation(11, 'DDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(12, 'LDDLLDDDDD');";

                //Check Licence
                string licenceOK = hyScript.call(cmdLicence);
                _logger.Info("Vérification Licence : " + licenceOK);


                if (licenceOK != "true")
                {
                    _logger.Info("Licence invalide.");
                    return false;
                }
                else
                {
                    _logger.Info("Clear : " + cmdClear);
                    //clear
                    hyScript.call(cmdClear);

                    //clear last exception
                    hyScript.call("clearLastException()");

                    //Set structure 
                    _logger.Info("Set structure ");
                    hyScript.call(cmdInitStructure);

                    //initialisation lecture
                    _logger.Info("Initialisation : " + cmdConfigOpen);
                    response = hyScript.call(cmdConfigOpen);

                    if (response != "true")
                    {
                        string exception = hyScript.call("return getLastException();");
                        _logger.Info("Last Exception: " + exception);
                        _logger.Info("Echec connexion Opto Irda.");
                        return false;
                    }
                    else
                    {
                        ConfigParam configParam;
                        string newMeterAddress = "";
                        string newRadioAddress = "";

                        //Vérifier que le module connecté est égale au module en cours de programmation
                        string productNumberConnect = hyScript.call("return dc:getValue('ProductionNumber')");
                        configParam = request.ConfigParams.Where(c => c.Name == "ProductionNumber").FirstOrDefault();

                        if (configParam.Value != productNumberConnect)
                        {
                            _logger.Info("Le module à configurer est différent du module connecté. Module connecté: " + productNumberConnect + " || Module à configurer: " + configParam.Value);
                            return false;
                        }
                        else
                        {
                            string structureRadioAddress = "";
                            StructureCompteur structureCompteur = new StructureCompteur();

                            //Contôle code fabricant
                            configParam = request.ConfigParams.Where(c => c.Name == "Manufacturer").FirstOrDefault();
                            string manufacturer = configParam.Value;

                            //Code fabricannt = "SAP"
                            if (manufacturer != null && manufacturer == "SAP")
                            {
                                //Liste des structures
                                List<StructureCompteur> structureList = new List<Contract.StructureCompteur>();
                                structureList = InitialiserListeStructure();

                                configParam = new ConfigParam();
                                configParam = request.ConfigParams.Where(c => c.Name == "RadioAddress").FirstOrDefault();
                                if (configParam != null)
                                {
                                    newRadioAddress = configParam.Value;
                                }
                                else
                                {
                                    configParam = new ConfigParam();
                                    configParam = request.ConfigParams.Where(c => c.Name == "MeterAddress").FirstOrDefault();
                                    if (configParam != null)
                                        newMeterAddress = configParam.Value;
                                }
                                    
                                //Contôle structure
                                if (newRadioAddress != "" || newRadioAddress != null)
                                {
                                    structureRadioAddress = StructureCompteur(newRadioAddress);
                                    structureCompteur = structureList.Where(s => s.Format == structureRadioAddress).FirstOrDefault();
                                }
                                else
                                {
                                    if (newMeterAddress != "" || newMeterAddress != null)
                                    {
                                        structureRadioAddress = StructureCompteur(newMeterAddress);
                                        structureCompteur = structureList.Where(s => s.Format == structureRadioAddress).FirstOrDefault();
                                    }
                                }

                                
                                if(structureCompteur != null)
                                {
                                    //Traitement structure  de compteur standard
                                    if(structureCompteur.Format == "LDDLLDDDDDD" || structureCompteur.Format == "LDDLLDDDDDDd")
                                    {
                                        if(newRadioAddress != "" || newRadioAddress != null)
                                        {
                                            sRep = hyScript.call("return dc:setValue('RadioAddress', '" + newRadioAddress + "')");
                                            _logger.Info("Parameter RadioAddress set status: " + sRep);
                                        }
                                        else
                                        {
                                            if (newMeterAddress != "" || newMeterAddress != null)
                                            {
                                                sRep = hyScript.call("return dc:setValue('MeterAddress', '" + newMeterAddress + "')");
                                                _logger.Info("Parameter MeterAddress set status: " + sRep);
                                            }
                                        }   
                                    }
                                    else
                                    {
                                        //Traitemenet structure spécifique GPMM
                                        if (newRadioAddress != "" || newRadioAddress != null)
                                        {
                                            //Init structure
                                            hyScript.call(cmdInitStructure);

                                            string numeroSerie = "PSAP" + structureCompteur.Index + "" + newRadioAddress + "000";
                                            _logger.Info(" Numéro de série: " + numeroSerie);


                                            string radioAddressJSON = hyScript.call("return SPDEDeviceID.new('" + numeroSerie + "');");
                                            _logger.Info(" RadioAddressObj: " + radioAddressJSON);

                                            //
                                            string cmdSetAdress = "d0 = SPDEDeviceID.new('" + numeroSerie + "') return dc:setValue('RadioAddressObj', d0)";
                                            sRep = hyScript.call(cmdSetAdress);

                                            if (sRep != "true")
                                            {
                                                string exception = hyScript.call("return getLastException();");
                                                _logger.Info("Last Exception: " + exception);
                                            }
                                            else
                                            {
                                                sRep = hyScript.call("return dc:setValue('RadioAddress', '" + newRadioAddress + "')");
                                                _logger.Info("Parameter RadioAddressObj set status: " + sRep);
                                            }
                                        }
                                        else
                                        {
                                            if (newMeterAddress != "" || newMeterAddress != null)
                                            {
                                                //Init structure
                                                hyScript.call(cmdInitStructure);

                                                string numeroSerie = "PSAP" + structureCompteur.Index + "" + newMeterAddress + "000";
                                                _logger.Info(" Numéro de série: " + numeroSerie);


                                                string meterAddressJSON = hyScript.call("return SPDEDeviceID.new('" + numeroSerie + "');");
                                                _logger.Info(" MeterAddressObj: " + meterAddressJSON);

                                                //
                                                sRep = hyScript.call("return dc:setValue(MeterAddressObj, '" + meterAddressJSON + "');");

                                                if (sRep != "true")
                                                {
                                                    string exception = hyScript.call("return getLastException();");
                                                    _logger.Info("Last Exception: " + exception);
                                                    _logger.Info("Echec set MeterAddressObj.");

                                                }
                                                else
                                                {
                                                    _logger.Info("Parameter MeterAddressObj set status: " + sRep);
                                                }
                                            }
                                        }

                                       
                                    }
                                }
                                else
                                {
                                    _logger.Info("Structure de compteur non prise en compte: " + structureRadioAddress);
                                }                            
                            }
                            else
                            {
                                //Code fabricannt = "HYP" ou "DME"
                                sRep = hyScript.call("return dc:setValue('RadioAddress', '" + productNumberConnect+ "')");
                                _logger.Info("Parameter RadioAddress/MeterAddress set status: " + sRep);
                            }


                            foreach (var item in request.ConfigParams)
                            {
                                configParam = (ConfigParam)item;

                                if (configParam.Name != "Description" && configParam.Name != "DMName" && configParam.Name != "ProductionNumber" && configParam.Name != "RadioAddress" && configParam.Name != "MeterAddress")
                                {
                                    sRep = hyScript.call("return dc:setValue('" + configParam.Name + "', '" + configParam.Value + "')");
                                    _logger.Info("Parameter " + configParam.Name + " set status: " + sRep);
                                }
                            }

                            result = hyScript.call("local bOk = dc:writeback() dc:close() return bOk");
                        }
                    }
                }
                if (result != "true")
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.Info($"Echec write back module : {ex}");
                return false;
            }
        }
        #endregion

        #region TOOLS

        private double CalculIndex(int value, int exponent)
        {
            double unite = uniteValeur(exponent);
            //double pulse = Math.Pow(10, exponent);
            return (value * unite);
        }

        private double uniteValeur(int exponent)
        {
            double valeur = 1;

            switch (exponent)
            {
                case -4:
                    valeur = 0.0000001;
                    break;
                case -3:
                    valeur = 0.000001;
                    break;
                case -2:
                    valeur = 0.00001;
                    break;
                case -1:
                    valeur = 0.0001;
                    break;
                case 0:
                    valeur = 0.001;
                    break;
                case 1:
                    valeur = 0.01;
                    break;
                case 2:
                    valeur = 0.1;
                    break;
                case 3:
                    valeur = 1;
                    break;
                case 4:
                    valeur = 10;
                    break;
                case 5:
                    valeur = 100;
                    break;
                case 6:
                    valeur = 1000;
                    break;
                default:
                    break;
            }
            return valeur;
        }

        private string TelegramFormat(string data)
        {
            string rawData = "";
            string[] myData = data.Split(' ');
            int nombre = Convert.ToInt32(myData[0], 16) + 1;

            for (int i = 0; i < nombre; i++)
            {
                rawData += myData[i];
            }
            return rawData;
        }

        private string TelegramAlarmFormat(string data)
        {
            string rawAlarme = "";
            string[] myData = data.Split(' ');
            rawAlarme = String.Join(String.Empty,
                  myData[0].Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                  )
                );
            rawAlarme += " " + String.Join(String.Empty,
                 myData[1].Select(
                   c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                 )
               );
            return rawAlarme;
        }

        private string AlarmString(string data)
        {
            string alarms = "";
            string[] alarmToken = data.Split(' ');
            string byte1 = String.Join(String.Empty,
                  alarmToken[0].Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                  )
                );
            string byte2 = String.Join(String.Empty,
                 alarmToken[1].Select(
                   c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                 )
               );

            if (byte1[0] == '1')
                alarms += "| Leak current ";

            if (byte1[1] == '1')
                alarms += "| Leak previously ";

            if (byte1[2] == '1')
                alarms += "| Blocked ";

            if (byte2[0] == '1')
                alarms += "| Backflow ";

            if (byte2[1] == '1')
                alarms += "| Underflow ";

            if (byte2[2] == '1')
                alarms += "| Overflow ";

            if (byte2[3] == '1')
                alarms += "| Submarine ";

            if (byte2[4] == '1')
                alarms += "| Magnetic electric current ";

            if (byte2[5] == '1')
                alarms += "| Magnetic electric previously ";

            if (byte2[6] == '1')
                alarms += "| Mechanic current ";

            if (byte2[7] == '1')
                alarms += "| Mechanic previously";

            return alarms;

        }


        private string ExtractSerial_SPDE(string data)
        {
            int c = 0;
            int l;
            string sTmp;
            string sRes = "";
            int j = 0;
            byte[] result1 = new byte[3];
            byte[] result2 = new byte[4];

            string[] tokens = data.Split(' ');

            for (int i = 0; i < 6; i++)
            {
                if (i < 3)
                {
                    result1[i] = Convert.ToByte(tokens[i], 16);
                }
                if (i == 2)
                {
                    result2[j] = Convert.ToByte(tokens[i], 16);
                    j++;
                }
                if (i >= 3)
                {
                    result2[j] = Convert.ToByte(tokens[i], 16);
                    j++;
                }

            }

            //Traitement de C
            string hexStringC = HexStr(result2);
            string binarystringC = String.Join(String.Empty,
              hexStringC.Select(caract => Convert.ToString(Convert.ToInt32(caract.ToString(), 16), 2).PadLeft(4, '0')
              ).ToArray()
            );

            //29 bits
            string bitsC = binarystringC.Substring(3);


            c = Convert.ToInt32(bitsC, 2);
            c = c & 0x1FFFFFFF;
            sTmp = c.ToString();

            //Traitement L
            string hexStringL = HexStr(result1);
            string binarystringL = String.Join(String.Empty,
              hexStringL.Select(caract => Convert.ToString(Convert.ToInt32(caract.ToString(), 16), 2).PadLeft(4, '0')
              ).ToArray()
            );

            //5 bits + 64
            string bitsL = binarystringL;
            l = Convert.ToInt32(bitsL, 2);

            if (l > 0 && sTmp.Length == 8)
            {
                int l1, l2, l3;
                l = l >> 5;
                l3 = (l & 0x1F) + 64;
                l = l >> 5;
                l2 = (l & 0x1F) + 64;
                l = l >> 5;
                l1 = (l & 0x1F) + 64;

                if (l1 > 64)
                {
                    var res = System.Convert.ToChar(l1);
                    sRes += res;
                }
                sRes += sTmp.Substring(0, 2);
                if (l2 > 64)
                {
                    var res = System.Convert.ToChar(l2);
                    sRes += res;
                }
                if (l3 > 64)
                {
                    var res = System.Convert.ToChar(l3);
                    sRes += res;
                }

                sRes += sTmp.Substring(2, 6);
            }
            else
            {
                sRes = sTmp;
            }

            return sRes;
        }

        private string HexStr(byte[] p)
        {

            char[] c = new char[p.Length * 2];

            byte b;

            for (int y = 0, x = 0; y < p.Length; ++y, ++x)
            {

                b = ((byte)(p[y] >> 4));

                c[x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

                b = ((byte)(p[y] & 0xF));

                c[++x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

            }

            return new string(c);

        }

        private string ConvertHexStringToStringWithSpace(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            string result = "";
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);

                if (index == (HexAsBytes.Length - 1))
                    result += byteValue;
                else
                    result += byteValue + " ";

            }

            return result;
        }


        private string[] ExtractInteger(string data, int longueur)
        {
            var response = new string[] { };
            string[] myData = data.Split(' ');
            for (int i = 0; i < longueur; i++)
            {
                string val = myData[i];
                response[i] = val;
            }
            return response;
        }

        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes
                = System.Convert.FromBase64String(encodedData);
            string returnValue =
               System.Text.ASCIIEncoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes
                  = System.Text.ASCIIEncoding.UTF8.GetBytes(toEncode);
            string returnValue
                  = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        private string ExtractSPDEString(string s)
        {
            int l = 0;
            int nb = 0;
            int startIndex = 0;

            if (s.Length > 5)
            {
                l = s.Length;
                startIndex = 5;
                nb = (l - 3) - startIndex;
            }
           
            return s.Substring(startIndex, nb);
        }




        private string Interpreter(string rawData)
        {
            HyScript hyScript = new HyScript();
            string response = "false";
            string telegram = "";
            string cmdInitStructure = "IDeviceID.setAddressInterpretationMode(0);" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(1, 'LLDDDDDD');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(2, 'ddDDDDDDLLL');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(3, 'DDDDDDDDL');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(4, 'DDLLLDDDDD');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(5, 'DDDDDDDD');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(6, 'DDLLDDDDDD');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(7, 'DDDDDDDDDD');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(8, 'LDDDDDDDDL');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(9, 'LDDDDDD');" +
                                    "IDeviceID.setMeterNumberAddressInterpretation(10, 'XXXXXXXXXXXX');"+
                                    "IDeviceID.setMeterNumberAddressInterpretation(11, 'DDDDDD');" +
                                     "IDeviceID.setMeterNumberAddressInterpretation(12, 'LDDLLDDDDD');";

            string cmdSPDEFormater = "f = ToStringFormater.new() " +
                                     "f:setAddSPDEAddress(true) " +
                                     "setFormater(f) ";

            response = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");

            if (response == "true")
            {

                if (rawData != "")
                {
                    string cmd = "local ri = nil " +
                               "local mi = nil " +
                               " ri = RadioInterpreter.new() " +
                               " mi= MBusInterpreter.new() " +
                               " ri:setProcessLevel(PROCLEV_HEAD) " +
                               " local dl = nil;" +
                               " dl  = DeviceList.new();" +
                               " mi:addManuSpec2TelegramConverter(HydrometerSpec2TelegramConverter.new());" +
                               " mi:setMergeEmbeddedData(true);" +
                               " mi: setEnableInterpretationExtension(bit.bor(SDH_TELEGRAM, SDH_COMPACTPROFILE, SDH_DMFUNCTIONBLOCK, SDH_HYDVERSION, SDH_DEVICEID_MBUS, SDH_DEVICEID_SPDE)) " +
                               " ri:setMBusInterpreter(mi);" +
                               " ri:setDeviceList(ref(dl));" +
                               " h1 = HexString.new('" + rawData + "');" +
                               " telegramMbt = mi:interpret(h1);" +
                               " telegramRt  = ri:interpret(h1)" +
                               " if (telegramMbt:getInterpretationError() == INTPRET_NO_ERROR) then return telegramMbt else return telegramRt end ";

                    hyScript.call(cmdInitStructure);

                    hyScript.call(cmdSPDEFormater);

                    telegram = hyScript.call(cmd);


                }

            }

            return telegram;
        }
        private string StructureCompteur(string cpt)
        {
            if (cpt != null)
            {
                cpt = cpt.Replace("A", "ù");
                cpt = cpt.Replace("B", "ù");
                cpt = cpt.Replace("C", "ù");
                cpt = cpt.Replace("D", "ù");
                cpt = cpt.Replace("E", "ù");
                cpt = cpt.Replace("F", "ù");
                cpt = cpt.Replace("G", "ù");
                cpt = cpt.Replace("H", "ù");
                cpt = cpt.Replace("I", "ù");
                cpt = cpt.Replace("J", "ù");
                cpt = cpt.Replace("K", "ù");
                cpt = cpt.Replace("L", "ù");
                cpt = cpt.Replace("M", "ù");
                cpt = cpt.Replace("N", "ù");
                cpt = cpt.Replace("O", "ù");
                cpt = cpt.Replace("P", "ù");
                cpt = cpt.Replace("Q", "ù");
                cpt = cpt.Replace("R", "ù");
                cpt = cpt.Replace("S", "ù");
                cpt = cpt.Replace("T", "ù");
                cpt = cpt.Replace("U", "ù");
                cpt = cpt.Replace("V", "ù");
                cpt = cpt.Replace("W", "ù");
                cpt = cpt.Replace("Y", "ù");
                cpt = cpt.Replace("Z", "ù");

                cpt = cpt.Replace("0", "D");
                cpt = cpt.Replace("1", "D");
                cpt = cpt.Replace("2", "D");
                cpt = cpt.Replace("3", "D");
                cpt = cpt.Replace("4", "D");
                cpt = cpt.Replace("5", "D");
                cpt = cpt.Replace("6", "D");
                cpt = cpt.Replace("7", "D");
                cpt = cpt.Replace("8", "D");
                cpt = cpt.Replace("9", "D");

                cpt = cpt.Replace("ù", "L");

            }

            return cpt;
        }

        private List<StructureCompteur> InitialiserListeStructure()
        {
            List<StructureCompteur> maListe = new List<StructureCompteur>();
            StructureCompteur maStructure;

            maStructure = new StructureCompteur
            {
                Index = 0,
                Format = "LDDLLDDDDDD"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 1,
                Format = "LLDDDDDD"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 2,
                Format = "DDDDDDLLL"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 3,
                Format = "DDDDDDDDL"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 4,
                Format = "DDLLLDDDDD"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 5,
                Format = "DDDDDDDD"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 6,
                Format = "DDLLDDDDDD"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 7,
                Format = "DDDDDDDDDD"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 8,
                Format = "LDDDDDDDDL"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 9,
                Format = "CDDDDDD"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 10,
                Format = "XXXXXXXXXXXX"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 11,
                Format = "DDDDDD"
            };
            maListe.Add(maStructure);

            maStructure = new StructureCompteur
            {
                Index = 12,
                Format = "LDDLLDDDDD"
            };
            maListe.Add(maStructure);

            return maListe;
        }

        #endregion

    }
}
