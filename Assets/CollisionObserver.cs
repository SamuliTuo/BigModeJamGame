using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CollisionObserver : MonoBehaviour
{
    ThirdPersonController _controller;
    PlayerAttacks _attacks;
    ObjectCollector _objectCollector;

    void Start()
    {
        _controller = GetComponent<ThirdPersonController>();
        _attacks = GetComponent<PlayerAttacks>();
        _objectCollector = GetComponent<ObjectCollector>();
    }

    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("EnemyStomper"))
        {
            hit.gameObject.GetComponent<EnemyStompCollider>()?.GetStompedOn();
            _controller.StompJump();
        }

        else if (hit.collider.CompareTag("Enemy"))
        {
            _controller.GotHit();
        }

        else if (hit.collider.CompareTag("Melon"))
        {
            //_objectCollector.CollectMelon();
        }
    }
}
