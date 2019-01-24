using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    enum Mode
    {
        start,
        game,
        result,
    }
    Mode mode = Mode.start;

    const string PREFAB_PATH = "Prefab";
    const string MAP_PATH = PREFAB_PATH + "/Map";
    const string BLOCK_TAG = "Block";
    GameObject[] mapObject;         //マップ
    List<Map> nowMapObject = new List<Map>();  //現在生成されているオブジェクト

    GameObject cameraObject;
    CameraScript cameraScript;
    GameObject playerObject;
    GameObject canvas;
    GameObject locket;
    GameObject resultText;
    Player playerScript;
    ScoreText scoreTextScript;
    Expbar expbar;

    [HideInInspector]
    public float cameraXMax;        //中心から端までのカメラの距離

    //Itemマップの数
    const int ITEM_MAP_NUM = 1;
    //出現したアイテムの数
    int itemNum = 0;
    const float ITEM_INTERVAL = 200f;

    IEnumerator locketCreate;
    const float LOCKET_START_POS = 10f;

    float startTimer = 3f;
    void Awake()
    {
        Time.timeScale = 0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        mapObject = Resources.LoadAll<GameObject>(MAP_PATH);
        cameraXMax = Camera.main.orthographicSize * Camera.main.aspect;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        locket = Resources.Load<GameObject>(Player.ENEMY_PATH + "/Locket");
        resultText = Resources.Load<GameObject>(Player.OBJECT_PATH + "/resultText");
        playerScript = playerObject.GetComponent<Player>();
        cameraScript = cameraObject.GetComponent<CameraScript>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        foreach (Transform child in canvas.transform)
        {
            if(child.gameObject.tag == "ScoreText")
            {
                scoreTextScript = child.gameObject.GetComponent<ScoreText>();
            }

            if(child.gameObject.tag == "Expbar")
            {
                expbar = child.gameObject.GetComponent<Expbar>();
            }
        }

        //                                                         1.5fは初期位置                                     +2fは地面の上に立たせるため
        playerObject.transform.position = new Vector3(-cameraXMax + 1.5f, -cameraXMax / Camera.main.aspect + 2f, 0f);
        //+1fは地面を見せるため
        CreateStage(new Vector3(-cameraXMax, -cameraXMax / Camera.main.aspect + 1f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        switch (mode)
        {
            case Mode.start:
                StartMode();
                break;
            case Mode.game:
                GameMode();
                break;

            case Mode.result:
                ResultMode();
                break;
        }
    }

    void StartMode()
    {
        Camera.main.transform.position = new Vector3(0f, Camera.main.transform.position.y, Camera.main.transform.position.z);
        startTimer -= Time.unscaledDeltaTime;

        if(startTimer < 0f)
        {
            Time.timeScale = 1f;
            mode = Mode.game;
        }
    }

    void GameMode()
    {
        if (nowMapObject[nowMapObject.Count - 1].collider2D.bounds.max.x <= Camera.main.transform.position.x + cameraXMax)
        {
            CreateStage(CreatePos());
        }

        if (nowMapObject[0].collider2D.bounds.max.x <= Camera.main.transform.position.x - cameraXMax)
        {
            Destroy(nowMapObject[0].thisGo);
            nowMapObject.RemoveAt(0);
            cameraScript.minCameraPos = CameraMin();
        }

        if(locketCreate == null && Camera.main.transform.position.x >= LOCKET_START_POS)
        {
            locketCreate = LocketCreate();
            StartCoroutine(locketCreate);
        }
    }

    void ResultMode()
    {

    }

    IEnumerator Result()
    {
        yield return new WaitForSecondsRealtime(1f);

        while(true)
        {
            if (Button.leftButtonUp || Button.rightButtonUp)
            {
                SceneManager.LoadScene("Title");
                yield break;
            }

            yield return null;
        }
    }


    void CreateStage(Vector3 pos)
    {
        GameObject go;
        if (cameraScript.transform.position.x > (itemNum + 1) * ITEM_INTERVAL)
        {
            itemNum++;
            go = NextStage(pos, mapObject.Length - 1);
        }
        else
        {
            go = NextStage(pos,Random.Range(0,mapObject.Length - ITEM_MAP_NUM));
        }

        nowMapObject.Add(new Map(go, MapCollider(go)));

        cameraScript.minCameraPos = CameraMin();
    }

    GameObject NextStage(Vector3 pos,int number)
    {
        GameObject go = Instantiate(mapObject[number]);
        go.transform.position = pos;

        return go;
    }

    float CameraMin()
    {
        float f = nowMapObject[0].collider2D.bounds.min.y;

        for(int i = 1;i < nowMapObject.Count;i++)
        {
            if(nowMapObject[i].collider2D.bounds.min.y > f)
            {
                f = nowMapObject[i].collider2D.bounds.min.y;
            }
        }
        return f;
    }

    Vector3 CreatePos()
    {
        Collider2D nowCol = nowMapObject[nowMapObject.Count - 1].collider2D;
        GameObject nowGo = nowMapObject[nowMapObject.Count - 1].thisGo;
        //-1fは１段の段差を埋めるため
        return new Vector3(nowCol.bounds.max.x, nowCol.bounds.max.y - 1f, 0f);
    }

    Collider2D MapCollider(GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            if (child.gameObject.tag == BLOCK_TAG)
            {
                return child.gameObject.GetComponent<CompositeCollider2D>();
            }
        }

        Debug.Log("エラー");
        return null;
    }

    public void GameOver()
    {
        if (mode == Mode.result)
            return;
        
        playerScript.lifeScript.LifeChange(0);
        Time.timeScale = 0f;
        ResultTextCreate();
        StartCoroutine(Result());
        mode = Mode.result;
    }

    void ResultTextCreate()
    {
        GameObject go = Instantiate(resultText);
        go.transform.parent = canvas.transform;
        go.transform.localPosition = Vector3.zero;
        go.GetComponent<Text>().text = ((int)((expbar.getExp / 100f + 1) * Camera.main.transform.position.x)).ToString() + "score"; 
    }

    IEnumerator LocketCreate()
    {
        float f = 4f;
        while(true)
        {
            CreateLocket();
            f *= 1f - (expbar.level * 0.01f);
            if (f < 0)
                f = 0;
            yield return new WaitForSeconds(f + 0.4f);
        }
    }

    void CreateLocket()
    {
        GameObject go = Instantiate(locket);
        go.transform.position = Camera.main.transform.position + new Vector3(Camera.main.orthographicSize * Camera.main.aspect + 5f,Random.Range(-5f,3f), Mathf.Abs(Camera.main.transform.position.z));
    }
}

public class Map : ScriptableObject
{
    public GameObject thisGo;
    public Collider2D collider2D;

    public Map(GameObject go, Collider2D col)
    {
        thisGo = go;
        collider2D = col;
    }
}
