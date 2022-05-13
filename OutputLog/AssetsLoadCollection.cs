using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class AssetsLoadCollection
{
    public class LoadItem
    {
        public string path;
        public string funcName;
        public int count;
        public float timeTotal;//ms
        public int frameTotal;

        public float beginTime;
        public int beginFrame;

        public float CostTime(float time)
        {
            return time - beginTime;
        }

        public int CostFrame(int frame)
        {
            return frame - beginFrame;
        }


        public int frameMax = 1;

        int curframe;
        int frameCount;
        public void CalculateFrameMax(int frame)
        {
            if (frame != curframe) { curframe = frame; frameCount = 0; }
            else
            {
                frameCount++;
                if (frameCount > frameMax) frameMax = frameCount;
            }
        }
    }

    static Dictionary<string, LoadItem> total_items = new Dictionary<string, LoadItem>();

    [System.Diagnostics.Conditional("DEBUG")]
    public static void BeginSampler(string path,string functioName = "")
    {

        if (!total_items.TryGetValue(path, out LoadItem item))
        {
            item = new LoadItem();
            item.funcName = string.IsNullOrEmpty(functioName) ? "QLoader.LoadAssetAsync()" : functioName;
            item.path = path;
            total_items.Add(path, item);
        }


        item.beginTime = Time.realtimeSinceStartup * 1000;//ms
        item.beginFrame = Time.frameCount;

        item.CalculateFrameMax(Time.frameCount);

    }

    [System.Diagnostics.Conditional("DEBUG")]
    public static void EndSampler(string path)
    {
        if (!total_items.TryGetValue(path, out LoadItem item)) return;

        item.count++;

        item.timeTotal += item.CostTime(Time.realtimeSinceStartup * 1000);
        item.frameTotal += item.CostFrame(Time.frameCount);

    }
    protected static void SortItems(List<LoadItem> list)
    {
        list.Sort((a, b) => -a.timeTotal.CompareTo(b.timeTotal));//
    }
    protected static StringBuilder datastring;
    [System.Diagnostics.Conditional("DEBUG")]
    public static void SaveData()
    {
        Profiler.BeginSample("AssetsLoadCollection");

        datastring = new StringBuilder();
        datastring.Append("资源路径,调用函数,总加载次,单帧最大加载次,均耗时(ms),总耗时(ms)\n");

        List<LoadItem> items = new List<LoadItem>();

        foreach (var map in total_items)
        {
            items.Add(map.Value);
        }

        SortItems(items);
        foreach (var item in items)
        {
            datastring.Append(item.path);
            datastring.Append(",");
            datastring.Append(item.funcName);
            datastring.Append(",");
            datastring.Append(item.count);
            datastring.Append(",");
            datastring.Append(item.frameMax);
            datastring.Append(",");
            datastring.Append((float)item.timeTotal/(float)item.count);
            datastring.Append(",");
            datastring.Append(item.timeTotal);
            datastring.Append("\n");
        }


        string floder = Path.Combine(Application.dataPath, "../log_client/LoadAssets/");
        if (!Directory.Exists(floder)) Directory.CreateDirectory(floder);

        string savePath = floder + "AssetsLoad.csv";
        if (File.Exists(savePath)) File.Delete(savePath);

        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);
        StreamWriter streamWriter = new StreamWriter(file);//
        streamWriter.Write(datastring.ToString());
        streamWriter.Flush();
        streamWriter.Close();
        Profiler.EndSample();
    }

}
