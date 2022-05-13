using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace OutputLog
{ 
    public class ParticlesOutput : OutputLogviewBase
    {

        //输出运行时，场景内全部的物体粒子情况
        //自动输出

        protected Dictionary<string, OBGItem> m_totalItems = new Dictionary<string, OBGItem>();

        protected override string Subfolders()
        {
            return "particlesoutput/";
        }

        protected override void Update()
        {

            if (outIime == 0 || Time.time - outIime > outInterval)
            {
                outIime = Time.time;

                CollectInformation();
            }

        }

        protected virtual void CollectInformation()
        {
            string sceneName = String.Empty;
            m_totalItems.Clear();
            m_maxParticle = 0;
            m_runParticle = 0;

            UnityEngine.Object[] totol = Resources.FindObjectsOfTypeAll(typeof(ParticleSystem));
            StringBuilder fullPath = new StringBuilder();
            foreach (var item in totol)
            {
                if (item.hideFlags == HideFlags.NotEditable || item.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                ParticleSystem particlSys = item as ParticleSystem;
                if (particlSys == null) continue;
                GameObject obj = particlSys.gameObject;
                if (!obj.scene.isLoaded) continue;

                sceneName = obj.scene.name;

                fullPath.Clear();
                fullPath.Append(obj.name);
                Transform pparent = obj.transform.parent;
                while (pparent != null)
                {
                    fullPath.Insert(0, "/");
                    fullPath.Insert(0, pparent.name);
                    pparent = pparent.parent;
                }

                string fullName = fullPath.ToString();
                OBGItem mbitem = null;
                if (!m_totalItems.TryGetValue(fullName, out mbitem))
                {
                    mbitem = new OBGItem();
                    mbitem.fullPath = fullName;
                    mbitem.count = 1;
                    m_totalItems.Add(fullName, mbitem);
                }
                else
                {
                    mbitem.count++;
                }

                mbitem.isActive = obj.activeInHierarchy;
                int runparticN = 0;
                int maxparticN = 0;
                int count = 0;
                object[] invokeArgs = new object[] { count, 0.0f, Mathf.Infinity };
                m_CalculateEffectUIDataMethod.Invoke(particlSys, invokeArgs);
                //运行粒子
                runparticN = (int)invokeArgs[0];
                m_runParticle += runparticN;
                //粒子最大数
                maxparticN = particlSys.main.maxParticles;
                m_maxParticle += maxparticN;

                mbitem.runPartic += runparticN;
                mbitem.maxPartic += maxparticN;

            }

            string savePath = saveFolder + "particlesCount_" + Mathf.Floor(Time.time) + ".csv";
            RecordLog(sceneName, savePath);
        }


        protected virtual void SortItems(List<OBGItem> list)
        {
            list.Sort((a, b) => -a.maxPartic.CompareTo(b.maxPartic));
        }

        protected virtual void RecordLog(string sceneName,string fileName)
        {
            StringBuilder datastring = new StringBuilder();
            datastring.Append("游戏运行时间,");
            datastring.Append(Time.time);
            datastring.Append("\n场景名字,");
            datastring.Append(sceneName);

            datastring.Append("\nTotal GameObject in Scene,");
            datastring.Append(m_totalItems.Count);

            datastring.Append("\n场景运行粒子总数,");
            datastring.Append(m_runParticle);
            datastring.Append("\n场景maxParticles总数,");
            datastring.Append(m_maxParticle);
            datastring.Append("\n");

            datastring.Append("场景内名字,同名个数,激活,runParticles,maxParticles");
            datastring.Append("\n");


            teamList.Clear();
            foreach (var item in m_totalItems) teamList.Add(item.Value);

            m_totalItems.Clear();

            SortItems(teamList);
            for (int i = 0; i < teamList.Count; i++)
            {
                var newItem = teamList[i];
                datastring.Append(newItem.fullPath);
                datastring.Append(",");
                datastring.Append(newItem.count);
                datastring.Append(",");
                datastring.Append(newItem.isActive ? "Yes":"No");
                datastring.Append(",");
                datastring.Append(newItem.runPartic);
                datastring.Append(",");
                datastring.Append(newItem.maxPartic);
                datastring.Append("\n");
            }

            teamList.Clear();

     
            WriteFile(fileName, datastring.ToString());
        }

        protected virtual void OnGUI()
        {
            GUILayout.Label(string.Format("<size=20>total run Particle : {0}</size>", m_runParticle));
            GUILayout.Label(string.Format("<size=20>total max Particle : {0}</size>", m_maxParticle));
        }

    }
}