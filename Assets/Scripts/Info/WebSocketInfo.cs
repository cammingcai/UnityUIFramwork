using System;
using System.Collections.Generic;
/// <summary>
/// WebSocket 消息发送info
/// </summary>
[Serializable]
class WebSocketInfo
{

    public string active;
    public string token;
    public Dictionary<string, string> data;
}