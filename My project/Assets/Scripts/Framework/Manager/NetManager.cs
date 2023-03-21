using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace manager
{
    public class NetManager : MonoSingleton<NetManager>
    {
        NetClient m_NetClient;
        Queue<KeyValuePair<int, string>> m_MessageQueue = new Queue<KeyValuePair<int, string>>();
        XLua.LuaFunction ReceiveMessage;

        public void Init()
        {
            m_NetClient = new NetClient();
            ReceiveMessage = GameManager.Lua.luaEnv.Global.Get<XLua.LuaFunction>("ReceiveMessage");
        }
        // 发送消息
        public void SendMessage(int messageId, string message)
        {
            m_NetClient.SendMessage(messageId, message);
        }
        //链接服务器
        public void ConnectedServer(string host, int port)
        {
            m_NetClient.OnConnectServer(host, port);
        }
        public void OnNetConnected()
        {

        }
        public void OnDisConnected()
        {

        }
        public void Receive(int msgId, string message)
        {
            m_MessageQueue.Enqueue(new KeyValuePair<int, string>(msgId, message));
        }
        private void Update()
        {
            if (m_MessageQueue.Count > 0)
            {
                KeyValuePair<int, string> msg = m_MessageQueue.Dequeue();
                ReceiveMessage?.Call(msg.Key, msg.Value);
            }
        }
    }
}
