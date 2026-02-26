using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private ItemData itemdata;
    [SerializeField] private int amount = 1;

    public ItemData Item => itemdata;
    public int Amount => amount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(InventoryManager.Instance.AddItem(itemdata ,amount))
            {
                Destroy(gameObject);
            }
        }
    }
}
