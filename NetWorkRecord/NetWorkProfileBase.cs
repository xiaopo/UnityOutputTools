using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class CMDPProfileItem
{
    public long session;
    public int cmd;
    public float maxSize;
    public float totalSize;
    public float averageSize;
    public string cmdName;
    public double totalTime;
    public double maxPassTime;
    public double averageTime;
    public double time;
    public int count;
    public string des;

    public int curFrame;

    public int framecount;
    public double frameTime;
    public float frameSize;

    public int frameMaxcount;
    public double frameMaxTime;
    public float frameMaxSize;
}

public class PBInfoItem
{
    public int cmd;
    public string des;
    public Dictionary<string, PBSizeItem> pbinfos = new Dictionary<string, PBSizeItem>();
}
public class PBSizeItem
{
    public string pdName;
    public int count;
    public float maxSize;
    public float totalSize;
    public float average;
}


public class NetWorkProfileBase<T> where T : class, new()
{
    public static bool startPBRecord = false;
    public bool brecrodd = false;
    protected Dictionary<int, CMDPProfileItem> revice_list;
    protected Dictionary<int, PBInfoItem> revicePB_list;
    protected Dictionary<string, CMDPProfileItem> event_list;
    protected TimeSpan startSpan;

    private static T m_instance;
    public static T instance 
    { 
        get 
        {
            if (m_instance == null) m_instance = new T();


            return m_instance;
        } 
    
    }

    #region save cmd Info

    public virtual void OnRecordReive(Dictionary<int, CMDPProfileItem> list,int cmd, float size, double time, bool start = true)
    {
        if (!brecrodd)
        {
            brecrodd = true;
            startSpan = new TimeSpan(DateTime.Now.Ticks);
        }

        CMDPProfileItem item = null;
        if (!list.TryGetValue(cmd, out item))
        {
            item = new CMDPProfileItem();
            item.cmd = cmd;
            list.Add(cmd, item);
        }
       
        if (start)
        {
            item.time = time;
            item.totalSize += size;
            item.count++;

            item.averageSize = item.totalSize / item.count;

            if (item.maxSize < size) item.maxSize = size;
        }
        else
        {
            //耗时
            double passTime = time - item.time;
            item.totalTime += passTime;

            if (item.maxPassTime < passTime) item.maxPassTime = passTime;

            item.averageTime = item.totalTime / item.count;


            if (item.curFrame!= Time.frameCount)
            {
                //不同帧
                item.curFrame = Time.frameCount;
                item.framecount = 1;
                item.frameSize = size;
                item.frameTime = passTime;

            }
            else
            {
                //同一帧
                item.framecount++;
                item.frameSize += size;
                item.frameTime += passTime;
            }

            if (item.frameMaxcount < item.framecount) item.frameMaxcount = item.framecount;
            if (item.frameMaxTime < item.frameTime) item.frameMaxTime = item.frameTime;
            if (item.frameMaxSize < item.frameSize) item.frameMaxSize = item.frameSize;
        }

       
        
    }

    private double saveTime = 0;
    private string titleString = "消息号,次数,单帧最大次数,单次最大SIZE,单帧最大SIZE,平均SIZE,总SIZE,单次最大耗时,单帧最大耗时,平均耗时,总耗时";
    private string foramtStr1 = "\n{0},{1},{2},{3},{4},{5},{6},{7}ms,{8}ms,{9}ms,{10}ms";
    public virtual void SaveCMDFile(Dictionary<int, CMDPProfileItem> writelist,string FileName, Comparison<CMDPProfileItem> comparison)
    {
        StringBuilder datastring = null;
        string savePath = AssetDefine.ExternalSDCardsPath;
        if (!WriteReady(ref saveTime, ref savePath, out datastring, FileName, titleString)) return;
        List<CMDPProfileItem> list = new List<CMDPProfileItem>();
        foreach (var item in writelist)
        {
            list.Add(item.Value);
        }
       // list.Sort((a, b) => -a.averageTime.CompareTo(b.averageTime));//降序
        list.Sort(comparison);
        for (int i = 0; i < list.Count; i++)
        {
            CMDPProfileItem item = list[i];
            string str = string.Format(foramtStr1, item.cmd, item.count, item.frameMaxcount, formatSize(item.maxSize), formatSize(item.frameMaxSize), formatSize(item.averageSize), formatSize(item.totalSize), item.maxPassTime.ToString("f2"), item.frameMaxTime.ToString("f2"), item.averageTime.ToString("f2"), item.totalTime.ToString("f2"));
            datastring.Append(str);
        }


        WriteFile(savePath, datastring.ToString());

        datastring.Clear();


    }
    #endregion


    #region Event INFO save

    public void OnRecordEvent(string eventName, float size, double time, bool start = true)
    {
        if (!brecrodd)
        {
            brecrodd = true;
            startSpan = new TimeSpan(DateTime.Now.Ticks);
        }

        CMDPProfileItem item = null;
        if (!event_list.TryGetValue(eventName, out item))
        {
            item = new CMDPProfileItem();
            item.des = eventName;
            event_list.Add(eventName, item);
        }

        if (start)
        {
            item.time = time;
            item.totalSize += size;
            item.count++;

            item.averageSize = item.totalSize / item.count;

            if (item.maxSize < size) item.maxSize = size;
        }
        else
        {
            //耗时
            double passTime = time - item.time;
            item.totalTime += passTime;

            if (item.maxPassTime < passTime) item.maxPassTime = passTime;

            item.averageTime = item.totalTime / item.count;
        }

        SaveEventFile();
    }

    private double saveTimeEvent = 0;
    protected string foramtStr2 = "\n{0},{1},{2},{3},{4},{5}ms,{6}ms,{7}ms";
    public void SaveEventFile()
    {
        StringBuilder datastring = null;
        string savePath = AssetDefine.ExternalSDCardsPath;
        if (!WriteReady(ref saveTimeEvent, ref savePath, out datastring, "{0}Event_LOG.csv", "事件,次数,单次最大SIZE,平均SIZE,总SIZE,单次最大耗时,平均耗时,总耗时")) return;
        List<CMDPProfileItem> list = new List<CMDPProfileItem>();
        foreach (var item in event_list)
        {
            list.Add(item.Value);
        }
        list.Sort((a, b) => -a.totalTime.CompareTo(b.totalTime));//降序

        for (int i = 0; i < list.Count; i++)
        {
            CMDPProfileItem item = list[i];
            string str = string.Format(foramtStr2, item.des, item.count, formatSize(item.maxSize), formatSize(item.averageSize), formatSize(item.totalSize), item.maxPassTime.ToString("f2"), item.averageTime.ToString("f2"), item.totalTime.ToString("f2"));
            datastring.Append(str);
        }


        WriteFile(savePath, datastring.ToString());

        datastring.Clear();


    }


    #endregion
    public bool WriteReady(ref double time, ref string savePath, out StringBuilder datastring, string fileS, string tileS)
    {
        TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);

        datastring = null;

        if (ts.TotalSeconds - time < 1) return false;
        time = ts.TotalSeconds;


        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        string datestr = DateTime.Now.ToString("yyyyMMdd-hh").Replace(':', '-');
        savePath += string.Format(fileS, datestr);
        datastring = new StringBuilder();

        TimeSpan nowSpan = new TimeSpan(DateTime.Now.Ticks);
        TimeSpan passSpan = nowSpan - startSpan;
        datastring.Append(string.Format("记录总时间,{0}秒\n", passSpan.TotalSeconds));
        datastring.Append(tileS);

        return true;
    }



    public  void WriteFile(string savePath, string datastr)
    {
        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);
        StreamWriter streamWriter = new StreamWriter(file);//
        streamWriter.Write(datastr);
        streamWriter.Flush();
        streamWriter.Close();
    }


    private float mbsize = 1024.0f * 1024.0f;
    public  string formatSize(float size)
    {
        if (size < mbsize)
            return (size / 1024.0f).ToString("f2") + "kb";
        else
            return (size / mbsize).ToString("f2") + "mb";

    }

}
