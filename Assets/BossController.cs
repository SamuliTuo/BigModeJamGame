using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum BossActions { NONE, JUMP, THROW, SUMMON_WALRUS, }
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
    public List<Transform> walrusSpawnSpots_left = new List<Transform>();
    public List<Transform> walrusSpawnSpots_right = new List<Transform>();
    public List<Transform> walrusSpawnSpots_up = new List<Transform>();
    public List<Transform> birdSpawnSpots = new List<Transform>();
    public Transform bossSummoningSpot;
    public Transform tauntSpot;
    public float trashcanInterval_normal;
    public float trashcanInterval_rapid;
    public float trashcanThrowSpeed_normal = 10;
    public float trashcanThrowSpeed_fast = 20;

    private BossActions nextAction = BossActions.NONE;
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
        if (nextAction != BossActions.NONE)
        {
            switch (nextAction)
            {
                case BossActions.JUMP: StartCoroutine(ChangeToSpot(barrelThrowSpots[Random.Range(0, barrelThrowSpots.Count)])); break;
                case BossActions.THROW: StartCoroutine(ThrowBarrel(currentSpot, trashcanThrowSpeed_normal, Random.Range(2, 7))); break;
                case BossActions.SUMMON_WALRUS: StartCoroutine(Summon(walrus, currentSpot)); break;
            }
            return;
        }

        float rand = Random.Range(0.00f, 10.00f);
        if (rand > 5)
            StartCoroutine(ThrowBarrel(currentSpot, trashcanThrowSpeed_normal, Random.Range(2, 7)));
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
        nextAction = BossActions.NONE;
        busy = false;
    }
    IEnumerator ThrowBarrel(Transform _throwSpot, float _throwSpeed, int _count)
    {
        int i = 0;
        while (i < _count)
        {
            print("i "+i);
            Vector3 spot = _throwSpot.position + (transform.right * Random.Range(-4, 4));
            var clone = Instantiate(trashcan, spot, Quaternion.LookRotation(_throwSpot.right));
            clone.GetComponent<TrashcanController>().GotThrown(spot, _throwSpot.forward, _throwSpot.right, _throwSpeed);
            float t = 0;
            while (t < trashcanInterval_normal)
            {
                t += Time.deltaTime;
                yield return null;
            }
            i++;
            yield return null;
        }
        nextAction = BossActions.JUMP;
        busy = false;
    }

    public float summonForwardOffset = 3;
    public float summonSpeed = 2;
    public float walrusSummonY = 8.309f;
    IEnumerator Summon(GameObject _enemy, Transform _spawnSpot)
    {
        bool facingRight = false;
        bool facingDown = false;

        if (Vector3.Dot(_spawnSpot.forward, new Vector3(1, 0, 0)) >= 0.8f)
            facingDown = true;
        else if (Vector3.Dot(_spawnSpot.forward, new Vector3(0, 0, 1)) >= 0.8f)
            facingRight = true;

        float i = 0;
        int summons = Random.Range(5, 10);
        Transform spownposition;

        nextAction = BossActions.JUMP;
        busy = false;

        float t2 = 0;
        while (t2 < 0.4f)
        {
            t2 += Time.deltaTime;
            yield return null;
        }

        while (i < summons)
        {
            if (facingRight)
                spownposition = walrusSpawnSpots_left[Random.Range(0, walrusSpawnSpots_left.Count)];
            else if (facingDown)
                spownposition = walrusSpawnSpots_up[Random.Range(0, walrusSpawnSpots_up.Count)];
            else
                spownposition = walrusSpawnSpots_right[Random.Range(0, walrusSpawnSpots_right.Count)];
            Instantiate(_enemy, spownposition.position, _spawnSpot.rotation);

            float t = 0;
            while (t < summonSpeed)
            {
                t += Time.deltaTime;
                yield return null;
            }
            i++;
            yield return null;
        }
    }




    // Helpers
    public static Vector3 Bezier2(Vector3 s, Vector3 p, Vector3 e, float t)
    {
        float rt = 1 - t;
        return rt * rt * s + 2 * rt * t * p + t * t * e;
    }
}
