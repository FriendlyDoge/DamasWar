using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPlayerText : MonoBehaviour
{
    Text txt;


    // Use this for initialization
    void Start()
    {
        txt = gameObject.GetComponent<Text>();
        txt.text = " ";
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public void changePlayerText(string p)
    {
        txt.text = p;
    }

    public string getActualText()
    {
        return txt.text;
    }
}
