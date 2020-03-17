using UnityEngine;
using System.Collections;

public class SystemPanel : BasePanel {


    void Start()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
    }


    public override void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null)
    {

    }
    public override void OnEnter()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public override void OnExit()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    //public void OnClosePanel()
    //{
    //    UIManager.Instance.PopPanel();
    //}
}
