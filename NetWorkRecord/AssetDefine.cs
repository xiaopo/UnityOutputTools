using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

 public class AssetDefine
{
#if !_SERVER
    public static string LogFolder = "/log_client_network/";
#else
    public static string LogFolder = "/log_server_network/";
#endif
    public readonly static string dataPath = Application.dataPath;
    public readonly static string persistentDataPath = Application.persistentDataPath;
    public readonly static string streamingAssetsPath = Application.streamingAssetsPath;
    public readonly static string temporaryCachePath = Application.temporaryCachePath;

    //扩展卡资源
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public static string ExternalSDCardsPath = Path.Combine(AssetDefine.dataPath, "../" + AssetDefine.LogFolder);
#elif UNITY_IOS
        public static string ExternalSDCardsPath = AssetDefine.temporaryCachePath + AssetDefine.LogFolder;
#else
    public static string ExternalSDCardsPath = AssetDefine.persistentDataPath + AssetDefine.LogFolder;
#endif


    //首包内资源
#if UNITY_EDITOR
        public static string BuildinAssetPath = AssetDefine.streamingAssetsPath + AssetDefine.LogFolder;
#elif UNITY_ANDROID
        public static string BuildinAssetPath = AssetDefine.streamingAssetsPath + AssetDefine.LogFolder;
#else
    public static string BuildinAssetPath = AssetDefine.streamingAssetsPath + AssetDefine.LogFolder;
#endif

#if UNITY_EDITOR
        public static string DataDataPath = Path.Combine(AssetDefine.dataPath, "../");
#elif UNITY_STANDALONE_WIN
         public static string DataDataPath = AssetDefine.dataPath;
#else
         public static string DataDataPath = "/data/data/" + Application.identifier + "/files/";
#endif

        public static string DllPath = DataDataPath + "Assembly-CSharp.dll";
    }

