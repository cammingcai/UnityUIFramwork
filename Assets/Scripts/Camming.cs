using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 测试Android和unity的交互
/// </summary>
public class Camming : MonoBehaviour
{

    private const string androidPackageName = "com.tfw.unity.unitytestlibrary.AndroidToUnity";

    private AndroidJavaObject javaObject;
    public  Text tipTxt;
    public Text receiverTxt;

  //  public static Action<string> showTxtAction;

    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_ANDROID
             print("UNITY_ANDROID");
            javaObject = new AndroidJavaObject(androidPackageName);
        #endif

    }

  

    public void OnBtnClick(string msg)
    {
        print("msg="+msg);
#if UNITY_ANDROID
        javaObject.Call("showTips", "你好！Android！我是unity，我给你发消息啦"+ msg);
#endif
        receiverTxt.text = "你好！Android！我是unity，我给你发消息啦" + msg;
    }
    public void FromAndroid(string msg)
    {
       ///javaObject.Call("showTips", "收到Android发来的消息" + msg);
     
        tipTxt.text = "收到Android发来的消息"+msg;
    }
    
    public  void FromAndroid111(string msg)
    {
        ///javaObject.Call("showTips", "收到Android发来的消息" + msg);

        tipTxt.text = "收到Android发来的消息" + msg;
    }
// Update is called once per frame
void Update()
    {
        
    }
  
}
