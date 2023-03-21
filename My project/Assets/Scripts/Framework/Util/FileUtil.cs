using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class FileUtil
{
    public static bool IsExists(string path)
    {
        FileInfo file = new FileInfo(path);
        return file.Exists;
    }

    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    public static void WriteFile(string path, byte[] data)
    {
        path = PathUtil.GetStandardPath(path);
        // 从起始到最后一个斜杠之前的路径，是一个文件的文件夹的路径
        string dir = path.Substring(0, path.LastIndexOf("/"));
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        FileInfo info = new FileInfo(path);
        // 由于没有本身的覆盖式写入，我们需要先删除后写入
        if (info.Exists)
        {
            info.Delete();
        }
        try
        {
            // Using的用法，在离开这个代码段时，希望能够直接处理掉这些对象
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
            }
        }
        catch(IOException e)
        {
            Debug.LogError(e.Message);
        }
    }

    /// <summary>
    /// 获取文件的MD5码
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileMD5(string path)
    {
        string str_md5 = string.Empty;
        try
        {
            using (FileStream fs = File.OpenRead(path))
            {
                MD5 md5 = MD5.Create();
                byte[] fileMd5Bytes = md5.ComputeHash(fs);
                str_md5 = BitConverter.ToString(fileMd5Bytes).Replace("-", "").ToLower();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.StackTrace);
        }

        return str_md5;
    }
}
