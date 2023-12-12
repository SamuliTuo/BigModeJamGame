using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyController_basic : MonoBehaviour
{
    [SerializeField] private float timeBetweenMoves = 0.5f;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private bool useSmoothStep = false;
    [SerializeField] private bool useEaseOut = false;
    [SerializeField] private Animator animator = null;
    [SerializeField] private bool useMoveAnticipation = false;
    [SerializeField] private float anticipationTime = 0.0f;
    [SerializeField] private audios movesound = audios.None;
    [SerializeField] private float deathTimer = 1.0f;

    public List<Vector3> targets;
    int currentTarget = -1;
    Coroutine moveRoutine = null;
    private Collider col, stompCol;
    public bool dead;

    void Start()
    {
        dead = false;
        animator = GetComponentInChildren<Animator>();
        col = transform.GetChild(0).GetComponent<Collider>();
        stompCol = transform.GetChild(1).GetComponent<Collider>();


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
        Vector3 dir = endpos - startpos;
        float maxT = (startpos - endpos).magnitude;
        float t = 0;
        float perc;

        animator.Play("attackAnticipation", 0, 0);
        Quaternion startrot = transform.rotation;
        while (t < anticipationTime)
        {
            transform.rotation = Quaternion.Slerp(startrot, Quaternion.LookRotation(dir, Vector3.up), t/anticipationTime);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        animator.Play("move", 0, 0);
        AudioManager.instance.PlayClip(movesound, transform.position);
        while (t < maxT)
        {
            perc = t / maxT;
            if (useSmoothStep)
                perc = perc * perc * (3f - 2f * perc);
            else if (useEaseOut)
                perc = Mathf.Sin(perc * Mathf.PI * 0.5f);

            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startpos, endpos, perc);
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
            yield return null;
        }

        if (timeBetweenMoves > 0)
        {
            animator.CrossFade("idle",0.2f,0,0);
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
        if (!dead)
        {
            dead = true;
            animator.Play("die", 0);
            AudioManager.instance.PlayClip(audios.WALRUS_DIE, transform.position);
            StopCoroutine(moveRoutine);
            StartCoroutine(Die());
        }
    }
    public void GotStomped()
    {
        if (!dead)
        {
            dead = true;
            animator.Play("squash", 0);
            AudioManager.instance.PlayClip(audios.WALRUS_SQUASH, transform.position);
            StopCoroutine(moveRoutine);
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        col.enabled = false;
        stompCol.enabled = false;
        float t = 0;
        while(t < deathTimer)
        {
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
