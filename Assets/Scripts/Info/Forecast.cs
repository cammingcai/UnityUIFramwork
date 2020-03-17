using System;

[Serializable]
//预报
public class Forecast
    {

    /// <summary>
    /// 
    /// </summary>
    public string date;
    /// <summary>
    /// 高温 5℃
    /// </summary>
    public string high;
    /// <summary>
    /// 低温 -3℃
    /// </summary>
    public string low;
    /// <summary>
    /// 
    /// </summary>
    public DateTime ymd;
    /// <summary>
    /// 星期五
    /// </summary>
    public string week;
    /// <summary>
    /// 
    /// </summary>
    public string sunrise;
    /// <summary>
    /// 
    /// </summary>
    public string sunset;
    /// <summary>
    /// 
    /// </summary>
    public int aqi;
    /// <summary>
    /// 西风
    /// </summary>
    public string fx;
    /// <summary>
    /// <3级
    /// </summary>
    public string fl;
    /// <summary>
    /// 多云
    /// </summary>
    public string type;
    /// <summary>
    /// 阴晴之间，谨防紫外线侵扰
    /// </summary>
    public string notice;
    
}
