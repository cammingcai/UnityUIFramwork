using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIManager {

    /// 
    /// 单例模式的核心
    /// 1，定义一个静态的对象 在外界访问 在内部构造
    /// 2，构造方法私有化

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIManager();
            }
            return _instance;
        }
    }
    //public static UIManager Instance1()
    //{

    //     if (_instance == null)
    //            _instance = new UIManager();

    //        return _instance;

    //}

    // 根节点 Canvas
  //  public Canvas m_UiRootCanvas;
    private Transform canvasTransform;
    public Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canavs").transform;
            }
            return canvasTransform;
        }
    }
    private Dictionary<UIPanelType, string> panelPathDict;//存储所有面板Prefab的路径
    private Dictionary<UIPanelType, BasePanel> panelDict;//保存所有实例化面板的游戏物体身上的BasePanel组件
    private Stack<BasePanel> panelStack;

    private UIManager()
    {
        //m_UiRootCanvas = GameObject.Find("Canvas");
        ParseUIPanelTypeJson();
    }


    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="type"> 消息类型</param>
    /// <param name="par1"> 第一个参数</param>
    /// <param name="par2"> 第二个参数</param>
    public void SendMessage(UIPanelType type, object par1 = null, object par2 = null)
    {
        BasePanel panel = UIManager.Instance.GetPanel(type);
        Debug.Log("message panel="+ panel);
        if (panel != null)
            panel.ReceiveMessage(type, par1, par2);
        else
            throw new System.Exception("Not found Current Pannel");

    }


    /// <summary>
    /// 把某个页面入栈，  把某个页面显示在界面上
    /// </summary>
    public void PushPanel(UIPanelType panelType)
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        //判断一下栈里面是否有页面 如果有页面将顶部的页面暂停
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();
            topPanel.OnPause();
        }

        BasePanel panel = GetPanel(panelType);
        panel.OnEnter();
        panelStack.Push(panel);
    }
    /// <summary>
    /// 出栈 ，把页面从界面上移除
    /// </summary>
    public void PopPanel()
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        if (panelStack.Count <= 0) return;

        //关闭栈顶页面的显示
        BasePanel topPanel = panelStack.Pop();
        topPanel.OnExit();

        if (panelStack.Count <= 0) return;
        BasePanel topPanel2 = panelStack.Peek();
        topPanel2.OnResume();

    }

    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAllPanel()
    {
        if (panelStack == null)
            return;

        do
        {
            //从栈中取出并移除
            BasePanel topPanel =  panelStack.Pop();
            topPanel.OnExit();

        } while (panelStack.Count > 0);
    }

    //public void ClosePanel(UIPanelType panelType)
    //{
    //    if (panelStack == null) return;

    //    if (panelStack.Count <= 0) return;

    //    //将需要关闭的panel放在顶部
    //    PushPanel(panelType);
    //    //关闭栈顶页面的显示
    //    BasePanel topPanel = panelStack.Pop();
    //    topPanel.OnExit();

    //    if (panelStack.Count <= 0) return;
    //    BasePanel topPanel2 = panelStack.Peek();
    //    topPanel2.OnResume();

    //}

    /// <summary>
    /// 根据面板类型 得到实例化的面板
    /// </summary>
    /// <returns></returns>
    public BasePanel GetPanel(UIPanelType panelType)
    {
        if (panelDict == null)
        {
            panelDict = new Dictionary<UIPanelType, BasePanel>();
        }

        //BasePanel panel;
        //panelDict.TryGetValue(panelType, out panel);//TODO

        BasePanel panel = panelDict.TryGet(panelType);

        if (panel == null)
        {
            //如果找不到，那么就找这个面板的prefab的路径，然后去根据prefab去实例化面板
            //string path;
            //panelPathDict.TryGetValue(panelType, out path);
            string path = panelPathDict.TryGet(panelType);
            Debug.LogError("path="+ path);
            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            Debug.LogError("instPanel=" + instPanel);
            instPanel.transform.SetParent(CanvasTransform,false);
            panelDict.Add(panelType, instPanel.GetComponent<BasePanel>());
            return instPanel.GetComponent<BasePanel>();
        }
        else
        {
            return panel;
        }

    }

    [Serializable]
    class UIPanelTypeJson
    {
        public List<UIPanelInfo> infoList;
    }
    private void ParseUIPanelTypeJson()
    {
        panelPathDict = new Dictionary<UIPanelType, string>();

        TextAsset ta = Resources.Load<TextAsset>("UIPanelType");

        UIPanelTypeJson jsonObject = JsonUtility.FromJson<UIPanelTypeJson>(ta.text);

        foreach (UIPanelInfo info in jsonObject.infoList) 
        {
            //Debug.Log(info.panelType);
            panelPathDict.Add(info.panelType, info.path);
        }
    }

    /// <summary>
    /// just for test
    /// </summary>
    public void Test()
    {
        string path ;
        panelPathDict.TryGetValue(UIPanelType.Knapsack,out path);
        Debug.Log(path);
    }


}
