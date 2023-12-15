using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float spotChangeTime = 1;
    public float bezierUpOffset = 10f;

    public GameObject trashcan;
    public GameObject walrus;
    public GameObject bird;
    public GameObject car;
    public GameObject trashTruck;

    public List<Transform> barrelThrowSpots = new List<Transform>();
    public List<Transform> walrusSpawnSpots = new List<Transform>();
    public List<Transform> birdSpawnSpots = new List<Transform>();
    public Transform bossSummoningSpot;
    public Transform tauntSpot;
    public float trashcanInterval_normal;
    public float trashcanInterval_rapid;
    public float trashcanThrowSpeed_normal = 10;
    public float trashcanThrowSpeed_fast = 20;

    private Transform currentSpot;
    private int phase = 0;
    private bool busy = false;

    private void Update()
    {
        if (busy)
            return;

        else if (phase == 0)
            PhaseZero();
        else if (phase == 1)
            PhaseOne();
        else if (phase == 2)
            PhaseTwo();
        else if (phase == 3)
            PhaseThree();
    }



    // =====  PHASES  =====
    void PhaseZero()
    {
        busy = true;
        StartCoroutine(ChangeToSpot(barrelThrowSpots[Random.Range(0, barrelThrowSpots.Count)]));
        phase = 1;
        // taunt oh yeaaaaaah!
    }

    void PhaseOne()
    {
        busy = true;
        float rand = Random.Range(0.00f, 10.00f);
        if (rand > 8)
            StartCoroutine(ChangeToSpot(barrelThrowSpots[Random.Range(0, barrelThrowSpots.Count)]));
        else if (rand > 4)
            StartCoroutine(ThrowBarrel(currentSpot, trashcanThrowSpeed_normal, Random.Range((int)1, (int)5)));
        else
            StartCoroutine(Summon(walrus, currentSpot));
            // throw barrels
            // calc offset
            // ThrowBarrel(currentSpot + offset, throwSpeed_normal);

        // summon 
    }

    void PhaseTwo()
    {
        busy = true;
        // sama mut myös lintui?
        // summonoi roskis-autot jollekin sivulle/sivuille
    }

    void PhaseThree()
    {
        busy = true;
        // barreleita sieltä täältä
        // vihuja lähes jatkuvasti
        // välillä barreleita yhdestä suunnasta hullun paljon
    }



    // =====  ACTIONS  =====
    IEnumerator ChangeToSpot(Transform spot)
    {
        Vector3 startpos = transform.position;
        Quaternion startrot = transform.rotation;
        Vector3 midpos = Vector3.Lerp(startpos, spot.position, 0.5f) + Vector3.up * bezierUpOffset;
        Vector3 endpos = spot.position - spot.forward * 0.5f;
        float t = 0;
        while (t < spotChangeTime)
        {
            transform.position = Bezier2(startpos, midpos, endpos, t);
            transform.rotation = Quaternion.Slerp(startrot, spot.rotation, t);
            t += Time.deltaTime;
            yield return null;
        }
        currentSpot = spot;
        busy = false;
    }
    IEnumerator ThrowBarrel(Transform _throwSpot, float _throwSpeed, int _count)
    {
        var clone = Instantiate(trashcan, _throwSpot.position, Quaternion.LookRotation(_throwSpot.right));
        clone.GetComponent<TrashcanController>().GotThrown(_throwSpot, _throwSpeed);
        float t = 0;
        while (t < trashcanInterval_normal)
        {
            t += Time.deltaTime;
            yield return null;
        }
        busy = false;
        yield return null;
    }

    public float summonForwardOffset = 3;
    public float summonSpeed = 2;
    public float walrusSummonY = 8.309f;
    IEnumerator Summon(GameObject _enemy, Transform _spawnSpot)
    {
        Vector3 pos = transform.position + transform.forward * summonForwardOffset;
        pos = new Vector3(pos.x, walrusSummonY, pos.z);
        Instantiate(_enemy, pos, transform.rotation);
        float t = 0;
        while (t < summonSpeed) 
        {
            t += Time.deltaTime;
            yield return null;
        }
        busy = false;
    }




    // Helpers
    public static Vector3 Bezier2(Vector3 s, Vector3 p, Vector3 e, float t)
    {
        float rt = 1 - t;
        return rt * rt * s + 2 * rt * t * p + t * t * e;
    }
}
