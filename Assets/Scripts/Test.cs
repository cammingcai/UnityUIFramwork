using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Test : MonoBehaviour, IEndDragHandler,IBeginDragHandler
{
    //目标位置
    public float targetPos;
    private ScrollRect scrollRect;
    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
       //  print("Updatex=" + scrollRect.horizontalNormalizedPosition);
    }
    float curBeginPos;
    public void OnBeginDrag(PointerEventData eventData)
    {
        curBeginPos = scrollRect.horizontalNormalizedPosition;
      //  print(" BeginDrag Pos=" + curPos);
       // Debug.LogError("BeginDrag Pos=" + curPos);
    }

    //拖拽结束的回调
    public void OnEndDrag(PointerEventData eventData)
    {
        float curPos = scrollRect.horizontalNormalizedPosition;
       
       // print("EndDragPos=" + curPos);
        Debug.LogError(" curPos - curBeginPos=" + (curPos - curBeginPos));
        //float offet = curPos - targetPos;
        //print("targetPos=" + targetPos);
        //targetPos = curPos;
  
        //print("滑动的差值offet=" + offet);

    }

    public void OnValue(Vector2 value)
    {
       // print("value X=" + value.x);
    }
}
