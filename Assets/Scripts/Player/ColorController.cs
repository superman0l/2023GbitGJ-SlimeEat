using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    public enum Color{
        Original,
        Pink,
        Purple,
        Green
    }

    private Color myColor = Color.Original;
    [SerializeField] private float radius = 0.5f;
    private LayerMask colorBrickMask;

    private void Start(){
        colorBrickMask = 1 << 10;
    }
    private void Update(){
        var hit = Physics2D.OverlapCircle(transform.position, radius, colorBrickMask);
        if(null == hit)return;
        else{
            if(hit.transform.GetComponent<ColorBrick>().GetColor == myColor){
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hit, true);
            }
            else{
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hit, false);
            }
        }
    }

    public void SetColor(ColorController.Color color){
        myColor = color;
    }

    public void ResetColor(){
        myColor = Color.Original;
    }
    public void SetPinkColor(){
        myColor = Color.Pink;
    }
    public void SetPurpleColor(){
        myColor = Color.Purple;
    }
}
