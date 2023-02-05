using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnManager : Singleton<BtnManager>
{
    public void Tab(GameObject obj)
    {
        if (!obj.activeSelf)
            obj.SetActive(true);
        else
            obj.SetActive(false);
    }
}
