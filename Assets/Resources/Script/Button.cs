using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    enum Mode
    {
        debug,
        ios
    }
    Mode mode = Mode.debug;

    public static bool leftButton = false;
    public static bool leftButtonLog = false;
    public static bool leftButtonDown = false;
    public static bool leftButtonUp = false;

    public static bool rightButton = false;
    public static bool rightButtonLog = false;
    public static bool rightButtonDown = false;
    public static bool rightButtonUp = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool left = false;
        bool right = false;

        switch (mode)
        {
            case Mode.debug:
                left = Input.GetKey(KeyCode.Space);
                right = Input.GetKey(KeyCode.Return);
                break;

            case Mode.ios:

               left = ButtonIf(0);
               right = ButtonIf(1);
                break;
        }


        leftButton = left;
        rightButton = right;

        leftButtonDown = left && !leftButtonLog;
        rightButtonDown = right && !rightButtonLog;

        leftButtonUp = !left && leftButtonLog;
        rightButtonUp = !right && rightButtonLog;

        leftButtonLog = left;
        rightButtonLog = right;
    }

    //i == 0 が左　1が右
    bool ButtonIf(int i)
    {
        if(Input.touchCount != 0)
        {
            for(int j = 0;i<Input.touchCount;j++)
            {
                if(i == 0)
                {
                    if(Input.touches[j].position.x < 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if(Input.touches[j].position.x >= 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
