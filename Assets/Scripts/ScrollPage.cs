using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollPage : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    //滑动组件
    public ScrollRect scrollRect;

    //每一页滚动区域的定位坐标
    public float[] posArr = new float[] { 0, 0.3333f, 0.666f, 1 };

    //页码索引
    public int pageIndex = 0;

    //目标位置
    public float targetPos;

    //是否拖拽
    public bool isDrag;
    

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
     
      
    }

    void Update()
    {
        if (!isDrag)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetPos, Time.deltaTime * 5);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        isDrag = false;
        /*第一种方法
         
        float curPos = scrollRect.horizontalNormalizedPosition;
        //鼠标（滑条的水平值scrollRect.horizontalNormalizedPosition）和每个页码的距离最小，就去哪一页
        //offset最小距离，先假设第一页和鼠标的距离差;
        pageIndex = 0;
        float offset = Mathf.Abs(curPos - posArr[pageIndex]);
        for (int i = 1; i < posArr.Length; i++) {
        float temp = Mathf.Abs(curPos - posArr[i]);
        if(temp < offset)
        {
        offset = temp;
        pageIndex = i;
        }
        }
        //更新目标位置
        targetPos = posArr[pageIndex];
        //单选按钮为真
        toggles[pageIndex].isOn = true;
         
        */


        //第二种方法

        float curPos = scrollRect.horizontalNormalizedPosition;
        print("curPos="+ curPos);
        //根据当前和原始的位置差，是正是负，判断向左向右
        //如果当前和原始的位置差小于一个值，就不改变当前的页码了
        //  print("curPos - targetPos="+(curPos - targetPos));
        // if (Mathf.Abs(curPos - targetPos) < 0.07f) { return; }
        //正值右滑
        if (curPos - targetPos > 0)
        {
            pageIndex = pageIndex + 1 > posArr.Length - 1 ? posArr.Length - 1 : pageIndex + 1;
            Debug.Log(pageIndex);
        }
        else
        {
            pageIndex = pageIndex - 1 < 0 ? 0 : pageIndex - 1;
        }

        targetPos = posArr[pageIndex];
       

    }
}
