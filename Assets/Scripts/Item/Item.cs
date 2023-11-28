using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]protected ItemSO itemSO;

    public ItemSO GetItemSO => itemSO;

    public virtual void DestroySelf(){
        Destroy(gameObject);
    }
}
