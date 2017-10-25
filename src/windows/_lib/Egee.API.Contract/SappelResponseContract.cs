﻿using System;
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

    public class DeviceId
    {
        public int classid { get; set; }
        public Hex hex { get; set; }
        public ManuByte manuByte { get; set; }
        public string manuString { get; set; }
        public int reference { get; set; }
        public int subunit { get; set; }
        public int fieldOrder { get; set; }
        public bool valid { get; set; }
        public object master { get; set; }
        public string idAsString { get; set; }
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

    public class Hex2
    {
        public string data { get; set; }
    }

    public class ManuByte2
    {
        public string data { get; set; }
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
        public bool valid { get; set; }
        public object master { get; set; }
        public string idAsString { get; set; }
    }

    public class AlarmField
    {
        public string data { get; set; }
    }

    public class Configuration
    {
        public string data { get; set; }
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

}