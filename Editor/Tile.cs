using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool isRoom = false;
    public bool isHallway = false;
    public int RoomNumber = 0;
    public int RoomLength = 0;
    public int RoomWidth = 0;
    public bool isStair = false;
    public bool hasCoins = false;
    public bool isSpawnPoint = false;

    public bool isInnerCorner = false;
    public bool isOuterCorner = false;
    public bool isRight = false;
    public bool isLeft = false;
    public bool isUp = false;
    public bool isDown = false;
    public bool hasWall = false;
    public bool duplicateWall = false;
    public bool isHalfFloor = false;
}
