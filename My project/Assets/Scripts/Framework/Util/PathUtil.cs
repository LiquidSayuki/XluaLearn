using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

/// <summary>
/// ��Ŀ����Ҫ���õ�һϵ��·��
/// </summary>
public class PathUtil
{
    // root directory
    public static readonly string AssetsPath = Application.dataPath;

    // Resources that need to be pack to bundles 
    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";

    // Bundle output path
    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    //ReadOnlyPath
    public static readonly string ReadPath = Application.streamingAssetsPath;

    //Read and Write Path
    public static readonly string ReadWritePath = Application.persistentDataPath;

    public static readonly string LuaPath = "Assets/BuildResources/LuaScripts";

    //Bundle resources path
    public static string BundleResourcesPath
    {
        get { 
            if(AppConst.GameMode == GameMode.UpdateMode)
            {
                return ReadWritePath;
            }
            return ReadPath; 
        }
    }


    /// <summary>
    /// Ϊ��ȡ��Դ����������һ��·��
    /// </summary>
    /// <param name="name">��Դ��</param>
    /// <returns></returns>
    public static string GetLuaPath(string name)
    {
        return string.Format("Assets/BuildResources/LuaScripts/{0}.bytes", name);
    }
    public static string GetUIPath(string name)
    {
        return string.Format("Assets/BuildResources/UI/Prefabs/{0}.prefab", name);
    }
    public static string GetMusicPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Music/{0}", name);
    }
    public static string GetSoundPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Sound/{0}", name);
    }
    public static string GetEffectPath(string name)
    {
        return string.Format("Assets/BuildResources/Effect/Prefabs/{0}.prefab", name);
    }
    public static string GetSpritePath(string name)
    {
        return string.Format("Assets/BuildResources/Sprites/{0}", name);
    }
    public static string GetModelPath(string name)
    {
        return string.Format("Assets/BuildResources/Model/Prefabs/{0}.prefab", name);
    }
    public static string GetScenePath(string name)
    {
        return string.Format("Assets/BuildResources/Scene/{0}.unity", name);
    }

    /// <summary>
    ///  ��ȡunity���·��
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        return path.Substring(path.IndexOf("Assets"));
    }

    /// <summary>
    ///  ��·���� "/" ��"\" ��һ��
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }

        return path.Trim().Replace("\\", "/");
    }
}
