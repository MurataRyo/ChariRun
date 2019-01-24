using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector]
    public int exp = 0;
    Player player;

    GameObject expText;

    GameObject DestoryParticle;
    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        expText = Resources.Load<GameObject>(Player.OBJECT_PATH + "/ExpText");
        DestoryParticle = Resources.Load<GameObject>(Player.OBJECT_PATH + "/DestoryEnemy");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Hit()
    {
        AddExp();
    }

    public virtual void  OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Player>() != null)
        {
            col.gameObject.GetComponent<Player>().Hit(-1);
        }
    }

    public void CreateDestoryParticle()
    {
        GameObject go = Instantiate(DestoryParticle);
        go.transform.position = transform.position + new Vector3(0f, 0f, -1f);
        ParticleSystem particleSystem = go.GetComponent<ParticleSystem>();
        Destroy(go, particleSystem.main.startLifetime.constant);
    }

    public virtual void AddExp()
    {
        if (exp == 0)
            return;

        TextCreate();
        player.GetExp(exp);
    }

    void TextCreate()
    {
        GameObject go = Instantiate(expText);
        TextMesh textMesh = go.GetComponent<TextMesh>();

        textMesh.text = exp + "EXP";
        go.transform.position = transform.position + new Vector3(0f,0.5f,0f);
    }
}
