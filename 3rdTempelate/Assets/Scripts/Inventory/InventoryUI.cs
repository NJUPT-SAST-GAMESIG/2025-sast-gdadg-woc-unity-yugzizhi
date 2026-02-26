using UnityEngine.UI;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text countText;

    private InventorySlot currentSlotData;

    public void UpdateSlot(InventorySlot slotData)
    {
        currentSlotData = slotData;

        iconImage.sprite = slotData.item.icon;
        if (slotData.item.isStackable&&slotData.count > 1)
        {
            countText.text = slotData.count.ToString();
            countText.gameObject.SetActive(true);
        }
        else
        {
            countText.gameObject.SetActive(false);
        }
    }

    public void OnSlotClicked()
    {
        if(currentSlotData !=null&&currentSlotData .item!=null)
        {
            InventoryUIManager.Instance.ShowItemDetails(currentSlotData.item); 
        } 
    }
}
