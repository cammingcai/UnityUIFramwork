using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ViewPagerPanel1 : BasePanel,IBeginDragHandler,IEndDragHandler {

    // 题目父组件
    //public Transform TestPaperContent;
    private Transform contentVp;

    private ScrollRect vpScroll;
  
    public string name;
    //总的页数
    private int itemNum = 20;
    //存储每一页的位置
    private float[] pagerArray;
    //目标位置
    public float targetPos = 0;
    //当前位置
    public float curentPos = 0;
    //当前页数
    private int currentPostion = 0;
    //晃动的速度
    private float scrollSmoothingSpeed = 10;
    //是否是滚动结束了
    private bool isScrollEnd = true;
    // public AuxiliaryScrollRect_1 vpScrollRect = null;
    private RectTransform sizeTransform;
    private RectTransform sizeCanavsTransform;

    void Start()
    {
        contentVp = transform.Find("Viewport/Content");
        //sizeTransform = transform.Find("Viewport/ContentSize").GetComponent<RectTransform>();
        sizeTransform = transform.Find("Viewport").GetComponent<RectTransform>();
        sizeCanavsTransform = UIManager.Instance.CanvasTransform.GetComponent<RectTransform>();
        vpScroll = GetComponent<ScrollRect>();
        for (int i = 0; i < itemNum; i++)
        {

            Transform vpItem = Instantiate(transform.Find("Viewport/VpItem"));
            vpItem.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeTransform.rect.width,sizeTransform.rect.height);
            Text vpTxt = vpItem.Find("Text").GetComponent<Text>();
           // vpItem.position = Vector3.zero;
             //  vpItem.position = Vector3.one;
            vpItem.SetParent(contentVp);
            vpTxt.text = (i + 1) + "";
        }
        initPager();




        // vpScroll.onValueChanged.AddListener(OnScrollValue);




        // GoPagerToIndex(7);


    }

    

    private void initPager() {
        //页数的总数的数组
        pagerArray = new float[itemNum];
        //测试
        for (int i = 0; i < itemNum; i++)
        {

            if (i == itemNum - 1)
                pagerArray[i] = 1;
            else
                pagerArray[i] = i / (float)(itemNum - 1);

          //  print("pagerArray[" + i + "]=" + pagerArray[i]);

        }
    }

    public override void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null)
    {

    }
    public override void OnEnter()
    {
       
    }

    /// <summary>
    /// 处理页面的 关闭
    /// </summary>
    public override void OnExit()
    {
       
    }


    private void GoPagerToIndex(int index)
    {
        if (index >= 0 && index < itemNum)
        {
            targetPos = pagerArray[index];
            isScrollEnd = false;
        }
    }


    private void Update()
    {
        print("width="+ sizeTransform.rect.width+ ",height="+ sizeTransform.rect.height);
        //print("Canavs width=" + sizeCanavsTransform.rect.width + ",Canavs height=" + sizeCanavsTransform.rect.height);
        //if (!isScrollEnd) {
            
            
        //    float currenPosition = KeepDecimalNum(vpScroll.horizontalNormalizedPosition,4);
        //    float targetPostion = KeepDecimalNum(targetPos,4);
        //    if (currenPosition != targetPostion)
        //    {
          
        //        //从一个值，到另一个值的过渡
        //        vpScroll.horizontalNormalizedPosition = Mathf.Lerp(vpScroll.horizontalNormalizedPosition,
        //            targetPos, Time.deltaTime * scrollSmoothingSpeed);
        //    }
          

        //}        //print("Canavs width=" + sizeCanavsTransform.rect.width + ",Canavs height=" + sizeCanavsTransform.rect.height);
        //if (!isScrollEnd) {
            
            
        //    float currenPosition = KeepDecimalNum(vpScroll.horizontalNormalizedPosition,4);
        //    float targetPostion = KeepDecimalNum(targetPos,4);
        //    if (currenPosition != targetPostion)
        //    {
          
        //        //从一个值，到另一个值的过渡
        //        vpScroll.horizontalNormalizedPosition = Mathf.Lerp(vpScroll.horizontalNormalizedPosition,
        //            targetPos, Time.deltaTime * scrollSmoothingSpeed);
        //    }
          

        //}
       
    }

    //保留多少位小数
    private float KeepDecimalNum(float dec,int num)
    {
        int[] numArr = { 10,100,1000,10000, 100000, 1000000, 10000000, 100000000};
        int number = (int)(dec * numArr[num - 1]);
        return number / (float)numArr[num - 1];
    }
 
    public void OnBeginDrag(PointerEventData eventData)
    {
    
        isScrollEnd = true;
    }

   
    //拖拽结束的回调
    public void OnEndDrag(PointerEventData eventData)
    {
       
        isScrollEnd = false;
        curentPos = vpScroll.horizontalNormalizedPosition;
      
        float offet = curentPos - targetPos;
        print("offet=" + offet);
        //如果当前和原始的位置差小于一个值，就不改变当前的页码了
        // if (Mathf.Abs(curentPos - targetPos) < 0.07f) { return; }
        if (curentPos - targetPos > 0)//表示向右滑动
            currentPostion = currentPostion++ > itemNum - 1 ? itemNum - 1 : currentPostion++;
        else//表示向左滑动
            currentPostion = currentPostion-- <0 ? 0:currentPostion--;


        targetPos = pagerArray[currentPostion];
        //这样没有晃动效果
        /// vpScroll.horizontalNormalizedPosition = targetPosition;
    }



    public void OnClosePanel()
    {
        UIManager.Instance.PopPanel();
    }
}
