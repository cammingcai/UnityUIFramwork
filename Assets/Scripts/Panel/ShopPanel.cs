using UnityEngine;
using System.Collections;

public class ShopPanel : BasePanel {
    

    void Start()
    {
        base.Start();
       // if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
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
    //    print("ShopPanel OnClosePanel");
    //    // UIManager.Instance.PopPanel();
    //    //UIManager.Instance.ClosePanel(UIPanelType.MainMenu);

    //}

}
