using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctaveChange : MonoBehaviour
{

    public float transpose = 0;
    float note = -99, transposeTimer = 16f, modeTimer = 8f;
    [Range(0.5f, 2.5f)]
    public float tempo = 0.25f;
    public float beatSignature = 0;
    float[] countdowns = { 0.25f,0.25f,0.25f}, notes = { 0, 2, 4, 5, 7, 9, 11, 12 };
    new AudioSource[] audio;
    List<float> objectJacuzzi;
    public bool pianoMode;

    public enum Mode
    {
        Ionian,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Aeolian

    };

    public Mode mode;
    // Start is called before the first frame update
    void Start()
    {
        pianoMode = false;
        objectJacuzzi = new List<float>();
        audio = GetComponents<AudioSource>();
        for (int i = 0; i < countdowns.Length; i++)
        {
            countdowns[i] = tempo * Mathf.Pow(beatSignature, i);
        }
        for (int i = 0; i < 5; i++)
        {
            objectJacuzzi.Add(-99);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetMode();
        if(pianoMode)
             DoChange();
        for (int i = 0; i < countdowns.Length; i++)
        {
           // DoChange();
            if (countdowns[i] <= 0f)
            {
                if(!pianoMode)
                    DoChange();
                countdowns[i] = tempo * Mathf.Pow(beatSignature, i);
            }
            countdowns[i] -= Time.unscaledDeltaTime;
        }
        note = -99;
        transposeTimer -= Time.unscaledDeltaTime;
        if (transposeTimer <= 0f)
        {
        //   transpose = notes[Random.Range(0, 5)];
            transposeTimer = 16f;
        }
        modeTimer -= Time.unscaledDeltaTime;
        if (modeTimer <= 0f)
        {
          //  SetMode();
            modeTimer = 8f;
        }
    }

    
        void DoChange()
        {
        int index = HotNoat();
        objectJacuzzi[index] = -99;
        if (pianoMode)
        {
            if (Input.anyKeyDown)
            {
                #region Pianokeys
                if (Input.GetKeyDown(KeyCode.A)) objectJacuzzi[index] = notes[0];  // C
                if (Input.GetKeyDown(KeyCode.S)) objectJacuzzi[index] = notes[1];   // D
                if (Input.GetKeyDown(KeyCode.D)) objectJacuzzi[index] = notes[2];  // E
                if (Input.GetKeyDown(KeyCode.F)) objectJacuzzi[index] = notes[3];   // F
                if (Input.GetKeyDown(KeyCode.G)) objectJacuzzi[index] = notes[4];   // G
                if (Input.GetKeyDown(KeyCode.H)) objectJacuzzi[index] = notes[5];   // A
                if (Input.GetKeyDown(KeyCode.J)) objectJacuzzi[index] = notes[6];  // B
                if (Input.GetKeyDown(KeyCode.K)) objectJacuzzi[index] = notes[7];  // C
                if (Input.GetKeyDown(KeyCode.L)) objectJacuzzi[index] = notes[1] + 12;  // D    


                if (Input.GetKeyDown(KeyCode.Q)) objectJacuzzi[index] = notes[2] + 12;  // C
                if (Input.GetKeyDown(KeyCode.W)) objectJacuzzi[index] = notes[3] + 12;   // D
                if (Input.GetKeyDown(KeyCode.E)) objectJacuzzi[index] = notes[4] + 12;  // E
                if (Input.GetKeyDown(KeyCode.R)) objectJacuzzi[index] = notes[5] + 12;   // F
                if (Input.GetKeyDown(KeyCode.T)) objectJacuzzi[index] = notes[6] + 12;   // G
                if (Input.GetKeyDown(KeyCode.Y)) objectJacuzzi[index] = notes[7] + 12;   // A
                if (Input.GetKeyDown(KeyCode.U)) objectJacuzzi[index] = notes[1] + 12 + 12;  // B
                if (Input.GetKeyDown(KeyCode.I)) objectJacuzzi[index] = notes[2] + 12 + 12;  // C
                if (Input.GetKeyDown(KeyCode.O)) objectJacuzzi[index] = notes[3] + 12 + 12;  // D    
                if (Input.GetKeyDown(KeyCode.P)) objectJacuzzi[index] = notes[4] + 12 + 12;  // E    


                if (Input.GetKeyDown(KeyCode.Z)) objectJacuzzi[index] = notes[0] - 12;  // C
                if (Input.GetKeyDown(KeyCode.X)) objectJacuzzi[index] = notes[1] - 12;   // D
                if (Input.GetKeyDown(KeyCode.C)) objectJacuzzi[index] = notes[2] - 12;  // E
                if (Input.GetKeyDown(KeyCode.V)) objectJacuzzi[index] = notes[3] - 12;   // F
                if (Input.GetKeyDown(KeyCode.B)) objectJacuzzi[index] = notes[4] - 12;   // G
                if (Input.GetKeyDown(KeyCode.N)) objectJacuzzi[index] = notes[5] - 12;   // A
                if (Input.GetKeyDown(KeyCode.M)) objectJacuzzi[index] = notes[6] - 12;  // B

                #endregion
            }
        }
        else objectJacuzzi[index] = notes[Random.Range(0, 8)];

            if (objectJacuzzi[index] >= -98)
            {
                AudioSource a = LatestAudioSource();
                if (a)
                {
                    a.pitch = Mathf.Pow(2, (objectJacuzzi[index] + transpose) / 12.0f);
                    objectJacuzzi[index] = -99;
                    a.Play();
                }
            }
        }

    void SetMode()
    {
        notes = new float[] { 0, 2, 4, 5, 7, 9, 11, 12 };
      //  mode = (Mode)Random.Range(0, 6);
        switch(mode)
        {
            case Mode.Ionian:
                break;
            case Mode.Dorian:
                notes[2] -= 1;
                notes[6] -= 1;
                break;
            case Mode.Phrygian:
                notes[1] -= 1;
                notes[2] -= 1;
                notes[5] -= 1;
                notes[6] -= 1;
                break;
            case Mode.Lydian:
                notes[3] += 1;
                break;
            case Mode.Mixolydian:
                notes[6] -= 1;
                break;
            case Mode.Aeolian:
                notes[2] -= 1;
                notes[5] -= 1;
                notes[6] -= 1;
                break;

        }
    }

    AudioSource LatestAudioSource()
    {
        for (int i = 0; i < audio.Length; i++)
        {  
            if (!audio[i].isPlaying)
            {
                
                return audio[i];
                
            }
            

        }

        return audio[Random.Range(0,audio.Length)];
    }

    int HotNoat()
    {
        foreach (var o in objectJacuzzi)
            if (o == -99)
                return objectJacuzzi.IndexOf(o);
        float newNoat = -99;
        objectJacuzzi.Add(newNoat);
        Debug.Log(objectJacuzzi.Count);
        return objectJacuzzi.Count-1;
    }
}
