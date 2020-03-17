using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestHTTP;
using BestHTTP.WebSocket;
using System;
using System.Threading;
using LitJson;
/// <summary>
/// create by camming 2020/02/13
/// </summary>
public class WebSocketPanel : MonoBehaviour
{
    //连接成功了
    private const string onOpen =  "onOpen";
    //线上
    //private string socketUrl = "wss://bws.tfwangs.com/aidea";
    //线下
    private string socketUrl = "wss://bws.tfwangs.com/aidea";
    //接收数据
    private Action<string> receiverMsgAction = null;
    //失败的数据
    private Action<string> failedAction = null;
    //心跳包数据
    private Action<string> heartbestdAction = null;

    //发送消息的消息队列
    private Queue<string> sendMsgQueue = new Queue<string>();
    private bool isSendMsg = false;
    //接收消息的消息队列
    private Queue<string> receiverMsgQueue = new Queue<string>();
    private bool isReceiverMsg = false;
    //重连线程
    private Thread reconnectThread = null;
    //是否重连
    private bool isReconnect = false;


    private WebSocket webSocket;
    private static WebSocketPanel instance;


    public bool isOpenWebSocket = false;

    public static WebSocketPanel Instance
    {
        get
        {
           /// if (instance == null)
            ///{
               // instance = new WebSocketPanel();
               // instance = this;
           // }
            return instance;
        }
    }
    void Start()
    {
        instance = this;
        //  base.Start()

        if (isOpenWebSocket) {
            bool isSuccessSocket = InitSocket();
            Debug.Log("----------isSuccessSocket=" + isSuccessSocket);
        }
            
          


       
    }
    /// <summary>
    /// 初始化socket
    /// </summary>
    /// <returns></returns>
    private bool InitSocket()
    {
        if (null == webSocket)
        {
            webSocket = new WebSocket(new Uri(socketUrl));
            if (webSocket == null) return false;
            webSocket.OnOpen += OnOpen;
            webSocket.OnMessage += OnMessage;
            webSocket.OnError += OnError;
            webSocket.OnClosed += OnClosed;
        }
       // if(webSocket!=null)
            //ConnectSocket();

        return ConnectSocket();
    }
    private void OnOpen(WebSocket ws)
    {
       
        Debug.Log("connecting..." + ws.IsOpen);
    }
    
    /// <summary>
    /// 接收服务端发来的消息
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="msg"></param>
    private void OnMessage(WebSocket ws,string msg)
    {
        
        Debug.Log("OnMessage...msg="+ msg);
        //加入到接收数据的消息队列
        if (msg != null && receiverMsgQueue != null)
        {
            isReceiverMsg = true;
            receiverMsgQueue.Enqueue(msg);
        }
           
       
        
    }

    /// <summary>
    /// 对接收的数据进行处理
    /// </summary>
    /// <param name="data"></param>
    private void ParseData(string msg)
    {
        if (!msg.Equals("ping"))
        {

            //转为json对象
            JsonData jsonData = JsonMapper.ToObject(msg);
            if (int.Parse(jsonData["code"].ToString()) == 0)//服务端返回数据
            {

                if (jsonData["active"].ToString().Equals(onOpen))
                {
                    StartHeartbestPackage();
                }
                if (receiverMsgAction != null)
                    receiverMsgAction(msg);


            }
            else
            {
                if (failedAction != null)
                {
                    string failedMsg = jsonData["message"].ToString();
                    if (failedMsg.IsNullOrEmpty())
                        failedAction(msg);
                    else
                        failedAction(failedMsg);

                    //failedAction =  failedMsg.IsNullOrEmpty()? failedAction(msg) : failedAction(failedMsg) ;
                }
                // failedAction(msg);asdfasdf:

            }
        }
        else
        {
            Debug.Log("OnMessage.收到心跳包的回复.msg=" + msg);
        }
    }
  

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="ex"></param>
    private void OnError(WebSocket ws, Exception ex)
    {
        Debug.Log("OnError...");

        string errorMsg = string.Empty;
        if (ws.InternalRequest.Response != null)
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}",
                ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
        Debug.LogError("OnError webSocket.IsOpen.=" + webSocket.IsOpen);
        //DestroyWebSocket();
        Debug.Log(" webSocket=" + webSocket);

        if (ex != null)
        {
            //重新连接
            DestroyWebSocket();
            StartReconnectThread(recon: true);
            //isReconnect = true;
        }
        
    }

    /// <summary>
    /// 关闭socket
    /// </summary>
    /// <param name="ws"></param>
    /// <param name="code"></param>
    /// <param name="message"></param>
    private void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        Debug.LogError("OnClosed webSocket.IsOpen.=" + webSocket.IsOpen);
        Debug.Log("OnClosed...");
        DestroyWebSocket(); 
        CloseWebSocket();
      //  StartReconnectThread(recon:true);
    }
    /// <summary>
    /// 开始连接socket
    /// </summary>
    /// <returns></returns>
    public bool ConnectSocket()
    {
       
        if (webSocket == null)
            return false;
        Debug.Log(" ConnectSocket webSocket.IsOpen.=" + webSocket.IsOpen);
        if (!webSocket.IsOpen)
            webSocket.Open();
      //  Debug.Log("OnClosed...");
      
        return webSocket.IsOpen;
    }

    /// <summary>
    /// 开始心跳包
    /// </summary>
    private void StartHeartbestPackage()
    {

        Debug.Log("---------------连接成功-------------------------");
        if (webSocket.IsOpen)
        {
            Debug.Log("-------开始心跳包线程---");
            // 开始心跳包协程
            if (heartbeatThread == null)
            {
                heartbeatThread = new Thread(OnStartHeartbeatThread);
                isHeartbeat = true;
                heartbeatThread.IsBackground = true;
                heartbeatThread.Start();
            }
        }
    }

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg">消息</param>
    public void SendMessage(string msg)
    {
     //   Debug.LogError("开始发送的消息------webSocket="+ webSocket);
        if (webSocket == null)
            return;
        if (msg.IsNullOrEmpty()) {
            Debug.LogError("发送的消息为空");
            return;
        }
        Debug.LogError("发送的消息="+msg);

        isSendMsg = true;
        sendMsgQueue.Enqueue(msg);
      //  webSocket.Send(msg);
    }
    /// <summary>
    /// 发送消息  带回调
    /// </summary>
    /// <param name="msg">消息</param>
    public void SendMessage(string msg,Action<string> receiverAct = null)
    {
      //  Debug.LogError("开始发送的消息------webSocket=" + webSocket);
        if (webSocket == null)
            return;
        if (msg.IsNullOrEmpty())
        {
            Debug.LogError("发送的消息为空");
            return;
        }



        if (webSocket.IsOpen) {
            receiverMsgAction = receiverAct;
            Debug.LogError("发送的消息=" + msg);
            sendMsgQueue.Enqueue(msg);
        }
    
        // webSocket.Send(msg);
    }

    /// <summary>
    /// 销毁socket
    /// </summary>
    private void DestroyWebSocket()
    {
        if (webSocket != null)
        {
            webSocket.Close();
            webSocket.OnOpen = null;
            webSocket.OnMessage = null;
            webSocket.OnError = null;
            webSocket.OnClosed = null;
            webSocket = null;
        }

        // 关闭心跳包协程
        if (heartbeatThread != null)
        {
            isHeartbeat = false;
            timer = 0;
            heartbeatThread = null;
        }
        isSendMsg = false;
        isReceiverMsg = false;
    }

    private void CloseWebSocket()
    {
        //if (webSocket != null)
        //{
        //    webSocket.Close();
        //}
        isHeartbeat = false;
        timer = 0;
        isSendMsg = false;
        isReceiverMsg = false;

    }
  
    void Update()
    {
        // Debug.Log("webSocket=" + webSocket);
        //取出发送的数据
        if (isSendMsg&&sendMsgQueue != null && sendMsgQueue.Count > 0)
        {
            //先进先出  拿出开头数据 并删除
            if (webSocket != null ) {
                string topMsg = sendMsgQueue.Dequeue();
                webSocket.Send(topMsg);
                if (sendMsgQueue.Count == 0)
                    isSendMsg = false;
            }
               
        }

        //取出接收的数据
        if (isReceiverMsg && receiverMsgQueue != null && receiverMsgQueue.Count > 0)
        {
            //先进先出  拿出开头数据 并删除
            string topMsg = receiverMsgQueue.Dequeue();
            ParseData(topMsg);
            if (sendMsgQueue.Count == 0)
                isReceiverMsg = false;

        }
    }


    #region 心跳包
    //心跳包线程
    public Thread heartbeatThread = null;
    //是否开始心跳包线程
    private bool isHeartbeat = false;
    // 计时
    private int timer = 0;
    // 时隔 3分钟重新一次心跳包
    private int heartbeatTimer = 180;

    /// <summary>
    /// 开启心跳包线程
    /// </summary>
    private void OnStartHeartbeatThread()
    {
        while (isHeartbeat)
        {
            timer++;
            if (timer >= heartbeatTimer)
            {
                SendMessage("ping");
                timer = 0;
            }
            Thread.Sleep(1000);
        }
    }
    #endregion
    //开始重连线程
    private void StartReconnectThread(bool recon)
    {
        CloseWebSocket();
        if (reconnectThread == null)
        {
            reconnectThread = new Thread(StartReconnect);
            reconnectThread.IsBackground = true;
            reconnectThread.Start();
        }
        isReconnect = true;
        //if(webSocket!=null)
    }
    //重连次数
    private int connectNum = 0;
    private int reconnectTime = 2000;
    private void StartReconnect()
    {
        while (isReconnect)
        {
            connectNum++;
            Debug.LogError("重连第"+connectNum+"次");
           // bool connecting = ConnectSocket();
            bool connecting = InitSocket();
            if (connecting) {
                Debug.LogError("--------重连成功了--------");
                connectNum = 0;
                isReconnect = false;
            }
            if (connectNum >= 10)
            {
                connectNum = 0;
                isReconnect = false;
                Debug.LogError("--------重连超过次数了，停止连接 直接退出程序吧-------");
            }
               
            Thread.Sleep(reconnectTime);
        }
    }
    #region

    #endregion


    private void OnDestroy()
    {
        DestroyWebSocket();
    }

}
