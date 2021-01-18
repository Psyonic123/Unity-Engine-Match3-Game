using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class ClothingItem : Item
{
    public enum ClothingType
    {
        hat,
        armor,
        shoe
    };

    public ClothingType type;
    public Image img;
}
