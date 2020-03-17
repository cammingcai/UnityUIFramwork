using System;
using UnityEngine;

public class PayAndroid : MonoSingleton<PayAndroid>
{
    private const string payPackageName = "pay.dora.gz.com.pay.PayUnityUtils";
    private AndroidJavaObject payObject;

    public Action<string> payAction;
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID
        print("UNITY_ANDROID");
        payObject = new AndroidJavaObject(payPackageName);
        #endif
    }
    public const string WECHAT = "WECHAT";
    public const string ALI = "ALI";
    //支付类型 支付类型分别是：ALI、WECHAT
    public void StartPay(string payType,string payJson,Action<string> callback=null)
    {
        this.payAction = callback;
    #if UNITY_ANDROID

        if (payObject == null)
        {
            throw new System.Exception("没有Android 支付对象");
            return;
        }
        payObject.Call("payDoraVip", payType, payJson);
        
        #endif
    }


    /// <summary>
    ///  支付回调
    /// </summary>
    public void payStatus(string args)
    {
        Debug.Log("  ------------- 支付回调args ---------------------- ：" + args);
        if (payAction != null)
        {
            payAction(args);

        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
