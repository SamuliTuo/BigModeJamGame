using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasetteController : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private float bobSpeed = 1.0f;
    [SerializeField] private float bobRange = 1.0f;
    [SerializeField] private Transform receiver = null;

    private Vector3 startPos;
    private Transform casetteModel;
    private List<Vector3> teleporterPositions = new List<Vector3>();

    private void Start()
    {
        startPos = transform.position;
        casetteModel = transform.GetChild(0);
        teleporterPositions.Add(transform.position);
        if (receiver != null) 
            teleporterPositions.Add(receiver.position);
    }

    void Update()
    {
        float distance = Mathf.Sin(Time.timeSinceLevelLoad * bobSpeed);
        casetteModel.position = startPos + Vector3.up * distance * bobRange;
        casetteModel.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            other.SendMessage("CassetteCollected", teleporterPositions);
        }
    }
}
