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
    /// ���������׶εķ������У�
    /// �����ȡ����ģ�巿�� N �����������һ���̵귿��ĩβ׷�� BossRoomLevel{stageIndex}
    /// </summary>
    public List<string> BuildStageRooms(int stageIndex)
    {
        // У��׶����
        stageIndex = Mathf.Clamp(stageIndex, 1, totalStages);

        // ��������ҳ���ģ�巿��
        var pool = new List<string>(regularRoomSceneNames);
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var tmp = pool[i];
            pool[i] = pool[j];
            pool[j] = tmp;
        }

        // ȡǰ N �����淿
        var sequence = new List<string>();
        int take = Mathf.Min(regularRoomsPerStage, pool.Count);
        for (int i = 0; i < take; i++)
            sequence.Add(pool[i]);

        // ���λ�ò����̵귿
        int insertIdx = Random.Range(0, sequence.Count + 1);
        sequence.Insert(insertIdx, shopRoomSceneName);

        // ĩβ׷�� Boss ����
        sequence.Add($"BossRoomLevel{stageIndex}");

        return sequence;
    }

    /// <summary>
    /// һ���Թ������н׶εķ������У����׶�ƴ�ӣ�
    /// </summary>
    public List<string> BuildAllStages()
    {
        var all = new List<string>();
        for (int stage = 1; stage <= totalStages; stage++)
            all.AddRange(BuildStageRooms(stage));
        return all;
    }
}
