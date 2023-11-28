using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbableItem : Item
{
    protected virtual void BeAbsorbed(){
        Debug.Log("be absorbed");
    }

}
