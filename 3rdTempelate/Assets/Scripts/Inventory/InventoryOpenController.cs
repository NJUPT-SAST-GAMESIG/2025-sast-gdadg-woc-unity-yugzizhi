using UnityEngine;

public class InventoryOpenController : MonoBehaviour
{
    [SerializeField] private GameObject Inventory;
    [SerializeField] private InventoryUIManager inventoryUIManager;

    private bool isInventoryOpen = false;

    void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        Inventory.SetActive(isInventoryOpen);
        if(isInventoryOpen)
        {
            Time.timeScale = 0f;
            inventoryUIManager.RefreshUI();
            SetCursorState(true);
        }
        else
        {
            Time.timeScale = 1f;
            inventoryUIManager.CloseDetails();
            SetCursorState(false);
        }
    }
    private void SetCursorState(bool isMenuOpen)
    {
        if(isMenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false ;
        }
    }
}
