﻿using JT808.Protocol.Enums;
using JT808.Protocol.Extensions;
using JT808.Protocol.Formatters;
using JT808.Protocol.Interfaces;
using JT808.Protocol.MessagePack;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace JT808.Protocol.MessageBody.CarDVR
{
    /// <summary>
    /// 采集记录仪执行标准版本
    /// 返回：记录仪执行标准的年号及修改单号
    /// </summary>
    public class JT808_CarDVR_Up_0x00 : JT808CarDVRUpBodies, IJT808MessagePackFormatter<JT808_CarDVR_Up_0x00>, IJT808Analyze
    {
        public override byte CommandId => JT808CarDVRCommandID.采集记录仪执行标准版本.ToByteValue();
        /// <summary>
        /// 记录仪执行标准年号后 2 位  BCD 码
        /// 无应答则默认为 03
        /// </summary>
        public string StandardYear { get; set; }
        /// <summary>
        /// 修改单号
        /// 无修改单或无应答则默认为 00H
        /// </summary>
        public byte ModifyNumber { get; set; }
        public override string Description => "记录仪执行标准的年号及修改单号";

        public void Analyze(ref JT808MessagePackReader reader, Utf8JsonWriter writer, IJT808Config config)
        {

        }

        public void Serialize(ref JT808MessagePackWriter writer, JT808_CarDVR_Up_0x00 value, IJT808Config config)
        {
            writer.WriteBCD(value.StandardYear, 2);
            writer.WriteByte(value.ModifyNumber);
        }

        public JT808_CarDVR_Up_0x00 Deserialize(ref JT808MessagePackReader reader, IJT808Config config)
        {
            JT808_CarDVR_Up_0x00 value = new JT808_CarDVR_Up_0x00();
            value.StandardYear = reader.ReadBCD(2);
            value.ModifyNumber = reader.ReadByte();
            return value;
        }
    }
}
