﻿using JT808.Protocol.Enums;
using JT808.Protocol.Extensions;
using JT808.Protocol.Formatters;
using JT808.Protocol.Interfaces;
using JT808.Protocol.MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace JT808.Protocol.MessageBody.CarDVR
{
    /// <summary>
    /// 采集车辆信息
    /// 返回：车辆识别代号、机动车号牌号码和机动车号牌分类
    /// </summary>
    public class JT808_CarDVR_Down_0x05 : JT808CarDVRDownBodies
    {
        public override byte CommandId => JT808CarDVRCommandID.采集车辆信息.ToByteValue();

        public override string Description => "车辆识别代号、机动车号牌号码和机动车号牌分类";

        public override bool SkipSerialization { get; set; } = true;
    }
}
