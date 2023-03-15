using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Util;

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

        // 文件信息列表
        List<string> bundleInfos = new List<string> ();

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

            // 获取assetbundle的依赖，制作成bundle版本号
            List<string> dependencesInfo = GetDependences(assetName);
            // 结构： AssetName|BundleName|dependences|dependences
            string bundleInfo = assetName + "|" + bundleName + ".ab";
            if (dependencesInfo.Count > 0)
            {
                bundleInfo = bundleInfo + "|" + string.Join("|", dependencesInfo);
            }
            bundleInfos.Add(bundleInfo);
        }
        // 递归删除已有,重新创建
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);

        //输出bundles，输出bundles dependencies文件
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, targetPlatform);
        File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName, bundleInfos);
        AssetDatabase.Refresh();
    }


    static List<string> GetDependences(string curFile)
    {
        List<string> dependences = new List<string>();
        string[] files = AssetDatabase.GetDependencies(curFile);
        // 去除c#脚本， 去除自身
        dependences = files.Where( file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToList();
        return dependences;
    }
}
