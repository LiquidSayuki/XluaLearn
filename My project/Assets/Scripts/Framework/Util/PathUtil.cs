using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    // root directory
    public static readonly string AssetsPath = Application.dataPath;

    // Resources that need to be pack to bundles 
    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";

    // Bundle output path
    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    //Bundle resources path
    public static string BundleResourcesPath
    {
        get { return Application.streamingAssetsPath; }
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
    ///  ��·���� / \ ��һ��
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
