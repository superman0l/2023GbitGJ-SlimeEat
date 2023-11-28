using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFunctionManager : MonoSingleton<ItemFunctionManager>
{
    [SerializeField] private ItemSO higherItemSO;
    [SerializeField] private ItemSO lowerItemSO;
    [SerializeField] private ColorItemSO pinkItemSO;
    [SerializeField] private ColorItemSO purpleItemSO;
    [SerializeField] private PlatformJump.MoveController moveController;
    [SerializeField] private ColorController colorController;

    public delegate void OnAbsorbCallBack();
    public delegate void OnSpitCallBack();
    public Dictionary<ItemSO,OnAbsorbCallBack> AbsorbCallBack;
    public Dictionary<ItemSO,OnSpitCallBack> SpitCallBack;


    void Start()
    {
        AbsorbCallBack = new Dictionary<ItemSO, OnAbsorbCallBack>();
        SpitCallBack = new Dictionary<ItemSO, OnSpitCallBack>();

        AbsorbCallBack.Add(higherItemSO, moveController.BeHigher);
        AbsorbCallBack.Add(lowerItemSO, moveController.BeLower);
        AbsorbCallBack.Add(pinkItemSO, colorController.SetPinkColor);
        AbsorbCallBack.Add(purpleItemSO, colorController.SetPurpleColor);

        SpitCallBack.Add(higherItemSO, moveController.BeNormal);
        SpitCallBack.Add(lowerItemSO, moveController.BeNormal);
        SpitCallBack.Add(pinkItemSO, colorController.ResetColor);
        SpitCallBack.Add(purpleItemSO, colorController.ResetColor);
    }
}
