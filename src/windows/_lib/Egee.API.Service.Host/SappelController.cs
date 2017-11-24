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

            _logger.Info("commande1 : Check Licence");
            response = hyScript.call("setLogLevel(-1); setLogFileName('IzarCSI.log'); return License.check('EGEEEGEE','I1ARCQI0')");
            _logger.Info("resultat cmd1 : " + response);
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
                            " ds:config('timeoutAfterTelegram', 10); ds:startASync() return true";


                _logger.Info("commande2: " + cmd);
                response = hyScript.call(cmd);

                _logger.Info("resultat cmd2 : " + response);
                if (response == "true")
                {
                 
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
                    if (Convert.ToInt32(foundedDevice) > 0)
                    {
                        _logger.Info("commande5 :dc:getDeviceList()");
                        dataJSON = hyScript.call("return dc:getDeviceList():__tostring(dc:getDeviceList():getBeginDeviceDescriptionIterator(true))");
                        _logger.Info("resultat commande5 : " + dataJSON);

                        FrameResponse dataFrame;
                        SappelResponseContract sappelResponseContract = JsonConvert.DeserializeObject<SappelResponseContract>(dataJSON);

                        List<Entry> entries = sappelResponseContract.entries.ToList();


                        foreach (var entry in entries)
                        {
                            List<Telegram> telegrams = entry.value.meteringPoint.telegrams.ToList();

                            foreach (var telegram in telegrams)
                            {
                                dataFrame = new FrameResponse();

                                if (telegram.telegramTypeSpecifica.qualityIndicator.rssi > 50)
                                {
                                    //Formatage idAsString
                                    string deviceData = "";
                                    
                                    if(telegram.mBusData.deviceId.idAsString.Length < 20)
                                        deviceData = telegram.mBusData.deviceId.idAsString.Substring(4, 12);
                                    else
                                        deviceData = telegram.mBusData.deviceId.idAsString.Substring(5, 12);

                                    deviceData = ConvertHexStringToStringWithSpace(deviceData);
                                    dataFrame.deviceId = ExtractSerial_SPDE(deviceData);


                                    //dataFrame.deviceId = telegram.mBusData.deviceId.idAsString;


                                    dataFrame.manuString = telegram.mBusData.deviceId.manuString;
                                    dataFrame.ciField = telegram.mBusData.ciField;

                                    string rawData = telegramFormat(telegram.mBusData.rawData.data);
                                    //On récupére la raw data
                                    rawDataInterpretJSON = Interpreter(rawData);
                                    Telegram telegramResponse = JsonConvert.DeserializeObject<Telegram>(rawDataInterpretJSON);
                                    List<MBusValue> mBusValues = telegramResponse.mBusData.mBusValues.ToList();
                                    foreach (var mBusValue in mBusValues)
                                    {
                                        if ( (mBusValue.valid == true))
                                        {
                                            if (mBusValue.dimension.stringId == "VOLUME")
                                            {
                                                //volume
                                                if(mBusValue.storageNumber == 0 && mBusValue.tariffNumber == 0 && mBusValue.subUnitNumber == 0)
                                                {
                                                    dataFrame.volumeValue = Convert.ToInt32(mBusValue.formated);
                                                    dataFrame.volumeIndex = calculIndex(Convert.ToInt32(mBusValue.formated), Convert.ToInt32(mBusValue.exponent));
                                                    dataFrame.volumeCode = mBusValue.dimension.stringId;
                                                    dataFrame.volumeUnite = mBusValue.unit.stringId;
                                                    dataFrame.volumeExponent = Convert.ToInt32(mBusValue.exponent);
                                                    dataFrame.volumeFormat = dataFrame.volumeCode + "  " + dataFrame.volumeIndex + "  m3";
                                                }                                                
                                            }
                                            if ( mBusValue.dimension.stringId == "ENERGY")
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

                                            if (mBusValue.dimension.stringId == "ERRORFLAG")
                                            {
                                                //Alarmes
                                                dataFrame.alarmeCode = mBusValue.dimension.stringId;
                                                dataFrame.alarmeFormated = Convert.ToInt32(mBusValue.formated);

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

                                    if(!myFrameList.Where(x => x.deviceId == dataFrame.deviceId).Any())
                                    {
                                        if((dataFrame.volumeCode != null|| dataFrame.energyCode != null) && dataFrame.ciField != 0)
                                            myFrameList.Add(dataFrame);
                                    }
                                }
                                
                            }
                        }
                       
                    }

                    hyScript.call("dc:getDeviceList():clear()");

                }

            }
            else
            {
                dataJSON = hyScript.call("dc:clearLastException() val=dc:getValue(valueIdent) ret = {val, dc:getLastException()} return ret");
                _logger.Info("resultat : " + dataJSON);

            }
            _logger.Info("resultat : " + myFrameList.ToString());
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
        [Route("getDeviceConfiguration/{portcom}")]
        public string GetDeviceConfiguration(int portcom)
        {
            HyScript hyScript = new HyScript();
            string response = "false";
            string dataJSON = "";
            string parameterList = "";


            _logger.Info("commande1 : Check Licence");
            response = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
            _logger.Info("resultat cmd1 : " + response);
            if (response == "true")
            {
                string cmd = " if (dc == nil) then "+
                            " dc = DeviceConfigurator.new();" +
                            " p = IPhysicalLayer.new(" + "'com://" + portcom + "');" +
                            " cl = ICommunicationLayer.new('hybtoh://', ref(p));" +
                            " dc:setCommunicationLayer(ref(c));" +
                            " dc:config('stopOnDeviceDetect', 1);" +
                            " else dc:close() end " +
                            " ds:config('timeoutAfterTelegram', 10) return (dc:open(1))";


                response = hyScript.call(cmd);

                if(response == "true")
                {
                    //Retourne la liste des paramètres du module en cours de traitement
                    parameterList = hyScript.call("return dc:getParamNameList()");

                    //Parse parameter list to get the value of parameters

                }
            }

            return dataJSON;
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
