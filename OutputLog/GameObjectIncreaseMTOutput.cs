using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

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

        datastring.Append("时间,FPS,Total GameObject,Total Memory(KB),New GameObject,New Memory(KB),EmptyViewTemplate\n");

        fpsCtool = new FPSCountTool();
        fpsCtool.Init();
    }

    protected override string Subfolders()
    {
        return "gameobjectsstatistics/";
    }


    protected override void RecordLog(string sceneName, string fileName)
    {
        datastring.Append(Mathf.Floor(Time.time));
        datastring.Append(",");
        datastring.Append(fpsCtool.Fps);
        datastring.Append(",");
        datastring.Append(totalItemCount);
        datastring.Append(",");
        datastring.Append(FormatSize(totalRunMemorySize));
        datastring.Append(",");
        datastring.Append(newitemCount);
        datastring.Append(",");
        datastring.Append(FormatSize(newRunMemorySize));
        datastring.Append(",");
        datastring.Append(emptyViewCount);
        datastring.Append("\n");
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

    private int mtcount = 1;
    private void Output()
    {
        Profiler.BeginSample("ParticlesStatisticsOutput");

        string savePath = saveFolder + "gameObjectsData.csv";
        WriteFile(savePath, datastring.ToString());

        string filesss = saveFolder + newitemCount + "——新增物体统计_"+ mtcount;
        //截图一张
        ScreenCapture.CaptureScreenshot(filesss + ".png");

        savePath = filesss + ".csv";
        base.RecordLog("瞬间统计_" + newitemCount, savePath);
        mtcount++;
        Profiler.EndSample();
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        if (GUI.Button(new Rect(20, 100, 120, 50), "新物数据 Tab"))
        {
            Output();
        }
    }
}
