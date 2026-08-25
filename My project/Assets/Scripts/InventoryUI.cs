using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private Transform selectObject;
    [SerializeField] private GameObject RadialMenuRoot;
    private bool isRadialMenuActive;

    private int currentIndex = -1;

    private void Start()
    {
        isRadialMenuActive = false;

    }

    private void Update()
    {
        if (isRadialMenuActive)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 delta = Mouse.current.position.ReadValue() - screenCenter;

        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        if (angle < 0)
            angle += 360f;

        float sliceSize = 90f; // 4 slots
        int index = Mathf.FloorToInt(angle / sliceSize);

        if (index != currentIndex)
        {
            currentIndex = index;
        }

        float snappedAngle = index * sliceSize;

        selectObject.localEulerAngles = new Vector3(0, 0, snappedAngle);
    }

    public void CloseUI()
    {
        isRadialMenuActive = false;
        RadialMenuRoot.SetActive(false);
    }

    public void OpenUI()
    {
        isRadialMenuActive = true;
        RadialMenuRoot.SetActive(true);
    }

    public int GetSelectedIndex()
    {
        return currentIndex;
    }
}
