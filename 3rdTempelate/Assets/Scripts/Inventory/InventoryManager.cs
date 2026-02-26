using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [SerializeField] private List<InventorySlot> slotList = new List<InventorySlot>();
    public List<InventorySlot> slots => slotList;

    [SerializeField] private int capacity = 20;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    public bool AddItem(ItemData item,int amount)
    {
        if(item.isStackable)
        {
            foreach(var slot in slotList)
            {
                if(slot.item == item)
                {
                    slot.count = amount;
                    return true;
                }
            } 
        }
        if(slotList .Count < capacity)
        {
            slotList.Add(new InventorySlot(item, amount));
            return true;
        }
        return false;
    }
}
