using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
namespace OutputLog
{ 
    public class TailSystemOutput : OutputLogviewBase
    {

        //输出运行时，场景内全部拖尾情况
        //自动输出
        protected int m_tailRender = 0;
        protected FPSCountTool fpsCtool;
        protected StringBuilder datastring = new StringBuilder();
        protected override void Start()
        {
            base.Start();
            fpsCtool = new FPSCountTool();
            fpsCtool.Init();

            datastring.Append("时间,FPS,TailRenderer\n");

        }
        protected override string Subfolders()
        {
            return "tailRendererOutput/";
        }

        protected override void Update()
        {

            fpsCtool.Update();

            if (outIime == 0 || Time.time - outIime > outInterval)
            {
                outIime = Time.time;

                CollectInformation();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Output();
            }

        }

        protected virtual void CollectInformation()
        {
            string sceneName = String.Empty;
            m_maxParticle = 0;
            m_runParticle = 0;
            m_tailRender = 0;
            UnityEngine.Object[] totol = Resources.FindObjectsOfTypeAll(typeof(TrailRenderer));
            foreach (var item in totol)
            {
                if (item.hideFlags == HideFlags.NotEditable || item.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                TrailRenderer trail = item as TrailRenderer;
                if (trail == null || !trail.enabled) continue;

                if (!trail.gameObject.activeInHierarchy) continue;

                m_tailRender++;

            }

            RecordLog("", "");
        }


        private int mtcount = 1;
        private void Output()
        {
            Profiler.BeginSample("ParticlesStatisticsOutput");

            string savePath = saveFolder + "tailRendererData.csv";
            WriteFile(savePath, datastring.ToString());

            string filesss = saveFolder + "TailRenderer_" + mtcount;
            //截图一张
            ScreenCapture.CaptureScreenshot(filesss + ".png");

            Profiler.EndSample();
        }

        protected virtual void RecordLog(string sceneName,string fileName)
        {
            datastring.Append(Mathf.Floor(Time.time));
            datastring.Append(",");
            datastring.Append(fpsCtool.Fps);
            datastring.Append(",");
            datastring.Append(m_tailRender);
            datastring.Append("\n");

        }

        protected virtual void OnGUI()
        {
        
            GUILayout.Label(string.Format("<size=20>FPS : {0}</size>", fpsCtool.Fps));
            GUILayout.Label(string.Format("<size=20>total run Tail : {0}</size>", m_tailRender));

            if (GUI.Button(new Rect(20, 100, 120, 50), "新物数据 Tab"))
            {
                Output();
            }
        }

    }
}