using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocketEnemy : Enemy
{
    Rigidbody2D rigidbody2;
    const float SPEED = -1.5f;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        exp = 25;
        rigidbody2 = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        rigidbody2.velocity = transform.right * SPEED;
    }

    public override void Hit()
    {
        base.Hit();
        CreateDestoryParticle();
        Destroy(gameObject);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "MainCamera")
            Destroy(gameObject);
    }
}
