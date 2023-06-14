using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacManager : Singleton<CharacManager>
{
    [HideInInspector] public int proportionHP;
    private void DoCharac(CharacteristicStat characStat)
    {
        switch (characStat.ID)
        {
            case 0:
                proportionHP += characStat.condition;
                break;
            case 1:
                break;
        }
    }
}
