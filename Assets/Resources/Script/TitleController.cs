using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(1f);

        while (true)
        {
            if (Button.leftButtonUp || Button.rightButtonUp)
            {
                SceneManager.LoadScene("Game");
                yield break;
            }
            yield return null;
        }
    }
}
