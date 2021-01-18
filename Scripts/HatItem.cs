using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HatItem : ClothingItem
{
    public void initialize(Sprite sprite)
    {
        img = GetComponent<Image>();
        img.sprite = sprite;
    }
}
