using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingPanel : BasePanel {
    private Image loading;
   

  //  public GIFPlay gIFPlay;
    void Start()
    {


        
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if(loading==null)
            loading = transform.Find("LoadType/Panel/Image").GetComponent<Image>();

        print("loading=" + loading);
      //  loadingCg = loading.transform.GetComponent<CanvasGroup>();
    }

    public override void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null)
    {

    }
    public override void OnEnter()
    {


     
     
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = true;

        canvasGroup.DOFade(1, .5f);
        if (loading == null)
            loading = transform.Find("LoadType/Panel/Image").GetComponent<Image>();

       //if (loading != null)
            //loading.transform.DORotate(Vector3.right, 10000);

        //  loading.DOLOCAL
        // loadingCg.do
    }

    private void Update()
    {
        loading.transform.Rotate(Vector3.back*Time.deltaTime*500);
    }
    public override void OnExit()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, .5f);

       
    }

    public void OnClosePanel()
    {
        UIManager.Instance.PopPanel();
    }

}
