using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private PlayerInputActions input;

    [SerializeField] private List<ClubData> inventoyItems = new List<ClubData>();
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private Transform holdPosition;

    public event Action<ClubData> OnClubSelected;

    private ClubData currentHeldObject;

    private void Awake()
    {
        input = new PlayerInputActions();
        input.PlayerActions.OpenInventory.started += context => OpenInventory();
        input.PlayerActions.OpenInventory.canceled += context => CloseInventory();
    }

    private void OnEnable() => input.PlayerActions.Enable();
    private void OnDisable() => input.PlayerActions.Disable();

    private void OpenInventory()
    {
        inventoryUI.OpenUI();
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseInventory()
    {
        inventoryUI.CloseUI();
        Cursor.lockState = CursorLockMode.Locked;

        SelectCurrentItem();
    }

    private void SelectCurrentItem()
    {
        int currentIndex = inventoryUI.GetSelectedIndex();

        if (currentIndex < 0 || inventoyItems.Count <= currentIndex) return;

        ClubData selectedItem = inventoyItems[currentIndex];

        //If already holding an item
        if (holdPosition.childCount > 0)
        {
            Destroy(holdPosition.GetChild(0).gameObject);
        }

        //If selecting "None"
        if (selectedItem == null)
        {
            currentHeldObject = null;
            return;
        }

        currentHeldObject = selectedItem;
        OnClubSelected.Invoke(currentHeldObject);

        GameObject obj = Instantiate(selectedItem.prefab, holdPosition);
        obj.transform.SetParent(holdPosition);
    }

    public ClubData GetCurrentHeldObject()
    {
        return currentHeldObject;
    }
}
