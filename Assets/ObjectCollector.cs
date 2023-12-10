using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollector : MonoBehaviour
{
    public float collectSpeed = 1.0f;
    public int melons = 0;

    private int melonsBeingCollected = 0;

    private Coroutine collectRoutine = null;


    void Start()
    {
        
    }

    public void CollectMelon(int amount)
    {
        melonsBeingCollected += amount;

        if (collectRoutine == null )
        {
            collectRoutine = StartCoroutine(CollectRoutine());
        }
    }

    IEnumerator CollectRoutine()
    {
        float t = 0;

        while (melonsBeingCollected > 0)
        {
            if (t < 1)
            {
                //t += Time.deltaTime * ;
            }

            yield return null;
        }
    }
}
