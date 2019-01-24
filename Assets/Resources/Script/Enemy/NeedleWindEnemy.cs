using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NeedleWindEnemy : NeedleEnemy
{
    float defaultHeight;
    const float WIND_RANGE = 4f;
    bool upFlag;
    const float UP_SPEED = 1f;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        exp = 8;
        //0か1を入れる
        upFlag = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        defaultHeight = transform.position.y;
        transform.position += new Vector3(0f, UnityEngine.Random.Range(0f, WIND_RANGE), 0f);
        StartCoroutine(enumerator());
    }

    IEnumerator enumerator()
    {
        while(true)
        {
            transform.position = UpPos();
            yield return null;
        }
    }

    Vector3 UpPos()
    {
        float h = UpHeight();
        if(h == defaultHeight || 
           h == defaultHeight + WIND_RANGE)
        {
            upFlag = !upFlag;
        }

        return new Vector3(transform.position.x, h, transform.position.z);
    }

    float UpHeight()
    {
        float h = transform.position.y;
        return Mathf.MoveTowards(h, defaultHeight + (upFlag ? WIND_RANGE : 0f), UP_SPEED * Time.deltaTime);
    }
}
