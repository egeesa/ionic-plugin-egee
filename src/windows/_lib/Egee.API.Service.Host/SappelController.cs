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

        [HttpGet]
        [Route("init")]
        public void Init()
        {
            HyScript hyScript = new HyScript();
            string initResponse = hyScript.call("init");
            string licenseResponse = hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return License.check('EGEEEGEE','I1ARCQI0')");
        }

  
        [Route("frames")]
        [HttpGet, HttpPost]
        public string Read()
        {
            HyScript hyScript = new HyScript();
            string response = "false";
            string dataJSON = "";
            string cmdClear = "if(dc~=nil) then dc:close(); cl=nil; pl=nil; dc=nil";

            _logger.Info("commande1 : Check Licence");
            response = hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return License.check('EGEEEGEE','I1ARCQI0')");
            _logger.Info("resultat cmd1 : " + response);
            if (response == "true")
            {
                hyScript.call(cmdClear);

                string cmd = "dc = DataConcentrator.new();" +
                            " ds = BtReceiverDataSource.new();" +
                            " ds:setDataConcentrator(ref(dc));" +
                            " pl = IPhysicalLayer.new('com://6');" +
                            " cl = ICommunicationLayer.new('hybtoh://', ref(pl));" +
                            " ds:setCommunicationLayer(ref(cl));" +
                            " local ri = RadioInterpreter.new();"+
                            " ri:setProcessLevel();"+
                            " dc:setRadioInterpreter(ri);"+
                            " ri:enableDecryption(false);" +
                            " tsf=ToStringFormater.new();" +
                            " tsf:setAddSPDEAddress(true);" +
                            " setFormater(tsf);" +
                            " ds:setSourceId('0012f318e03e');" +
                            " ds:config('timeoutNoAnswer', 150);" +
                            " ds:config('timeoutNoDataFollows', 2);" +
                            " ds:config('timeoutAfterTelegram', 10) return true";

                _logger.Info("commande2: " + cmd);
                response = hyScript.call(cmd);

                _logger.Info("resultat cmd2 : " + response);
                if (response == "true")
                {
                    //startASync
                    _logger.Info("commande3: ds:startASync()");
                    hyScript.call("ds:startASync()");

              
                    Thread.Sleep(2000);

                    //stopASync
                    _logger.Info("commande 4 : ds:stopASync()");
                    hyScript.call("ds:stopASync()");

                    string telegramProcessed = hyScript.call("return dc:getNumberOfProcessedTelegrams()");
                    _logger.Info("resultat number of telegram Processed: " + telegramProcessed);


                    string telegramCollected = hyScript.call("return dc:getNumberOfCollectedTelegrams()");
                    _logger.Info("resultat number of telegram collected: " + telegramCollected);


                    string falseTelegram = hyScript.call("return dc:getNumberOfFalseTelegrams()");
                    _logger.Info("resultat number of false telegram : " + falseTelegram);


                    string foundedDevice = hyScript.call("return dc:getNumberOfFoundedDevices()");
                    _logger.Info("resultat number of devices found : " + foundedDevice);

                    //device founded
                    if(Convert.ToInt32(foundedDevice) > 0)
                    {
                        _logger.Info("commande5 :dc:getDeviceList()");
                        dataJSON = hyScript.call("return dc:getDeviceList():__tostring(dc:getDeviceList():getBeginDeviceDescriptionIterator(true))");
                        _logger.Info("resultat commande5 : " + dataJSON);
                    }
              
                    hyScript.call("dc:getDeviceList():clear()");

                }

            }
            else
            {
                dataJSON = hyScript.call("dc:clearLastException() val=dc:getValue(valueIdent) ret = {val, dc:getLastException()} return ret");

            }

            return dataJSON;
        }

        [Route("telegrams/{portcom}/{macadresseprt}")]
        [HttpGet, HttpPost]
        public List<FrameResponse> ReadAllTelegram(string portcom, string macadresseprt)
        {
            List<FrameResponse> myFrameList = new List<FrameResponse>();
           
            HyScript hyScript = new HyScript();
            int port = Convert.ToInt32(portcom);
            string comPort = "com://" + port;
            string response = "false";
            string dataJSON = "";
            string rawDataInterpretJSON = "";
            string cmdClear = "if(dc~=nil) then dc:close(); cl=nil; pl=nil; dc=nil";

            _logger.Info("Check Licence");
            response = hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return License.check('EGEEEGEE','I1ARCQI0')");
            _logger.Info("resultat Check Licence : " + response);
            if (response == "true")
            {
                hyScript.call(cmdClear);

                string cmd = "dc = DataConcentrator.new();" +
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


                _logger.Info("Init: " + cmd);
                response = hyScript.call(cmd);

                _logger.Info("resultat Init : " + response);

                if (response == "true")
                {
                    //startASync
                    _logger.Info("ds:startASync()");
                    hyScript.call("ds:startASync()");

                    Thread.Sleep(2000);

                    //stopASync
                    _logger.Info("ds:stopASync()");
                    hyScript.call("ds:stopASync()");

                    string telegramProcessed = hyScript.call("return dc:getNumberOfProcessedTelegrams()");
                    _logger.Info("Nombre de télégrammes traités: " + telegramProcessed);


                    string telegramCollected = hyScript.call("return dc:getNumberOfCollectedTelegrams()");
                    _logger.Info("Nombre de télégrammes collectés: " + telegramCollected);


                    string falseTelegram = hyScript.call("return dc:getNumberOfFalseTelegrams()");
                    _logger.Info("Nombre de faux télégrammes : " + falseTelegram);


                    string foundedDevice = hyScript.call("return dc:getNumberOfFoundedDevices()");
                    _logger.Info("Nombre de modules trouvés : " + foundedDevice);

                    //device founded
                    if (Convert.ToInt32(foundedDevice) > 0)
                    {
                        _logger.Info("dc:getDeviceList()");
                        dataJSON = hyScript.call("return dc:getDeviceList():__tostring(dc:getDeviceList():getBeginDeviceDescriptionIterator(true))");
                        _logger.Info("resultat getDeviceList : " + dataJSON);

                        try
                        {
                            FrameResponse dataFrame;
                            SappelResponseContract sappelResponseContract = JsonConvert.DeserializeObject<SappelResponseContract>(dataJSON);

                            List<Entry> entries = sappelResponseContract.entries.ToList();


                            foreach (var entry in entries)
                            {
                                List<Telegram> telegrams = entry.value.meteringPoint.telegrams.ToList();

                                foreach (var telegram in telegrams)
                                {
                                    dataFrame = new FrameResponse();

                                    if (telegram.telegramTypeSpecifica.qualityIndicator.rssi > 25)
                                    {
                                        
                                        dataFrame.deviceId = telegram.mBusData.deviceId.spdeid.spde;
                                        dataFrame.manuString = telegram.mBusData.deviceId.manuString;
                                        dataFrame.ciField = telegram.mBusData.ciField;
                                        dataFrame.structure = telegram.mBusData.deviceId.spdeid.format;

                                        //Récupération des alarmes
                                        if (telegram.mBusData.alarmField.data != "00" && telegram.mBusData.alarmField.data != "00 00")
                                        {
                                            dataFrame.alarmeCode = telegramAlarmFormat(telegram.mBusData.alarmField.data);

                                        }


                                        string rawData = telegramFormat(telegram.mBusData.rawData.data);

                                        //On récupére la raw data
                                        rawDataInterpretJSON = Interpreter(rawData);
                                        Telegram telegramResponse = JsonConvert.DeserializeObject<Telegram>(rawDataInterpretJSON);
                                        List<MBusValue> mBusValues = telegramResponse.mBusData.mBusValues.ToList();
                                        foreach (var mBusValue in mBusValues)
                                        {
                                            if ((mBusValue.valid == true))
                                            {
                                                if (mBusValue.dimension.stringId == "VOLUME")
                                                {
                                                    //volume
                                                    if (mBusValue.storageNumber == 0 && mBusValue.tariffNumber == 0 && mBusValue.subUnitNumber == 0)
                                                    {
                                                        dataFrame.volumeValue = Convert.ToInt32(mBusValue.formated);
                                                        dataFrame.volumeIndex = calculIndex(Convert.ToInt32(mBusValue.formated), Convert.ToInt32(mBusValue.exponent));
                                                        dataFrame.volumeCode = mBusValue.dimension.stringId;
                                                        dataFrame.volumeUnite = mBusValue.unit.stringId;
                                                        dataFrame.volumeExponent = Convert.ToInt32(mBusValue.exponent);
                                                        dataFrame.volumeFormat = dataFrame.volumeCode + "  " + dataFrame.volumeIndex + "  m3";
                                                    }
                                                }
                                                if (mBusValue.dimension.stringId == "ENERGY")
                                                {
                                                    //Energy
                                                    if (mBusValue.storageNumber == 0 && mBusValue.tariffNumber == 0 && mBusValue.subUnitNumber == 0)
                                                    {
                                                        dataFrame.energyValue = Convert.ToInt32(mBusValue.formated);
                                                        dataFrame.energyIndex = calculIndex(Convert.ToInt32(mBusValue.formated), Convert.ToInt32(mBusValue.exponent));
                                                        dataFrame.energyCode = mBusValue.dimension.stringId;
                                                        dataFrame.energyUnite = mBusValue.unit.stringId;
                                                        dataFrame.energyExponent = Convert.ToInt32(mBusValue.exponent);
                                                        dataFrame.energyFormat = dataFrame.energyCode + "  " + dataFrame.energyIndex + "  " + dataFrame.energyUnite;
                                                    }
                                                }

                                                if (mBusValue.dimension.stringId == "OPERATIONTIMEBATTERY")
                                                {
                                                    dataFrame.batteryCode = mBusValue.dimension.stringId;
                                                    dataFrame.batteryFormated = Convert.ToInt32(mBusValue.formated);
                                                    dataFrame.batteryUnite = mBusValue.unit.stringId;
                                                }
                                                if (mBusValue.dimension.stringId == "TIMEPOINT")
                                                {
                                                    dataFrame.date = mBusValue.formated.ToString();
                                                }
                                            }

                                        }

                                        if (!myFrameList.Where(x => x.deviceId == dataFrame.deviceId).Any())
                                        {
                                            if ((dataFrame.volumeCode != null || dataFrame.energyCode != null) && dataFrame.ciField != 0)
                                                myFrameList.Add(dataFrame);
                                        }
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Info("ERREUR : " + ex.Message);
                        }
                    }

                    hyScript.call("dc:getDeviceList():clear()");

                }
                else
                {
                    dataJSON = hyScript.call("dc:clearLastException() val=dc:getValue(valueIdent) ret = {val, dc:getLastException()} return ret");
                    _logger.Info("Erreur Init and StartASync: " + dataJSON);

                }

            }
            else
            {
                dataJSON = hyScript.call("dc:clearLastException() val=dc:getValue(valueIdent) ret = {val, dc:getLastException()} return ret");
                _logger.Info("Erreur check licence: " + dataJSON);

            }
            _logger.Info("ResultMyFrameList : " + myFrameList.ToString());
            return myFrameList;
        }

        private double calculIndex(int value, int exponent)
        {
            return (value * Math.Pow(10, exponent)) / 1000;
        }

        private string telegramFormat(string data)
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

        private string telegramAlarmFormat(string data)
        {
            string rawAlarme = "";
            string[] myData = data.Split(' ');
            rawAlarme = String.Join(String.Empty,
                  myData[0].Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                  )
                );
            rawAlarme += " "+ String.Join(String.Empty,
                 myData[1].Select(
                   c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                 )
               );
            return rawAlarme;
        }

        private  string AlarmString(string data)
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

            //if(byte1.Length == 8 && byte2.Length == 8)
            //{
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
            //}
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

        private  string HexStr(byte[] p)
        {

            //char[] c = new char[p.Length * 2 + 2];
            char[] c = new char[p.Length * 2];

            byte b;

            //c[0] = '0'; c[1] = 'x';

            for (int y = 0, x = 0; y < p.Length; ++y, ++x)
            {

                b = ((byte)(p[y] >> 4));

                c[x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

                b = ((byte)(p[y] & 0xF));

                c[++x] = (char)(b > 9 ? b + 0x37 : b + 0x30);

            }

            return new string(c);

        }

        private  string ConvertHexStringToStringWithSpace(string hexString)
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

  

        



        private string Interpreter(string rawData)
        {
            HyScript hyScript = new HyScript();
            string response = "false";
            string telegram = "";

            response = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
           
            if (response == "true")
            {
               
                if (rawData != "")
                {
                    string cmd = "local ri = nil " +
                       " ri  = RadioInterpreter.new() " +
                       " ri:setProcessLevel() " +
                       " local dl = nil;" +
                       " dl  = DeviceList.new();" +
                       " mi= MBusInterpreter.new() " +
                       " mi:addManuSpec2TelegramConverter(HydrometerSpec2TelegramConverter.new());" +
                       " mi:setMergeEmbeddedData(true);"+
                       " mi: setEnableInterpretationExtension(bit.bor(SDH_TELEGRAM, SDH_COMPACTPROFILE, SDH_DMFUNCTIONBLOCK, SDH_HYDVERSION, SDH_DEVICEID_MBUS, SDH_DEVICEID_SPDE)) " +
                       " ri:setMBusInterpreter(mi);" +
                       " ri:setDeviceList(ref(dl));" +
                       " h1 = HexString.new('" + rawData + "');" +
                       " telegramMbt = mi:interpret(h1);" +
                       " telegramRt  = ri:interpret(h1)" +
                       " if (telegramMbt:getInterpretationError() == INTPRET_NO_ERROR) then return telegramMbt else return telegramRt end ";

                    telegram = hyScript.call(cmd);
                   

                }

            }

            return telegram;
        }

        [HttpGet]
        [Route("configuration/{portcom}")]
        public List<ConfigParam> GetDeviceConfiguration(string portcom)
        {
            HyScript hyScript = new HyScript();
            //string errorMessage = "";
            //string cmd = "";
            int port = Convert.ToInt32(portcom);
            string comPort = "com://" + port;
            string sBTHead = "hybtoh://";//"btspp://00802543739B";
            string response = "false";
            string sScript = "";
            //string sScriptModule = "";
            string sRep = "";
            string sList = "";
            string sS1 = "";
            string sS2 = "";
            string sS3 = "";
            //string paramsList = "";
            //string deviceModulePath = "C:\\Temp\\IZAR@CSI\\DeviceModules";
            List<ConfigParam> configParamList = new List<ConfigParam>();
            ConfigParam configParam = new ConfigParam();
            string luaScriptPath = "C:\\Temp\\IZAR@CSI\\Script\\CSILib_OPTO.lua";

            _logger.Info("----------------------------------------------");
            _logger.Info("--------CONFIGURATION MODULE------------------");
            _logger.Info("----------------------------------------------");

            _logger.Info("--- Check licence");
            response = hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return License.check('EGEEEGEE','I1ARCQI0')");
            _logger.Info("--- Resultat : " + response);

            if (response == "true")
            {
                
                sScript = luaScriptPath.Replace(@"\", @"\\");
                //Chargement du script de configuration
                sRep = hyScript.call("return exec('" + sScript + "')");


                if (sRep == luaScriptPath)
                {
                    // Start identification of a product 
                    sRep = hyScript.call("return indentifyProduct('" + comPort + "', '" + sBTHead + "', '" + sS1 + "', '" + sS2 + "', '" + sS3 + "')");

                    if (sRep == "true")
                    {
                        //Description: Nom du produit
                        string getDescription = hyScript.call("return getDescription()");
                        _logger.Info("--- Nom du produit: " + getDescription);

                        //Nom du script de config en cours d'emploi
                        string getDMName = hyScript.call("return getDMName()");
                        _logger.Info("--- Nom du produit: " + getDMName);

                        //Version du script de config en cours d'emploi
                        string getDMVersion = hyScript.call("return getDMVersion()");
                        _logger.Info("--- Nom du produit: " + getDMVersion);

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

                        if (tabNames != null)
                        {
                            foreach (var nomParam in tabNames)
                            {
                                configParam.Name = nomParam;
                                configParam.Value = hyScript.call("return  dc:getValue('" + nomParam + "')");
                                _logger.Info(configParam.Name + " ====> " + configParam.Value);
                                configParamList.Add(configParam);
                            }
                        }
                    }
                    else
                    {
                        _logger.Info("ERREUR :  NO DEVICE DETECTED");
                    }
                }
                else
                {
                    _logger.Info("ERREUR:  Script execution : " + sScript + " : Result = " + sRep);
                }

            }
            else
            {
                _logger.Info("ERREUR:   License activation ");
            }
            _logger.Info("Parameter List: " + configParamList.ToString());
            return configParamList;
        }

        [HttpGet]
        [Route("configuration/{portcom}/{configParamList}")]
        public string SetDeviceConfiguration(string portcom, List<ConfigParam> configParamList)
        {
            HyScript hyScript = new HyScript();
            string result = "";
            int port = Convert.ToInt32(portcom);
            string comPort = "com://" + port;
            string sBTHead = "hybtoh://";
            string response = "false";
            string sScript = "";
            string sRep = "";
            string sS1 = "";
            string sS2 = "";
            string sS3 = "";
            string radioAddress = "";

            ConfigParam configParam = new ConfigParam();
            string luaScriptPath = "C:\\Temp\\IZAR@CSI\\Script\\CSILib_OPTO.lua";

            _logger.Info("----------------------Writeback------------------------");

            _logger.Info("--- Check licence");
            sRep = hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return License.check('EGEEEGEE','I1ARCQI0')");
            _logger.Info("--- Resultat : " + response);

            if (sRep == "true" && configParamList != null)
            {
                sScript = luaScriptPath.Replace(@"\", @"\\");
                //Chargement du script de configuration
                sRep = hyScript.call("return exec('" + sScript + "')");

                if(sRep == luaScriptPath)
                {
                    // Start identification of a product 
                    sRep = hyScript.call("return indentifyProduct('" + comPort + "', '" + sBTHead + "', '" + sS1 + "', '" + sS2 + "', '" + sS3 + "')");

                    if (sRep == "true")
                    {
                        //vérifier que le module en cours de configuration est égale à celui passer en paramètre
                        radioAddress = hyScript.call("return  dc:getValue('RadioAddress')");
                        configParam = (ConfigParam)configParamList.Where(c => c.Value == radioAddress);

                        if(configParam != null)
                        {
                            foreach (var item in configParamList)
                            {
                                configParam = (ConfigParam)item;
                                sRep = hyScript.call("return setValue('" + configParam.Name + "', '" + configParam.Value + "')");
                            }

                            result = hyScript.call("return writeback()");
                        }
                        else
                        {
                            result = "false";
                        }

                    }
                    else
                    {
                        result = "false";
                    }

                }
                else
                {
                    result = "false";
                }

            }
            else
            {
                result = "false";
            }
            return result;
        }

        [HttpGet]
        [Route("getVersion")]
        public string GetVersion()
        {
            HyScript hyScript = new HyScript();
            return hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return Environment.getVersion()");
        }
    }
}
