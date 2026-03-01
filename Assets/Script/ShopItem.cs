using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItem : MonoBehaviour, IPointerClickHandler
{
    public ItemData data; // Drag your ItemData asset here
    private StatManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<StatManager>();
        if (manager == null) {
        Debug.LogError("ShopItem cannot find the StatManager in the scene! " +
                       "Make sure the script is attached to a GameObject.");
    }
    }

    // This triggers when the player clicks the object
    public void OnPointerClick(PointerEventData eventData)
    {
        manager.SelectItem(data);
    
    }
}