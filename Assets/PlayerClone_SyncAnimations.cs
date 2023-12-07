using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClone_SyncAnimations : MonoBehaviour
{
    Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }


    }
}
