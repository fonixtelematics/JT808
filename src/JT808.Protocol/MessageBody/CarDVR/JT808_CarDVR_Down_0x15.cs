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
    /// 采集指定的速度状态日志
    /// 返回：符合条件的速度状态日志
    /// </summary>
    public class JT808_CarDVR_Down_0x15 : JT808CarDVRDownBodies, IJT808MessagePackFormatter<JT808_CarDVR_Down_0x15>
    {
        public override byte CommandId => JT808CarDVRCommandID.采集指定的速度状态日志.ToByteValue();

        public override string Description => "符合条件的速度状态日志";
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 最大单位数据块个数
        /// </summary>
        public ushort Count { get; set; }

        public void Serialize(ref JT808MessagePackWriter writer, JT808_CarDVR_Down_0x15 value, IJT808Config config)
        {
            writer.WriteDateTime6(value.StartTime);
            writer.WriteDateTime6(value.EndTime);
            writer.WriteUInt16(value.Count);
        }

        public JT808_CarDVR_Down_0x15 Deserialize(ref JT808MessagePackReader reader, IJT808Config config)
        {
            JT808_CarDVR_Down_0x15 value = new JT808_CarDVR_Down_0x15();
            value.StartTime = reader.ReadDateTime6();
            value.EndTime = reader.ReadDateTime6();
            value.Count = reader.ReadUInt16();
            return value;
        }
    }
}
