using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData; // Assign your ScriptableObject here
    private StatManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<StatManager>();
    }

    // This built-in function detects clicks on objects with Colliders
    private void OnMouseDown()
    {
        if (manager != null)
        {
            manager.SelectItem(itemData);
        }
    }
}