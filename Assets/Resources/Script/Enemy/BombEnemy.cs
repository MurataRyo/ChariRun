using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEnemy : Enemy
{
    Sprite bomb;
    const string IMAGE_PATH = "Image";
    const string BOMB_PATH = IMAGE_PATH + "/Bomb";

    GameController gameController;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        exp = 3;
        bomb = Resources.Load<Sprite>(BOMB_PATH);
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Hit()
    {
        base.Hit();
        Explosion();
    }

    public override void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag != "Player")
            return;

        Explosion();
        base.OnTriggerStay2D(col);
    }

    void Explosion()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (renderer.sprite.name == bomb.name)
            return;

        renderer.sprite = bomb;
        gameObject.transform.localScale = Vector3.one * 3f;

        Destroy(GetComponent<Collider2D>(), 0.15f);

        Destroy(gameObject, 1f);
    }
}
