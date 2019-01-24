using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleEnemy : Enemy
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        exp = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Hit()
    {
        base.Hit();
        CreateDestoryParticle();
        Destroy(gameObject);
    }
}
