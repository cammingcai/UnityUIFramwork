using System;
using System.Collections.Generic;

[Serializable]
public  class CityData
    {

    /// <summary>
    /// 
    /// </summary>
    public string shidu;
    /// <summary>
    /// 
    /// </summary>
    public int pm25;
    /// <summary>
    /// 
    /// </summary>
    public int pm10;
    /// <summary>
    /// 轻度污染
    /// </summary>
    public string quality;
    /// <summary>
    /// 
    /// </summary>
    public string wendu;
    /// <summary>
    /// 儿童、老年人及心脏、呼吸系统疾病患者人群应减少长时间或高强度户外锻炼
    /// </summary>
    public string ganmao;
    /// <summary>
    /// 
    /// </summary>
    public List<Forecast> forecast;
    /// <summary>
    /// 
    /// </summary>
    public Yesterday yesterday;
    
}
