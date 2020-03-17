#if !BESTHTTP_DISABLE_WEBSOCKET

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BestHTTP.Extensions;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR
    using System.Runtime.InteropServices;
#else
using BestHTTP.WebSocket.Frames;
using BestHTTP.WebSocket.Extensions;
#endif

namespace BestHTTP.WebSocket
{
    /// <summary>
    ///基础浏览器的WebSocket实现状态。
    /// </summary>
    public enum WebSocketStates : byte
    {
        Connecting = 0,
        Open = 1,
        Closing = 2,
        Closed = 3,
        Unknown
    };

    public delegate void OnWebSocketOpenDelegate(WebSocket webSocket);
    public delegate void OnWebSocketMessageDelegate(WebSocket webSocket, string message);
    public delegate void OnWebSocketBinaryDelegate(WebSocket webSocket, byte[] data);
    public delegate void OnWebSocketClosedDelegate(WebSocket webSocket, UInt16 code, string message);
    public delegate void OnWebSocketErrorDelegate(WebSocket webSocket, Exception ex);
    public delegate void OnWebSocketErrorDescriptionDelegate(WebSocket webSocket, string reason);

#if (!UNITY_WEBGL || UNITY_EDITOR)
    public delegate void OnWebSocketIncompleteFrameDelegate(WebSocket webSocket, WebSocketFrameReader frame);
#else
    delegate void OnWebGLWebSocketOpenDelegate(uint id);
    delegate void OnWebGLWebSocketTextDelegate(uint id, string text);
    delegate void OnWebGLWebSocketBinaryDelegate(uint id, IntPtr pBuffer, int length);
    delegate void OnWebGLWebSocketErrorDelegate(uint id, string error);
    delegate void OnWebGLWebSocketCloseDelegate(uint id, int code, string reason);
#endif

    public sealed class WebSocket
    {
        #region Properties

#if !UNITY_WEBGL || UNITY_EDITOR
        public WebSocketStates State { get; private set; }
#else
        public WebSocketStates State { get { return ImplementationId != 0 ? WS_GetState(ImplementationId) : WebSocketStates.Unknown; } }
#endif

        /// <summary>
        /// 与WebSocket服务器的连接是否已打开
        /// </summary>
        public bool IsOpen
        {
            get
            {
#if (!UNITY_WEBGL || UNITY_EDITOR)
                //Debug.Log("是否打开？=="+ webSocket != null);
                return webSocket != null && !webSocket.IsClosed;
#else
                return ImplementationId != 0 && WS_GetState(ImplementationId) == WebSocketStates.Open;
#endif
            }
        }

        public int BufferedAmount
        {
            get
            {
#if (!UNITY_WEBGL || UNITY_EDITOR)
                return webSocket.BufferedAmount;
#else
                return WS_GetBufferedAmount(ImplementationId);
#endif
            }
        }

#if (!UNITY_WEBGL || UNITY_EDITOR)
        /// <summary>
        /// 设置为true以启动新线程以将ping发送到WebSocket服务器
        /// </summary>
        public bool StartPingThread { get; set; }

        /// <summary>
        /// 两次Ping之间的延迟（毫秒）。最小值为100，默认值为1000。
        /// </summary>
        public int PingFrequency { get; set; }

        /// <summary>
        /// 如果startpingthread设置为true，则该插件将关闭连接并发出onerror/onerrordesc事件（如果否）。
        /// 在给定时间内从服务器接收消息。它的默认值是10秒
        /// </summary>
        public TimeSpan CloseAfterNoMesssage { get; set; }

        /// <summary>
        /// 内部httpRequest对象
        /// </summary>
        public HTTPRequest InternalRequest { get; private set; }

        /// <summary>
        /// 插件将与要使用的服务器协商的IExtension实现。
        /// </summary>
        public IExtension[] Extensions { get; private set; }

        /// <summary>
        /// 根据乒乓消息往返时间计算的延迟。
        /// </summary>
        public int Latency { get { return webSocket.Latency; } }

#endif

        /// <summary>
        /// 在与WebSocket服务器建立连接时调用。
        /// </summary>
        public OnWebSocketOpenDelegate OnOpen;

        /// <summary>
        /// 当从服务器接收到新的文本消息时调用
        /// </summary>
        public OnWebSocketMessageDelegate OnMessage;

        /// <summary>
        /// 当从服务器接收到新的二进制消息时调用
        /// </summary>
        public OnWebSocketBinaryDelegate OnBinary;

        /// <summary>
        /// 当WebSocket连接关闭时调用
        /// </summary>
        public OnWebSocketClosedDelegate OnClosed;

        /// <summary>
        ///遇到错误时调用。异常参数可能为空。
        /// </summary>
        public OnWebSocketErrorDelegate OnError;

        /// <summary>
        /// 遇到错误时调用。参数将是错误的描述
        /// </summary>
        public OnWebSocketErrorDescriptionDelegate OnErrorDesc;

#if (!UNITY_WEBGL || UNITY_EDITOR)
        /// <summary>
        /// 收到不完整的帧时调用。不会尝试在内部重新组装这些片段，并且在该事件之后不会存储对此帧的引用
        /// </summary>
        public OnWebSocketIncompleteFrameDelegate OnIncompleteFrame;
#endif

        #endregion

        #region Private Fields

#if (!UNITY_WEBGL || UNITY_EDITOR)
        /// <summary>
        ///指示我们何时向服务器发送连接请求
        /// </summary>
        private bool requestSent;

        /// <summary>
        /// 内部WebSocketResponse对象
        /// </summary>
        private WebSocketResponse webSocket;
#else
        internal static Dictionary<uint, WebSocket> WebSockets = new Dictionary<uint, WebSocket>();

        private uint ImplementationId;
        private Uri Uri;
        private string Protocol;

#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a WebSocket instance from the given uri.
        /// </summary>
        /// <param name="uri">The uri of the WebSocket server</param>
        public WebSocket(Uri uri)
            : this(uri, string.Empty, string.Empty)
        {
#if (!UNITY_WEBGL || UNITY_EDITOR) && !BESTHTTP_DISABLE_GZIP
            this.Extensions = new IExtension[] { new PerMessageCompression(/*compression level: */           Decompression.Zlib.CompressionLevel.Default,
                                                                           /*clientNoContextTakeover: */     false,
                                                                           /*serverNoContextTakeover: */     false,
                                                                           /*clientMaxWindowBits: */         Decompression.Zlib.ZlibConstants.WindowBitsMax,
                                                                           /*desiredServerMaxWindowBits: */  Decompression.Zlib.ZlibConstants.WindowBitsMax,
                                                                           /*minDatalengthToCompress: */     PerMessageCompression.MinDataLengthToCompressDefault) };
#endif
        }

        /// <summary>
        /// Creates a WebSocket instance from the given uri, protocol and origin.
        /// </summary>
        /// <param name="uri">The uri of the WebSocket server</param>
        /// <param name="origin">Servers that are not intended to process input from any web page but only for certain sites SHOULD verify the |Origin| field is an origin they expect.
        /// If the origin indicated is unacceptable to the server, then it SHOULD respond to the WebSocket handshake with a reply containing HTTP 403 Forbidden status code.</param>
        /// <param name="protocol">The application-level protocol that the client want to use(eg. "chat", "leaderboard", etc.). Can be null or empty string if not used.</param>
        /// <param name="extensions">Optional IExtensions implementations</param>
        public WebSocket(Uri uri, string origin, string protocol
#if !UNITY_WEBGL || UNITY_EDITOR
            , params IExtension[] extensions
#endif
            )

        {
            string scheme = HTTPProtocolFactory.IsSecureProtocol(uri) ? "wss" : "ws";
            int port = uri.Port != -1 ? uri.Port : (scheme.Equals("wss", StringComparison.OrdinalIgnoreCase) ? 443 : 80);

            // Somehow if i use the UriBuilder it's not the same as if the uri is constructed from a string...
            //uri = new UriBuilder(uri.Scheme, uri.Host, uri.Scheme.Equals("wss", StringComparison.OrdinalIgnoreCase) ? 443 : 80, uri.PathAndQuery).Uri;
            uri = new Uri(scheme + "://" + uri.Host + ":" + port + uri.GetRequestPathAndQueryURL());

#if !UNITY_WEBGL || UNITY_EDITOR
            // Set up some default values.
            this.PingFrequency = 1000;
            this.CloseAfterNoMesssage = TimeSpan.FromSeconds(10);

            InternalRequest = new HTTPRequest(uri, OnInternalRequestCallback);

            // Called when the regular GET request is successfully upgraded to WebSocket
            InternalRequest.OnUpgraded = OnInternalRequestUpgraded;

            //http://tools.ietf.org/html/rfc6455#section-4

            //The request MUST contain a |Host| header field whose value contains /host/ plus optionally ":" followed by /port/ (when not using the default port).
            if ((!HTTPProtocolFactory.IsSecureProtocol(uri) && uri.Port != 80) && (HTTPProtocolFactory.IsSecureProtocol(uri) && uri.Port != 443))
                InternalRequest.SetHeader("Host", uri.Host + ":" + uri.Port);
            else
                InternalRequest.SetHeader("Host", uri.Host);

            // The request MUST contain an |Upgrade| header field whose value MUST include the "websocket" keyword.
            InternalRequest.SetHeader("Upgrade", "websocket");

            // The request MUST contain a |Connection| header field whose value MUST include the "Upgrade" token.
            InternalRequest.SetHeader("Connection", "Upgrade");

            // The request MUST include a header field with the name |Sec-WebSocket-Key|.  The value of this header field MUST be a nonce consisting of a
            // randomly selected 16-byte value that has been base64-encoded (see Section 4 of [RFC4648]).  The nonce MUST be selected randomly for each connection.
            InternalRequest.SetHeader("Sec-WebSocket-Key", GetSecKey(new object[] { this, InternalRequest, uri, new object() }));

            // The request MUST include a header field with the name |Origin| [RFC6454] if the request is coming from a browser client.
            // If the connection is from a non-browser client, the request MAY include this header field if the semantics of that client match the use-case described here for browser clients.
            // More on Origin Considerations: http://tools.ietf.org/html/rfc6455#section-10.2
            if (!string.IsNullOrEmpty(origin))
                InternalRequest.SetHeader("Origin", origin);

            // The request MUST include a header field with the name |Sec-WebSocket-Version|.  The value of this header field MUST be 13.
            InternalRequest.SetHeader("Sec-WebSocket-Version", "13");

            if (!string.IsNullOrEmpty(protocol))
                InternalRequest.SetHeader("Sec-WebSocket-Protocol", protocol);

            // Disable caching
            InternalRequest.SetHeader("Cache-Control", "no-cache");
            InternalRequest.SetHeader("Pragma", "no-cache");

            this.Extensions = extensions;

#if !BESTHTTP_DISABLE_CACHING
            InternalRequest.DisableCache = true;
            InternalRequest.DisableRetry = true;
#endif

            InternalRequest.TryToMinimizeTCPLatency = true;

#if !BESTHTTP_DISABLE_PROXY
            // WebSocket is not a request-response based protocol, so we need a 'tunnel' through the proxy
            HTTPProxy httpProxy = HTTPManager.Proxy as HTTPProxy;
            if (httpProxy != null)
                InternalRequest.Proxy = new HTTPProxy(httpProxy.Address,
                                                      httpProxy.Credentials,
                                                      false, /*turn on 'tunneling'*/
                                                      false, /*sendWholeUri*/
                                                      httpProxy.NonTransparentForHTTPS);
#endif
#else
            this.Uri = uri;
            this.Protocol = protocol;
#endif

            // Under WebGL when only the WebSocket protocol is used Setup() isn't called, so we have to call it here.
            HTTPManager.Setup();
        }

        #endregion

        #region Request Callbacks

#if (!UNITY_WEBGL || UNITY_EDITOR)
        private void OnInternalRequestCallback(HTTPRequest req, HTTPResponse resp)
        {
            string reason = string.Empty;

            switch (req.State)
            {
                case HTTPRequestStates.Finished:
                    if (resp.IsSuccess || resp.StatusCode == 101)
                    {
                        // The request finished without any problem.
                        HTTPManager.Logger.Information("WebSocket", string.Format("Request finished. Status Code: {0} Message: {1}", resp.StatusCode.ToString(), resp.Message));

                        return;
                    }
                    else
                        reason = string.Format("Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                        resp.StatusCode,
                                                        resp.Message,
                                                        resp.DataAsText);
                    break;

                // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                case HTTPRequestStates.Error:
                    reason = "Request Finished with Error! " + (req.Exception != null ? ("Exception: " + req.Exception.Message + req.Exception.StackTrace) : string.Empty);
                    break;

                // The request aborted, initiated by the user.
                case HTTPRequestStates.Aborted:
                    reason = "Request Aborted!";
                    break;

                // Connecting to the server is timed out.
                case HTTPRequestStates.ConnectionTimedOut:
                    reason = "Connection Timed Out!";
                    break;

                // The request didn't finished in the given time.
                case HTTPRequestStates.TimedOut:
                    reason = "Processing the request Timed Out!";
                    break;

                default:
                    return;
            }

            if (this.State != WebSocketStates.Connecting || !string.IsNullOrEmpty(reason))
            {
                if (OnError != null)
                    OnError(this, req.Exception);

                if (OnErrorDesc != null)
                    OnErrorDesc(this, reason);

                if (OnError == null && OnErrorDesc == null)
                    HTTPManager.Logger.Error("WebSocket", reason);
            }
            else if (OnClosed != null)
                OnClosed(this, (ushort)WebSocketStausCodes.NormalClosure, "Closed while opening");

            if (!req.IsKeepAlive && resp != null && resp is WebSocketResponse)
                (resp as WebSocketResponse).CloseStream();
        }

        private void OnInternalRequestUpgraded(HTTPRequest req, HTTPResponse resp)
        {
            webSocket = resp as WebSocketResponse;

            if (webSocket == null)
            {
                if (OnError != null)
                    OnError(this, req.Exception);

                if (OnErrorDesc != null)
                {
                    string reason = string.Empty;
                    if (req.Exception != null)
                        reason = req.Exception.Message + " " + req.Exception.StackTrace;

                    OnErrorDesc(this, reason);
                }

                this.State = WebSocketStates.Closed;
                return;
            }

            // If Close called while we connected
            if (this.State == WebSocketStates.Closed)
            {
                webSocket.CloseStream();
                return;
            }

            webSocket.WebSocket = this;

            if (this.Extensions != null)
            {
                for (int i = 0; i < this.Extensions.Length; ++i)
                {
                    var ext = this.Extensions[i];

                    try
                    {
                        if (ext != null && !ext.ParseNegotiation(webSocket))
                            this.Extensions[i] = null; // Keep extensions only that successfully negotiated
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("WebSocket", "ParseNegotiation", ex);

                        // Do not try to use a defective extension in the future
                        this.Extensions[i] = null;
                    }
                }
            }

            this.State = WebSocketStates.Open;
            if (OnOpen != null)
            {
                try
                {
                    OnOpen(this);
                }
                catch (Exception ex)
                {
                    HTTPManager.Logger.Exception("WebSocket", "OnOpen", ex);
                }
            }

            webSocket.OnText = (ws, msg) =>
            {
                if (OnMessage != null)
                    OnMessage(this, msg);
            };

            webSocket.OnBinary = (ws, bin) =>
            {
                if (OnBinary != null)
                    OnBinary(this, bin);
            };

            webSocket.OnClosed = (ws, code, msg) =>
            {
                this.State = WebSocketStates.Closed;

                if (OnClosed != null)
                    OnClosed(this, code, msg);
            };

            if (OnIncompleteFrame != null)
                webSocket.OnIncompleteFrame = (ws, frame) =>
                {
                    if (OnIncompleteFrame != null)
                        OnIncompleteFrame(this, frame);
                };

            if (StartPingThread)
                webSocket.StartPinging(Math.Max(PingFrequency, 100));

            webSocket.StartReceive();
        }
#endif

        #endregion

        #region Public Interface

        /// <summary>
        /// 开始打开web过程。
        /// </summary>
        public void Open()
        {
#if (!UNITY_WEBGL || UNITY_EDITOR)
            if (requestSent)
            {
           //     GameStart.OpenCommonConfirm("请求失败", "打开已调用！不能重用此WebSocket实例", null, null);
                throw new InvalidOperationException("打开已调用！不能重用此WebSocket实例！");
            }

            if (this.Extensions != null)
            {
                try
                {
                    for (int i = 0; i < this.Extensions.Length; ++i)
                    {
                        var ext = this.Extensions[i];
                        if (ext != null)
                            ext.AddNegotiation(InternalRequest);
                    }
                }
                catch (Exception ex)
                {
                    //GameStart.OpenCommonConfirm("连接到web服务器捕获异常", ex.Message, () => { });
                    HTTPManager.Logger.Exception("WebSocket", "Open", ex);
                }
            }

            InternalRequest.Send();
            requestSent = true;
            this.State = WebSocketStates.Connecting;
#else
            try
            {
                ImplementationId = WS_Create(this.Uri.OriginalString, this.Protocol, OnOpenCallback, OnTextCallback, OnBinaryCallback, OnErrorCallback, OnCloseCallback);
                WebSockets.Add(ImplementationId, this);
            }
            catch(Exception ex)
            {
                Facade.Instance.SendNotification(ProConst.Msg_ShowHintInfo, new MsgModel("连接到web服务器捕获异常:"+ex, Color.red));
                HTTPManager.Logger.Exception("WebSocket", "Open", ex);
            }
#endif
        }

        /// <summary>
        /// 它将在一个帧中将给定的消息发送到服务器
        /// </summary>
        public void Send(string message)
        {
            Debug.LogError("----------------------------------------1111---message!-----------------------------------：" + message);
            if (!IsOpen)
            {
                //Facade.Instance.SendNotification(ProConst.Msg_ShowHintInfo, new MsgModel("与服务器的websocket没有建立连接，无法发送消息..", Color.red));
              //  GameStart.OpenCommonConfirm("连接失败", "与服务器的websocket连接异常，即将退出", () => { Application.Quit(); });
                Debug.LogError("与服务器的websocket没有建立连接，无法发送消息..");
                return;
            }

#if (!UNITY_WEBGL || UNITY_EDITOR)
            webSocket.Send(message);
#else
            WS_Send_String(this.ImplementationId, message);
#endif
        }

        /// <summary>
        /// It will send the given data to the server in one frame.
        /// </summary>
        public void Send(byte[] buffer)
        {
            if (!IsOpen)
            {
             //   GameStart.OpenCommonConfirm("连接失败", "与服务器的websocket建立连接失败，点击确定退出程序", () => { Application.Quit(); }, null);
                return;
            }
#if (!UNITY_WEBGL || UNITY_EDITOR)
            webSocket.Send(buffer);
#else
            WS_Send_Binary(this.ImplementationId, buffer, 0, buffer.Length);
#endif
        }

        /// <summary>
        ///将从偏移量开始从字节数组发送count字节。
        /// </summary>
        public void Send(byte[] buffer, ulong offset, ulong count)
        {
            if (!IsOpen)
            {
                Debug.LogError("与服务器的建立连接失败");
                //GameStart.OpenCommonConfirm("连接失败", "与服务器的websocket建立连接失败，点击确定退出程序", () => { Application.Quit(); }, null);
                return;
            }
#if (!UNITY_WEBGL || UNITY_EDITOR)
            webSocket.Send(buffer, offset, count);
#else
            WS_Send_Binary(this.ImplementationId, buffer, (int)offset, (int)count);
#endif
        }

#if (!UNITY_WEBGL || UNITY_EDITOR)
        /// <summary>
        /// It will send the given frame to the server.
        /// </summary>
        public void Send(WebSocketFrame frame)
        {
            if (!IsOpen)
            {
             //   GameStart.OpenCommonConfirm("连接失败", "与服务器的websocket建立连接失败，点击确定退出程序", () => { Application.Quit(); }, null);
                return;
            }

            webSocket.Send(frame);
        }
#endif

        /// <summary>
        ///它将启动与服务器的连接的关闭
        /// </summary>
        public void Close()
        {
            //requestSent = false;
            if (State >= WebSocketStates.Closing)
                return;
          
#if !UNITY_WEBGL || UNITY_EDITOR
            if (this.State == WebSocketStates.Connecting)
            {
                this.State = WebSocketStates.Closed;
                if (OnClosed != null)
                    OnClosed(this, (ushort)WebSocketStausCodes.NoStatusCode, string.Empty);
            }
            else
            {
                this.State = WebSocketStates.Closing;
                webSocket.Close();
            }
#else
            WS_Close(this.ImplementationId, 1000, "Bye!");
#endif
        }

        /// <summary>
        /// 它将启动与发送给定代码和消息的服务器的连接的关闭
        /// </summary>
        public void Close(UInt16 code, string message)
        {
            if (!IsOpen)
            {
              //  GameStart.OpenCommonConfirm("连接失败", "与服务器的websocket建立连接失败，点击确定退出程序", () => { Application.Quit(); }, null);
                return;
            }
#if (!UNITY_WEBGL || UNITY_EDITOR)
            webSocket.Close(code, message);
#else
            WS_Close(this.ImplementationId, code, message);
#endif
        }

        public static byte[] EncodeCloseData(UInt16 code, string message)
        {
            //If there is a body, the first two bytes of the body MUST be a 2-byte unsigned integer
            // (in network byte order) representing a status code with value /code/ defined in Section 7.4 (http://tools.ietf.org/html/rfc6455#section-7.4). Following the 2-byte integer,
            // the body MAY contain UTF-8-encoded data with value /reason/, the interpretation of which is not defined by this specification.
            // This data is not necessarily human readable but may be useful for debugging or passing information relevant to the script that opened the connection.
            int msgLen = Encoding.UTF8.GetByteCount(message);
            using (BufferPoolMemoryStream ms = new BufferPoolMemoryStream(2 + msgLen))
            {
                byte[] buff = BitConverter.GetBytes(code);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buff, 0, buff.Length);

                ms.Write(buff, 0, buff.Length);

                buff = Encoding.UTF8.GetBytes(message);
                ms.Write(buff, 0, buff.Length);

                return ms.ToArray();
            }
        }

        #endregion

        #region Private Helpers

#if !UNITY_WEBGL || UNITY_EDITOR
        private string GetSecKey(object[] from)
        {
            byte[] keys = new byte[16];
            int pos = 0;

            for (int i = 0; i < from.Length; ++i)
            {
                byte[] hash = BitConverter.GetBytes((Int32)from[i].GetHashCode());

                for (int cv = 0; cv < hash.Length && pos < keys.Length; ++cv)
                    keys[pos++] = hash[cv];
            }

            return Convert.ToBase64String(keys);
        }
#endif

        #endregion

        #region WebGL Static Callbacks
#if UNITY_WEBGL && !UNITY_EDITOR

        [AOT.MonoPInvokeCallback(typeof(OnWebGLWebSocketOpenDelegate))]
        static void OnOpenCallback(uint id)
        {
            WebSocket ws;
            if (WebSockets.TryGetValue(id, out ws))
            {
                if (ws.OnOpen != null)
                {
                    try
                    {
                        ws.OnOpen(ws);
                    }
                    catch(Exception ex)
                    {
                        HTTPManager.Logger.Exception("WebSocket", "OnOpen", ex);
                    }
                }
            }
            else
                HTTPManager.Logger.Warning("WebSocket", "OnOpenCallback - No WebSocket found for id: " + id.ToString());
        }

        [AOT.MonoPInvokeCallback(typeof(OnWebGLWebSocketTextDelegate))]
        static void OnTextCallback(uint id, string text)
        {
            WebSocket ws;
            if (WebSockets.TryGetValue(id, out ws))
            {
                if (ws.OnMessage != null)
                {
                    try
                    {
                        ws.OnMessage(ws, text);
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("WebSocket", "OnMessage", ex);
                    }
                }
            }
            else
                HTTPManager.Logger.Warning("WebSocket", "OnTextCallback - No WebSocket found for id: " + id.ToString());
        }

        [AOT.MonoPInvokeCallback(typeof(OnWebGLWebSocketBinaryDelegate))]
        static void OnBinaryCallback(uint id, IntPtr pBuffer, int length)
        {
            WebSocket ws;
            if (WebSockets.TryGetValue(id, out ws))
            {
                if (ws.OnBinary != null)
                {
                    try
                    {
                        byte[] buffer = new byte[length];

                        // Copy data from the 'unmanaged' memory to managed memory. Buffer will be reclaimed by the GC.
                        Marshal.Copy(pBuffer, buffer, 0, length);

                        ws.OnBinary(ws, buffer);
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("WebSocket", "OnBinary", ex);
                    }
                }
            }
            else
                HTTPManager.Logger.Warning("WebSocket", "OnBinaryCallback - No WebSocket found for id: " + id.ToString());
        }

        [AOT.MonoPInvokeCallback(typeof(OnWebGLWebSocketErrorDelegate))]
        static void OnErrorCallback(uint id, string error)
        {
            WebSocket ws;
            if (WebSockets.TryGetValue(id, out ws))
            {
                WebSockets.Remove(id);

                if (ws.OnError != null)
                {
                    try
                    {
                        ws.OnError(ws, new Exception(error));
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("WebSocket", "OnError", ex);
                    }
                }

                if (ws.OnErrorDesc != null)
                {
                    try
                    {
                        ws.OnErrorDesc(ws, error);
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("WebSocket", "OnErrorDesc", ex);
                    }
                }
            }
            else
                HTTPManager.Logger.Warning("WebSocket", "OnErrorCallback - No WebSocket found for id: " + id.ToString());

            try
            {
                WS_Release(id);
            }
            catch(Exception ex)
            {
                HTTPManager.Logger.Exception("WebSocket", "WS_Release", ex);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(OnWebGLWebSocketCloseDelegate))]
        static void OnCloseCallback(uint id, int code, string reason)
        {
            // To match non-webgl behavior, we have to treat this client-side generated message as an error
            if (code == (int)WebSocketStausCodes.ClosedAbnormally)
            {
                OnErrorCallback(id, "Abnormal disconnection.");
                return;
            }

            WebSocket ws;
            if (WebSockets.TryGetValue(id, out ws))
            {
                WebSockets.Remove(id);

                if (ws.OnClosed != null)
                {
                    try
                    {
                        ws.OnClosed(ws, (ushort)code, reason);
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("WebSocket", "OnClosed", ex);
                    }
                }
            }
            else
                HTTPManager.Logger.Warning("WebSocket", "OnCloseCallback - No WebSocket found for id: " + id.ToString());

            try
            {
                WS_Release(id);
            }
            catch(Exception ex)
            {
                HTTPManager.Logger.Exception("WebSocket", "WS_Release", ex);
            }
        }

#endif
        #endregion

        #region WebGL Interface
#if UNITY_WEBGL && !UNITY_EDITOR

        [DllImport("__Internal")]
        static extern uint WS_Create(string url, string protocol, OnWebGLWebSocketOpenDelegate onOpen, OnWebGLWebSocketTextDelegate onText, OnWebGLWebSocketBinaryDelegate onBinary, OnWebGLWebSocketErrorDelegate onError, OnWebGLWebSocketCloseDelegate onClose);

        [DllImport("__Internal")]
        static extern WebSocketStates WS_GetState(uint id);
        
        [DllImport("__Internal")]
        static extern int WS_GetBufferedAmount(uint id);

        [DllImport("__Internal")]
        static extern int WS_Send_String(uint id, string strData);

        [DllImport("__Internal")]
        static extern int WS_Send_Binary(uint id, byte[] buffer, int pos, int length);

        [DllImport("__Internal")]
        static extern void WS_Close(uint id, ushort code, string reason);

        [DllImport("__Internal")]
        static extern void WS_Release(uint id);

#endif
        #endregion
    }
}

#endif