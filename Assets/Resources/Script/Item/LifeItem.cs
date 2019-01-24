using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeItem : Item
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(PLAYER_TAG).GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void GetItem()
    {
        player.Hit(1);
        base.GetItem();
    }
}
