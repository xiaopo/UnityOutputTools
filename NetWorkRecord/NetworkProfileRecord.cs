using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class NetworkProfileRecord: NetWorkProfileBase<NetworkProfileRecord>
{

    public void InitClient()
    {

        revice_list = new Dictionary<int, CMDPProfileItem>();
        revicePB_list = new Dictionary<int, PBInfoItem>();
        event_list = new Dictionary<string, CMDPProfileItem>();

       startPBRecord = true;
        NetworkClientManager.GetInstance().GameNetwork.OnRecord = (ushort cmd, byte[] data, uint size) =>
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            OnRecordReive(revice_list, cmd, size, ts.TotalMilliseconds, true);
        };

        NetworkClientManager.GetInstance().GameNetwork.OnRecordEnd = (ushort cmd, byte[] data, uint size) =>
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            OnRecordReive(revice_list, cmd, size, ts.TotalMilliseconds, false);

            SaveCMDFile(revice_list, "{0}CMD_LOG.csv", (a, b) => -a.averageTime.CompareTo(b.averageTime));//保存
        };


    }



 
    #region record PBINFO

    /// <summary>
    /// 记录PB文件大小
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="size"></param>
    /// <param name="PBName"></param>
    public void OnRecordDateil(int cmd, float size, string PBName, string des = "")
    {
        if (!brecrodd)
        {
            brecrodd = true;
            startSpan = new TimeSpan(DateTime.Now.Ticks);
        }

        PBInfoItem item = null;
        if (!revicePB_list.TryGetValue(cmd, out item))
        {
            item = new PBInfoItem();
            item.cmd = cmd;
            item.des = des;
            revicePB_list.Add(cmd, item);
        }

        PBSizeItem sizeInfo = null;
        if (!item.pbinfos.TryGetValue(PBName, out sizeInfo))
        {
            sizeInfo = new PBSizeItem();
            sizeInfo.pdName = PBName;
            item.pbinfos.Add(PBName, sizeInfo);
        }
        sizeInfo.count++;
        sizeInfo.totalSize += size;
        if (sizeInfo.maxSize < size) sizeInfo.maxSize = size;
        sizeInfo.average = sizeInfo.totalSize / sizeInfo.count;

        SavePBFile();
    }

    private double savePBTime = 0;
    private string foram2tStr = "\n{0},{1},{2},{3},{4},{5},{6}";
    public void SavePBFile()
    {
        StringBuilder datastring = null;
        string savePath = AssetDefine.ExternalSDCardsPath;
        if (!WriteReady(ref savePBTime, ref savePath, out datastring, "{0}PB_LOG.csv", "消息号,事件,PB包,次数,单次最大SIZE,平均SIZE,总SIZE")) return;

        foreach (var item in revicePB_list)
        {
            List<PBSizeItem> list = new List<PBSizeItem>();

            foreach (var pbInfo in item.Value.pbinfos)
            {
                list.Add(pbInfo.Value);
            }
            list.Sort((a, b) => -a.totalSize.CompareTo(b.totalSize));//降序

            for (int i = 0; i < list.Count; i++)
            {
                PBSizeItem sizeInfo = list[i];
                string str = string.Format(foram2tStr, item.Value.cmd, item.Value.des, sizeInfo.pdName, sizeInfo.count, formatSize(sizeInfo.maxSize), formatSize(sizeInfo.average), formatSize(sizeInfo.totalSize));
                datastring.Append(str);
            }

        }

        WriteFile(savePath, datastring.ToString());

    }
    #endregion

   

   
}


