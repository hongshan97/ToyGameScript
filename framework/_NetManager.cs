using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
// using UnityEngine.UI;
using System;

using proto.Enter;


public class _NetManager
{
    // socket
    static Socket socket;
    
    // 接收
    static ByteArray readBuff;
    static List<MsgBase> msgList = new List<MsgBase>();
    static int msgCount = 0;
    readonly static int MAX_MSG_FIRE = 10; // 每帧处理多少msg

    // 发送
    static Queue<ByteArray> writeQueue;  

    // 网络事件
    public enum NetEvent {
        ConnectSucc = 1,
        ConnectFail = 2,
        ConnectClose = 3,
    }
    public delegate void EventListener(string msg); // 网络事件委托类型
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>(); // 监听网络事件列表

    // 消息事件
    public delegate void MsgListener(MsgBase msg); // 消息事件委托类型
    public static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    // 状态
    static bool isConnecting = false;
    static bool isClosing = false;

        // 获取描述
    public static string GetDesc() {
        if(socket == null) 
            return "";
        if(!socket.Connected)
            return "";

        return socket.LocalEndPoint.ToString();
    }


    // 添加监听事件
    public static void AddEventListener(NetEvent netEvent, EventListener listener) {
        if(eventListeners.ContainsKey(netEvent))
            eventListeners[netEvent] += listener;
        else
            eventListeners[netEvent] = listener;
    }
    
    // 删除监听事件
    public static void RemoveEventListener(NetEvent netEvent, EventListener listener) {
        if(eventListeners.ContainsKey(netEvent)) {
            eventListeners[netEvent] -= listener;
            if(eventListeners[netEvent] == null)
                eventListeners.Remove(netEvent);
        }
    }

    // 事件分发
    public static void FireEvent(NetEvent netEvent, string msg) {
        if(eventListeners.ContainsKey(netEvent)) 
            eventListeners[netEvent](msg);  // 这是在异步线程中执行的，不能操作unity部件，可以用mq传消息让主线程做
        else
Debug.Log("不存在的netEvent事件处理");
    }

    public static void Connect(string ip, int port) {
        if(socket != null && socket.Connected) {
Debug.Log("connect失败，已经连接");
            return;
        } else if(isConnecting) {
Debug.Log("connect失败，正在连接");
            return;
        }
        InitState();
        socket.NoDelay = true;
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallBack, socket);
    }

    private static void InitState() {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        readBuff = new ByteArray();
        writeQueue = new Queue<ByteArray>();
        isConnecting = false;
        isClosing = false;
        msgList = new List<MsgBase>();
        msgCount = 0;
    }

    private static void ConnectCallBack(IAsyncResult ar) {
        try {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndConnect(ar);
Debug.Log("connect成功");

            MsgBase msgBase = new MsgBase("ConnectSucc");
            lock(msgList) {
                msgList.Add(msgBase);
            }
            ++msgCount;

            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;

            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallBack, socket);
        }
        catch (SocketException ex) {
Debug.Log("connect失败：" + ex.ToString());

            MsgBase msgBase = new MsgBase("ConnectFail");
            lock(msgList) {
                msgList.Add(msgBase);
            }
            ++msgCount;

            FireEvent(NetEvent.ConnectFail, "");
            isConnecting = false;
        }
    }

    public static void ReceiveCallBack(IAsyncResult ar) {
        Socket socket = (Socket)ar.AsyncState;
        int count = socket.EndReceive(ar);
Debug.Log("收到" + count);
        if(count == 0) {
            Close();
            return;
        }
        readBuff.writeIdx += count;
        OnReceiveData();
        if(readBuff.remain < 8) {
            readBuff.MoveBytes();
            readBuff.ReSize(readBuff.length * 2);
        }
        socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallBack, socket);
    }

    public static void OnReceiveData() {
        if(readBuff.length < 2)
            return;
        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
        if(readBuff.length < bodyLength + 2)
            return;
        readBuff.readIdx += 2;

        int nameCount = 0;
        string msgName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if(msgName == "")
            return;
        readBuff.readIdx += nameCount; // 不在if前？

        int bodyCount = bodyLength - nameCount;
// Debug.Log("收到msgName" + msgName + "         count :" + msgCount);
        MsgBase msgBase = MsgBase.Decode(msgName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();
        lock(msgList) {
            msgList.Add(msgBase);
        }
        ++msgCount;
        
        if(readBuff.length > 2) 
            OnReceiveData();
    }

    public static void Close() {
        if(socket == null || !socket.Connected || isClosing) 
            return;
        if(writeQueue.Count > 0)
            isClosing = true;
        else {
            socket.Close();

            MsgBase msgBase = new MsgBase("ConnectClose");
            lock(msgList) {
                msgList.Add(msgBase);
            }

            FireEvent(NetEvent.ConnectClose, "");
        }
    }

    // 发送数据
    public static void Send(MsgBase msgBase) {
        if(socket == null || !socket.Connected || isConnecting || isClosing) {
            return;
        }
        
        byte[] name = MsgBase.EncodeName(msgBase);
        byte[] body = MsgBase.Encode(msgBase.msgBody);
        int len = name.Length + body.Length;

        byte[] sendBytes = new byte[len + 2];
        sendBytes[0] = (byte)(len % 256);
        sendBytes[1] = (byte)(len / 256);
        Array.Copy(name, 0, sendBytes, 2, name.Length);
        Array.Copy(body, 0, sendBytes, 2 + name.Length, body.Length);
        ByteArray ba = new ByteArray(sendBytes);
        
        int count = 0;
        lock(writeQueue) {
            writeQueue.Enqueue(ba);
            count = writeQueue.Count;
        }

        if(count == 1)  {// 当前writeQueue中只有刚刚加入的ba，直接发送 
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallBack, socket);
        }
    }

    public static void SendCallBack(IAsyncResult ar) {
        Socket socket = (Socket)ar.AsyncState;
        if(socket == null || !socket.Connected) 
            return;
        int count = socket.EndSend(ar);
Debug.Log("发了" + count);

        ByteArray ba;
        lock(writeQueue) {
            ba = writeQueue.Peek();
        }
        ba.readIdx += count;
        if(ba.length == 0) {
            lock(writeQueue) {
                writeQueue.Dequeue();
                ba = writeQueue.Peek();
            }
        }
        if(ba != null) 
            socket.BeginSend(ba.bytes, ba.readIdx, ba.length, 0, SendCallBack, socket);
        else if(isClosing) 
            socket.Close();
    }

    public static void AddMsgListener(string msgName, MsgListener listener) {
        if(msgListeners.ContainsKey(msgName)) 
            msgListeners[msgName] += listener;
        else 
            msgListeners[msgName] = listener;
    }

    public static void RemoveMsgListener(string msgName, MsgListener listener) {
        if(msgListeners.ContainsKey(msgName)) {
            msgListeners[msgName] -= listener;
            if(msgListeners[msgName] == null) 
                msgListeners.Remove(msgName);
        }
    }

    private static void FireMsg(string msgName, MsgBase msgBase) {
        if(msgListeners.ContainsKey(msgName)) {
            msgListeners[msgName](msgBase); // 这是在异步线程中执行的，不能操作unity部件，mq传消息让主线程读取mq
        }
    }

    public static void MsgUpdate() {
        if(msgCount == 0)
            return;
        for(int i = 0; i < MAX_MSG_FIRE; ++i) {
            MsgBase msgBase = null;
            lock(msgList) {
                if(msgList.Count > 0) {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    --msgCount;
                }
            }
            if(msgBase != null) {
                FireMsg(msgBase.msgName, msgBase);
            }
            else {
                break;
            }
        }
    }
}
