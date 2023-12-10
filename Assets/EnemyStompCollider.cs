using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStompCollider : MonoBehaviour
{
    EnemyController_basic controller;

    void Start()
    {
        controller = GetComponentInParent<EnemyController_basic>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.tag);
        if (collision.collider.CompareTag("Player"))
        {
            //collision.collider push up
            controller.GotStompedOn();
        }
    }
}
