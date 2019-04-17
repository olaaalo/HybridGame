using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicule : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public bool canMove = true;
    public float speed = 150;
    public float baseSpeed;
    public int life = 3;

    public void Start(){
        baseSpeed = speed;
    }

    public void ApplyColor(Color c)
    {
        meshRenderer.material.color = c;

        if (!canMove)
            transform.position = Vector3.up * 50000;
    }

    public void FixedUpdate()
    {
        if (GameManager.instance.gameIsEnded || !canMove) return;

        transform.Translate(Vector3.right * speed / 10 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 9){
            GameManager.instance.gameIsEnded = true;
        }
    }
}
