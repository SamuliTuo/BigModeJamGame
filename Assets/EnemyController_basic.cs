using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_basic : MonoBehaviour
{
    

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotStompedOn()
    {
        Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
