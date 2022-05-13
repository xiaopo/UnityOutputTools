
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
namespace OutputLog 
{ 
    public class GameObjectIncreaseOutput : OutputLogviewBase
    {
        protected int newitemCount = 0;
        protected int totalItemCount = 0;
        protected float totalRunMemorySize = 0;
        protected float newRunMemorySize = 0;
        //自动输出
        protected override string Subfolders()
        {
            return "gameobjects/";
        }

        protected List<string> specialKey = new List<string>(){
             "PhysicsRoot/PhysicsComponent"
        };

        protected virtual string GetCountKey(string okey)
        {
            foreach(var item in specialKey)
            {
                if (okey.Contains(item)) return item;
            }

            return okey;
        }
        protected override void Update()
        {
            if (outIime == 0 || Time.time - outIime > outInterval)
            {
                outIime = Time.time;

                OnCaculateNew();
                string savePath = saveFolder + "gameObjCount_" + Mathf.Floor(Time.time) + ".csv";
                RecordLog(savePath);
            }

        }

        protected virtual void OnCaculateNew()
        {
            UnityEngine.Object[] totol = Resources.FindObjectsOfTypeAll(typeof(GameObject));

            new_items.Clear();
            newitemCount = 0;
            totalItemCount = 0;
            m_maxParticle = 0;
            m_runParticle = 0;
            totalRunMemorySize = 0;
            newRunMemorySize = 0;

            Profiler.BeginSample("GameObjectIncreaseOutput.Caculate");
            foreach (var item in totol)
            {
                if (item.hideFlags == HideFlags.NotEditable || item.hideFlags == HideFlags.HideAndDontSave) continue;

                GameObject obj = item as GameObject;
                if (obj == null) continue;

                string objName = obj.name;
                if (string.IsNullOrEmpty(objName)) continue;

                float runtimeMemory = Profiler.GetRuntimeMemorySizeLong(obj);
                OBGItem objItem = null;
                if (!haditems.TryGetValue(obj.GetInstanceID(), out bool hadb))
                {
                    haditems.Add(obj.GetInstanceID(), true);

                    string fullName = GetFullScenePath(obj.transform);
                    string countKey = GetCountKey(fullName);

                    if (new_items.TryGetValue(countKey, out objItem))
                    {
                        //存在了
                        objItem.count++;

                    }
                    else
                    {
                        objItem = new OBGItem();
                        objItem.name = objName;
                        objItem.countKey = countKey;
                        objItem.count = 1;

                        objItem.fullPath = fullName;

                        new_items.Add(countKey, objItem);
                    }

                    objItem.runtimememorySize += runtimeMemory;

                    newRunMemorySize += runtimeMemory;
                    newitemCount++;
                }

                if (objItem != null)
                    objItem.isActive = obj.activeInHierarchy;


                totalRunMemorySize += runtimeMemory;
            }

            totalItemCount = totol.Length;

            //unityTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong() / 1024f / 1024f;
            //unityTotalReservedMemory = Profiler.GetTotalReservedMemoryLong() / 1024f / 1024f;
            //unityUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong() / 1024f / 1024f;

            //monoUseMemory = Profiler.GetMonoUsedSizeLong() / 1024f / 1024f;
            //monoHeapMemory = Profiler.GetMonoHeapSizeLong() / 1024f / 1024f;

            Profiler.EndSample();
        }

        private StringBuilder recordstring = new StringBuilder();
        protected virtual void RecordLog(string fileName)
        {
            if (new_items.Count == 0) return;

            recordstring.Clear();
            recordstring.Append("游戏运行时间,");
            recordstring.Append(Time.time);

            recordstring.Append("\nTotal GameObject in Scene,");
            recordstring.Append(totalItemCount);
            recordstring.Append(",");
            recordstring.Append(FormatSize(totalRunMemorySize));
            recordstring.Append("KB\n");
            recordstring.Append("本次新增GameObject,");
            recordstring.Append(newitemCount);
            recordstring.Append(",");
            recordstring.Append(FormatSize(newRunMemorySize));
            recordstring.Append("KB\n");

            recordstring.Append("场景内名字,新增同名个数,消耗内存");
            recordstring.Append("\n");


            teamList.Clear();
            foreach (var newItem in new_items)
            {
                teamList.Add(newItem.Value);

            }
            new_items.Clear();

            SortItems(teamList);

            for (int i = 0; i < teamList.Count; i++)
            {
                var newItem = teamList[i];
                recordstring.Append(newItem.countKey);
                //datastring.Append(",");
                //datastring.Append(newItem.isActive ? "Yes":"No");
                recordstring.Append(",");
                recordstring.Append(newItem.count);
                recordstring.Append(",");
                recordstring.Append(newItem.runtimememorySize);
                recordstring.Append("\n");
            }

            teamList.Clear();

            WriteFile(fileName, recordstring.ToString());

            recordstring.Clear();
        }
        protected virtual void SortItems(List<OBGItem> list)
        {
            list.Sort((a, b) => -a.count.CompareTo(b.count));//
        }
        protected string FormatSize(float size)
        {
            return (size / 1024).ToString("f2");
        }

        protected virtual void OnGUI()
        {
            GUILayout.Label(string.Format("<size=20>GameObjects in Scene : {0}    Memory :{1}KB</size>", totalItemCount, FormatSize(totalRunMemorySize)));
            GUILayout.Label(string.Format("<size=20>New GameObjects : {0}    Memory :{1}KB</size>", newitemCount, FormatSize(newRunMemorySize)));
        }
    }
}