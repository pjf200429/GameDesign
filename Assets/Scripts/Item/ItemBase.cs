// ItemBase.cs
// 抽象基类：所有物品都继承自此类，定义通用接口
using UnityEngine;

public abstract class ItemBase
{
    // 唯一 ID，用于与数据库和 DropTable 对应
    public string ItemID { get; protected set; }

    // 显示用名称、图标等可由基类存储或让子类扩展
    public string DisplayName { get; protected set; }
    public Sprite Icon { get; protected set; }

    // 使用物品的抽象方法，由子类实现
    // target 通常是 GameObject（玩家、敌人等） 
    public abstract void Use(GameObject target);
}
