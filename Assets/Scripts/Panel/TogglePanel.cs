using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class TogglePanel : BasePanel {

   
    private Transform toggleTransform;
    private Toggle[] toggles;

    void Start()
    {
        base.Start();

        toggleTransform = transform.Find("ToggelPanel/Togglebg");
        print("toggleTransform=" + toggleTransform);

        toggles = toggleTransform.GetComponentsInChildren<Toggle>();

        foreach (Toggle tg in toggles)
        {
            tg.onValueChanged.AddListener(
                    (bool value) => {

                        print("value=" + value);
                        ToggleTest(value, tg);
                    }
                );

           
        }
    }


    public override void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null)
    {

    }
    private void ToggleTest(bool value, Toggle tl)
    {
        if (value)
        {

            print("  tl.transform.name=" + tl.transform.name);
        }

    }
    public override void OnEnter()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = true;

        canvasGroup.DOFade(1, .5f);
    }

    /// <summary>
    /// 处理页面的 关闭
    /// </summary>
    public override void OnExit()
    {
        canvasGroup.blocksRaycasts = false;

        canvasGroup.DOFade(0, .5f);

   
    }

    //public void OnClosePanel()
    //{
    //    UIManager.Instance.PopPanel();
    //}

 
}
