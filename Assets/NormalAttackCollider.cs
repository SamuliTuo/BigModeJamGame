using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackCollider : MonoBehaviour
{
    private PlayerAttacks attScript;
    private List<GameObject> hitObjects = new List<GameObject>();
    private bool attacking = false;

    private void Start()
    {
        attScript = GetComponentInParent<PlayerAttacks>();
    }

    public void InitAttack()
    {
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

            // boxes
            if (other.CompareTag("Breakable"))
            {
                other.GetComponent<BreakableObject>().DestroyMe();
            }

            // enemies
            if (other.CompareTag("Enemy"))
            {
                other.GetComponentInParent<EnemyController_basic>().GotAttacked();
            }
        }
    }
}
