using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
public class OBGItem
{
    public string name;
    public string fullPath;
    public string countKey;
    public int count;//新增obj数量
    public int runPartic;
    public int maxPartic;
    public float runtimememorySize;//运行内存大小
    public bool isActive;
}

public class OutputLogviewBase : MonoBehaviour
{
    public int outInterval = 30;

    protected float outIime = 0;
    protected Dictionary<int, bool> haditems = new Dictionary<int, bool>();
    protected List<OBGItem> teamList = new List<OBGItem>();
    protected Dictionary<string, OBGItem> new_items = new Dictionary<string, OBGItem>();

    protected int m_maxParticle = 0;
    protected int m_runParticle = 0;
    protected MethodInfo m_CalculateEffectUIDataMethod = null;

    protected string saveFolder;
    protected virtual void Start()
    {

        saveFolder = Path.Combine(Application.dataPath, "../log_client/"+ Subfolders());

        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        m_CalculateEffectUIDataMethod = typeof(ParticleSystem).GetMethod("CalculateEffectUIData", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    protected virtual string Subfolders()
    {
        return "";
    }

    // Update is called once per frame
    protected virtual void Update()
    {


       
    }

    public void WriteFile(string savePath, string datastr)
    {
        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);
        StreamWriter streamWriter = new StreamWriter(file);//
        streamWriter.Write(datastr);
        streamWriter.Flush();
        streamWriter.Close();
    }

}
