using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombItem : ComposedItem
{
    [SerializeField] private float radius = 0.5f;
    private LayerMask breakableMask;


    private void Start(){
        breakableMask = 1 << 8;
    }
    private void Update(){
        CheckBomb();
    }

    private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

    private void CheckBomb(){
        var hit = Physics2D.OverlapCircle(transform.position, radius, breakableMask);
        if(null == hit)return;
        else{
            //bomb anim
            Destroy(hit.gameObject);
            DestroySelf();
        }
    }
}
