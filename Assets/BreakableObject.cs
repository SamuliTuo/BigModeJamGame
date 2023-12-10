using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void DestroyMe()
    {
        ParticleEffects.instance.PlayParticle(Particles.CRATE_BREAK, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void PushMe(Vector3 pusherPos, float pushForce)
    {
        Vector3 dir = transform.position - pusherPos;
        dir.y *= 0;
        StartCoroutine(Pushed(dir.normalized, pushForce));
    }

    IEnumerator Pushed(Vector3 dir, float pushForce)
    {
        rb.AddForce((dir + Vector3.up * 0.3f) * pushForce * rb.mass, ForceMode.Impulse);
        yield return null;
    }
}
