using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    // 특정 문자 패턴을 찾는 클래스 Regex
    // 특수문자를 활용하여 특정 String을 검색
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };
	
    // 파일을 받아와서 읽는 함수
	public static List<Dictionary<string, object>> Read(string file)
	{
        // 스트링, 오브젝트 형식으로 딕셔너리 리스트를 만든다
        var list = new List<Dictionary<string, object>>();
        // 파일을 텍스트로 읽어들인다
		TextAsset data = Resources.Load (file) as TextAsset;
	    
        // 데이터의 텍스트를 LIne Split에서 설정한 것으로 나눈다
		var lines = Regex.Split (data.text, LINE_SPLIT_RE);
		
        // 라인이 있따면 list를 return한다
		if(lines.Length <= 1) return list;

        // 헤더 = SPLIT_RE에서 설정한 것으로 첫번째 줄(엑셀의 1번 줄)을 나누고 헤더로 설정한다
        var header = Regex.Split(lines[0], SPLIT_RE);
        // 라인의 길이만큼 반복
		for(var i=1; i < lines.Length; i++) {

            // 라인을 쉼표로 구분
            var values = Regex.Split(lines[i], SPLIT_RE);
            // 만약 라인에 아무것도 없거나, 첫번째 라인이 아무것도 없다면 계속해라
			if(values.Length == 0 ||values[0] == "") continue;
			
            // <문자열, 오브젝트>가 저장된 딕셔너리
			var entry = new Dictionary<string, object>();
            // 헤더의 길이만큼, 그리고 라인의 길이만큼 반복
			for(var j=0; j < header.Length && j < values.Length; j++ ) {
                // value에 각 라인을 담고
				string value = values[j];
                // 값을 공백으로 변경
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                // finalvalue에 value값을 저장
				object finalvalue = value;
				int n;
				float f;
                // 만약 int라면
                // 받은 value를 인트값으로 변경해서 n에 저장
				if(int.TryParse(value, out n)) {
					finalvalue = n;
				}
                // 만약 float이라면
                // 받은 float값을 플롯값으로 변경해서 f에 저장
                else if (float.TryParse(value, out f)) {
					finalvalue = f;
				}
                // 헤더별로 분류
				entry[header[j]] = finalvalue;
			}
            // 리스트에 추가
			list.Add (entry);
		}
        // 최종적으로 완성된 리스트를 리턴
		return list;
	}
}
