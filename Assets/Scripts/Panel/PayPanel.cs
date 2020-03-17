using System;
using UnityEngine;
using UnityEngine.UI;
public class PayPanel:BasePanel
{

    //微信 {
    //"appid": "wx9e968e821e635cd0",
    //"partnerid": "1544697161",
    //"prepayid": "wx161039251211676bdf16728a1702758200",
    //"timestamp": "1581820765",
    //"noncestr": "uj15SmyeKMfgUBCk",
    //"package": "Sign=WXPay",
    //"sign": "4B21D2D30C152561D1C5179CDC19B79F"
    //

       
    //[Serializable]
    // public  void Button wxBtn;


    public Text showTxt;

     private string payJson = "{\n" +
            "    \"appid\": \"wx9e968e821e635cd0\",\n" +
            "    \"partnerid\": \"1544697161\",\n" +
            "    \"prepayid\": \"wx16134419537960f8987435611702813200\",\n" +
            "    \"timestamp\": \"1581831859\",\n" +
            "    \"noncestr\": \"NPzPEfBCk4Y7sLtH\",\n" +
            "    \"package\": \"Sign=WXPay\",\n" +
            "    \"sign\": \"A3E41860CC78C6EF8B66086A76102887\"\n" +
            "}";
    private string payJsonAli = "app_id=2019022263273481&format=JSON&charset=utf-8&sign_type=RSA2&version=1.0&notify_url=http%3A%2F%2Fcou.tfwangs.com%3A8081%2Fapi%2Falipay%2Fverify&timestamp=2020-02-16+11%3A49%3A11&biz_content=%7B%22out_trade_no%22%3A%2220200216114911995395%22%2C%22total_amount%22%3A%220.02%22%2C%22subject%22%3A%221%5Cu4e2a%5Cu6708%22%2C%22product_code%22%3A%22QUICK_MSECURITY_PAY%22%7D&method=alipay.trade.app.pay&sign=rqD06VZKB5kaVtkTPsZ0pPpaq%2BcvJgQZugRz4nIipMVmGAuzMbl1oRbz8S9V9EFe%2BtMTcOP9cn8hYNmQdIt36q446gSztLpK4Zq5tToawMn9Y4qEqwPGPwVRpjtV9VkICWTOCDnLkLEGqPtOGJWAeWyhGpQUAFJoHLp%2BvRYRvb1S10PDgzEOig%2F%2BGI%2Bkk6q6q4v0SW8JrnmYbtmPiUixJ7GnwhcIPzlJpmCI0ZLpiVHuGZEMSB2%2BeHqWxT8syxweAscbsRi%2FOFVipM1Uhj6OW9ZvMKNAjSc2Fp49ZMdFVserpMBeg6BJkdFHdJdJaIUpZ9SSW9RKoFGHVDZDb7sxHQ%3D%3D";
  void Start()
    {
        base.Start();

        print("payJson="+ payJson);
       
    }
    public void OnClickPay(string pay)
    {
        if (pay == null) return;
        ////string url = "http://192.168.11.14/api/wxpay/createOrder";
        ////WWWForm form = new WWWForm();
        ////form.AddField("id", "2");
        ////form.AddField("study_coin", "0");
        ////form.AddField("platform", "1");
        if (pay.Equals("wechat"))
        {
           // HttpRequestUtils.Instance().HttpPost(url,form, PayBack);
           //微信支付包名需要与微信后台保持一致
            PayAndroid.Instance.StartPay(PayAndroid.WECHAT, payJson, PayCallBack);
        }
        else {
            PayInfo info = new PayInfo();
            info.code = 0;
            info.message = "支付";
            info.data = payJsonAli;
            PayAndroid.Instance.StartPay(PayAndroid.ALI, JsonUtility.ToJson(info), PayCallBack);
        }
    }

    private void PayBack(string info)
    {
        print("info="+ info);
        if (info == null) return;
        showTxt.text = info;

        PayAndroid.Instance.StartPay(PayAndroid.WECHAT,info, PayCallBack);
    }

    private void PayCallBack(string back)
    {
        print("back=" + back);
//#if UNITY_ANDROID

//#endif
    }
    public override void ReceiveMessage(UIPanelType type, object par1 = null, object par2 = null)
    {

    }

}
