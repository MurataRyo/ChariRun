using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScript : ImageUi
{
    [HideInInspector]
    public Text text;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        text = GetComponent<Text>();
        PosEnter();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PosEnter()
    {
        text.fontSize = (int)(text.fontSize * (Screen.width / 1920f));
    }
}
