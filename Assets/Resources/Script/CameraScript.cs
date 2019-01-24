using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [HideInInspector]
    public float speed;
    const float START_SPEED = 3f;
    const float MAX_SPEED = 7f;

    const float ADD_SPEED = 0.05f;

    //プレイヤーの足の位置(下を０と考えて)
    const float PLAYER_POS = 4f;
    GameObject player;
    const string PLAYER_TAG = "Player";

    [HideInInspector]
    public float minCameraPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
        speed = START_SPEED;
        StartCoroutine(SpeedAdd());
    }

    // Update is called once per frame
    void Update()
    {
        PosUpdate();
    }

    void PosUpdate()
    {
        Vector3 pos = new Vector3(transform.position.x + speed * Time.deltaTime, Height(), -10f);
        float height = Camera.main.orthographicSize;
        
        if(pos.y < minCameraPos + height)
        {
            pos.y = minCameraPos + height;
        }

        transform.position = pos;
    }

    IEnumerator SpeedAdd()
    {
        while(speed < MAX_SPEED)
        {
            speed = Mathf.Clamp(speed + (ADD_SPEED * Time.deltaTime), 0f, MAX_SPEED);
            yield return null;
        }
        yield break;
    }

    float Height()
    {
        float f = Camera.main.orthographicSize - PLAYER_POS;
        return player.transform.position.y + f;
    }
}
