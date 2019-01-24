using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageUi : MonoBehaviour
{
    RectTransform rectTransform;
    Vector2 defaultSize = new Vector2(1920f, 1080f);
    // Start is called before the first frame update
    public virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Vector2 nowPos = rectTransform.localPosition;
        Vector2 nowScale = rectTransform.sizeDelta;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        rectTransform.sizeDelta = NextSize(screenSize, nowScale, defaultSize);
        rectTransform.localPosition = NextSize(screenSize, nowPos, defaultSize);
    }
    
    Vector2 NextSize(Vector2 screen, Vector2 nowScale, Vector2 defaultSize)
    {
        return new Vector2(NextSize(screen.x, nowScale.x, defaultSize.x), NextSize(screen.y, nowScale.y, defaultSize.y));
    }

    float NextSize(float screen,float nowScale,float defaultSize)
    {
        return screen / defaultSize * nowScale;
    }
}
