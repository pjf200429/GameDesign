using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomBuilder", menuName = "Game/RoomBuilder")]
public class RoomBuilder : ScriptableObject
{
    [Header("Regular Rooms (Template)")]
    [Tooltip("List of regular room scene names to pick from")]
    public List<string> regularRoomSceneNames;

    [Header("Shop Room")]
    [Tooltip("Fixed shop room scene name")]
    public string shopRoomSceneName = "ShopRoom";

    [Header("Stage Settings")]
    [Tooltip("How many regular rooms per stage")]
    public int regularRoomsPerStage = 4;
    [Tooltip("Total number of stages")]
    public int totalStages = 3;

    /// <summary>
    /// 构建单个阶段的房间序列：
    /// 随机抽取常规模板房间 N 个，随机插入一个商店房，末尾追加 BossRoomLevel{stageIndex}
    /// </summary>
    public List<string> BuildStageRooms(int stageIndex)
    {
        // 校验阶段序号
        stageIndex = Mathf.Clamp(stageIndex, 1, totalStages);

        // 深拷贝并打乱常规模板房间
        var pool = new List<string>(regularRoomSceneNames);
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var tmp = pool[i];
            pool[i] = pool[j];
            pool[j] = tmp;
        }

        // 取前 N 个常规房
        var sequence = new List<string>();
        int take = Mathf.Min(regularRoomsPerStage, pool.Count);
        for (int i = 0; i < take; i++)
            sequence.Add(pool[i]);

        // 随机位置插入商店房
        int insertIdx = Random.Range(0, sequence.Count + 1);
        sequence.Insert(insertIdx, shopRoomSceneName);

        // 末尾追加 Boss 场景
        sequence.Add($"BossRoomLevel{stageIndex}");

        return sequence;
    }

    /// <summary>
    /// 一次性构建所有阶段的房间序列（按阶段拼接）
    /// </summary>
    public List<string> BuildAllStages()
    {
        var all = new List<string>();
        for (int stage = 1; stage <= totalStages; stage++)
            all.AddRange(BuildStageRooms(stage));
        return all;
    }
}
