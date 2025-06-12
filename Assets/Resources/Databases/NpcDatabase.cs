using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "NPCDatabase", menuName = "Game/NPC Database")]
public class NPCDatabase : ScriptableObject
{
    public List<NPC> NpcList;

    private void OnEnable()
    {
        InitNPCs();
    }

    // 根据ID查找事件
    public NPC GetNPCByName(string name) {
        return NpcList.Find(e => e.npcName == name);
    }

    public List<NPC> GetNPCsByLevel(int level) {
        return NpcList.Where(e => e.level == level).ToList();
    }

    private void InitNPCs() {
        foreach (var e in NpcList) {
            Debug.Log(e.npcName);
        }
    }

}