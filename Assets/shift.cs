using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shift : MonoBehaviour
{
    public float timer = 0f;
    // Start is called before the first frame update


    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.75f)
        {
            Vector3 randomPos = Random.onUnitSphere * 5;
            StartCoroutine(Lerp(randomPos));
           
            //timer = 0f;
        }

    }

    IEnumerator Lerp(Vector3 targetPos)
    {
        while (Vector3.SqrMagnitude(transform.position - targetPos) > 4f)
        {
            timer = 0.5f;
            transform.Translate((targetPos - transform.position) * Time.deltaTime); 
            if (Vector3.SqrMagnitude(transform.position - targetPos) < 9f)
            {
                timer = 0f;
            }

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
