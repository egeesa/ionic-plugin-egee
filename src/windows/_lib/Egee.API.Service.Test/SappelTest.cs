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

namespace Egee.API.Service.Test
{
    [TestFixture]
    public class SappelTest
    {
        SappelController SappelController { get; set; }
        

        [OneTimeSetUp]
        public void Init()
        {
            SappelController = new SappelController();
        }

        [Test]
        [Category("SappelTest")]
        public void ReadTelegram()
        {
            string data = "";
            string json = SappelController.Read();
            
            SappelResponseContract sappelResponseContract = JsonConvert.DeserializeObject<SappelResponseContract>(json);

            //string numeroCompteurHexa = "30 4C F6 4F C1 03 00 00".Replace(" ", "");

            //Entry entryCompteur = sappelResponseContract.entries.Where(e => e.value.deviceId.hex.data.Replace(" ", "") == numeroCompteurHexa).FirstOrDefault();

            //Telegram telegram = entryCompteur.value.meteringPoint.telegrams.FirstOrDefault();

            //if (telegram.telegramTypeSpecifica.qualityIndicator.rssi == 100)
            //{
            //    //On récupére la data
            //    string data = telegram.mBusData.rawData.data;
            //}

            List<Entry> entries = sappelResponseContract.entries.ToList();

            foreach (var entry in entries)
            {
                List<Telegram> telegrams = entry.value.meteringPoint.telegrams.ToList();

                foreach (var telegram in telegrams)
                {
                    if (telegram.telegramTypeSpecifica.qualityIndicator.rssi == 100)
                    {
                        //On récupére la data
                        data += " | "+telegram.mBusData.rawData.data.ToString();
                    }
                }
            }

            
        }

        [Test]
        [Category("SappelTest")]
        public void ReadAllTelegram()
        {
            List<FrameResponse> data = new List<FrameResponse>();
            data = SappelController.ReadAllTelegram("6", "0012f18e03e");
        }

        [Test]
        [Category("SappelTest")]
        public void GetDeviceConfiguration()
        {
            List<ConfigParam> dataParam = new List<ConfigParam>();
            dataParam = SappelController.GetDeviceConfiguration("6");
        }

    }
}
