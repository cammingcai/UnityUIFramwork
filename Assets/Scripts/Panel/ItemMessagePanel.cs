using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ItemMessagePanel : BasePanel {
   

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

        transform.localScale = Vector3.zero;
        transform.DOScale(1, .5f);
    }

    public override void OnExit()
    {
        //canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        transform.DOScale(0, .5f).OnComplete(() => canvasGroup.alpha = 0);
    }

    //public override void OnClosePanel()
    //{
    //    UIManager.Instance.PopPanel();
    //}
}
