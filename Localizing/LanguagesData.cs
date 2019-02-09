using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Location
{
    English,
    Korean,
    Japanese
}
public class LanguagesData
{

    public static LanguagesData _Instance;
    public static LanguagesData Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new LanguagesData();
            }
            return _Instance;
        }
    }

    private LanguagesData()
    {
        //GetLanData();
        LoadExcelData();
    }

    // 엑셀 파일 담을 곳
    List<Dictionary<string, object>> dataList;
    // <키, <지역, text>>
    Dictionary<string, Dictionary<Location, string>> dic;
    void LoadExcelData()
    {
        // csv파일 읽어오기
        dataList = CSVReader.Read("LanguageData");

        // 선언
        dic = new Dictionary<string, Dictionary<Location, string>>();
        string key;
        for (int i = 0; i < dataList.Count; i++)
        {
            // 키 = 데이터의 첫번째 세로 줄
            key = dataList[i]["Key"] as string;
            // 저장 <키, <,>>
            dic.Add(key, new Dictionary<Location, string>());
            // 키의 길이만큼 반복(세로)
            // 0번째 값은 Key이기 때문에 1부터 시작
            for (int j = 1; j < dataList[i].Count; j++)
            {
                // 저장 <키, <지역, text>>
                // 지역: English가 첫번째이기 때문에 + j를 더하고 -1을 한다
                //       또한 연산이 들어가므로 int로 변환시킨 후 다시 Location으로 변환시킨다
                // text: [Key순서][string, object]이기 때문에 먼저 로케이션의 이름을 String으로 가져온 뒤,
                //       마찬가지로 지역마다의 값을 가져온 뒤 object를 string으로 변환시킨다.
                // 
                dic[key].Add((Location)((int)Location.English + j-1), 
                    dataList[i][Enum.GetName(typeof(Location), ((int)Location.English + j-1))] as string);
            }
        }
    }

    public string GetData(string key, Location loc)
    {
        return dic[key][loc];
    }
}
