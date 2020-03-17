using CqCore;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityCore;

public class AuxiliaryScrollRect_1 : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Action<int> EndSlideAction;
    
    public int _index;
    public CqTweenVector3 ctv;
    AuxiliaryScrollRect_2[] children;
    float childWidth;

    [CheckBox("水平滑动")]
    public bool horizontal;
    [CheckBox("垂直滑动")]
    public bool vertical;

    // 当前滑动方向  1-左  2-右边 
    private int isDirection = 0;
    
    public void OnUpdateChildren()
    {
        children = null;
        children = GetComponentsInChildren<AuxiliaryScrollRect_2>();
        childWidth = transform.GetChild(0).GetComponent<RectTransform>().rect.width;
    }

    // 改变缓动时间
    public void OnChnageDuration(float timer)
    {
        ctv.duration = timer;
    }

    public void OnPageWinkTo(int index)
    {
        ctv.Mode = UnityCore.TweenMode.ToEnd;
        ctv.mEnd = Vector3.left * index * childWidth;
        ctv.Play();
        _index = index;

        //Debug.LogError("-----------------------OnPageWinkTo-------------------------------_index:" + _index);
        //if (EndSlideAction != null)
        //{
        //    EndSlideAction.Invoke(_index);
        //}
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.LogError("eventData.delta.x:" + eventData.delta.x);
        if (eventData.delta.x<0)
        {
            isDirection = 2;
        }
        else if(eventData.delta.x > 0)
        {
            isDirection = 1;
        }
        else
        {
            isDirection = 0;
        }
        ctv.Stop();
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            if (horizontal)
            {
                transform.localPosition += Vector3.right * eventData.delta.x;
            }
        }
        else
        {
            if (vertical)
            {
                children[_index].OnDrag(eventData.delta);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        children[_index].EndDragTweenTo();
        if (horizontal)
        {
            var posX = -transform.localPosition.x / childWidth;
            MathUtil.BetweenRange(ref posX, 0, children.Length - 1);
            if (isDirection == 1)
            {
                posX -= 0.3f;
            }
            if (isDirection == 2)
            {
                posX += 0.3f;
            }
            var targetIndex = Mathf.RoundToInt(posX);
            ctv.Mode = UnityCore.TweenMode.ToEnd;
            ctv.mEnd = Vector3.left * targetIndex * childWidth;
            //if (index != targetIndex)
            //{
            //    var t = index;
            //    ctv.PlayAndDo(() =>
            //    {
            //        children[t].ToTop();
            //    });
            //}
            //else
            {
                ctv.Play();
            }
            _index = targetIndex;
            if (EndSlideAction != null)
            {
                EndSlideAction.Invoke(_index);
            }
        }

    }

    // 退出当前
    public void OnCloseCur()
    {
        ctv.duration = 0.3f;
        _index = 0;
        children = null;
    }
}
