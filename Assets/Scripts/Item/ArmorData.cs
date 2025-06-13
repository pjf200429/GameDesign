// ArmorData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Item/ArmorData")]
public class ArmorData : EquipmentData
{
    [Header("Defense Attribute")]
    public int DefenseValue;      

    [Header("Addition attribute")]
    public float Weight;          
  
    public string SetName;        
}
