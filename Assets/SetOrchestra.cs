using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOrchestra : MonoBehaviour
{
    public enum OrchestraModes {Combat, Puzzle, Morning, Afternoon, Evening, Night };
    OctaveChange[] instruments;
    public OrchestraModes orchestraMode;
    public float TimeOfDayCycleTime = 10;

    float newTempo = 0,tempTODtimer = 0;
    int TODIndex = 0;
    OctaveChange.Mode newMode; 
    // Start is called before the first frame update
    void Start()
    {
        instruments = gameObject.GetComponentsInChildren<OctaveChange>();
        orchestraMode = OrchestraModes.Morning;
        tempTODtimer = TimeOfDayCycleTime;
    }

    // Update is called once per frame
    void Update()
    {
        tempTODtimer -= Time.unscaledDeltaTime;
        if (tempTODtimer < 0)
        {
            TODIndex = (TODIndex + 1) % ((int)OrchestraModes.Night - 1);
            orchestraMode = (OrchestraModes)(TODIndex + 2);
            tempTODtimer = TimeOfDayCycleTime;
        }

        switch(orchestraMode)
        {
            case OrchestraModes.Morning:
                newMode = OctaveChange.Mode.Ionian;
                newTempo = 1.5f;
                break;
            case OrchestraModes.Afternoon:
                newMode = OctaveChange.Mode.Dorian;
                newTempo = 1.75f;
                break;
            case OrchestraModes.Evening:
                newMode = OctaveChange.Mode.Aeolian;
                newTempo = 2f;
                break;
            case OrchestraModes.Night:
                newMode = OctaveChange.Mode.Mixolydian;
                newTempo = 2.25f;
                break;
            case OrchestraModes.Puzzle:
                newMode = OctaveChange.Mode.Lydian;
                newTempo = 1.25f;
                break;
            case OrchestraModes.Combat:
                newMode = OctaveChange.Mode.Phrygian;
                newTempo = 0.75f;
                break;
        }
        foreach(var i in instruments)
        {
            i.mode = newMode;
            i.tempo = newTempo;
        }
        
    }
}
