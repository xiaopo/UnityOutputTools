using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class ParticlesStatisticsOutput : ParticlesOutput
{

    //手动输出

    private StringBuilder datastring = new StringBuilder();
    protected FPSCountTool fpsCtool;

    protected override void Start()
    {
        base.Start();

        this.outInterval = 1;//一秒输出一次
        datastring.Append("时间,FPS,RunParticles,MaxParticles\n");

        fpsCtool = new FPSCountTool();
        fpsCtool.Init();
    }

    protected override string Subfolders()
    {
        return "particlesstatistics/";
    }


    protected override void SortItems(List<OBGItem> list)
    {
        list.Sort((a, b) => -a.runPartic.CompareTo(b.runPartic));
    }

    protected override void RecordLog(string sceneName, string fileName)
    {
        datastring.Append(Mathf.Floor(Time.time));
        datastring.Append(",");
        datastring.Append(fpsCtool.Fps);
        datastring.Append(",");
        datastring.Append(m_runParticle);
        datastring.Append(",");
        datastring.Append(m_maxParticle);
        datastring.Append("\n");

    }


    private void Output()
    {
        Profiler.BeginSample("ParticlesStatisticsOutput");

        this.CollectInformation();

        string savePath = saveFolder + "particlesData.csv";
        WriteFile(savePath, datastring.ToString());

        string filesss = saveFolder + m_runParticle + "——运行粒子统计";
        //截图一张
        ScreenCapture.CaptureScreenshot(filesss + ".png");


        savePath = filesss + ".csv";

        base.RecordLog("瞬间统计_" + m_runParticle, savePath);

        Profiler.EndSample();
    }

    protected override void Update()
    {
        fpsCtool.Update();

        base.Update();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Output();
        }

    }

    protected override void OnGUI()
    {
        base.OnGUI();

        
        if (GUI.Button(new Rect(20, 100, 120, 50), "粒子数据 Tab"))
        {
            Output();
        }
    }
}
