using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrbanAttackCollider : MonoBehaviour
{
    private PlayerAttacks attScript;
    private List<GameObject> hitObjects = new List<GameObject>();
    private bool attacking = false;
    private Transform root;
    float pushForce;

    private void Start()
    {
        attScript = GetComponentInParent<PlayerAttacks>();
        root = transform.parent;
    }

    public void InitAttack(float pushForce)
    {
        this.pushForce = pushForce;
        attacking = true;
        hitObjects.Clear();
    }
    public void StopAttack()
    {
        attacking = false;
        hitObjects.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!hitObjects.Contains(other.gameObject) && !other.CompareTag("Player"))
        {
            hitObjects.Add(other.gameObject);
            if (other.CompareTag("Breakable"))
            {
                other.GetComponent<BreakableObject>().PushMe(root.position + Vector3.up * 0.65f, pushForce);
            }
        }
    }
}
