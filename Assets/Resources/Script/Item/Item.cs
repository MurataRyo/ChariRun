using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    bool isHit = false;
    public const string PLAYER_TAG = "Player";

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag != PLAYER_TAG || isHit)
            return;

        GetItem();
    }

    public virtual void GetItem()
    {
        isHit = true;
        Destroy(gameObject);
    }
}
