using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    Rigidbody2D rigidbody2;
    CameraScript cameraScript;
    GameObject endBullet;

    float timer;
    const float END_TIME = 1f;
    // Start is called before the first frame update
    void Start()
    {
        cameraScript = Camera.main.GetComponent<CameraScript>();
        endBullet = Resources.Load<GameObject>(Player.OBJECT_PATH + "/EndBullet");
        rigidbody2 = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > END_TIME)
        {
            DestroyGo();
        }
        rigidbody2.velocity = new Vector2(cameraScript.speed * 1.5f, 0f);
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Enemy>() == null)
            return;

        col.gameObject.GetComponent<Enemy>().Hit();
        DestroyGo();
    }

    void DestroyGo()
    {
        GameObject go = Instantiate(endBullet);
        go.transform.position = transform.position;
        Destroy(go, 3f);
        Destroy(gameObject);
    }
}
