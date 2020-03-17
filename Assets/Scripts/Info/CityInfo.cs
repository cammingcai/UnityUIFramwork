using System;
[Serializable]
public class CityInfo
    {


    /// <summary>
    /// 城市
    /// </summary>
    public string city;
    /// <summary>
    /// 
    /// </summary>
    public string citykey;
    /// <summary>
    /// 省
    /// </summary>
    public string parent;
    /// <summary>
    /// 更新时间
    /// </summary>
    public string updateTime;



    public override string ToString()
    {
        return "city="+ city+"," + "citykey=" + citykey + "," + "parent=" + parent + "," + "updateTime=" + updateTime ;
    }
}

