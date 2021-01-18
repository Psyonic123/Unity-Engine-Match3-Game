using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private bool equipped;
    private int requiredClass;
    //change to enum?
    private List<ItemBonus> bonuses;

    public bool Equipped { get => equipped; set => equipped = value; }

    public void initialize(int r)
    {
        Equipped = false;
        requiredClass = r;
        bonuses = new List<ItemBonus>();
        bonuses.Add(new ItemBonus
        {
            type = ItemBonus.BonusType.MaxHealth,
            amount = 20
        });
    }

   /* public void displayIcon(vector2 pos) //TODO
    {
        //display the item's icon
    }

    public void displayInList(vector2 pos)
    {
        //display the item in the inventory list
    }

    public void displayStats(vector2 pos)
    {
        //display the item's bonuses, required class, etc.
    }
    */
}
