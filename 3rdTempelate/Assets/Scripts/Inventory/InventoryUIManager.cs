using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [SerializeField] private InventoryManager manager;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform grid;

    [Header("œÍ«È√Ê∞Â")]
    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private GameObject detailsBackGround;
    [SerializeField] private Text nameText;
    [SerializeField] private Image detailIcon;
    [SerializeField] private Text descriptionText;


    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        foreach (Transform child in grid)
        {
            Destroy(child.gameObject);
        }
        foreach (InventorySlot slot in manager.slots)
        {
            GameObject newSlot = Instantiate(slotPrefab, grid);
            newSlot.GetComponent<InventoryUI>().UpdateSlot(slot);
        }
    }

    public void ShowItemDetails(ItemData item)
    {
        detailsPanel .SetActive(true);
        detailsBackGround.SetActive(true);
        nameText.text = item.itemName;
        detailIcon.sprite = item.icon;
        descriptionText.text = item.description;
    }
    public void CloseDetails()
    {
        detailsPanel.SetActive(false);
        detailsBackGround.SetActive(false);
    }
}
