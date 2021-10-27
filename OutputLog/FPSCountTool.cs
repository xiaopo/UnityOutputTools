using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCountTool
{
    public float m_UpdateInterval = 0.5F;
    private double m_LastInterval = 0;
    private int m_Frames = 0;
    private int m_Fps = 0;

    public void Init()
    {
        m_LastInterval = Time.realtimeSinceStartup;
        m_Frames = 0;
    }


    public void Update()
    {
        ++m_Frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > m_LastInterval + m_UpdateInterval)
        {
            m_Fps = (int)(m_Frames / (timeNow - m_LastInterval));
            m_Frames = 0;
            m_LastInterval = timeNow;
        }
    }
    public int Fps
    {
        get
        {
            return m_Fps;
        }
    }
}
