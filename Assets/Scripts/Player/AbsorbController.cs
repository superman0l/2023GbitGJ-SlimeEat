using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AbsorbController : MonoBehaviour
{
    [SerializeField] private Vector3 canAbsorbPos;
    [SerializeField] private Transform spitPos;
    [SerializeField] private List<ComposeTableSO> allComposeTable;
    [SerializeField] private float canAbsorbItemTipDistance;

    private List<ItemSO> itemsAbsorbed;
    private LayerMask itemMask;
    private ItemSO selectedItem;

    public event Action<Vector3, ItemSO> OnAbsorbItem;

    private void Start(){
        itemsAbsorbed = new List<ItemSO>();
        itemMask = 1 << 7 | 1 << 11;
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.E)){
            Absorb();
        }
        if(Input.GetKeyDown(KeyCode.Q)){
            Spit();
        }
        Debug.DrawLine(transform.position, transform.position + canAbsorbPos * transform.localScale.x, Color.red);
    }

    private void CheckCompose(){
        if(itemsAbsorbed.Count < 2)return;

        foreach (var item in allComposeTable)
        {
            int composeItemCount = 0;
            List<ItemSO> willComposeItems = new List<ItemSO>();
            foreach(var willComposeItem in item.composeTableSO){
                bool haveItem = false;
                foreach(var absorbedItem in itemsAbsorbed){
                    if(absorbedItem == willComposeItem){
                        haveItem = true;
                        composeItemCount++;
                        willComposeItems.Add(absorbedItem);
                        break;
                    }
                }
                if(!haveItem)break;
            }
            if(composeItemCount == 2){
                ItemSO newItem = item.composedItemSO;
                foreach(var willComposeItem in willComposeItems){
                    RemoveItem(willComposeItem);
                }
                AddItem(newItem);
                Debug.Log("spawn");
            }
        }
    }

    private void Absorb(){
        if(itemsAbsorbed.Count == 3){
            //anim
            return;
        }
        

        var hit = Physics2D.Linecast(transform.position, transform.position + canAbsorbPos * transform.localScale.x, itemMask);
        if(null == hit.collider)return;
        Item item = hit.transform.GetComponent<Item>();
        ItemSO itemSO = item.GetItemSO;
        AddItem(itemSO);

        OnAbsorbItem?.Invoke(hit.transform.position, itemSO);

        item.DestroySelf();

        CheckCompose();
        //AbsorbFunction();
    }

    private void Spit(){
        if(itemsAbsorbed.Count == 0)return;

        selectedItem = itemsAbsorbed.Last();//之后删掉这行
        Transform spawnSpitedItem = selectedItem.prefab;
        var tmp = Instantiate(spawnSpitedItem);
        tmp.transform.position = spitPos.transform.position;

        RemoveItem(selectedItem);
    }

    private void AddItem(ItemSO itemSO){
        itemsAbsorbed.Add(itemSO);
        if(ItemFunctionManager.Instance.AbsorbCallBack.ContainsKey(itemSO))
            ItemFunctionManager.Instance.AbsorbCallBack[itemSO]();
    }

    private void RemoveItem(ItemSO itemSO){
        itemsAbsorbed.Remove(itemSO);
        if(ItemFunctionManager.Instance.AbsorbCallBack.ContainsKey(itemSO))
            ItemFunctionManager.Instance.SpitCallBack[itemSO]();
    }

    private ItemSO GetNearestAbsorbItem(){
        var hit = Physics2D.OverlapCircle(transform.position, canAbsorbItemTipDistance, itemMask);
        if(null == hit)return null;
        else{
            return hit.GetComponent<Item>().GetItemSO;
        }
    }

    public List<ItemSO> GetItemsAbsorbed(){
        return itemsAbsorbed;
    }

    public void SetSelectedItem(ItemSO itemSO){
        selectedItem = itemSO;
    }
}
