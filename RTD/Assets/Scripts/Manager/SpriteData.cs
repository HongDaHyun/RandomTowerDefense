using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteData : Singleton<SpriteData>
{
    public Sprite[] Stars;
    public Sprite[] CharacIcon;
    public Sprite[] CharacMaxIcon;
    public Sprite[] typeIcon;
    public TileMapSprite[] tileMapSprites;
}

[System.Serializable]
public struct TileMapSprite
{
    public Sprite ground1;
    public Sprite ground2;
}
