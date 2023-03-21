using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using Util;

public class HotUpdate : MonoBehaviour
{
    byte[] m_ReadPathFileListData;
    byte[] m_ServerFileListData;

    // 下载文件的总数
    int m_DownloadCount;

    GameObject loadingObj;
    LoadingUI loadingUI;
    private void Start()
    {
        GameObject go = Resources.Load<GameObject>("UI/LoadingUI");
        loadingObj = Instantiate(go);
        loadingObj.transform.SetParent(this.transform.Find("UI"),false);
        loadingUI = loadingObj.GetComponent<LoadingUI>();

        if (IsFirstInstall())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }

    private bool IsFirstInstall()
    {
        // 判断只读目录是否存在版本文件
        bool isExistsReadPath = FileUtil.IsExists(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));
        // 判断可读写目录是否有版本文件
        bool isExistsReadWritePath = FileUtil.IsExists(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));
        // 只读目录有版本文件，且读写目录没有版本文件
        return isExistsReadPath && !isExistsReadWritePath;
    }

    /// <summary>
    /// 初次安装释放本地资源文件
    /// </summary>
    private void ReleaseResources()
    {
        m_DownloadCount = 0;
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        // 从本地的只读目录下载filelist
        StartCoroutine(DownLoadFile(info, OnDownLoadReadPathFileComplete));
    }

    private void OnDownLoadReadPathFileComplete(DownFileInfo file)
    {
        // 下载好的fileList，得到其中所有需要释放的资源文件列表
        m_ReadPathFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        // 从本地只读目录下载所有filelist中的资源
        StartCoroutine(DownLoadFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));

        // 修改加载进度条状态
        loadingUI.InitProgress(fileInfos.Count, "正在释放资源");
    }
    private void OnReleaseFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("ReleaseFile:"+ fileInfo.url);
        //下载好的文件写入到本地的RW目录之中
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileInfo.bundleName);
        FileUtil.WriteFile(writeFile, fileInfo.fileData.data);

    }
    private void OnReleaseAllFileComplete()
    {
        //写入全部完成后，向本地的RW目录写入当前新的filelist
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadPathFileListData);
        CheckUpdate();

        // 修改加载进度条状态
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }

    #region Update
    /// <summary>
    /// 检查资源文件更新
    /// </summary>
    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResourcesUrl, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadServerFileListComplete));
    }

    private void OnDownLoadServerFileListComplete(DownFileInfo info)
    {
        m_DownloadCount = 0;

        // info是从服务器拉取的filelist文件
        m_ServerFileListData = info.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(info.fileData.text, AppConst.ResourcesUrl);

        // 这是待下载的文件列表
        List<DownFileInfo> downListFiles = new List<DownFileInfo>();

        for(int i = 0; i< fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].bundleName);

            //如果在本地的RW文件夹下没有找到服务器拉取的filelist里要求的资源
            if (!FileUtil.IsExists(localFile))
            {
                fileInfos[i].url = Path.Combine(AppConst.ResourcesUrl, fileInfos[i].bundleName);
                // 存入需要下载的文件列表
                downListFiles.Add(fileInfos[i]);
            }
            // 校验本地与远程的bundle的MD5
            else if (fileInfos[i].MD5 != FileUtil.GetFileMD5(localFile))
            {
                // MD5码不同，就重新下载
                fileInfos[i].url = Path.Combine(AppConst.ResourcesUrl, fileInfos[i].bundleName);
                downListFiles.Add(fileInfos[i]);
            }
        }

        if(downListFiles.Count > 0)
        {
            StartCoroutine(DownLoadFile(fileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));

            // 修改加载进度条状态
            loadingUI.InitProgress(downListFiles.Count, "正在更新中");
        }
        else
        {
            EnterGame();
        }
    }

    private void OnUpdateFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("UpdateFile:" + fileInfo.url);
        // 从远程下载的文件写入到RW目录中
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileInfo.bundleName);
        FileUtil.WriteFile(writeFile, fileInfo.fileData.data);

        // 修改加载进度条状态
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }
    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();

        // 修改加载进度条状态
        loadingUI.InitProgress(0, "正在载入中");
    }
    #endregion

    private void EnterGame()
    {
        GameManager.Event.Fire((int)GameEvent.GameInit);
        Destroy(loadingObj);
    }
    private void OnComplete(UnityEngine.Object obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }

    #region File Download
    internal class DownFileInfo
    {
        public string url;
        public string bundleName;

        //校验本地与远程MD5
        public string MD5;

        public DownloadHandler fileData;
    }
    /// <summary>
    /// 下载单个文件
    /// </summary>
    /// <param name="url"></param>
    /// <param name="Complete"></param>
    /// <returns></returns>
    IEnumerator DownLoadFile(DownFileInfo info, Action<DownFileInfo> Complete)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("文件下载出错：" + info.url);
            yield break;
            // TODO: 重试逻辑
        }

        //测试逻辑
        //yield return new WaitForSeconds(0.5f);

        info.fileData = webRequest.downloadHandler;
        Complete?.Invoke(info);
        webRequest.Dispose();
    }

    /// <summary>
    /// 下载多个文件
    /// </summary>
    /// <param name="infos">文件信息集</param>
    /// <param name="Complete">单个文件完成</param>
    /// <param name="DownloadAllComplete">全部完成</param>
    /// <returns></returns>
    IEnumerator DownLoadFile(List<DownFileInfo> infos, Action<DownFileInfo> Complete, Action DownloadAllComplete)
    {
        foreach (var info in infos)
        {
            yield return DownLoadFile(info, Complete);
        }
        DownloadAllComplete?.Invoke();
    }

    /// <summary>
    /// 将filelist文件中的每一个bundle转化为可以获取到它的路径
    /// </summary>
    /// <param name="fileData"> fileList文件内容 </param>
    /// /// <param name="path"> 文件路径 </param>
    /// <returns>包含url和bundlename的Info的列表</returns>
    private List<DownFileInfo> GetFileList(string fileData, string path)
    {
        //txt文件可能存在的错误，无意义存在的\r
        string content = fileData.Trim().Replace("\r", "");

        string[] files = content.Split("\n");
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split("|");
            DownFileInfo fileinfo = new DownFileInfo();
            //fileList的第二格是bundleName
            fileinfo.bundleName = info[1];

            //fileList的最后一位是MD5校验码
            fileinfo.MD5 = info[info.Length - 1];

            //文件路径+bundleName，成为可以获取文件的地址名
            fileinfo.url = Path.Combine(path, info[1]);
            downFileInfos.Add(fileinfo);
        }
        return downFileInfos;
    }
    #endregion
}
