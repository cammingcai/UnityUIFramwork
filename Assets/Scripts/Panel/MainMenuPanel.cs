using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LitJson;
using System.Collections.Generic;

public class MainMenuPanel : BasePanel
{

    public Button postBtn;
    public Button getBtn;
    public Text showText;


   
    void Start()
    {
        base.Start();

   


    }
    private void Pager(int num)
    {
        print("主菜单监听到"+num+"页");
    }
    public void OnHttpData(string name)
    {

        if (name.Equals("get"))
        {
            // StartCoroutine(HttpRequestUtils.Instance().Request_GET(TestBack, 1));
            HttpRequestUtils.Instance().HttpGet("",TestBack);
        }
        if (name.Equals("post"))
        {
            WWWForm form = new WWWForm();
            form.AddField("phone", "13560048370");
            form.AddField("password", "123456");
            // HttpRequestUtils.Instance().HttpPostTest( form, PostBack);
            HttpRequestUtils.Instance().HttpPostTest(form,PostBack);
        }
    
    }

    public override void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null)
    {
        print("MainMenuPanel 接收到消息");
        print("par1="+ par1 as string);
    }

    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;//当弹出新的面板的时候，让主菜单面板 不再和鼠标交互
    }
    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void OnSendMsgBtn(string msg)
    {

        
        WebSocketInfo info = new WebSocketInfo();
        info.active = "user.login";
        info.data = new Dictionary<string, string>();
        info.data.Add("phone","13560048370");
        info.data.Add("password", "111111");
        //测试数据  测试登录
        if (WebSocketPanel.Instance.ConnectSocket())
            WebSocketPanel.Instance.SendMessage(JsonMapper.ToJson(info), ReceiverMsg);
    }

    private void ReceiverMsg(string msg)
    {
        Debug.LogError("主面板接收到消息="+msg);
    }

    public void OnPushPanel(string str)
    {

        //Debug.LogError("panelTypeString:" + panelTypeString);
        if (str.IsNullOrEmpty() || str.Equals(""))
            return;
       
        UIPanelType panelType = (UIPanelType)System.Enum.Parse(typeof(UIPanelType), str);
        UIManager.Instance.PushPanel(panelType);
    }
    private void PostBack(string backInfo)
    {
        Debug.Log("backInfo=="+ backInfo);
        showText.text = backInfo;
       // Debug.LogError("message = " + city.message);
    }
    private void TestBack(string backInfo)
    {
       // showText.text = backInfo;

        CityRoot city = JsonUtility.FromJson<CityRoot>(backInfo);
        Debug.LogError("message = " + city.message);
        Debug.LogError("city = " + city);
        Debug.LogError("cityInfo = " + city.cityInfo);
        Debug.LogError("cityInfo name = " + city.cityInfo.city);
        showText.text = city.cityInfo.ToString();
        // Debug.LogError("city name = "+ city.cityInfo.city);
        //  Debug.LogError("backInfo:" + backInfo);

    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
