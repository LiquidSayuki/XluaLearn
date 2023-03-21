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
    /// д���ļ�
    /// </summary>
    /// <param name="path"></param>
    /// <param name="data"></param>
    public static void WriteFile(string path, byte[] data)
    {
        path = PathUtil.GetStandardPath(path);
        // ����ʼ�����һ��б��֮ǰ��·������һ���ļ����ļ��е�·��
        string dir = path.Substring(0, path.LastIndexOf("/"));
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        FileInfo info = new FileInfo(path);
        // ����û�б���ĸ���ʽд�룬������Ҫ��ɾ����д��
        if (info.Exists)
        {
            info.Delete();
        }
        try
        {
            // Using���÷������뿪��������ʱ��ϣ���ܹ�ֱ�Ӵ������Щ����
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
    /// ��ȡ�ļ���MD5��
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
