using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCanvas : MonoBehaviour
{
    private int width = 0;
    private int height = 0;

    private float FractionRate_1 = 0;         // 当前屏幕的尺寸比例
    private float FractionRate_2 = 0;         // 固定定义的尺寸比例
    private float FractionRate_3 = 0;         // 当前屏幕的尺寸比例 - 固定定义的尺寸比例

    private RectTransform m_CurRectTransform = null;

    public bool ZoomInY = false;
    public bool ZoomInBottom = false;
    public float numberBottom = 0;

    public bool ZoomInScale = false;

    private void Awake()
    {
        FractionRate_2 = (float)1920 / 1080;
        
        width = Screen.width;
        height = Screen.height;
        FractionRate_1 = (float)width / height;

        // 大于0则表示当前屏幕   属于窄长屏
        // 小于0则表示当前屏幕   属于Ipa
        FractionRate_3 = FractionRate_1 - FractionRate_2;
        //Debug.LogError("FractionRate_1 : " + FractionRate_1);
        //Debug.LogError("FractionRate_2 : " + FractionRate_2);
        //Debug.Log("FractionRate_3：" + FractionRate_3);
        m_CurRectTransform = this.GetComponent<RectTransform>();
    }

    private void Start()
    {
        if (ZoomInY)
        {
            if (FractionRate_2 < FractionRate_1)
            {
                Debug.LogError("(FractionRate_2 < FractionRate_1)");
                Vector2 v2 = m_CurRectTransform.sizeDelta;
                m_CurRectTransform.sizeDelta = new Vector2(v2.x, v2.y * (1 - FractionRate_3 / 2));
            }
            else
            {
                Debug.LogError("(FractionRate_2 >= FractionRate_1)");
            }
        }
        if (ZoomInBottom)
        {
            if (FractionRate_2 < FractionRate_1)
            {
                Vector2 v2 = m_CurRectTransform.sizeDelta;
                m_CurRectTransform.sizeDelta = new Vector2(v2.x, (numberBottom * FractionRate_3));
            }
        }
        if (ZoomInScale)
        {
            if (FractionRate_2 < FractionRate_1)
            {
                this.transform.localScale = Vector3.one * (1 - FractionRate_3 / 2);
            }
        }
        
    }
}
