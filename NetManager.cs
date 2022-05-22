using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net.Sockets;
// using UnityEngine.UI;
using System;

public class NetManager
{
    static Socket socket;

    static byte[] readBuff = new byte[1024]; // 接收缓冲区

    public delegate void MsgListener(string str); // 委托类型

    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>(); // 消息列表

    static List<string> msgList = new List<string>();

    // 添加监听
    public static void AddListener(string msgName, MsgListener listener) {
        listeners[msgName] = listener;
    }

    // 获取描述
    public static string GetDesc() {
        if(socket == null) 
            return "";
        if(!socket.Connected)
            return "";

        return socket.LocalEndPoint.ToString();
    }

    // 连接
    public static void Connect(string ip, int port) {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        socket.Connect(ip, port);

        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
    }

    // receive回调
    private static void ReceiveCallBack(IAsyncResult ar) {
        try {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
        }
        catch(SocketException ex) {
            Debug.Log("Socket Receive Fail" + ex.ToString());
        }
    }

    // 发送
    public static void Send(string sendStr) {
        if(socket == null) 
            return;
        if(!socket.Connected) 
            return;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    // 
    public static void Update() {
        if(msgList.Count <= 0) 
            return;
        string msgStr = msgList[0];
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];

        if(listeners.ContainsKey(msgName)) {
            listeners[msgName](msgArgs);
        }
        else {
Debug.Log("不存在" + msgName);
        }
    }


}
