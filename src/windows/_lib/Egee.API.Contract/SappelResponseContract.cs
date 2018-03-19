using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egee.API.Contract
{
    public class SappelResponseContract
    {
        public List<Entry> entries { get; set; }
    }

    public class Hex
    {
        public string data { get; set; }
    }

    public class ManuByte
    {
        public string data { get; set; }
    }

    public class Hex2
    {
        public string data { get; set; }
    }

    public class ManuByte2
    {
        public string data { get; set; }
    }

    public class Spdeid
    {
        public int classid { get; set; }
        public Hex2 hex { get; set; }
        public ManuByte2 manuByte { get; set; }
        public string manuString { get; set; }
        public string spde { get; set; }
        public int reference { get; set; }
        public int subunit { get; set; }
        public int fieldOrder { get; set; }
        public string format { get; set; }
        public string supplierCode { get; set; }
        public string manufactureYear { get; set; }
        public string meterType { get; set; }
        public string diameter { get; set; }
        public string serialNumber { get; set; }
        public bool valid { get; set; }
        public object master { get; set; }
        public string idAsString { get; set; }
    }

    public class DeviceId
    {
        public int classid { get; set; }
        public Hex hex { get; set; }
        public ManuByte manuByte { get; set; }
        public string manuString { get; set; }
        public int reference { get; set; }
        public int subunit { get; set; }
        public int fieldOrder { get; set; }
        public Spdeid spdeid { get; set; }
        public bool valid { get; set; }
        public object master { get; set; }
        public string idAsString { get; set; }
        public int? version { get; set; }
        public int? type { get; set; }
        public int? ident { get; set; }
        public string identStr { get; set; }
    }

    public class QualityIndicator
    {
        public int rssi { get; set; }
    }

    public class TelegramTypeSpecifica
    {
        public int numberOfCryptedBytes { get; set; }
        public QualityIndicator qualityIndicator { get; set; }
    }

    public class RawData
    {
        public string data { get; set; }
    }

    public class Timestamp
    {
        public object milliseconds { get; set; }
        public bool valid { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public int day { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        public int second { get; set; }
        public int millisecond { get; set; }
        public bool dayLightSavingTime { get; set; }
        public int dayLightSavingTimeDeviation { get; set; }
        public string timeZoneInfo { get; set; }
    }

    public class Hex3
    {
        public string data { get; set; }
    }

    public class ManuByte3
    {
        public string data { get; set; }
    }
    public class Hex4
    {
        public string data { get; set; }
    }

    public class ManuByte4
    {
        public string data { get; set; }
    }

    public class Spdeid2
    {
        public int classid { get; set; }
        public Hex4 hex { get; set; }
        public ManuByte4 manuByte { get; set; }
        public string manuString { get; set; }
        public string spde { get; set; }
        public int reference { get; set; }
        public int subunit { get; set; }
        public int fieldOrder { get; set; }
        public string format { get; set; }
        public string supplierCode { get; set; }
        public string manufactureYear { get; set; }
        public string meterType { get; set; }
        public string diameter { get; set; }
        public string serialNumber { get; set; }
        public bool valid { get; set; }
        public object master { get; set; }
        public string idAsString { get; set; }
    }

    public class DeviceId2
    {
        public int classid { get; set; }
        public Hex2 hex { get; set; }
        public ManuByte2 manuByte { get; set; }
        public string manuString { get; set; }
        public int reference { get; set; }
        public int subunit { get; set; }
        public int fieldOrder { get; set; }
        public Spdeid2 spdeid { get; set; }
        public bool valid { get; set; }
        public object master { get; set; }
        public string idAsString { get; set; }
        public int? version { get; set; }
        public int? type { get; set; }
        public int? ident { get; set; }
        public string identStr { get; set; }
    }

    public class AlarmField
    {
        public string data { get; set; }
    }

    public class Configuration
    {
        public string data { get; set; }
    }

    public class UncryptedData
    {
        public string data { get; set; }
    }

    public class Dib
    {
        public string data { get; set; }
    }

    public class Vib
    {
        public string data { get; set; }
    }

    public class Dimension
    {
        public string stringId { get; set; }
    }

    public class Unit
    {
        public string stringId { get; set; }
    }

    public class MBusValue
    {
        public Dib dib { get; set; }
        public Vib vib { get; set; }
        public Value value { get; set; }
        public int dataField { get; set; }
        public int functionField { get; set; }
        public int storageNumber { get; set; }
        public int tariffNumber { get; set; }
        public int subUnitNumber { get; set; }
        public int dataTypeRaw { get; set; }
        public int dataIsDate { get; set; }
        public int dataType { get; set; }
        public Dimension dimension { get; set; }
        public List<object> dimensionExtension { get; set; }
        public Unit unit { get; set; }
        public List<object> unitExtension { get; set; }
        public int exponent { get; set; }
        public bool isManuSpec { get; set; }
        public bool valid { get; set; }
        public object formated { get; set; }
    }

    public class MBusData
    {
        public int type { get; set; }
        public RawData rawData { get; set; }
        public Timestamp timestamp { get; set; }
        public string sourceId { get; set; }
        public string group { get; set; }
        public int additionalByteCountBegin { get; set; }
        public int additionalByteCountEnd { get; set; }
        public int interpretationStatus { get; set; }
        public int interpretationError { get; set; }
        public List<object> followingTelegrams { get; set; }
        public int direction { get; set; }
        public int headType { get; set; }
        public DeviceId2 deviceId { get; set; }
        public int ciField { get; set; }
        public int cField { get; set; }
        public int aField { get; set; }
        public AlarmField alarmField { get; set; }
        public Configuration configuration { get; set; }
        public bool isInstallation { get; set; }
        public int checkSumOrg { get; set; }
        public int checkSumCalc { get; set; }
        public int lengthOrg { get; set; }
        public int lengthCalc { get; set; }
        public int? accessNo { get; set; }
        public int? state { get; set; }
        public int? hopCounter { get; set; }
        public bool? isSynchronized { get; set; }
        public int? contentType { get; set; }
        public int decryptionState { get; set; }
        public UncryptedData uncryptedData { get; set; }
        public bool moreDataInFollowingTelegram { get; set; }
        public List<MBusValue> mBusValues { get; set; }
    }

    public class Telegram
    {
        public int type { get; set; }
        public TelegramTypeSpecifica telegramTypeSpecifica { get; set; }
        public MBusData mBusData { get; set; }
    }

    public class MeteringPoint
    {
        public List<Telegram> telegrams { get; set; }
    }

    public class Value
    {
        public DeviceId deviceId { get; set; }
        public int addType { get; set; }
        public int mBusReadoutBaudrate { get; set; }
        public object applicationReset { get; set; }
        public List<object> individualRfKeys { get; set; }
        public List<object> individualOptoKeys { get; set; }
        public MeteringPoint meteringPoint { get; set; }
    }

    public class Entry
    {
        public string key { get; set; }
        public Value value { get; set; }
    }

    public class RadioAddressObj
    {
        public int classId { get; set; }
        public Hex hex { get; set; }
        public ManuByte manuByte { get; set; }
        public string manuString { get; set; }
        public string spde { get; set; }
        public int reference { get; set; }
        public int subunit { get; set; }
        public int fieldOrder { get; set; }
        public string format { get; set; }
        public string supplierCode { get; set; }
        public string manufactureYear { get; set; }
        public string meterType { get; set; }
        public string diameter { get; set; }
        public string serialNumber { get; set; }
        public string valid { get; set; }
        public string master { get; set; }
        public string idAsString { get; set; }
    }
}
