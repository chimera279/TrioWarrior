using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameFlower : MonoBehaviour
{
    Vector3 defaultPos, defaultRot;
    GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        panel = GameObject.FindGameObjectWithTag("StartScreen");
        panel.SetActive(true);
        StartCoroutine(RotatePlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey&& Moveonterrain.totalLevel != 27)
        {
            Time.timeScale = 1;
            panel.SetActive(false);
        }
        if (Moveonterrain.totalLevel == 27)
        {
            Time.timeScale = 0;
            panel.SetActive(true);

            StartCoroutine(RotatePlayer());
            if (Input.anyKey)
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    IEnumerator RotatePlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player").transform;
        var rotdir = Random.onUnitSphere;
        while(!Input.anyKey)
        {
            if (Random.Range(0, 200) == 1)
                rotdir = Random.onUnitSphere;
            player.Rotate(rotdir * Time.unscaledDeltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
