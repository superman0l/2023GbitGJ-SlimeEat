using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1_Button : MonoBehaviour
{
    [SerializeField] private Transform door;

    private bool isTriggered = false;
    private float new_y = -4.1f;


    private void Update(){
        if(door.transform.position.y == new_y)return;
        if(isTriggered){
            Vector3 newPos = door.transform.position;
            newPos.y = new_y;
            door.transform.position = Vector3.Lerp(door.transform.position, newPos, 0.2f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isTriggered){
            isTriggered = true;
        }
        else return;
    }
}
