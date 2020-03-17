using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ViewPagerPanel : BasePanel{

    void Start()
    {
        base.Start();

    }

    
    
    public override void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null)
    {

    }
   

    private void Update()
    {
     
       
    }



    //public void OnClosePanel()
    //{
    //    Debug.Log("----------OnClosePanel=" );
    //    UIManager.Instance.PopPanel();
    //    //UIManager.Instance.CloseAllPanel();
    //}
}
