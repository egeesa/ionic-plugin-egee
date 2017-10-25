using Egee.API.Contract;
using IZAR_CSIXLib;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Egee.API.Service.Host
{
    public class SappelController : ApiController
    {
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        [HttpGet]
        public void Init()
        {
            HyScript hyScript = new HyScript();
            string initResponse = hyScript.call("init");
            string licenseResponse = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
        }

        [HttpGet]
        public string Read()
        {
            HyScript hyScript = new HyScript();
            string response = "false";
            string dataJSON = "";


            _logger.Info("commande1 : Check Licence");
            response = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
            _logger.Info("resultat cmd1 : " + response);
            if (response == "true")
            {
                string cmd = "dc = DataConcentrator.new();" +
                            " ds = BtReceiverDataSource.new();" +
                            " ds:setDataConcentrator(ref(dc));" +
                            " pl = IPhysicalLayer.new('com://6');" +
                            " cl = ICommunicationLayer.new('hybtoh://', ref(pl));" +
                            " ds:setCommunicationLayer(ref(cl));" +
                            " ds:setSourceId('0012f18e03e');" +
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

            return dataJSON;
        }

        [HttpGet]
        public string GetTelegram(string adresseMACPRT, int numeroPORT, string numeroCompteurHexa)
        {
            HyScript hyScript = new HyScript();
            string response = "";
            string data = "";


            response = hyScript.call("return License.check('EGEEEGEE','I1ARCQI0')");
           
            if (response == "true")
            {
                string cmd = "dc = DataConcentrator.new();" +
                            " ds = BtReceiverDataSource.new();" +
                            " ds:setDataConcentrator(ref(dc));" +
                            " pl = IPhysicalLayer.new("+"'com://"+ numeroPORT + "');" +
                            " cl = ICommunicationLayer.new('hybtoh://', ref(pl));" +
                            " ds:setCommunicationLayer(ref(cl));" +
                            " ds:config('readIntervalTimeout', -1)" +
                            " ds:config('readTotalTimeoutConstant', 1)" +
                            " ds:config('readTotalTimeoutMultiplier', -1)" +
                            " ds:config('writeTotalTimeoutConstant', 0)" +
                            " ds:config('writeTotalTimeoutMultiplier', 0)" +
                            " ds:setDeviceID(MBusDeviceID.new('RSAP000003C14FF6000'))" +
                            " ds:setSourceId(" + adresseMACPRT + ");" +
                            " ds:config('timeoutNoAnswer', 150);" +
                            " ds:config('timeoutNoDataFollows', 2);" +
                            " ds:config('timeoutAfterTelegram', 10) return true";


                response = hyScript.call(cmd);


                if (response == "true")
                {
                    //
                    hyScript.call("ds:startASync()");


                    Thread.Sleep(2000);


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
                        hyScript.call("ds:stopASync()");

                     
                        response = hyScript.call("return dc:getDeviceList():__tostring(dc:getDeviceList():getBeginDeviceDescriptionIterator(true))");
                        _logger.Info("response : " + response);



                        SappelResponseContract sappelResponseContract = JsonConvert.DeserializeObject<SappelResponseContract>(response);


                        Entry entryCompteur = sappelResponseContract.entries.Where(e => e.value.deviceId.hex.data.Replace(" ", "") == numeroCompteurHexa.Replace(" ", "")).FirstOrDefault();

                        Telegram telegram = entryCompteur.value.meteringPoint.telegrams.FirstOrDefault();

                        if (telegram.telegramTypeSpecifica.qualityIndicator.rssi == 100)
                        {
                            //On récupére la rawData
                            string rawData = telegram.mBusData.rawData.data;

                            //On récupére les alarmes
                            string alarmes = telegram.mBusData.alarmField.data;

                            data = "rawData: " + rawData + " alarmes: " + alarmes;
                            _logger.Info("resultat : " + data);

                        }
                        else
                        {
                            string rssi = telegram.telegramTypeSpecifica.qualityIndicator.rssi.ToString();
                            
                            string rawData = telegram.mBusData.rawData.data;

                            string alarmes = telegram.mBusData.alarmField.data;

                            data = "rssi: "+ rssi +" rawData: " + rawData + " alarmes: " + alarmes;
                            _logger.Info("resultat : " + data);
                        }

                        hyScript.call("ds:stopASync()");

                    }
                    else
                    {
                        _logger.Info("resultat dc:getDeviceList() : " + response);
                        hyScript.call("ds:stopASync()");
                    }

                    hyScript.call("dc:getDeviceList():clear()");

                }

            }

            return data;
        }

        [HttpGet]
        public string GetVersion()
        {
            HyScript hyScript = new HyScript();
            return hyScript.call("return Environment.getVersion()");
        }
    }
}
