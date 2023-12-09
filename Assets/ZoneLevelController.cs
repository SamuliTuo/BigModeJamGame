using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLevelController : MonoBehaviour
{
    public Transform zoneLevelMidPoint;
    public float spawnDistance = 5f;
    public Transform levelOrientation;
    public Vector2 screenSize;
    public Camera cam;

    public static ZoneLevelController instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void StartZoneModeLevel(ZoneLevelScriptable levelData)
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        StartCoroutine(ZoneLevelCoroutine(levelData));
    }

    IEnumerator ZoneLevelCoroutine(ZoneLevelScriptable levelData)
    {
        List<ZoneLevelObstacle> obstacles = new List<ZoneLevelObstacle>(levelData.obstacles);
        List<ZoneLevelMelonSpawn> melons = new List<ZoneLevelMelonSpawn>(levelData.melons);
        float t = 0;

        while (t < levelData.levelLengthInSeconds)
        {
            for (var i = obstacles.Count - 1; i >= 0; i--)
            {
                if (t >= obstacles[i].obstacleTime - obstacles[i].obstacleTimeToReachPlayer)
                {
                    StartCoroutine(ObstacleCoroutine(obstacles[i]));
                    obstacles.RemoveAt(i);
                }
            }
            for (var i = melons.Count - 1; i >= 0; i--)
            {
                if (t >= melons[i].melonTime - melons[i].melonTimeToReachPlayer)
                {
                    StartCoroutine(MelonCoroutine(melons[i]));
                    melons.RemoveAt(i);
                }
            }

            t += Time.deltaTime;
            yield return null;
        }
        //end level
    }



    // Update the instantiated objects:

    IEnumerator ObstacleCoroutine(ZoneLevelObstacle obstacle)
    {
        float playerDistFromCamera = (cam.transform.position - zoneLevelMidPoint.position).magnitude;
        Vector3 offset = cam.ScreenToWorldPoint(new Vector3(screenSize.x * 0.5f, screenSize.y * 0.5f, playerDistFromCamera))
            - cam.ScreenToWorldPoint(new Vector3(screenSize.x * obstacle.obstacleScreenPos_X, screenSize.y * obstacle.obstacleScreenPos_y, playerDistFromCamera));

        Vector3 startpos = zoneLevelMidPoint.position + (levelOrientation.forward * spawnDistance) + offset;
        Vector3 endpos = zoneLevelMidPoint.position + offset;

        GameObject clone = Instantiate(obstacle.obstaclePrefab, startpos, Quaternion.LookRotation(levelOrientation.up));
        var wallMesh = CreateMesh(clone, playerDistFromCamera);
        float t = 0;

        while (t < obstacle.obstacleTimeToReachPlayer)
        {
            clone.transform.position = Vector3.Lerp(startpos, zoneLevelMidPoint.position, t / obstacle.obstacleTimeToReachPlayer);
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(clone);
    }

    public Transform tester1, tester2;

    IEnumerator MelonCoroutine(ZoneLevelMelonSpawn melon)
    {
        float playerDistFromCamera = (cam.transform.position - zoneLevelMidPoint.position).magnitude;
        Vector3 offset = cam.ScreenToWorldPoint(new Vector3(screenSize.x * 0.5f, screenSize.y * 0.5f, playerDistFromCamera)) 
            - cam.ScreenToWorldPoint(new Vector3(screenSize.x * melon.melonScreenPos_X, screenSize.y * melon.melonScreenPos_y, playerDistFromCamera));

        Vector3 startpos = zoneLevelMidPoint.position + (levelOrientation.forward * spawnDistance) + offset;
        Vector3 endpos = zoneLevelMidPoint.position + offset;

        var clone = Instantiate(melon.melonPrefab, startpos, levelOrientation.rotation);
        float t = 0;

        while (t < melon.melonTimeToReachPlayer)
        {
            Debug.DrawLine(startpos, startpos, Color.yellow);

            clone.transform.position = Vector3.Lerp(startpos, endpos, t / melon.melonTimeToReachPlayer);
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(clone);
    }


    Mesh CreateMesh(GameObject wallHole, float playerDistFromCamera)
    {
        MeshFilter meshFilter = wallHole.transform.GetChild(0).GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        var bounds = wallHole.GetComponent<MeshRenderer>().bounds;

        Vector3 botLeftPoint = cam.ScreenToWorldPoint(new Vector3(0, 0, playerDistFromCamera)) + (levelOrientation.forward * spawnDistance);
        Vector3 topRightPoint = cam.ScreenToWorldPoint(new Vector3(screenSize.x, screenSize.y, playerDistFromCamera)) + (levelOrientation.forward * spawnDistance);
        float posZ = bounds.min.z;

        Vector3[] vertices = new[] {
            // creating vertices of quad. aligning them in shape of square
            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, bounds.max.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, topRightPoint.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, bounds.max.y, posZ)),
            wallHole.transform.InverseTransformPoint(topRightPoint),

            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, bounds.max.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(bounds.min.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(bounds.min.x, bounds.max.y, posZ)),

            wallHole.transform.InverseTransformPoint(new Vector3(bounds.max.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(bounds.max.x, bounds.max.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, bounds.max.y, posZ)),

            wallHole.transform.InverseTransformPoint(botLeftPoint),
            wallHole.transform.InverseTransformPoint(new Vector3(botLeftPoint.x, bounds.min.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, botLeftPoint.y, posZ)),
            wallHole.transform.InverseTransformPoint(new Vector3(topRightPoint.x, bounds.min.y, posZ)),
        };
        mesh.vertices = vertices;


        // generate uv
        Vector2[] uv = new[] {
            // generate uv for corresponding vertices also in form of square
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
            new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
        };
        mesh.uv = uv;
        Vector3[] normals = new[] {
            // normals same as tris
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
            -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
        };
        mesh.normals = normals;
        int[] triangles = new[] {
            0,1,2,// first tris
            2,1,3,// second tris

            4,5,6,
            6,5,7,

            8,9,10,
            10,9,11,

            12,13,14,
            14,13,15
        };


        mesh.triangles = triangles;
        meshFilter.mesh = mesh;

        return mesh;
    }
}