using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Text;
using System.Security.Cryptography;
using System.Linq;

public class GameTool
{
    private static AudioClip LotusPond = null;

    ///// <summary>
    ///// 播放MP3
    ///// </summary>
    ///// <param name="clip"></param>
    //public static void PlayMp3()
    //{
    //    if (LotusPond == null)
    //        LotusPond = (AudioClip)Resources.Load("LotusPond", typeof(AudioClip));

    //    TtsDemo.Instance.OnClickSynthesisButton(LotusPond);
    //}

    public static T OnFromJson<T>(string jsonStr)
    {
        T t = JsonUtility.FromJson<T>(jsonStr);
        return t;
    }

    //public static void PlayUIAnimation(int code, DOTweenAnimation dOTween, Action action = null)
    //{
    //    if (code == 0)
    //    {
    //        //Debug.Log(" ----------------- ZHENGXIANG动画 ----------------- ");
    //        dOTween.DOPlayForward();
    //        dOTween.tween.OnComplete(() =>
    //        {
    //           // dOTween.tween.OnStepComplete = null;
    //            if (action != null)
    //                action.Invoke();
    //        });
    //        //dOTween.tween.OnComplete = () =>
    //        //{
    //        //    dOTween.tween.onStepComplete = null;
    //        //    if (action != null)
    //        //        action.Invoke();
    //        //};
    //    }
    //    else
    //    {
    //        //Debug.Log(" ----------------- 反响动画 ----------------- ");
    //        dOTween.DOPlayBackwards();
    //        dOTween.tween.OnStepComplete(() => {
    //          //  dOTween.tween.onStepComplete = null;
    //            if (action != null)
    //                action.Invoke();
    //        });
    //    }
    //}


    /// <summary>
    /// 删除物体下所有的子物体
    /// </summary>
    /// <param name="parent"></param>
    public static void ClearSubObject(Transform parent, string other_A = null, string other_B = null, string other_C = null, string other_D = null, string other_E = null)
    {
        if (parent != null)
        {
            foreach (Transform t in parent.GetComponentInChildren<Transform>())
            {
                if (t.name != parent.name && t.name != other_A && t.name != other_B && t.name != other_C && t.name != other_D && t.name != other_E)
                {
                    GameObject.Destroy(t.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// 隐藏物体下所有的子物体
    /// </summary>
    /// <param name="parent"></param>
    public static void HideSubObject(Transform parent)
    {
        if (parent != null)
        {
            foreach (Transform t in parent.GetComponentInChildren<Transform>())
            {
                if (t.name != parent.name)
                {
                    t.gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// 从字符串前面删除指定字符个数
    /// </summary>
    /// <param name="s">字符串</param>
    /// <param name="len">个数</param>
    /// <returns>返回删除后的字符串</returns>
    public static string RemoveLeft(string s, int len)
    {
        return s.PadLeft(len).Remove(0, len);
    }

    /// <summary>
    /// 从字符串后面删除指定字符个数
    /// </summary>
    /// <param name="s">字符串</param>
    /// <param name="len">个数</param>
    /// <returns>返回删除后的字符串</returns>
    public static string RemoveRight(string s, int len)
    {
        s = s.PadRight(len);
        return s.Remove(s.Length - len, len);
    }



    //半径随机 ，弧度随机
    public static Vector2 GetCirclePoint(System.Random random, int m_Radius)
    {
        //随机获取弧度
        float radin = (float)GetRandomValue(random, 0, 2 * Mathf.PI);
        float x = m_Radius * Mathf.Cos(radin);
        float y = m_Radius * Mathf.Sin(radin);
        Vector2 endPoint = new Vector2(x, y);
        Debug.Log("endPoint:" + endPoint);
        return endPoint;
    }
    private static double GetRandomValue(System.Random random, double min, double max)
    {
        double v = random.NextDouble() * (max - min) + min;
        return v;
    }

    /// <summary>
    /// 网络可达性
    /// </summary> 
    /// <returns></returns>
    public static NetworkReachability IsNetworkReachability()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                //Debug.Log("当前使用的是：WiFi，请放心更新！");
                return NetworkReachability.ReachableViaLocalAreaNetwork;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                //Debug.Log("当前使用的是移动网络，是否继续更新？");
                return NetworkReachability.ReachableViaCarrierDataNetwork;
            default:
                Debug.Log("当前没有联网，请您先联网后再进行操作！");
                return NetworkReachability.NotReachable;
        }
    }

    /// <summary>  
    /// 时间戳转为C#格式时间  
    /// </summary>  
    /// <param name="timeStamp">Unix时间戳格式</param>  
    /// <returns>C#格式时间</returns>  
    public static DateTime GetTime(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
    /// <summary>  
    /// 时间戳转为周
    /// </summary>  
    public static int GetWeekId(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        return (int)dtStart.DayOfWeek;
    }

    /// <summary>  
    /// DateTime时间格式转换为Unix时间戳格式  
    /// </summary>  
    /// <param name="time"> DateTime时间格式</param>  
    /// <returns>Unix时间戳格式</returns>  
    public static int ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }
    /// <summary> 
    /// 获取时间戳 
    /// </summary> 
    /// <returns></returns> 
    public static string GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    ///// <summary>
    ///// 刷新UI组建
    ///// </summary>
    //private void updateLayoutUI()
    //{
    //    StartCoroutine(UIManager.Instance.UpdateLayout(OrderContent.GetComponent<RectTransform>()));
    //}

    /// <summary>
    /// Base64加密，采用utf8编码方式加密
    /// </summary>
    /// <param name="source">待加密的明文</param>
    /// <returns>加密后的字符串</returns>
    public static string Base64Encode(string source)
    {
        return Base64Encode(Encoding.UTF8, source);
    }

    /// <summary>
    /// Base64加密
    /// </summary>
    /// <param name="encodeType">加密采用的编码方式</param>
    /// <param name="source">待加密的明文</param>
    /// <returns></returns>
    public static string Base64Encode(Encoding encodeType, string source)
    {
        string encode = string.Empty;
        byte[] bytes = encodeType.GetBytes(source);
        try
        {
            encode = Convert.ToBase64String(bytes);
        }
        catch
        {
            encode = source;
        }
        return encode;
    }

    /// <summary>
    /// Base64解密，采用utf8编码方式解密
    /// </summary>
    /// <param name="result">待解密的密文</param>
    /// <returns>解密后的字符串</returns>
    public static string Base64Decode(string result)
    {
        return Base64Decode(Encoding.UTF8, result);
    }

    /// <summary>
    /// Base64解密
    /// </summary>
    /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
    /// <param name="result">待解密的密文</param>
    /// <returns>解密后的字符串</returns>
    public static string Base64Decode(Encoding encodeType, string result)
    {
        string decode = string.Empty;
        byte[] bytes = Convert.FromBase64String(result);
        try
        {
            decode = encodeType.GetString(bytes);
        }
        catch
        {
            decode = result;
        }
        return decode;
    }


    /// MD5　32位加密(大写)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static string UserMd5(string str)
    {
        string cl = str;
        string pwd = "";
        MD5 md5 = MD5.Create();//实例化一个md5对像
                               // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
        // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
        for (int i = 0; i < s.Length; i++)
        {
            // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符

            pwd = pwd + s[i].ToString("X");

        }
        return pwd;
    }
    /// <summary>
    /// MD5 16位加密 加密后密码为小写
    /// </summary>
    /// <param name="ConvertString"></param>
    /// <returns></returns>
    public static string GetMd5str(string ConvertString)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
        t2 = t2.Replace("-", "");
        t2 = t2.ToLower();
        return t2;
    }
    /// <summary>
    /// MD5 16位加密 加密后密码为大写
    /// </summary>
    /// <param name="ConvertString"></param>
    /// <returns></returns>
    public static string GetMd5Str(string ConvertString)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)), 4, 8);
        t2 = t2.Replace("-", "");
        return t2;
    }
    /// <summary>
    /// MD5加密(小写)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Md5Hash(string input)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }
    /// <summary>
    /// 计时
    /// </summary>
    /// <param name="miao"></param>
    public static string TimerToTextStr(int miao)
    {
        //Debug.Log("----------------------------------- miao :" + miao);
        if (miao < 60)
        {
            if (miao < 10)
            {
                return "00:0" + miao;
            }
            else
            {
                return "00:" + miao;
            }
        }
        else
        {
            int fen = miao / 60;
            int _miao = miao % 60;

            if (fen < 10)
            {
                if (_miao < 10)
                {
                    return "0" + fen + ":0" + _miao;
                }
                else
                {
                    return "0" + fen + ":" + _miao;
                }
            }
            else
            {
                if (_miao < 10)
                {
                    return fen + ":0" + _miao;
                }
                else
                {
                    return fen + ":" + _miao;
                }
            }
        }
    }

    /// <summary>
    /// 比较两个数组是否完全一致
    /// </summary>
    /// <param name="arr1">数组1</param>
    /// <param name="arr2">数组2</param>
    /// <returns>相同返回：true 不同返回：false</returns>
    public static bool CompareArrs(string[] arr1, string[] arr2)
    {
        try
        {
            if (arr1.Length != arr2.Length)
                return false; //数组数量不一致  无需比较
            List<string> arrList1 = arr1.ToList();
            List<string> arrList2 = arr2.ToList();
            for (int i = arrList1.Count - 1; i >= 0; i--)
            {
                for (int j = arrList2.Count - 1; j >= 0; j--)
                {
                    if (!string.Equals(arrList1[i], arrList2[j], StringComparison.OrdinalIgnoreCase))
                    {
                        //如果有一致的删除arrList1中一致的
                        arrList1.Remove(arrList1[i]);
                        break;
                    }
                }
            }
            if (arrList1.Count > 0) //arrList1中含有没有删除完的则证明不一致
                return false;
            else
                return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }



    /// <summary>
    /// 把秒转 00:00:00
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string TransTimeSecondIntToString(long second)
    {
        string str = "";
        long hour = second / 3600;
        long min = second % 3600 / 60;
        long sec = second % 60;
        if (hour < 10)
        {
            str += "0" + hour.ToString();
        }
        else
        {
            str += hour.ToString();
        }
        str += ":";
        if (min < 10)
        {
            str += "0" + min.ToString();
        }
        else
        {
            str += min.ToString();
        }
        str += ":";
        if (sec < 10)
        {
            str += "0" + sec.ToString();
        }
        else
        {
            str += sec.ToString();
        }
        return str;
    }

    /// <summary>
    /// 创建随机数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int OnCreateRandom(int max,int min = 0)
    {
        System.Random rnd = new System.Random(); //在外面生成对象
        return rnd.Next(min, max);
    }
}
