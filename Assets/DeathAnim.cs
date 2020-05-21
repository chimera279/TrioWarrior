using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DeathAnim : MonoBehaviour
{
    Rect rect;
    public Image img;
    float sizex, sizey;
    public static bool isActive;
    public static Attacks.AtkDefElement deathElement;
    public static float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
       // rect = GetComponent<Rect>();
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isActive)
        {
            Color c = img.color;
            switch (deathElement)
            {
                case Attacks.AtkDefElement.Rock:
                    c.r = Mathf.PingPong(timer / 1, 1f - 0.125f) + 0.125f;
                    break;
                case Attacks.AtkDefElement.Paper:
                    c.b = Mathf.PingPong(timer / 1, 1f - 0.125f) + 0.125f;
                    break;
                case Attacks.AtkDefElement.Scissors:
                    c.r = Mathf.PingPong(timer / 1, 1f - 0.125f) + 0.125f;
                    c.g = Mathf.PingPong(timer / 1, 1f - 0.125f) + 0.125f;
                    break;
            }
            c.a = Mathf.PingPong(timer / 1, 1f - 0.125f) + 0.125f;
            sizex = Mathf.PingPong(Time.time / 1, 2920 - 1280) + 1280;
            sizey = Mathf.PingPong(Time.time / 1, 2920 - 1280) + 1280;

            img.color = c;
            timer += Time.deltaTime%1;
            if (c.a == 1f)
                Time.timeScale = 0;
            if (Input.anyKey)
                Time.timeScale = 1;
          //  rect.size = new Vector2(sizex, sizey);
        }
        else img.color = Color.clear;
        
    }
}
