using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioClipManager
{
    public static AudioClip[,] attackClips;
    // Start is called before the first frame update
    public static void Initialize()
    {
        attackClips = new AudioClip[2, 3]
        {
            { Resources.Load<AudioClip>("Audio/Melee"),Resources.Load<AudioClip>("Audio/Ranged"),Resources.Load<AudioClip>("Audio/AoE")},
            { Resources.Load<AudioClip>("Audio/MeleeHit"),Resources.Load<AudioClip>("Audio/RangedHit"),Resources.Load<AudioClip>("Audio/AoEHit")}
        };
    }
}
