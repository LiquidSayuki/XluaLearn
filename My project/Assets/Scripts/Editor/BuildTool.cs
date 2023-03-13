using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildTool : Editor
{
    [MenuItem("Tools/Build WindowsBundle")]
    static void BundleBuildWindows()
    {
        Build(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Tools/Build AndroidBundle")]
    static void BundleBuildAndroid()
    {
        Build(BuildTarget.Android);
    }
    static void Build(BuildTarget targetPlatform)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild> ();
        // 需打包资源路径
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        for (int i=0; i<files.Length; i++)
        {
            // 忽略meta
            if (files[i].EndsWith(".meta"))
                continue;

            string fileName = PathUtil.GetStandardPath(files[i]);
            Debug.Log(fileName);

            AssetBundleBuild assetBundle = new AssetBundleBuild();
            // 资源做成单资源bundle
            string assetName = PathUtil.GetUnityPath(fileName); 
            assetBundle.assetNames = new string[] { assetName };

            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            // bundleName = bundleName.Remove(0,1);
            
            assetBundle.assetBundleName = bundleName + ".ab";

            assetBundleBuilds.Add(assetBundle);
        }
        // 递归删除已有,重新创建
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, targetPlatform);
    }
}
