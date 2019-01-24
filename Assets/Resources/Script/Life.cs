using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    const float SIZE = 180f;
    float size;
    const float INTERVAL = 150f;
    float interval = 150f;
    GameObject lifePrefab;
    List<GameObject> life = new List<GameObject>();
    Player playerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        size = SIZE * (Screen.width / 1920f);
        interval = size * (INTERVAL / SIZE);
        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Screen.width, Screen.height);
        lifePrefab = Resources.Load<GameObject>("Prefab/Object/Life");

        for (int i = 0;i < Player.PLAYER_LIFE_MAX;i++)
        {
            CreateLife(new Vector2(Screen.width - (i * interval),Screen.height));
        }
        LifeChange(Player.PLAYER_LIFE_MAX);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateLife(Vector2 pos)
    {
        GameObject go = Instantiate(lifePrefab);
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(size,size);
        rectTransform.position = new Vector3(pos.x, pos.y, 0f);
        go.transform.parent = transform;
        life.Add(go);
    }

    public void LifeChange(int nowLife)
    {
        for(int i = 0; i < life.Count;i++)
        {
            Image lifeImage = life[i].GetComponent<Image>();
            lifeImage.color = nowLife > i ? Color.white : Color.gray;
        }
    }
}
