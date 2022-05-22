using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MsgBase {
    public string msgName = "";
    public ProtoBuf.IExtensible msgBody= null;

    public MsgBase(string name = "", ProtoBuf.IExtensible body = null) {
        msgName = name;
        msgBody = body;
    }
    public static byte[] EncodeName(MsgBase msgBase) {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(msgBase.msgName);
        Int16 len = (Int16)bytes.Length;

        byte[] ret = new byte[2 + len];
        ret[0] = (byte)(len % 256);
        ret[1] = (byte)(len / 256);

        Array.Copy(bytes, 0, ret, 2, len);

        return ret;
    }

    public static string DecodeName(byte[] bytes, int offset, out int count) {
        count = 0;
        if(offset + 2 > bytes.Length)
            return "";
        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        if(len <= 0)
            return "";
        if(offset + 2 + len > bytes.Length)
            return "";
        count = 2 + len;
        return System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
    }

    public static byte[] Encode(ProtoBuf.IExtensible msgBody) {
        using (var memory = new System.IO.MemoryStream()) {
            ProtoBuf.Serializer.Serialize(memory, msgBody);
            return memory.ToArray();
        }
    }
    
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count) {
        using (var memory = new System.IO.MemoryStream(bytes, offset, count)) {
            System.Type t = System.Type.GetType("proto." + protoName + "."+ protoName);  // 约定proto协议命名空间和msg名称一样，如proto.Enter.Enter

            MsgBase msgBase = new MsgBase(protoName);
            if(t == null)
                return null;
            msgBase.msgBody = (ProtoBuf.IExtensible)ProtoBuf.Serializer.NonGeneric.Deserialize(t, memory);

            return msgBase;
        }
    }
}