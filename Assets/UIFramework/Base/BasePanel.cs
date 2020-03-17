using UnityEngine;
using System.Collections;
using DG.Tweening;

public abstract class BasePanel : MonoBehaviour {
    protected CanvasGroup canvasGroup;

    protected void Start()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }
    /// <summary>
    /// 接收消息
    /// </summary>
    /// <param name="type"> 消息类型</param>
    /// <param name="par1"> 第一个参数</param>
    /// <param name="par2"> 第二个参数</param>
    public abstract void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null);
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="type"> 消息类型</param>
    /// <param name="par1"> 第一个参数</param>
    /// <param name="par2"> 第二个参数</param>
    public void SendMessage(UIPanelType type, object par1 = null, object par2 = null) {
        BasePanel panel = UIManager.Instance.GetPanel(type);
        if (panel != null)
            panel.ReceiveMessage(type, par1, par2);
        else
            throw new System.Exception("Not found Current Pannel");

    }
    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = true;

            canvasGroup.DOFade(1, .5f);
        }
    }
    /// <summary>
    /// 处理页面的 关闭
    /// </summary>
    public virtual void OnExit()
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;

            canvasGroup.DOFade(0, .5f);
        }


    }
    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause()
    {

    }

  



    /// <summary>
    /// 界面继续
    /// </summary>
    public virtual void OnResume()
    {

    }

    //public abstract void OnClosePanel();

    public virtual void OnCloseTopPanel()
    {
        UIManager.Instance.PopPanel();
    }

}
