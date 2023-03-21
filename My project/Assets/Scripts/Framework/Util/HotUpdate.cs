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

    // �����ļ�������
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
        // �ж�ֻ��Ŀ¼�Ƿ���ڰ汾�ļ�
        bool isExistsReadPath = FileUtil.IsExists(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));
        // �жϿɶ�дĿ¼�Ƿ��а汾�ļ�
        bool isExistsReadWritePath = FileUtil.IsExists(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));
        // ֻ��Ŀ¼�а汾�ļ����Ҷ�дĿ¼û�а汾�ļ�
        return isExistsReadPath && !isExistsReadWritePath;
    }

    /// <summary>
    /// ���ΰ�װ�ͷű�����Դ�ļ�
    /// </summary>
    private void ReleaseResources()
    {
        m_DownloadCount = 0;
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        // �ӱ��ص�ֻ��Ŀ¼����filelist
        StartCoroutine(DownLoadFile(info, OnDownLoadReadPathFileComplete));
    }

    private void OnDownLoadReadPathFileComplete(DownFileInfo file)
    {
        // ���غõ�fileList���õ�����������Ҫ�ͷŵ���Դ�ļ��б�
        m_ReadPathFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        // �ӱ���ֻ��Ŀ¼��������filelist�е���Դ
        StartCoroutine(DownLoadFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));

        // �޸ļ��ؽ�����״̬
        loadingUI.InitProgress(fileInfos.Count, "�����ͷ���Դ");
    }
    private void OnReleaseFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("ReleaseFile:"+ fileInfo.url);
        //���غõ��ļ�д�뵽���ص�RWĿ¼֮��
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileInfo.bundleName);
        FileUtil.WriteFile(writeFile, fileInfo.fileData.data);

    }
    private void OnReleaseAllFileComplete()
    {
        //д��ȫ����ɺ��򱾵ص�RWĿ¼д�뵱ǰ�µ�filelist
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadPathFileListData);
        CheckUpdate();

        // �޸ļ��ؽ�����״̬
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }

    #region Update
    /// <summary>
    /// �����Դ�ļ�����
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

        // info�Ǵӷ�������ȡ��filelist�ļ�
        m_ServerFileListData = info.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(info.fileData.text, AppConst.ResourcesUrl);

        // ���Ǵ����ص��ļ��б�
        List<DownFileInfo> downListFiles = new List<DownFileInfo>();

        for(int i = 0; i< fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].bundleName);

            //����ڱ��ص�RW�ļ�����û���ҵ���������ȡ��filelist��Ҫ�����Դ
            if (!FileUtil.IsExists(localFile))
            {
                fileInfos[i].url = Path.Combine(AppConst.ResourcesUrl, fileInfos[i].bundleName);
                // ������Ҫ���ص��ļ��б�
                downListFiles.Add(fileInfos[i]);
            }
            // У�鱾����Զ�̵�bundle��MD5
            else if (fileInfos[i].MD5 != FileUtil.GetFileMD5(localFile))
            {
                // MD5�벻ͬ������������
                fileInfos[i].url = Path.Combine(AppConst.ResourcesUrl, fileInfos[i].bundleName);
                downListFiles.Add(fileInfos[i]);
            }
        }

        if(downListFiles.Count > 0)
        {
            StartCoroutine(DownLoadFile(fileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));

            // �޸ļ��ؽ�����״̬
            loadingUI.InitProgress(downListFiles.Count, "���ڸ�����");
        }
        else
        {
            EnterGame();
        }
    }

    private void OnUpdateFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("UpdateFile:" + fileInfo.url);
        // ��Զ�����ص��ļ�д�뵽RWĿ¼��
        string writeFile = Path.Combine(PathUtil.ReadWritePath, fileInfo.bundleName);
        FileUtil.WriteFile(writeFile, fileInfo.fileData.data);

        // �޸ļ��ؽ�����״̬
        m_DownloadCount++;
        loadingUI.UpdateProgress(m_DownloadCount);
    }
    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();

        // �޸ļ��ؽ�����״̬
        loadingUI.InitProgress(0, "����������");
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

        //У�鱾����Զ��MD5
        public string MD5;

        public DownloadHandler fileData;
    }
    /// <summary>
    /// ���ص����ļ�
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
            Debug.LogError("�ļ����س���" + info.url);
            yield break;
            // TODO: �����߼�
        }

        //�����߼�
        //yield return new WaitForSeconds(0.5f);

        info.fileData = webRequest.downloadHandler;
        Complete?.Invoke(info);
        webRequest.Dispose();
    }

    /// <summary>
    /// ���ض���ļ�
    /// </summary>
    /// <param name="infos">�ļ���Ϣ��</param>
    /// <param name="Complete">�����ļ����</param>
    /// <param name="DownloadAllComplete">ȫ�����</param>
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
    /// ��filelist�ļ��е�ÿһ��bundleת��Ϊ���Ի�ȡ������·��
    /// </summary>
    /// <param name="fileData"> fileList�ļ����� </param>
    /// /// <param name="path"> �ļ�·�� </param>
    /// <returns>����url��bundlename��Info���б�</returns>
    private List<DownFileInfo> GetFileList(string fileData, string path)
    {
        //txt�ļ����ܴ��ڵĴ�����������ڵ�\r
        string content = fileData.Trim().Replace("\r", "");

        string[] files = content.Split("\n");
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);
        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split("|");
            DownFileInfo fileinfo = new DownFileInfo();
            //fileList�ĵڶ�����bundleName
            fileinfo.bundleName = info[1];

            //fileList�����һλ��MD5У����
            fileinfo.MD5 = info[info.Length - 1];

            //�ļ�·��+bundleName����Ϊ���Ի�ȡ�ļ��ĵ�ַ��
            fileinfo.url = Path.Combine(path, info[1]);
            downFileInfos.Add(fileinfo);
        }
        return downFileInfos;
    }
    #endregion
}
