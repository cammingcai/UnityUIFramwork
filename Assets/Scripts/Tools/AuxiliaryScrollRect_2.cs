using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AuxiliaryScrollRect_2 : MonoBehaviour
{
    public float f;
    public CqTweenVector3 ctv;

    public void OnDrag(Vector2 delta)
    {
        ctv.Stop();
        transform.localPosition += Vector3.up * delta.y;
    }
    public void EndDragTweenTo()
    {
        ctv.Stop();
        if (transform.localPosition.y < ctv.mStart.y)
        {
            ctv.Mode = UnityCore.TweenMode.ToStart;
            ctv.Play();
        }
        else if (transform.localPosition.y > ctv.mEnd.y)
        {
            ctv.Mode = UnityCore.TweenMode.ToEnd;
            ctv.Play();
        }
    }

    public void ToTop()
    {
        ctv.SetCurrentByStart();
    }

    public void OnUpdateEnd()
    {
        StartCoroutine(DelayedRefresh());
    }

    // 延迟刷新
    private IEnumerator DelayedRefresh()
    {
        yield return new WaitForSeconds(0.5f);
  
        float f = Math.Abs(this.transform.parent.parent.GetComponent<AuxiliaryScrollRectChangeCanvas>().Hor.rect.height);
        float _y = Math.Abs(this.GetComponent<RectTransform>().rect.height) - f;

        if (_y <= 0)
        {
            _y = 0;
            //Debug.LogError("this.GetComponent<RectTransform>().sizeDelta.y:" + this.GetComponent<RectTransform>().sizeDelta.y);
            //Debug.LogError("ctv.mEnd:" + ctv.mEnd);
        }
        Vector3 v3 = ctv.mEnd;
        ctv.mEnd = new Vector3(v3.x, _y, v3.z);
    }
}
