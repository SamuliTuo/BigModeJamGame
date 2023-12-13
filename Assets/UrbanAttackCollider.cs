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
            if (other.CompareTag("Breakable"))
            {
                hitObjects.Add(other.gameObject);
                other.GetComponent<BreakableObject>().PushMe(transform.forward, pushForce);// root.position + Vector3.up * 0.7f, pushForce);
            }
            else if (other.CompareTag("Trashcan"))
            {
                var obj = other.GetComponentInParent<TrashcanController>();
                obj.GetKicked(transform, pushForce); //root.position + Vector3.up * 0.7f, pushForce);
                hitObjects.Add(obj.gameObject);
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    hitObjects.Add(obj.transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
