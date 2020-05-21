using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEntry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Factory.Instance.Initialize();
        EnemyManager.Instance.Initialize();
        EnvironmentManager.Instance.Initialize();
        ParticlesManager.Instance.Initialize();
        AudioManager.Instance.Initialize();
        AudioClipManager.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        Factory.Instance.Refresh();
        EnemyManager.Instance.Refresh();
        EnvironmentManager.Instance.Refresh();
        ParticlesManager.Instance.Refresh();
        AudioManager.Instance.Refresh();
    }
}
