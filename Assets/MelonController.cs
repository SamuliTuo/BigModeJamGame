using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonController : MonoBehaviour
{
    public bool zoneMode = false;
    public int value = 1;

    [HideInInspector] public bool wasCollected = false;

    [SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private float bobSpeed = 1.0f;
    [SerializeField] private float bobRange = 1.0f;

    private Transform model;
    //Vector3 startPos;


    private void Start()
    {
        //startPos = transform.position;
        model = transform.GetChild(0);
    }

    private void Update()
    {
        float distance = Mathf.Sin(Time.timeSinceLevelLoad * bobSpeed);
        model.localPosition = Vector3.up * distance * bobRange;
        model.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }

    public void ThisWasCollected()
    {
        wasCollected = true;
        Destroy(gameObject);
    }
}
