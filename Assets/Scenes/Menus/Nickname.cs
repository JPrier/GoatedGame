using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Nickname : MonoBehaviour

{

    public TextMeshProUGUI nickName;

    // Update is called once per frame
    void Update()
    {
        nickName.text = "Current Nickname: Player";
    }
}
