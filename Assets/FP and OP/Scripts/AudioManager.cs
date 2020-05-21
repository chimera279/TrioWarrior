using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager 
{

    #region AudioManager
    private static AudioManager instance = null;

    private AudioManager()
    {
    }

    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
                instance = new AudioManager();

            return instance;
        }
    }
    #endregion

    List<AudioSource> audios = new List<AudioSource>();
    GameObject audioObject;
    // Start is called before the first frame update
    public void Initialize()
    {
        audioObject = Resources.Load<GameObject>("Prefabs/AudioObject");
    }

    // Update is called once per frame
    public void Refresh()
    {
        foreach (var a in audios.ToArray())
            if(a)
            if (!a.isPlaying)
                AudioFinished(a); 
        
    }

    public void CreateAudio(Vector3 pos, AudioClip c,Transform parent)
    {
        AudioSource a = new AudioSource();
        if (ObjectPool.Instance.HasAudio())
        {
            a = ObjectPool.Instance.DepoolAudio();
        }
        else a = GameObject.Instantiate(audioObject, pos, Quaternion.identity).GetComponent<AudioSource>();
        a.transform.parent = parent;
        //a.Stop();
        a.clip = c;
        audios.Add(a);
        a.Play();
    }

    public void AudioFinished(AudioSource a)
    {
        if (audios.Contains(a))
        {
            audios.Remove(a);
           // a.gameObject.SetActive(false);
            ObjectPool.Instance.PoolAudio(a);
        }
    }
}
