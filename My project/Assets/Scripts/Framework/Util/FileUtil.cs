using System.Collections;
using System.Collections.Generic;
using System.IO;
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
}
