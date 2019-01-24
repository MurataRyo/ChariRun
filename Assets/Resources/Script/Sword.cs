using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public virtual void Start()
    {
        transform.position = transform.parent.transform.position + new Vector3(0.273f, 0.45f, 0f);
        StartCoroutine(ATK());
    }

    IEnumerator ATK()
    {
        while (true)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, -179f, 180f * Time.deltaTime / Player.SWORD_TIME);

            if (angle == -179f)
                Destroy(this.gameObject);

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay2D(Collider2D col)
    {
        //敵との当たり判定を検出
        if(col.gameObject.GetComponent<Enemy>() != null)
        {
            col.GetComponent<Enemy>().Hit();
        }
    }
}
