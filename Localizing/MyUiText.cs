using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyUiText : Text {

    public string UID;
    Location loc;
    //public Location loc;

    void Update()
    {
        loc = LanguageManager.nowLoc;
        this.text = LanguagesData.Instance.GetData(UID, loc);
    }
}
