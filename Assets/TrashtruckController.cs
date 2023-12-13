using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class TrashtruckController : MonoBehaviour
{
    [SerializeField] private int hp = 3;
    [SerializeField] private GameObject trashcan = null;
    [SerializeField] private Transform trashThrowSpot1 = null;
    [SerializeField] private Transform trashThrowSpot2 = null;
    [SerializeField] private Transform trashThrowSpot3 = null;
    [SerializeField] private float interval = 1.0f;
    [SerializeField] private float throwSpeed = 10;

    private Coroutine throwing = null;// private bool throwing = false;

    public void TakeDamage()
    {
        print("damg");
        //Animator.playe(dmg);
        hp--;
        if (hp == 0)
        {
            StopCoroutine(throwing);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (throwing == null && other.CompareTag("Player"))
        {
            throwing = StartCoroutine(Trashthrow());
        }
    }

    IEnumerator Trashthrow()
    {
        float t = 0;
        var clone = Instantiate(trashcan, trashThrowSpot1.position, Quaternion.LookRotation(trashThrowSpot1.right));
        clone.GetComponent<TrashcanController>().GotThrown(trashThrowSpot1, throwSpeed);
        while (t < interval) 
        { 
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        var clone2 = Instantiate(trashcan, trashThrowSpot2.position, Quaternion.LookRotation(trashThrowSpot1.right));
        clone2.GetComponent<TrashcanController>().GotThrown(trashThrowSpot2, throwSpeed);
        while (t < interval)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        var clone3 = Instantiate(trashcan, trashThrowSpot3.position, Quaternion.LookRotation(trashThrowSpot3.right));
        clone3.GetComponent<TrashcanController>().GotThrown(trashThrowSpot3, throwSpeed);
        while (t < interval)
        {
            t += Time.deltaTime;
            yield return null;
        }

        throwing = StartCoroutine(Trashthrow());
    }
}
