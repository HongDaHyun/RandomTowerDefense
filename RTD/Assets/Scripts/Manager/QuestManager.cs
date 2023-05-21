using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : Singleton<QuestManager>
{
    [HideInInspector] public int killEnemyNum, questNum, gatchaNum;
    [HideInInspector] public int evolNum;
    [HideInInspector] public List<UnitType> unitTypesList;
    [HideInInspector] public int[] tierNum = new int[] { 0, 0, 0, 0 };
    [HideInInspector] public int[] unitNum = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
}
