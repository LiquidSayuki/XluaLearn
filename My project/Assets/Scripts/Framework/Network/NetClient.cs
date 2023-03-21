using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class NetClient 
{
    private TcpClient m_client;
    private NetworkStream m_TcpStream;
    private const int BufferSize = 1024 * 64;
    private byte[] m_Buffer = new byte[BufferSize];
    private MemoryStream m_MemStream;
    private BinaryReader m_BinaryReader;

    public NetClient()
    {
        m_MemStream= new MemoryStream();
        m_BinaryReader= new BinaryReader(m_MemStream);
    }

    public void OnConnectServer(string host, int port)
    {
        try
        {
            IPAddress[] address = Dns.GetHostAddresses(host);
            if(address.Length == 0)
            {
                Debug.LogError("host invalid");
                return;
            }
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                m_client= new TcpClient(AddressFamily.InterNetworkV6);
            }
            else
            {
                //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_client = new TcpClient(AddressFamily.InterNetwork);
            }
            m_client.SendTimeout = 1000;
            m_client.ReceiveTimeout = 1000;
            m_client.NoDelay= true;

            // socket.BeginConnect(host, port, OnConnect, null);
            m_client.BeginConnect(host, port, OnConnect, null);

            //Socket����������
            //Tcp Client�Ǵ�������

        }catch(Exception e)
        {
            Debug.LogError("Connect fail" + e.ToString());
        }
    }

    private void OnConnect(IAsyncResult asyncResult)
    {
        if(m_client == null || !m_client.Connected)
        {
            Debug.LogError("connect server error");
            return;
        }
        GameManager.Net.OnNetConnected();
        // Stream�о��Ƿ��ص�����
        m_TcpStream = m_client.GetStream();
        m_TcpStream.BeginRead(m_Buffer, 0, BufferSize, OnRead, null);
    }

    private void OnRead(IAsyncResult asyncResult)
    {
        try
        {
            if(m_client == null || m_TcpStream == null)
            {
                return;
            }
            // �յ�����Ϣ�ĳ���
            int length = m_TcpStream.EndRead(asyncResult);
            if(length < 1)
            {
                OnDisConnected();
                return;
            }
            ReceiveData(length);
            lock (m_TcpStream)
            {
                Array.Clear(m_Buffer, 0, m_Buffer.Length);
                m_TcpStream.BeginRead(m_Buffer, 0, BufferSize, OnRead, null);
            }
        }catch(Exception e)
        {
            Debug.LogError(e.StackTrace);
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void ReceiveData(int len)
    {
        //Ѱ���ڴ�stream��ĩβ
        m_MemStream.Seek(0, SeekOrigin.End);
        //����ǰ����д��
        m_MemStream.Write(m_Buffer, 0, len);
        m_MemStream.Seek(0, SeekOrigin.Begin);
        while (RemainingBytesLength() >= 8)
        {
            int msgId = m_BinaryReader.ReadInt32();
            int msgLen = m_BinaryReader.ReadInt32();
            if(RemainingBytesLength() >= msgLen)
            {
                byte[] data = m_BinaryReader.ReadBytes(msgLen);
                string message = System.Text.Encoding.UTF8.GetString(data);

                //ת����lua
                GameManager.Net.Receive(msgId, message);
            }
            else
            {
                m_MemStream.Position = m_MemStream.Position - 8;
                break;
            }
        }
        // �����ȡ��ɺ�ʣ��δ�����ݲ���8�ֽ�
        // ���䱣���������������Ϻ��������ٶ�ȡ
        byte[] leftover = m_BinaryReader.ReadBytes(RemainingBytesLength());
        m_MemStream.SetLength(0);
        m_MemStream.Write(leftover, 0, leftover.Length);
    }

    // ʣ����ֽ���
    private int RemainingBytesLength()
    {
        return (int)(m_MemStream.Length - m_MemStream.Position);
    }

    public void SendMessage(int msgId, string message)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            ms.Position= 0;
            BinaryWriter bw = new BinaryWriter(ms);
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            bw.Write(msgId);
            bw.Write((int)data.Length);
            bw.Write(data);
            bw.Flush();
            if(m_client != null && m_client.Connected)
            {
                byte[] sendData = ms.ToArray();
                m_TcpStream.BeginWrite(sendData, 0, sendData.Length, OnEndSend, null);
            }
            else
            {
                Debug.LogError("Server not connected");
            }
        }
    }

    private void OnEndSend(IAsyncResult asyncResponse)
    {
        try
        {
            m_TcpStream.EndWrite(asyncResponse);
        }
        catch(Exception ex)
        {
            OnDisConnected();
            Debug.LogError(ex.Message);
        }
    }

    private void OnDisConnected()
    {
        if(m_client != null && m_client.Connected)
        {
            m_client.Close();
            m_client = null;

            m_TcpStream.Close();
            m_TcpStream = null;
        }
        GameManager.Net.OnDisConnected();
    }
}
