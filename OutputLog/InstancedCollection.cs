

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class InstancedCollection 
{
    public class ObjItem
    {
        public string path;
        public int instancedCount;
        public float intancedTimeTotal;//ms

        public float beginTime;
        
        public float CostTime(float time)
        {
            return time - beginTime;
        }
    }

    static Dictionary<string, ObjItem> total_items = new Dictionary<string, ObjItem>();

    public static void BeginSampler(string path)
    {
        

        if(!total_items.TryGetValue(path, out ObjItem item))
        {
            item = new ObjItem();
            item.path = path;
            total_items.Add(path, item);
        }


        item.beginTime = Time.realtimeSinceStartup * 1000;//ms

    }

    public static void EndSampler(string path)
    {
        if (!total_items.TryGetValue(path, out ObjItem item)) return;

        item.instancedCount++;

        item.intancedTimeTotal += item.CostTime(Time.realtimeSinceStartup * 1000);
    }

    protected static void SortItems(List<ObjItem> list)
    {
        list.Sort((a, b) => -a.instancedCount.CompareTo(b.instancedCount));//
    }

    protected static StringBuilder datastring;
    public static void SaveData()
    {
        datastring = new StringBuilder();
        datastring.Append("对象,次数,平均耗时(ms),总耗时(ms)\n");

        List<ObjItem> items = new List<ObjItem>();

        foreach (var map in total_items)
        {
            items.Add(map.Value);
        }

        SortItems(items);

        foreach (var item in items)
        {
            datastring.Append(item.path);
            datastring.Append(",");
            datastring.Append(item.instancedCount);
            datastring.Append(",");
            datastring.Append(item.intancedTimeTotal / item.instancedCount);
            datastring.Append(",");
            datastring.Append(item.intancedTimeTotal);
            datastring.Append("\n");
        }

        Profiler.BeginSample("ParticlesStatisticsOutput");


        string floder = Path.Combine(Application.dataPath, "../log_client/Instances/");
        if (!Directory.Exists(floder)) Directory.CreateDirectory(floder);

        string savePath = Path.Combine(Application.dataPath, "../log_client/Instances/") + "AssetsIsntances.csv";
    
        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);
        StreamWriter streamWriter = new StreamWriter(file);//
        streamWriter.Write(datastring.ToString());
        streamWriter.Flush();
        streamWriter.Close();
        Profiler.EndSample();
    }
}
