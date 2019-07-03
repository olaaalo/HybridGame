using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    public Rigidbody rb;
    private Rigidbody[] piecesRigidbodies;

    private void Start()
    {
        piecesRigidbodies = transform.GetComponentsInChildren<Rigidbody>();
    }

    public void DetachFromVehicle()
    {
        transform.SetParent(null);
        
        rb.useGravity = true;
        rb.isKinematic = false;

        rb.velocity = Vector3.up * Random.Range(0f, 3f);

        for (int i = 0; i < piecesRigidbodies.Length; ++i)
        {
            piecesRigidbodies[i].transform.SetParent(null);
            piecesRigidbodies[i].useGravity = true;
            piecesRigidbodies[i].isKinematic = false;
            piecesRigidbodies[i].velocity = Vector3.up * Random.Range(0f, 5f);
        }
    }
}
