using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Sword : Sword
{
    // Update is called once per frame
    public override void Start()
    {
        base.Start();
        GameObject go = Instantiate(Resources.Load<GameObject>(Player.BULLET_PATH));
        go.transform.position = transform.parent.transform.position + new Vector3(1f, 0.5f, -1f);
    }
}
