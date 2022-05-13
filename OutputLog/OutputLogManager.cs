using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
namespace OutputLog
{
    public class OutputLogManager
    {
        static string configName = "../../outMonobehaviour.txt";
        static string Trim(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            str = str.Trim();
            if (string.IsNullOrEmpty(str)) return "";
            if (str.Contains("//"))
            {
                string checks = str.Substring(0, str.IndexOf("//"));
                if (string.IsNullOrEmpty(checks)) return "";

                str = checks;
            }

            return str;
        }
        static Dictionary<string, List<string>> ReadConfig()
        {
            Dictionary<string, List<string>>  out_items = new Dictionary<string, List<string>>();
            string filePath = OutpulMonoBehaviour.basePath + configName;
            if (File.Exists(filePath))
            {
                string config = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(config))
                {
                    string[] lines = config.Split(new string[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);
                    List<string> class_types = null;
                    for (int i = 0;i< lines.Length;i++)
                    {
                        string line = Trim(lines[i]);
   
                        if (string.IsNullOrEmpty(line)) continue;
                        if (line.Contains(".dll"))
                        {
                            if (!out_items.TryGetValue(line, out class_types))
                            {
                                class_types = new List<string>();
                                out_items.Add(line, class_types);
                            }
                        }
                        else if (line == "-----------##-----------") continue;
                        else
                        {

                            class_types.Add(line);

                        }

                    }
                }

                Debug.Log("read setting config for MonoBehaviour!!");
            }


            return out_items;
        }


        public static void OutPutCSV()
        {
            Dictionary<string, List<string>> items = ReadConfig();

            foreach(var map in items)
            {
                if (map.Value.Count == 0) continue;
                string dllName = map.Key.Substring(0,map.Key.IndexOf(".dll"));
                Assembly assemblyc = Assembly.Load(dllName) ;
                if(assemblyc != null)
                {
                    foreach(var className in map.Value)
                    {
                        Type type = assemblyc.GetType(className);
                        OutpulMonoBehaviour.OutputTypeCSV(type);
                    }
                }
            }
        }
    }
}

