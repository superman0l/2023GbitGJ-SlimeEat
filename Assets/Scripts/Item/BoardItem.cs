using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardItem : Item
{
    [SerializeField] private Vector3 downCheck;
    [SerializeField] private float floatY;
    [SerializeField] private float fallSpeed = 0.03f;
    private LayerMask groundMask;


    private void Start(){
        groundMask = 1 << 6;
    }
    private void Update(){
        if(transform.position.y < floatY)return;

        Debug.DrawLine(transform.position, transform.position + downCheck, Color.blue);
        var hit = Physics2D.Linecast(transform.position, transform.position + downCheck, groundMask);
        if(null == hit.collider){
            transform.position += fallSpeed * Vector3.down;
        }
    }
}
