using UnityEngine;
using System.Collections;

public class GameRoot : MonoSingleton<GameRoot> {

    private string json;
    /// <summary>
    /// 重写  创建单例
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }
        void Start () {

        //TextAsset jsonAsset =  Resources.Load<TextAsset>("UIPanelType");

        //  print("json="+jsonAsset.text);
          UIManager.Instance.PushPanel(UIPanelType.MainMenu);


//    #if UNITY_ANDROID
//            print("UNITY_ANDROID");
//#endif
//#if UNITY_IOS
//        print("UNITY_IOS");
//#endif
//#if UNITY_WEBPLAYER
//        print("UNITY_WEBPLAYER");
//#endif
//#if UNITY_STANDALONE_WIN
//        print("UNITY_STANDALONE_WIN");
//#endif
    }

    public void FromAndroid(string msg)
    {
        ///javaObject.Call("showTips", "收到Android发来的消息" + msg);

       // Camming cam = (Camming)UIManager.Instance.GetPanel(UIPanelType.Camming);
       // cam.FromAndroid111(msg);
    }
}
