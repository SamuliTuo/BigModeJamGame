using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyController_basic : MonoBehaviour
{
    [SerializeField] private float timeBetweenMoves = 0.5f;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private bool useSmoothing = false;

    public List<Vector3> targets;
    int currentTarget = -1;
    Coroutine moveRoutine = null;

    void Start()
    {
        var moveTargets = transform.Find("moveTargets");
        for (int i = 0; i < moveTargets.childCount; i++)
        {
            targets.Add(moveTargets.GetChild(i).position);
            moveTargets.GetChild(i).gameObject.SetActive(false);
        }
        targets.Add(transform.position);
        
        if (targets.Count > 0)
        {   
            currentTarget = 0;
            moveRoutine = StartCoroutine(MoveCoroutine());
        }
    }

    IEnumerator MoveCoroutine()
    {
        Vector3 startpos = transform.position;
        Vector3 endpos = targets[currentTarget];
        float maxT = (startpos - endpos).magnitude;
        float t = 0;
        float perc;
        while (t < maxT)
        {
            perc = t / maxT;
            if (useSmoothing)
                perc = perc * perc * (3f - 2f * perc);
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startpos, endpos, perc);
            yield return null;
        }

        if (timeBetweenMoves > 0)
        {
            t = 0;
            while (t < timeBetweenMoves)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        currentTarget++;
        if (currentTarget >= targets.Count)
        {
            currentTarget = 0;
        }

        moveRoutine = StartCoroutine(MoveCoroutine());
    }

    public void GotAttacked()
    {
        StopCoroutine(moveRoutine);
        Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
