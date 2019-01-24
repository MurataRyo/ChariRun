using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreText : TextScript
{
    const int num = 2;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = TextCreate();
    }


    string TextCreate()
    {
        string m = Camera.main.transform.position.x.ToString();
        return MinText(m) + "メートル";
    }

    string MinText(string str)
    {
        string[] str2 = str.Split(char.Parse("."));
        
        if(str2.Length == 2)
        {
            int i = Mathf.Clamp(str2[1].Length - 1,0,num);
            string str3 = "";
            
            for(int j = 2;j > i;j--)
            {
                str3 += "0";
            }
            str2[1] = str2[1].Remove(i);

            return str2[0] + "." + str2[1] + str3;
        }
        else
        {
            return str + ".00";
        }
    }
}
