using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossActions { NONE, JUMP, THROW, SUMMON_WALRUS, SUMMON_BIRD, SUMMON_COACH, SUMMON_TRUCK }
public class BossController : MonoBehaviour
{
    public float spotChangeTime = 1;
    public float bezierUpOffset = 10f;

    public GameObject trashcan;
    public GameObject walrus;
    public GameObject walrus_coach;
    public GameObject bird;
    public GameObject car;
    public GameObject trashTruck;

    public Transform stunnedSpot;
    public List<Transform> barrelThrowSpots = new List<Transform>();
    public List<Transform> walrusSpawnPoints_coach = new List<Transform>();
    public List<Transform> walrusSpawnSpots_left = new List<Transform>();
    public List<Transform> walrusSpawnSpots_right = new List<Transform>();
    public List<Transform> walrusSpawnSpots_up = new List<Transform>();

    public List<Transform> birbSpawnSpots_left = new List<Transform>();
    public List<Transform> birbSpawnSpots_right = new List<Transform>();
    public List<Transform> birbSpawnSpots_up = new List<Transform>();

    public List<Transform> birdSpawnSpots = new List<Transform>();
    public Transform trashtruckSpawnSpot_left;
    public Transform trashtruckSpawnSpot_up;
    public Transform trashtruckSpawnSpot_right;

    public Transform bossSummoningSpot;
    public Transform tauntSpot;
    public float trashcanInterval_normal;
    public float trashcanInterval_rapid;
    public float trashcanThrowSpeed_normal = 10;
    public float trashcanThrowSpeed_fast = 20;
    [Space(10)]
    public float trashtruck_throwforce = 5;
    public float trashtruck_throwspeed = 5;

    private Animator _anim;
    private BossActions nextAction = BossActions.NONE;
    private Transform currentSpot;
    private int phase = 0;
    private bool busy = false;

    private Dictionary<Transform, EnemyController_basic> coachesSpawned = new Dictionary<Transform, EnemyController_basic>();
    private Dictionary<Transform, TrashtruckController> trucksSpawned = new Dictionary<Transform, TrashtruckController>();

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
    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        foreach (var item in walrusSpawnPoints_coach)
        {
            coachesSpawned.Add(item, null);
        }
        trucksSpawned.Add(trashtruckSpawnSpot_left, null);
        trucksSpawned.Add(trashtruckSpawnSpot_up, null);
        trucksSpawned.Add(trashtruckSpawnSpot_right, null);
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

        //
        StartCoroutine(ThrowBarrel(currentSpot, trashcanThrowSpeed_normal, Random.Range(2, 7), 0.5f));
        return;
        //

        if (nextAction != BossActions.NONE)
        {
            switch (nextAction)
            {
                case BossActions.JUMP: StartCoroutine(ChangeToSpot(barrelThrowSpots[Random.Range(0, barrelThrowSpots.Count)])); break;
                case BossActions.THROW: StartCoroutine(ThrowBarrel(currentSpot, trashcanThrowSpeed_normal, Random.Range(2, 7), Random.Range(0.3f, 0.7f))); break;
                case BossActions.SUMMON_WALRUS: StartCoroutine(Summon(walrus, currentSpot)); break;
                case BossActions.SUMMON_BIRD: StartCoroutine(Summon(bird, currentSpot)); break;
                case BossActions.SUMMON_COACH: StartCoroutine(SummonWalrusCoach()); break;
                case BossActions.SUMMON_TRUCK: StartCoroutine(SummonTrashtruck(2)); break;
            }
            return;
        }

        float rand = Random.Range(0, 5);
        if (rand == 4)
            StartCoroutine(ThrowBarrel(currentSpot, trashcanThrowSpeed_normal, Random.Range(2, 7), 0.3f));
        else if (rand == 3)
            StartCoroutine(Summon(walrus, currentSpot));
        else if (rand == 2)
            StartCoroutine(SummonWalrusCoach());
        else if (rand == 1)
            StartCoroutine(SummonTrashtruck(1));
        else if (rand == 0)
            StartCoroutine(ChangeToSpot(barrelThrowSpots[Random.Range(0, barrelThrowSpots.Count)]));
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
        _anim.Play("jump_start", 0, 0);
        while (t < spotChangeTime)
        {
            float perc = t / spotChangeTime;
            transform.position = Bezier2(startpos, midpos, endpos, perc);
            transform.rotation = Quaternion.Slerp(startrot, spot.rotation, perc);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = endpos;
        transform.rotation = spot.rotation;
        t = 0;
        _anim.Play("jump_landing", 0, 0);
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        _anim.Play("idle", 0, 0);
        currentSpot = spot;
        nextAction = BossActions.NONE;
        busy = false;
    }
    IEnumerator ThrowBarrel(Transform _throwSpot, float _throwSpeed, int _count, float upFactor)
    {
        int i = 0;
        float t = 0;
        _anim.Play("throw_start");
        while (t < 0.6f)
        {
            t += Time.deltaTime;
            yield return null;
        }

        while (i < _count)
        {
            Vector3 spot = _throwSpot.position + (transform.right * Random.Range(-4, 4));
            var clone = Instantiate(trashcan, spot, Quaternion.LookRotation(_throwSpot.right));
            clone.GetComponent<TrashcanController>().GotThrown(spot, _throwSpot.forward, _throwSpot.right, _throwSpeed, upFactor);
            t = 0;

            while (t < trashcanInterval_normal)
            {
                t += Time.deltaTime;
                yield return null;
            }
            i++;
            yield return null;
        }
        _anim.Play("throw_end");
        t = 0;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
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

    IEnumerator SummonWalrusCoach()
    {
        float t;
        List<Transform> availableSpots = new List<Transform>();
        foreach (KeyValuePair<Transform, EnemyController_basic> pair in coachesSpawned)
        {
            if (pair.Value == null)
                availableSpots.Add(pair.Key);
        }

        if (availableSpots.Count > 0)
        {
            t = 0;
            while (t < 1)
            {
                t += Time.deltaTime; yield return null;
            }
            Transform spot = availableSpots[Random.Range(0, availableSpots.Count)];
            var clone = Instantiate(walrus_coach, spot.position, Quaternion.LookRotation(spot.right, Vector3.up));
            coachesSpawned[spot] = clone.GetComponent<EnemyController_basic>();
        }

        t = 0;
        while (t < 2)
        {
            t += Time.deltaTime; yield return null;
        }
        nextAction = BossActions.JUMP;
        busy = false;
    }

    IEnumerator SummonTrashtruck(int count)
    {
        float t = 0;
        int i = 0;

        while (t < 1)
        {
            t += Time.deltaTime; yield return null;
        }

        while (i < count)
        {
            // Get free spots
            List<Transform> availableSpots = new List<Transform>();
            foreach (KeyValuePair<Transform, TrashtruckController> pair in trucksSpawned)
            {
                if (pair.Value == null)
                    availableSpots.Add(pair.Key);
            }

            // Spawn
            if (availableSpots.Count > 0)
            {
                Transform spot = availableSpots[Random.Range(0, availableSpots.Count)];
                var clone = Instantiate(trashTruck, spot.position, Quaternion.LookRotation(-spot.forward));
                clone.GetComponent<TrashtruckController>().InitBossMode(trashtruck_throwforce, trashtruck_throwspeed);


                trucksSpawned[spot] = clone.GetComponent<TrashtruckController>();
            }
            i++;
            yield return null;
        }

        t = 0;
        while (t < 2)
        {
            t += Time.deltaTime; yield return null;
        }
        nextAction = BossActions.JUMP;
        busy = false;
    }



    // Helpers
    public static Vector3 Bezier2(Vector3 s, Vector3 p, Vector3 e, float t)
    {
        float rt = 1 - t;
        return rt * rt * s + 2 * rt * t * p + t * t * e;
    }
    public void TrashcanHit()
    {
        nextAction = BossActions.JUMP;
        StopAllCoroutines();
        StartCoroutine(GetStunned());
    }

    IEnumerator GetStunned()
    {
        Vector3 startpos = transform.position;
        Quaternion startrot = transform.rotation;
        Vector3 midpos = Vector3.Lerp(startpos, stunnedSpot.position, 0.5f) + Vector3.up * bezierUpOffset;
        Vector3 endpos = stunnedSpot.position - stunnedSpot.forward * 0.5f;
        float t = 0;
        _anim.Play("jump_start", 0, 0);
        while (t < spotChangeTime)
        {
            float perc = t / spotChangeTime;
            transform.position = Bezier2(startpos, midpos, endpos, perc);
            transform.rotation = Quaternion.Slerp(startrot, stunnedSpot.rotation, perc);
            t += Time.deltaTime;
            yield return null;
        }
        stunned = true;

        transform.position = endpos;
        transform.rotation = stunnedSpot.rotation;
        t = 0;
        _anim.Play("jump_landing", 0, 0);
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        currentSpot = stunnedSpot;
        nextAction = BossActions.NONE;
        //busy = false;
    }
    public bool stunned = false;
    public void TakeDamage()
    {
        busy = true;
        phase++;
        StartCoroutine(ChangeToSpot(barrelThrowSpots[Random.Range(0, barrelThrowSpots.Count)]));
    }
}
