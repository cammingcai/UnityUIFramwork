using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuxiliaryScrollRectChangeCanvas : MonoBehaviour
{

    private GridLayoutGroup gridLayoutGroup = null;

    public RectTransform Hor = null;

    private int width = 0;
    private int height = 0;

    private float FractionRate_1 = 0;         // 当前屏幕的尺寸比例
    private float FractionRate_2 = 0;         // 固定定义的尺寸比例
    private float FractionRate_3 = 0;         // 当前屏幕的尺寸比例 - 固定定义的尺寸比例

    public bool isTransverse = false;
    public bool isVertical = false;

    //public GameObject item;
    //public Transform _consten;

    private void Awake()
    {
        FractionRate_2 = (float)1334 / 750;

        gridLayoutGroup = this.GetComponent<GridLayoutGroup>();
        width = Screen.width;
        height = Screen.height;
        FractionRate_1 = (float)width / height;

        // 大于0则表示当前屏幕   属于窄长屏
        // 小于0则表示当前屏幕   属于Ipa
        FractionRate_3 = FractionRate_1 - FractionRate_2;
        //Debug.LogError("FractionRate_1 : " + FractionRate_1);
        //Debug.LogError("FractionRate_2 : " + FractionRate_2);
        //   Debug.Log("FractionRate_2：" + FractionRate_2);

        //if (isTransverse && isVertical)
        //{
        //    foreach (Transform t in transform.GetComponentInChildren<Transform>())
        //    {
        //        t.GetComponent<RectTransform>().sizeDelta = new Vector2(Math.Abs(Hor.rect.width), Math.Abs(Hor.rect.height));
        //    }
        //}
    }
    

    //void Update()
    //{
    //    //Debug.Log("  :" + GetComponent<RectTransform>().sizeDelta);
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        for(int i = 0; i < 10; i++)
    //        {
    //            Transform _item = GameObject.Instantiate(item).transform;
    //            _item.SetParent(_consten);
    //            _item.localScale = Vector3.one;
    //        }

    //        if (isTransverse && isVertical)
    //        {
    //            foreach(Transform t in transform.GetComponentInChildren<Transform>())
    //            {
    //                t.GetComponent<RectTransform>().sizeDelta = new Vector2(Math.Abs(Hor.rect.width), Math.Abs(Hor.rect.height));
    //            }
    //        }
    //        _consten.GetComponent<AuxiliaryScrollRect_1>().enabled = true;
    //      //  test1.OnUpdate();
    //    }
    //}
}
