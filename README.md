# JT808 Protocol

[![MIT Licence](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/SmallChi/JT808/blob/master/LICENSE)![.NET Core](https://github.com/SmallChi/JT808/workflows/.NET%20Core/badge.svg?branch=master)

## JT808 data structure

### Data Package [JT808Package]

| Head | Message Header       | Message Body      | Check Code    | Tail |
| :----: | :---------: |  :---------: | :----------: | :----: |
| Begin  | JT808Header |  JT808Bodies |CheckCode | End    |
| 7E     | -           | -           | -         | 7E     |

### Data Header [JT808Header]

| Message ID | Message Body Properties | Protocol Version Number (2019 Version) | Terminal Phone Number | Message serial number | Total number of messages | Packet Sequence Number |
| :----: | :----------------------------: | :-------------: | :-------------: | :--------: | :---------: | :-------: |
| MsgId  | JT808HeaderMessageBodyProperty | ProtocolVersion | TerminalPhoneNo | MsgNum     | PackgeCount | PackageIndex |

#### Data Header - Message Body Property [JT808HeaderMessageBodyProperty]

|Version identification(2019 version)| Subcontracting | Data Encryption | Message Body Length |
|:------:| :------: | :------: | :--------: |
|VersionFlag| IsPackge | Encrypt  | DataLength |

#### Message Body Properties[JT808Bodies]

> According to the corresponding message ID: MsgId

***Note: the data content (excluding the head and tail identifiers) is escaped***

The escape rules are as follows:

1. If the character 0x7e appears in the data content, it should be replaced with the character 0x7d followed by the character 0x02;
2. If the character 0x7d appears in the data content, it should be replaced with the character 0x7d followed by the character 0x01;

Reason for the inversion: Confirm the TCP message boundary of the JT808 protocol.   

### Nutshell 1

#### 1.Grouping：

> MsgId 0x0200: Location information report  

``` package

JT808Package jT808Package = new JT808Package();

jT808Package.Header = new JT808Header
{
    MsgId = Enums.JT808MsgId.位置信息汇报,
    ManualMsgNum = 126,
    TerminalPhoneNo = "123456789012"
};

JT808_0x0200 jT808_0x0200 = new JT808_0x0200();
jT808_0x0200.AlarmFlag = 1;
jT808_0x0200.Altitude = 40;
jT808_0x0200.GPSTime = DateTime.Parse("2018-10-15 10:10:10");
jT808_0x0200.Lat = 12222222;
jT808_0x0200.Lng = 132444444;
jT808_0x0200.Speed = 60;
jT808_0x0200.Direction = 0;
jT808_0x0200.StatusFlag = 2;
jT808_0x0200.JT808LocationAttachData = new Dictionary<byte, JT808_0x0200_BodyBase>();

jT808_0x0200.JT808LocationAttachData.Add(JT808Constants.JT808_0x0200_0x01, new JT808_0x0200_0x01
{
    Mileage = 100
});

jT808_0x0200.JT808LocationAttachData.Add(JT808Constants.JT808_0x0200_0x02, new JT808_0x0200_0x02
{
    Oil = 125
});

jT808Package.Bodies = jT808_0x0200;

byte[] data = JT808Serializer.Serialize(jT808Package);

var hex = data.ToHexString();

// Output Hex：
// 7E 02 00 00 26 12 34 56 78 90 12 00 7D 02 00 00 00 01 00 00 00 02 00 BA 7F 0E 07 E4 F1 1C 00 28 00 3C 00 00 18 10 15 10 10 10 01 04 00 00 00 64 02 02 00 7D 01 13 7E
```

#### 2.Unpack manually：

``` unpackage
1.Initial Packet：
7E 02 00 00 26 12 34 56 78 90 12 00 (7D 02) 00 00 00 01 00 00 00 02 00 BA 7F 0E 07 E4 F1 1C 00 28 00 3C 00 00 18 10 15 10 10 10 01 04 00 00 00 64 02 02 00 (7D 01) 13 7E

2.Escaping
7D 02 ->7E
7D 01 ->7D
Escaping result
7E 02 00 00 26 12 34 56 78 90 12 00 7E 00 00 00 01 00 00 00 02 00 BA 7F 0E 07 E4 F1 1C 00 28 00 3C 00 00 18 10 15 10 10 10 01 04 00 00 00 64 02 02 00 7D 13 7E

3.Unpack
7E                  --Header Identification
02 00               --Header->Message ID
00 26               --Header->Message Body Properties
12 34 56 78 90 12   --Header->Terminal Phone Number
00 7E               --Header-> Message Serial Number
00 00 00 01         --Body->Alarm Flag
00 00 00 02         --Body->Status
00 BA 7F 0E         --Body->Latitude
07 E4 F1 1C         --Body->Longitude
00 28               --Body->Altitude
00 3C               --Body->Spedd
00 00               --Body->Heading
18 10 15 10 10 10   --Body->GPS Timestamp
01                  --Body->Additional Information->Mileage
04                  --Body->Additional Information->Length
00 00 00 64         --Body->Additional Information->Data
02                  --Body->Additional Information->Fuel Level
02                  --Body->Additional Information->Length
00 7D               --Body->Additional Information->Data
13                  --Check Code
7E                  --Tail Identification
```

#### 3.Unpacking：

``` unpackage2
//1.Convert to byte array
byte[] bytes = "7E 02 00 00 26 12 34 56 78 90 12 00 7D 02 00 00 00 01 00 00 00 02 00 BA 7F 0E 07 E4 F1 1C 00 28 00 3C 00 00 18 10 15 10 10 10 01 04 00 00 00 64 02 02 00 7D 01 13 7E".ToHexBytes();

//2.Deserialize array
var jT808Package = JT808Serializer.Deserialize(bytes);

//3.Packet Header
Assert.Equal(Enums.JT808MsgId.位置信息汇报, jT808Package.Header.MsgId);
Assert.Equal(38, jT808Package.Header.MessageBodyProperty.DataLength);
Assert.Equal(126, jT808Package.Header.MsgNum);
Assert.Equal("123456789012", jT808Package.Header.TerminalPhoneNo);
Assert.False(jT808Package.Header.MessageBodyProperty.IsPackge);
Assert.Equal(0, jT808Package.Header.PackageIndex);
Assert.Equal(0, jT808Package.Header.PackgeCount);
Assert.Equal(JT808EncryptMethod.None, jT808Package.Header.MessageBodyProperty.Encrypt);

//4.Packet Body
JT808_0x0200 jT808_0x0200 = (JT808_0x0200)jT808Package.Bodies;
Assert.Equal((uint)1, jT808_0x0200.AlarmFlag);
Assert.Equal((uint)40, jT808_0x0200.Altitude);
Assert.Equal(DateTime.Parse("2018-10-15 10:10:10"), jT808_0x0200.GPSTime);
Assert.Equal(12222222, jT808_0x0200.Lat);
Assert.Equal(132444444, jT808_0x0200.Lng);
Assert.Equal(60, jT808_0x0200.Speed);
Assert.Equal(0, jT808_0x0200.Direction);
Assert.Equal((uint)2, jT808_0x0200.StatusFlag);
//4.1.Additional Information 1
Assert.Equal(100, ((JT808_0x0200_0x01)jT808_0x0200.JT808LocationAttachData[JT808Constants.JT808_0x0200_0x01]).Mileage);
//4.2.Additional Information 2
Assert.Equal(125, ((JT808_0x0200_0x02)jT808_0x0200.JT808LocationAttachData[JT808Constants.JT808_0x0200_0x02]).Oil);
```

### Nutshell 2

``` create package
// Create JT808Package package using extension method of message Id
JT808Package jT808Package = Enums.JT808MsgId.位置信息汇报.Create("123456789012",
    new JT808_0x0200 {
        AlarmFlag = 1,
        Altitude = 40,
        GPSTime = DateTime.Parse("2018-10-15 10:10:10"),
        Lat = 12222222,
        Lng = 132444444,
        Speed = 60,
        Direction = 0,
        StatusFlag = 2,
        JT808LocationAttachData = new Dictionary<byte, JT808LocationAttachBase>
        {
            { JT808Constants.JT808_0x0200_0x01,new JT808_0x0200_0x01{Mileage = 100}},
            { JT808Constants.JT808_0x0200_0x02,new JT808_0x0200_0x02{Oil = 125}}
        }
});

byte[] data = JT808Serializer.Serialize(jT808Package);

var hex = data.ToHexString();
//Output Hex：
//7E 02 00 00 26 12 34 56 78 90 12 00 01 00 00 00 01 00 00 00 02 00 BA 7F 0E 07 E4 F1 1C 00 28 00 3C 00 00 18 10 15 10 10 10 01 04 00 00 00 64 02 02 00 7D 01 6C 7E
```

### Nutshell 3

``` config
// Initial configuration
IJT808Config DT1JT808Config = new DT1Config();
IJT808Config DT2JT808Config = new DT2Config();
// Registering a custom message external assembly
DT1JT808Config.Register(Assembly.GetExecutingAssembly());
// Skip checksum verification
DT1JT808Config.SkipCRCCode = true;
// Add custom message Id
DT1JT808Config.MsgIdFactory.SetMap<DT1Demo6>();
DT2JT808Config.MsgIdFactory.SetMap<DT2Demo6>();
// Serialization
JT808Serializer DT1JT808Serializer = new JT808Serializer(DT1JT808Config);
JT808Serializer DT2JT808Serializer = new JT808Serializer(DT2JT808Config);
```

[SimplesDemo6](https://github.com/SmallChi/JT808/blob/master/src/JT808.Protocol.Test/Simples/Demo6.cs)

## NuGet installation

| Package Name          | Version                                            | Downloads                                           |
| --------------------- | -------------------------------------------------- | --------------------------------------------------- |
| Install-Package JT808 | ![JT808](https://img.shields.io/nuget/v/JT808.svg) | ![JT808](https://img.shields.io/nuget/dt/JT808.svg) |
| Install-Package JT808.Protocol.Extensions.JT1078 | ![JT808.Protocol.Extensions.JT1078](https://img.shields.io/nuget/v/JT808.Protocol.Extensions.JT1078.svg) | ![JT808](https://img.shields.io/nuget/dt/JT808.Protocol.Extensions.JT1078.svg) |
| Install-Package JT808.Protocol.Extensions.JTActiveSafety| ![JT808.Protocol.Extensions.JTActiveSafety](https://img.shields.io/nuget/v/JT808.Protocol.Extensions.JTActiveSafety.svg) | ![JT808](https://img.shields.io/nuget/dt/JT808.Protocol.Extensions.JTActiveSafety.svg) |

## Use BenchmarkDotNet performance test report

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8700K CPU 3.70GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
  Job-DSNYYA : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT

Platform=AnyCpu  Server=False  Toolchain=.NET Core 3.1  

```
|                          Method |       Categories |      N |          Mean |         Error |        StdDev |      Gen 0 | Gen 1 | Gen 2 |    Allocated |
|-------------------------------- |----------------- |------- |--------------:|--------------:|--------------:|-----------:|------:|------:|-------------:|
|   **0x0200_All_AttachId_Serialize** | **0x0200Serializer** |    **100** |     **262.46 us** |      **3.178 us** |      **2.654 us** |    **31.7383** |     **-** |     **-** |    **196.09 KB** |
| 0x0200_All_AttachId_Deserialize | 0x0200Serializer |    100 |     812.97 us |      3.537 us |      2.953 us |    80.0781 |     - |     - |    493.75 KB |
|   **0x0200_All_AttachId_Serialize** | **0x0200Serializer** |  **10000** |  **26,537.30 us** |    **382.221 us** |    **319.172 us** |  **3187.5000** |     **-** |     **-** |  **19609.38 KB** |
| 0x0200_All_AttachId_Deserialize | 0x0200Serializer |  10000 |  82,939.55 us |  1,133.599 us |  1,060.370 us |  8000.0000 |     - |     - |  49376.19 KB |
|   **0x0200_All_AttachId_Serialize** | **0x0200Serializer** | **100000** | **261,695.51 us** |  **5,005.517 us** |  **5,355.847 us** | **32000.0000** |     **-** |     **-** | **196093.75 KB** |
| 0x0200_All_AttachId_Deserialize | 0x0200Serializer | 100000 | 824,904.93 us | 16,325.230 us | 15,270.630 us | 80000.0000 |     - |     - |    493750 KB |
|                                 |                  |        |               |               |               |            |       |       |              |
|                 **0x0100Serialize** | **0x0100Serializer** |    **100** |      **78.05 us** |      **0.984 us** |      **0.920 us** |    **10.6201** |     **-** |     **-** |     **65.63 KB** |
|               0x0100Deserialize | 0x0100Serializer |    100 |      81.54 us |      1.618 us |      1.798 us |    15.6250 |     - |     - |     96.09 KB |
|                 **0x0100Serialize** | **0x0100Serializer** |  **10000** |   **8,007.65 us** |     **97.177 us** |     **86.145 us** |  **1062.5000** |     **-** |     **-** |    **6562.5 KB** |
|               0x0100Deserialize | 0x0100Serializer |  10000 |   8,028.20 us |    123.340 us |    115.372 us |  1562.5000 |     - |     - |   9609.44 KB |
|                 **0x0100Serialize** | **0x0100Serializer** | **100000** |  **80,765.75 us** |  **1,016.904 us** |    **901.459 us** | **10571.4286** |     **-** |     **-** |  **65625.28 KB** |
|               0x0100Deserialize | 0x0100Serializer | 100000 |  80,955.97 us |    958.045 us |    849.282 us | 15571.4286 |     - |     - |  96093.87 KB |

## JT808 terminal communication protocol message comparison table

| ID  | Message ID        | Status Execution | Status Test | Message Body                     |2019 release|
| :---: | :-----------: | :------: | :------: | :----------------------------: |:----------------------------:|
| 1     | 0x0001        | √        | √        | Terminal universal response	                   |
| 2     | 0x8001        | √        | √        | Platform universal response	                   |
| 3     | 0x0002        | √        | √        | Terminal heartbeat	                       |
| 4     | 0x8003        | √        | √        | Subcontracting Request	                   |
| 5     | 0x0100        | √        | √        | Terminal registration	                       |modified|
| 6     | 0x8100        | √        | √        | Terminal registration response	                   |
| 7     | 0x0003        | √        | √        | Terminal logout	                       |
| 8     | 0x0102        | √        | √        | Terminal authentication	                       |modified|
| 9     | 0x8103        | √        | √        | Setting terminal parameters	                   |modified and add|
| 10    | 0x8104        | √        | √        | Query terminal parameters	                   |
| 11    | 0x0104        | √        | √        | Query terminal parameter response	               |
| 12    | 0x8105        | √        | √        | Terminal control	                       |
| 13    | 0x8106        | √        | √        | Querying specified terminal parameters	               |
| 14    | 0x8107        | √        | Msg Body is empty| Query terminal properties	                   |
| 15    | 0x0107        | √        | √        | Query terminal attribute response	               |
| 16    | 0x8108        | √        | √        | Issue terminal upgrade package	                 |
| 17    | 0x0108        | √        | √        | Terminal upgrade result notification	               |
| 18    | 0x0200        | √        | √        | Location Information Report	                   |Add additional information|
| 19    | 0x8201        | √        | √        | Location information query	                   |
| 20    | 0x0201        | √        | √        | Location information query response	               |
| 21    | 0x8202        | √        | √        | Temporary position tracking control	               |
| 22    | 0x8203        | √        | √        | Manually acknowledge alarm messages	               |
| 23    | 0x8300        | √        | √        | Text message delivery	                   |modified|
| 24    | 0x8301        | √        | √        | Event settings	                       |deleted|
| 25    | 0x0301        | √        | √        | Incident report	                       |deleted|
| 26    | 0x8302        | √        | √        | Questions issued	                       |deleted|
| 27    | 0x0302        | √        | √        | Question Answer	                       |deleted|
| 28    | 0x8303        | √        | √        | Information on demand menu settings	               |deleted|
| 29    | 0x0303        | √        | √        | Information On Demand / Cancel	                  |deleted|
| 30    | 0x8304        | √        | √        | Information service	                       |deleted|
| 31    | 0x8400        | √        | √        | Call back	                       |
| 32    | 0x8401        | √        | √        | Set up phone book	                     |
| 33    | 0x8500        | √        | √        | Vehicle control	                       |modified|
| 34    | 0x0500        | √        | √        | Vehicle control response	                   |
| 35    | 0x8600        | √        | √        | Set circular area	                   |modified|
| 36    | 0x8601        | √        | √        | Delete circular area	                   |
| 37    | 0x8602        | √        | √        | Set rectangular area	                   |modified|
| 38    | 0x8603        | √        | √        | Delete rectangular area	                   |
| 39    | 0x8604        | √        | √        | Set polygon area	                 |modified|
| 40    | 0x8605        | √        | √        | Delete polygon area	                 |
| 41    | 0x8606        | √        | √        | Set route	                       |modified|
| 42    | 0x8607        | √        | √        | Delete route	                       |
| 43    | 0x8700        | x        | x      | Tachograph data acquisition commands	         |Todo: To be developed
| 44    | 0x0700        | x        | x      | Tachograph data upload	             |Todo: To be developed
| 45    | 0x8701        | x        | x      | Drive recorder parameter download command	         |Todo: To be developed
| 46    | 0x0701        | √        | √        | Electronic waybill report	                   |
| 47    | 0x0702        | √        | √        | Collection and reporting of driver identity information	         |modified|
| 48    | 0x8702        | √        | Msg Body is empty| Request to report driver identity information	         |
| 49    | 0x0704        | √        | √        | Batch upload of positioning data	               |modified|
| 50    | 0x0705        | √        | √        | CAN bus data upload	               |modified|
| 51    | 0x0800        | √        | √        | Multimedia event information upload	             |
| 52    | 0x0801        | √        | √        | Multimedia data upload	                 |modified|
| 53    | 0x8800        | √        | √        | Multimedia data upload response	             |
| 54    | 0x8801        | √        | √        | Camera Instant Command	             |modified|
| 55    | 0x0805        | √        | √        | Camera captures command immediately	         |modified|
| 56    | 0x8802        | √        | √        | Stored multimedia data retrieval	             |
| 57    | 0x0802        | √        | √        | Store multimedia data retrieval response	         |
| 58    | 0x8803        | √        | √        | Store multimedia data upload	             |
| 59    | 0x8804        | √        | √        | Recording start command	                   |
| 60    | 0x8805        | √        | √        | Single storage multimedia data retrieval upload command	 |modified|
| 61    | 0x8900        | √        | √        | Data downlink transparent transmission	                   |modified|
| 62    | 0x0900        | √        | √        | Data uplink transparent transmission	                   |modified|
| 63    | 0x0901        | √        | √        | Data compression report	                   |
| 64    | 0x8A00        | √        | √        | Platform RSA Public Key	                |
| 65    | 0x0A00        | √        | √        | Terminal RSA Public Key                 |
| 66    | 0x8F00~0x8FFF | Keep     | Keep     | Platform downlink message retention               |
| 67    | 0x0F00~0x0FFF | Keep     | Keep     | Terminal uplink message retention               |
| 68    | 0x0004 | √     | √     | Query server time request	             |added|
| 69    | 0x8004 | √     | √     | Query server time response	             |added|
| 70    | 0x0005 | √     | √     | Terminal Sub-transmission Subcontracting Request	               |added|
| 71    | 0x8204 | √     | √     | Link detection	               |added|
| 72    | 0x8608 | √     | √     | Query area or line data	      |added|
| 73    | 0x0608 | √     | √     | Query area or line data response	  |added|
| 74    | 0xE000~0xEFFF | Keep     | Keep     | Vendor-defined upstream message	      |added|
| 75    | 0xF000~0xFFFF | Keep     | Keep     | Vendor-defined downstream message	  |added|
