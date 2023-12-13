using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    private Rigidbody rb;
    private bool wasPushed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void DestroyMe()
    {
        ParticleEffects.instance.PlayParticle(Particles.CRATE_BREAK, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void PushMe(Vector3 pusherForward, float pushForce)
    {
        if (wasPushed)
            return;
        wasPushed = true;

        //rb.AddForce(pusherForward * pushForce * rb.mass, ForceMode.Impulse);
        rb.AddForce((pusherForward + Vector3.up * 0.3f) * pushForce * rb.mass, ForceMode.Impulse);
        //StartCoroutine(Pushed(pusherForward, pushForce));
    }



    IEnumerator Pushed(Vector3 pusherForward, float pushForce)
    {
        //rb.AddForce((pusherForward + Vector3.up * 0.3f) * pushForce * rb.mass, ForceMode.Impulse);
        yield return null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (wasPushed)
        {
            wasPushed = false;
            DestroyMe();
        }
    }
}
