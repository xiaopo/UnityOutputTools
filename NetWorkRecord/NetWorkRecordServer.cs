using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NetWorkRecordServer : NetWorkProfileBase<NetWorkRecordServer>
{
    protected Dictionary<int, CMDPProfileItem> broadCast_list;
    protected Dictionary<int, CMDPProfileItem> send_list;
    protected Dictionary<int, CMDPProfileItem> recive_command_list;
    public void InitServer()
    {

        revice_list = new Dictionary<int, CMDPProfileItem>();
        broadCast_list = new Dictionary<int, CMDPProfileItem>();
        send_list = new Dictionary<int, CMDPProfileItem>();
        recive_command_list = new Dictionary<int, CMDPProfileItem>();//接收的客户端命令
        event_list = new Dictionary<string, CMDPProfileItem>();

        startPBRecord = true;

        RoomConnectManager.GetInstance().GameNetwork.OnRecord = (long sessionID, ushort cmd, byte[] data, uint size) =>
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            OnRecordReive(revice_list, cmd, size, ts.TotalMilliseconds, true);
        };

        RoomConnectManager.GetInstance().GameNetwork.OnRecordEnd = (long sessionID, ushort cmd, byte[] data, uint size) =>
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            OnRecordReive(revice_list, cmd, size, ts.TotalMilliseconds, false);
            SaveCMDFile(revice_list, "{0}_HandleMessage_LOG.csv", (a, b) => -a.totalTime.CompareTo(b.totalTime));//保存
        };


        RoomConnectManager.GetInstance().GameNetwork.onBroadcastRecord = (long sessionID, ushort cmd, byte[] data, uint size) =>
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            //广播
            OnRecordReive(broadCast_list, cmd, size, ts.TotalMilliseconds, true);
        };

        RoomConnectManager.GetInstance().GameNetwork.onBroadcastRecordEnd = (long sessionID, ushort cmd, byte[] data, uint size) =>
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            //广播
            OnRecordReive(broadCast_list, cmd, size, ts.TotalMilliseconds, false);
            SaveCMDFile(broadCast_list, "{0}_Broadcast(R2C_PROTOCOL).csv", (a, b) => -a.count.CompareTo(b.count));//保存
        };


        RoomConnectManager.GetInstance().GameNetwork.onSendRecord = (long sessionID, ushort cmd, byte[] data, uint size) =>
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            //服务器Send
            OnRecordReive(send_list, cmd, size, ts.TotalMilliseconds, true);
        };

        RoomConnectManager.GetInstance().GameNetwork.onSendRecordEnd = (long sessionID, ushort cmd, byte[] data, uint size) =>
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            //服务器Send
            OnRecordReive(send_list, cmd, size, ts.TotalMilliseconds, false);
            SaveCMDFile(send_list, "{0}_Send(R2C_PROTOCOL).csv", (a, b) => -a.totalTime.CompareTo(b.totalTime));//保存
        };
    }


    public void OnRecordCommands(int cmd, float size, bool start = true)
    {
        TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
        //服务器Send
        OnRecordReive(recive_command_list, cmd, size, ts.TotalMilliseconds, start);
        SaveCMDFile(recive_command_list, "{0}_Receive(C2R_PROTOCOL.C2RCommands).csv", (a, b) => -a.totalTime.CompareTo(b.totalTime));//保存
    }

}
