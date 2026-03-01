using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Shop/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int healthBonus;
    public int attackBonus;
    public int defenseBonus;
    [TextArea] public string description;
}