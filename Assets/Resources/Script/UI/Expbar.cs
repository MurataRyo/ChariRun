using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expbar : ImageUi
{
    Slider slider;
    int nextExp;        //レべルアップに必要な経験値

    int nowExp = 0;     //現在の経験値
    int nowBarExp = 0;  //見た目上の経験値
    public int getExp = 0;

    public const int LEVEL_MAX = 17;
    public int level = 1;
    public const int SKY_START_LEVEL = 3;
    public const int S_SWORD_START_LEVEL = 5;
    Player player;

    const float EXP_SPEED = 1f;       //expSpeedの割合
    float expSpeed;                     //１秒でいくつの経験値が反映されるか
    float timer = 0f;

    Text levelText;
    GameObject expText;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        slider = GetComponent<Slider>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        NextExp(level);
        expText = Resources.Load<GameObject>("Prefab/Object/ExpText");

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        foreach(Transform child in canvas.transform)
        {
            if(child.gameObject.tag == "LevelText")
            {
                levelText = child.gameObject.GetComponent<Text>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(level != LEVEL_MAX && nowExp != nowBarExp && Timer(expSpeed,ref timer))
        {
            AddExpbar(addNum(expSpeed,ref timer));
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            GetExp(200);
        }
    }

    public void GetExp(int exp)
    {
        getExp += exp;
        nowExp += exp;
    }

    void AddExpbar(int puls)
    {
        nowBarExp += puls;
        if(nowBarExp > nowExp)
        {
            nowBarExp = nowExp;
        }
        if (nowBarExp >= nextExp)
        {
            LevelUp();
        }

        slider.value = nowBarExp;
    }

    void NextExp(int level)
    {
        nextExp = (int)(level * 20f) + 40;
        slider.maxValue = nextExp;
        expSpeed =  EXP_SPEED / nextExp;
    }

    void LevelUp()
    {
        level++;
        if (level == LEVEL_MAX)
        {
            slider.value = slider.maxValue;
        }
        else
        {
            nowExp -= nextExp;
            nowBarExp -= nextExp;
            NextExp(level);
        }
        LevelUpdate();
    }

    void LevelUpdate()
    {
        if (level == 2 || level == 4)
        {
            JumpNumMaxAdd(1);
        }

        if(level == SKY_START_LEVEL)
        {
            player.WingBarCreate();
        }

        if(level == S_SWORD_START_LEVEL)
        {
            player.SwordBarCreate();
        }

        if(level == 5)
        {
            player.skyTimeMax += 1f;
            player.SkyBarMax();
            JumpNumMaxAdd(1);
        }

        if(level >= 6)
        {
            player.swordEnergyMax += 50;
            player.SwordBarMax();
            JumpNumMaxAdd(1);
        }

        if(level == LEVEL_MAX)
        {
            player.skyTimeMax += 10f;
            player.SkyBarMax();

            player.swordEnergyMax += 900;
            player.SwordBarMax();
        }

        if(level >= S_SWORD_START_LEVEL)
        {
            player.swordEnergy = player.swordEnergyMax;
            player.SwordBarUpdate();
        }
        
        if(level >= SKY_START_LEVEL)
        {
            player.sky_time = 0f;
            player.WingBarUpdate();
        }

        player.HpChange(Player.PLAYER_LIFE_MAX);
        LevelUpTextCreate();
        levelText.text = "Level" + (level == LEVEL_MAX ? "MAX" : level.ToString());
    }

    void JumpNumMaxAdd(int num)
    {
        player.jumpNumMax += num;
        player.JumpEffetUiAdd();
    }

    bool Timer(float time,ref float timer)
    {
        timer += Time.deltaTime;

        if(timer > time)
        {
            return true;
        }
        return false;
    }

    int addNum(float time, ref float timer)
    {
        int i = (int)(timer / time);
        timer = timer % time;
        return i;
    }

    void LevelUpTextCreate()
    {
        GameObject go = Instantiate(expText);
        TextMesh textMesh = go.GetComponent<TextMesh>();

        textMesh.text = "LEVEL UP";
        go.transform.position = player.transform.position + new Vector3(0f, 1f, 0f);
        go.transform.parent = player.transform;
    }
}
