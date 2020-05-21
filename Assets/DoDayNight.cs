using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDayNight : MonoBehaviour
{
    public enum CycleType { Morning, Noon, Evening, Night};
    public CycleType currentCycle;
    SetOrchestra theOrchestra;
    float[,] skyboxValueSet;
    float currCycleTimer;
    public Material skyMat;

    // Start is called before the first frame update
    void Start()
    {
        currentCycle = (int)0;
        currCycleTimer = 0;
        theOrchestra = GetComponent<SetOrchestra>();
        skyboxValueSet = new float[4, 2] { {2f, 1.25f}, { 4, 2.5f}, { 8, 5 }, { 0, 5 } };
        skyMat.SetFloat("_Exposure", 2f);
        skyMat.SetFloat("_AtmosphereThickness", 1.25f);
    }

    // Update is called once per frame
    void Update()
    {
        currCycleTimer += Time.deltaTime;
        float exposure = skyMat.GetFloat("_Exposure"), atmosphere = skyMat.GetFloat("_AtmosphereThickness");
        skyMat.SetFloat("_Exposure", Mathf.Lerp(skyboxValueSet[(int)currentCycle,0], skyboxValueSet[(int)(currentCycle+1)%(int)(CycleType.Night+1), 0], currCycleTimer /theOrchestra.TimeOfDayCycleTime));
        if(currentCycle!=CycleType.Night)
        skyMat.SetFloat("_AtmosphereThickness", Mathf.Lerp(skyboxValueSet[(int)currentCycle, 1], skyboxValueSet[(int)(currentCycle + 1) % (int)(CycleType.Night+1), 1], currCycleTimer / theOrchestra.TimeOfDayCycleTime));
        else
            skyMat.SetFloat("_AtmosphereThickness", Mathf.Lerp(0, skyboxValueSet[(int)(currentCycle + 1) % (int)(CycleType.Night + 1), 1], currCycleTimer / theOrchestra.TimeOfDayCycleTime));

        if (currCycleTimer > theOrchestra.TimeOfDayCycleTime)
        {
            currentCycle = (CycleType)(((int)currentCycle + 1) % ((int)CycleType.Night + 1));
            currCycleTimer = 0;
        RenderSettings.skybox = skyMat;
        }
        DynamicGI.UpdateEnvironment();

    }
}
