using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    enum PlayerMode
    {
        run,
        jump
    }
    PlayerMode playerMode = PlayerMode.jump;

    //可能な2段ジャンプの回数
    [HideInInspector]
    public int jumpNumMax;
    int jumpNum = 0;

    public float skyTimeMax;
    [HideInInspector]
    public float sky_time = 0;
    bool skyFlag = true;

    List<GameObject> jumpEffectUiList = new List<GameObject>();
    GameObject jumpEffectUi;
    Color NormalColor = new Color(1f, 1f, 1f, 1f);
    Color EndlColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    GameObject skyBarPrefab;
    GameObject skyBar;
    Slider skySlider;
    const float SKY_HEEL_SPEED = 1.5f;   //全回復する時間

    enum SwordMode
    {
        none,
        attack,
        interval
    }
    SwordMode swordMode = SwordMode.none;
    public GameObject sword;
    public GameObject sSword;
    GameObject swordBar;
    Slider swordSlider;
    public int swordEnergyMax = 100;
    public int sSwordEnergy = 10;
    public float swordEnergy = 100;
    const float SWORD_HEEL_SPEED = 30f;   //全回復する時間

    IEnumerator invincible;
    bool invincibleFlag = false;
    float invincibleTimer = 0f;
    const float INVINCIBLE_TIME = 1.5f;
    const float INVINCIBLE_CHANGE_TIME = 0.2f;
    [HideInInspector]
    public int playerLife;
    public const int PLAYER_LIFE_MAX = 3;

    const float SWORD_TIME_INTERVAL = 0.05f;
    float swordTimer = 0f;

    public const float SWORD_TIME = 0.1f;

    const float END_GRAVITY = 3f;   //ボタンを離したときの最低重力
    float gravity = 0f;
    const float GRAVITY_SIZE = -12f;
    bool isGround = false;
    const float JUMP_POWER = 8f;
    const float DOUBLE_JUMP_POWER = 7f;

    bool intervalFlag = false;
    const float JUMP_INTERVAl_TIME = 0.1f;
    float intervalTimer = 0f;

    public const float WALL_RANGE_MIN = 7f;    //平均的な壁との距離
    const float WALL_RANGE_MAX = 12f;    //平均的な壁との距離

    const float ADD_SPEED = 1.1f;
    const float DOWN_SPEED = 0.9f;
    const float RAY_DISTANCE = 0.05f;

    Rigidbody2D rigidbody2;
    CameraScript cameraScript;
    GameController gameController;
    GameObject jumpEffect;
    SpriteRenderer spriteRenderer;
    Expbar expbar;

    [HideInInspector]
    public Life lifeScript;
    GameObject canvas;

    [HideInInspector]
    public LayerMask rayBlockLayer;

    const string BLOCK_TAG = "Block";
    const string CAMERA_TAG = "MainCamera";
    bool cameraHitFlag = false;
    bool blockHitFlag = false;

    const string PREFAB_PATH = "Prefab";
    public const string ENEMY_PATH = PREFAB_PATH + "/Enemy";
    public const string OBJECT_PATH = PREFAB_PATH + "/Object";
    const string JUMP_EFFECT_PATH = OBJECT_PATH + "/JumpEffect";
    const string JUMP_EFFECT_UI_PATH = OBJECT_PATH + "/JumpEffectUi";
    const string WING_BAR_PATH = OBJECT_PATH + "/WingBar";
    const string SWORD_PATH = OBJECT_PATH + "/Sword";
    public const string S_SWORD_PATH = OBJECT_PATH + "/S_Sword";
    public const string SWORDBAR_PATH = OBJECT_PATH + "/SwordBar";
    public const string BULLET_PATH  = OBJECT_PATH + "/PlayerBullet";
    const string IMAGE_PATH = "Image";
    const string PLAYER_PATH = IMAGE_PATH + "/Player";

    // Start is called before the first frame update
    void Start()
    {
        jumpNumMax = 1;
        skyTimeMax = 1f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerLife = PLAYER_LIFE_MAX;
        rayBlockLayer = 1 << LayerMask.NameToLayer(BLOCK_TAG);
        rigidbody2 = GetComponent<Rigidbody2D>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        StartCoroutine(ImageChange());
        sword = Resources.Load<GameObject>(SWORD_PATH);
        sSword = Resources.Load<GameObject>(S_SWORD_PATH);
        swordBar = Resources.Load<GameObject>(SWORDBAR_PATH);
        jumpEffect = Resources.Load<GameObject>(JUMP_EFFECT_PATH);
        jumpEffectUi = Resources.Load<GameObject>(JUMP_EFFECT_UI_PATH);
        skyBarPrefab = Resources.Load<GameObject>(WING_BAR_PATH);
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        foreach (Transform child in canvas.transform)
        {
            if (child.gameObject.tag == "Life")
            {
                lifeScript = child.gameObject.GetComponent<Life>();
            }
            if(child.gameObject.tag == "Expbar")
            {
                expbar = child.gameObject.GetComponent<Expbar>();
            }
        }
        JumpEffetUiAdd();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
            return;

        Move();
        SwordMove();
        if (invincibleFlag && TimerFlag(INVINCIBLE_TIME, ref invincibleTimer))
        {
            invincibleTimer = 0f;
            EndInvincible();
        }

        if(transform.position.y < cameraScript.minCameraPos)
        {
            gameController.GameOver();
        }
    }

    void SwordMove()
    {
        if (swordMode == SwordMode.attack && TimerFlag(SWORD_TIME, ref swordTimer))
        {
            swordTimer = 0f;
            swordMode = SwordMode.interval;
        }
        if (swordMode == SwordMode.interval && TimerFlag(SWORD_TIME_INTERVAL, ref swordTimer))
        {
            swordTimer = 0f;
            swordMode = SwordMode.none;
        }
        else if (Button.rightButtonDown && swordMode == SwordMode.none)
        {
            swordMode = SwordMode.attack;
            SwordCreate();
        }

        if(expbar.level >= Expbar.S_SWORD_START_LEVEL)
        {
            swordEnergy += swordEnergyMax * Time.deltaTime / SWORD_HEEL_SPEED;
            if(swordEnergy > swordEnergyMax)
            {
                swordEnergy = swordEnergyMax;
            }
            SwordBarUpdate();
        }
    }

    void SwordCreate()
    {
        GameObject go;
        if(expbar.level >= Expbar.S_SWORD_START_LEVEL && swordEnergy >= sSwordEnergy)
        {
            go = Instantiate(sSword);
            swordEnergy -= sSwordEnergy;
            SwordBarUpdate();
        }
        else
        {
            go = Instantiate(sword);
        }
        go.gameObject.transform.parent = gameObject.transform;
    }

    void Move()
    {
        if (intervalFlag)
        {
            if (TimerFlag(JUMP_INTERVAl_TIME, ref intervalTimer))
            {
                intervalTimer = 0f;
                intervalFlag = false;
            }
        }

        if (Button.leftButtonDown)
        {
            Jump();
        }
        JumpControl();
        SkyMove();
        rigidbody2.velocity = Velosity();      
    }

    void JumpControl()
    {
        if(Button.leftButtonUp)
        {
            if (playerMode == PlayerMode.jump)
            {
                if (gravity > END_GRAVITY)
                {
                    gravity = END_GRAVITY;
                }
            }
        }
    }

    void SkyMove()
    {
        if (expbar.level >= Expbar.SKY_START_LEVEL && playerMode == PlayerMode.jump && !Button.leftButtonDown &&
           Button.leftButton && gravity <= 0f && skyFlag && sky_time < skyTimeMax)
        {
            gravity = 0f;
            sky_time += Time.deltaTime;
            WingBarUpdate();
        }

        if (gravity < GRAVITY_SIZE / 5f)
        {
            skyFlag = true;
        }
        else if (gravity < 0f)
        {
            skyFlag = false;
        }

        if(isGround)
        {
            sky_time -= skyTimeMax * Time.deltaTime / SKY_HEEL_SPEED;
            if (sky_time < 0f)
            {
                sky_time = 0f;
            }
            WingBarUpdate();
        }
    }

    void FixedUpdate()
    {
        if (cameraHitFlag && blockHitFlag)
        {
            gameController.GameOver();
        }
        else
        {
            cameraHitFlag = false;
            blockHitFlag = false;
        }

        GravityUpdate();

        if (!isGround && playerMode == PlayerMode.run)
        {
            playerMode = PlayerMode.jump;
        }

        isGround = false;
    }

    void GravityUpdate()
    {
        gravity = isGround ? 0f :  gravity + (GRAVITY_SIZE * Time.fixedDeltaTime);
    }

    void Jump()
    {
        if (jumpNum < jumpNumMax)
        {
            skyFlag = true;
            intervalFlag = true;
            gravity = playerMode == PlayerMode.run ? JUMP_POWER : DOUBLE_JUMP_POWER;

            if (playerMode == PlayerMode.jump)
            {
                DoubleJump();
            }
            else
            {
                isGround = false;
                playerMode = PlayerMode.jump;
            }
        }
    }

    void DoubleJump()
    {
        jumpNum++;
        GameObject go = Instantiate(jumpEffect);
        go.transform.position = transform.position;
        JumpEffectColorChange();
    }

    Vector2 Velosity()
    {
        return new Vector2(Speed(cameraScript.speed), gravity);
    }

    float Speed(float speed)
    {
        if (!FowardFlag())
        {
            return 0f;
        }

        float cPos = Camera.main.transform.position.x - gameController.cameraXMax;
        if (cPos + WALL_RANGE_MIN > transform.position.x)
        {
            speed *= ADD_SPEED;
        }
        else if (cPos + WALL_RANGE_MAX < transform.position.x)
        {
            speed *= DOWN_SPEED;
        }
        return speed;
    }

    //前に進めるかの処理
    bool FowardFlag()
    {
        CapsuleCollider2D cap = GetComponent<CapsuleCollider2D>();
        Vector2 size = cap.size;

        RaycastHit2D[] hit = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y) + (size / 2f), Vector2.right, RAY_DISTANCE, rayBlockLayer);

        return hit.Length % 2 == 0;
    }

    bool TimerFlag(float time, ref float timer)
    {
        timer += Time.deltaTime;
        return time < timer;
    }

    void IsGround(Collision2D col)
    {
        if (intervalFlag)
            return;

        if (col.gameObject.tag == CAMERA_TAG)
        {
            cameraHitFlag = true;
        }

        for (int i = 0; i < col.contacts.Length; i++)
        {
            if (col.contacts[i].normal.y > 0.5f)
            {
                jumpNum = 0;
                playerMode = PlayerMode.run;
                isGround = true;
                JumpEffectColorChange();
            }

            if (col.contacts[i].normal.x < -0.9f)
            {
                blockHitFlag = true;
            }

            //頭にぶろっくが当たった時
            if (col.contacts[i].normal.y < -0.5f && gravity > 0f)
            {
                gravity = 0f;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        IsGround(col);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        IsGround(col);
    }

    void OnCollisionStay2D(Collision2D col)
    {
        IsGround(col);
    }

    //ダメージは変動値(ダメージなら- 回復なら+)
    public void Hit(int damage)
    {
        if (damage < 0 && invincibleFlag)
            return;

        HpChange(damage);
        if(playerLife <= 0)
        {
            gameController.GameOver();
        }
    }

     public void HpChange(int damage)
    {
        playerLife = Mathf.Clamp(playerLife + damage,0,PLAYER_LIFE_MAX);
        if(damage < 0)
        {
            invincibleFlag = true;
            StartInvincible();
        }
        lifeScript.LifeChange(playerLife);
    }

    void StartInvincible()
    {
        LayerChange(LayerMask.NameToLayer("InvincblePlayer"));
        invincible = InvincibleMode();
        StartCoroutine(invincible);
    }

    void EndInvincible()
    {
        LayerChange(LayerMask.NameToLayer("Player"));
        spriteRenderer.enabled = true;
        StopCoroutine(invincible);
        invincibleFlag = false;
    }

    void LayerChange(LayerMask layer)
    {
        gameObject.layer = layer;
    }

    IEnumerator InvincibleMode()
    {
        while (true)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(INVINCIBLE_CHANGE_TIME);
        }
    }

    IEnumerator ImageChange()
    {
        Sprite[] sprites = new Sprite[2];
        sprites = Resources.LoadAll<Sprite>(PLAYER_PATH);
        while (true)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                spriteRenderer.sprite = sprites[i];
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void GetExp(int exp)
    {
        expbar.GetExp(exp);
    }

    public void JumpEffetUiAdd()
    {
        while(jumpNumMax > jumpEffectUiList.Count)
        {
            GameObject go = Instantiate(jumpEffectUi);
            go.transform.parent = canvas.transform;
            go.transform.localPosition = new Vector3(-960f + jumpEffectUiList.Count % 4 * 130f, 45f - jumpEffectUiList.Count / 4 * 90, 0f);
            jumpEffectUiList.Add(go);
        }

        while(jumpNumMax < jumpEffectUiList.Count)
        {
            jumpEffectUiList.RemoveAt(jumpEffectUiList.Count - 1);
        }
    }

    void JumpEffectColorChange()
    {
        for(int i = 0;i < jumpNumMax; i++)
        {
            Image image = jumpEffectUiList[i].GetComponent<Image>();
            image.color = i < jumpNum ? EndlColor : NormalColor;
        }
    }

    public void WingBarCreate()
    {
        GameObject go = Instantiate(skyBarPrefab);
        go.transform.parent = canvas.transform;
        go.transform.localPosition = new Vector3(-960f,175f, 0f);
        skySlider = go.GetComponent<Slider>();
        SkyBarMax();
    }

    public void WingBarUpdate()
    {
        if (skySlider == null)
            return;

        skySlider.value = (skyTimeMax - sky_time);
    }

    public void SkyBarMax()
    {
        skySlider.maxValue = skyTimeMax;
        sky_time = 0f;
    }

    public void SwordBarCreate()
    {
        GameObject go = Instantiate(swordBar);
        go.transform.parent = canvas.transform;
        go.transform.localPosition = new Vector3(-960f, -570f, 0f);
        swordSlider = go.GetComponent<Slider>();
        SwordBarMax();
    }

    public void SwordBarUpdate()
    {
        if (swordSlider == null)
            return;

        swordSlider.value = swordEnergy;
    }

    public void SwordBarMax()
    {
        swordSlider.maxValue = swordEnergyMax;
    }
}
