using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBrick : MonoBehaviour
{
    [SerializeField] private ColorController.Color color;

    public ColorController.Color GetColor => color;
}
