using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 帧动画机
/// </summary>
public class FrameAnimationPanel : BasePanel
{


    private string frameName = "FrameAnimation";

    private Image frameImg;
    private Animator frameAnimation;
    private bool isPlayAnimation = false;

    void Start()
    {
        base.Start();
        frameImg = transform.Find("FrameImage").GetComponent<Image>();
        frameAnimation = transform.Find("FrameImage").GetComponent<Animator>();


        bool state =  frameAnimation.GetBool(frameName);

        print("state="+ state);
    }



    public override void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null)
    {

    }

    public void OnStartAnimation()
    {
        if (frameAnimation != null)
        {
            isPlayAnimation = true;
            frameAnimation.SetBool("IsPlay", true);

        }
    }


    void Update()
    {
        if (frameAnimation != null && isPlayAnimation)
        {
            AnimatorStateInfo info = frameAnimation.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(frameName))
            {
                isPlayAnimation = false;
                frameAnimation.SetBool("IsPlay", false);
            }
        }
    }
   

    //public override  void OnClosePanel()
    //{
    //    UIManager.Instance.PopPanel();
    //}

    public void Test()
    { }
}
