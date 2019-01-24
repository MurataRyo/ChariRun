using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpText : MonoBehaviour
{
    TextMesh textMesh;
    float alpha;
    const float ALPHA_SPEED = 1f;   //消えるまで何秒かかるか
    const float UP_SPEED = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        alpha = 1f;
        textMesh = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        alpha -= Time.deltaTime / ALPHA_SPEED;
        if (alpha < 0)
        {
            Destroy(gameObject);
        }
        else
        {
            PosChange();
            AlphaCenge();
        }
    }

    void PosChange()
    {
        transform.position += new Vector3(0f, UP_SPEED * Time.deltaTime, 0f);
    }

    void AlphaCenge()
    {
        Color c = textMesh.color;
        textMesh.color = new Color(c.r,c.g,c.b,alpha);
    }
}
