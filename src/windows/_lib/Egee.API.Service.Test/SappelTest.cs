using Egee.API.Contract;
using Egee.API.Service.Host;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        public void Test1()
        {
            HttpClient client = new HttpClient();

            HttpResponseMessage response = client.GetAsync(http://172.21.1.112/sappel/read);
            if (response.IsSuccessStatusCode)
            {
                string json =  response.Content.ReadAsStringAsync().ToString();
            }

            //string json = File.ReadAllText(@"C:\temp\json.txt");
            SappelResponseContract sappelResponseContract = JsonConvert.DeserializeObject<SappelResponseContract>(json);

            string numeroCompteurHexa = "30 4C F6 4F C1 03 00 00".Replace(" ", "");

            Entry entryCompteur = sappelResponseContract.entries.Where(e => e.value.deviceId.hex.data.Replace(" ", "") == numeroCompteurHexa).FirstOrDefault();

            Telegram telegram =  entryCompteur.value.meteringPoint.telegrams.FirstOrDefault();

            if(telegram.telegramTypeSpecifica.qualityIndicator.rssi == 100)
            {
                //On récupére la data
                string data = telegram.mBusData.rawData.data;
            }
        }
    }
}
