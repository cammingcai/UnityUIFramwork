using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System.Text;

public class HttpRequestUtils
{
    //测试token
    private string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJodHRwOlwvXC9jb3UudGZ3YW5ncy5jb206ODA4MSIsInN1YiI6NjAsImlhdCI6MTU4MTgxOTA5OCwiZXhwIjoxNTgyNDIzODk4LCJyZnQiOjE1ODQ0MTEwOTgsImp0aSI6IjYwOjVlNDhhNGRhMzc4NzUifQ.nAu4E5Wsli1i7_YBRcH6fN_asRXi15fV2U9SUSmwfR8";
    private string m_Url;
    /// <summary>
    /// 是否开始下载
    /// </summary>
    protected bool m_StartDownLoad;

    // 请求的数据
    WWWForm _form = null;
    Action<string> _callback = null;

    private static HttpRequestUtils instance;

    public static HttpRequestUtils Instance()
    {
        
        if (instance == null) {
            instance = new HttpRequestUtils();
        }

        return instance;


    }

    public void HttpPost(string url, WWWForm form, Action<string> callback = null)
    {
        this.m_Url = url;
        GameRoot.Instance.StartCoroutine(Request_POST(form, callback));
    }
    public void HttpPostTest( WWWForm form, Action<string> callback = null)
    {
      
        GameRoot.Instance.StartCoroutine(Request_POST(form, callback));
    }
    public void HttpGet(string url, Action<string> callback = null)
    {
        this.m_Url = url;
        GameRoot.Instance.StartCoroutine(Request_GET(callback));
    }




    /// <summary>
    /// GET 请求
    /// </summary>
    /// <param name="action"></param>
    /// <param name="Type"> 0是打开加载效果，1是不打开 </param>
    private IEnumerator Request_GET(Action<string> callback = null)
    {
        Debug.Log("GET------当前请求的地址 URL ----------- ：" + m_Url);

        _callback = callback;
        
        
            UIManager.Instance.PushPanel(UIPanelType.Loading);
           
        
        // string Url = UrlConsts.newUrlPrefix + m_Url + "?token=" + PlayerPrefs.GetString(PlayerPrefsConst.token);

        string Url = "http://t.weather.sojson.com/api/weather/city/101030100";
        using (UnityWebRequest m_WebRequest = UnityWebRequest.Get(Url))
        {
            m_StartDownLoad = true;
            m_WebRequest.timeout = 10;
            yield return m_WebRequest.SendWebRequest();
            m_StartDownLoad = false;
            
            UIManager.Instance.PopPanel();
            if (m_WebRequest.isNetworkError)
            {
              //  Debug.LogError(string.Format("地址:{0}异常", m_Url) + m_WebRequest.error);
                if (m_WebRequest.error == "Request timeout")
                {
                    // 提示超时
                   // GameStart.OpenCommonConfirm("网络连接异常", "请求服务器发生超时...");
                }
                else
                {
                   // GameStart.OpenCommonConfirm("网络连接异常", "请求服务器发生错误异常...");
                }
            }
            else
            {

                if (m_WebRequest.responseCode == 200)//200表示接受成功
                {
                   
                   // JsonData jsonData = JsonMapper.ToObject(m_WebRequest.downloadHandler.text);

                    //测试
                    _callback(m_WebRequest.downloadHandler.text);

                    UIManager.Instance.SendMessage(UIPanelType.MainMenu, m_WebRequest.downloadHandler.text);


                }
                else
                {
                    Debug.LogError("失败！" + m_WebRequest.downloadHandler.text);
                    Debug.Log(" error:" + m_WebRequest.error);
                    //GameStart.OpenCommonConfirm("请求失败", "刷新异常，是否返回登录", () => {
                    //    GameStart.Instance.OnCancellationLogin();
                  
                    //}, null);
                }
            }
        }
        
    }

    /// <summary>
    /// post 请求
    /// </summary>
    /// <param name="form">传递的信息字段</param>
    /// <param name="callback">回调的委托</param>
    /// <param name="isShowLoading">请求时是否打开圈圈提示</param>
    /// <param name="isJudgeCode">是否不经过code判断就返回</param>
    /// <param name="_delayed">是否延时请求</param>
    /// <returns></returns>
    private IEnumerator Request_POST(WWWForm form, Action<string> callback = null, bool isJudgeCode = true,float _delayed = 0)
    {
        Debug.Log("-----------当前请求的地址 URL ----------- ：" + m_Url);

        //// 是否显示加载logo
        UIManager.Instance.PushPanel(UIPanelType.Loading);
        // 如果有请求延时需要
        if (_delayed != 0)
        {
            yield return new WaitForSeconds(_delayed);
        }

        _form = form;
        _callback = callback;



        //  string Url = UrlConsts.newUrlPrefix + m_Url + "?token=" + PlayerPrefs.GetString(PlayerPrefsConst.token);
        //string Url = "http://192.168.11.14/api/user/login";
        string Url = m_Url + "?token=" + token;
        using (UnityWebRequest www = UnityWebRequest.Post(Url, _form))
        {
            www.timeout = 10;
            yield return www.SendWebRequest();

            UIManager.Instance.PopPanel();
            if (www.isNetworkError)
            {
                Debug.LogError(string.Format("地址:{0}异常", m_Url)  + www.error);

                if (www.error == "Request timeout")
                {
                    // 提示超时
                  //  GameStart.OpenCommonConfirm("请求失败", "请求超时，请重试..", null, null);
                }
                else
                {
                 
                }
                yield return www.error;
            }
            else
            {
                Debug.Log("请求完成返回的是？" + www.responseCode);
                if (www.responseCode == 200)//200表示接受成功
                {
                    _callback(www.downloadHandler.text);
                    Debug.Log(www.downloadHandler.text);
                  
                }
                else
                {
                    _callback(www.downloadHandler.text);
                    Debug.Log("code :" + www.responseCode + "    " + www.downloadHandler.text);
                   // GameStart.OpenCommonConfirm("请求失败", "请求失败,请重新尝试", null, null);
                }
            }

        }
    }



    public IEnumerator SendPost( WWWForm wForm = null, Action<string> callback = null)
    {
        string url = "http://tcc.taobao.com/cc/json/mobile_tel_segment.htm";
        if (!string.IsNullOrEmpty(url))
        {
            WWW result = new WWW(url, wForm);

            yield return result;
            
            if (result.error != null)
            {
                Debug.Log("请求失败：" + result.error);
                callback("请求失败：" + result.error);
            }
            else
            {
               //;
                callback("请求成功：" + UTF8String(result.text));
                if (string.IsNullOrEmpty(result.text))
                {
                    Debug.LogError("返回值为空");
                }
                else
                {
                    Debug.Log(result.text);
                }
            }
        }
        else
        {
            Debug.LogError("URL不能为空");
        }

    }

    public string UTF8String(string input)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        return utf8.GetString(utf8.GetBytes(input));
    }

}
