using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace OutputLog
{ 
    public class GameObjectIncreaseMTOutput : GameObjectIncreaseOutput
    {
        //自动统计
        //手动输出
        protected StringBuilder datastring = new StringBuilder();
        protected FPSCountTool fpsCtool;
        protected override void Start()
        {
            base.Start();

            this.outInterval = 1;

            datastring.Append("时间,FPS,Total GameObject\n");

            //fpsCtool = new FPSCountTool();
            //fpsCtool.Init();
        }

        protected override string Subfolders()
        {
            return "gameobjectsstatistics/";
        }

        protected override void Update()
        {
            //fpsCtool.Update();

            //if (outIime == 0 || Time.time - outIime > outInterval)
            //{
            //    outIime = Time.time;

            //    Profiler.BeginSample("GameObjectIncreaseMTOutput.Caculate");
            //    UnityEngine.Object[] totol = Resources.FindObjectsOfTypeAll(typeof(GameObject));

            //    totalItemCount = totol.Length;

            //    datastring.Append(Mathf.Floor(Time.time));
            //    datastring.Append(",");
            //    datastring.Append(fpsCtool.Fps);
            //    datastring.Append(",");
            //    datastring.Append(totalItemCount);
            //    datastring.Append("\n");

            //    Profiler.EndSample();
            //}

        }

        private int mtcount = 1;
        private void Output()
        {
            Profiler.BeginSample("GameObjectsOutput");

            //string savePath = saveFolder + "gameObjectsData.csv";
            //WriteFile(savePath, datastring.ToString());

            string filesss = saveFolder + newitemCount + "——新增物体统计_"+ mtcount;
            //截图一张
            //ScreenCapture.CaptureScreenshot(filesss + ".png");

            string savePath = filesss + ".csv";
            base.RecordLog(savePath);
            mtcount++;
            Profiler.EndSample();
        }
    

        protected override void OnGUI()
        {
            if (GUI.Button(new Rect(20, 100, 200, 100), "新物数据 Tab"))
            {
                OnCaculateNew();
                Output();

                OutputLogManager.OutPutCSV();
            }
        }
    }
}