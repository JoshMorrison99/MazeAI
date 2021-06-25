using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject[] Tile;
    int groundOffsetX = 0;
    int groundOffsetZ = 0;
    public int boardSize = 50;
    public int dungeonRoomWidth = 0;
    public int dungeonRoomLength = 0;
    public Tile CheckRoomTile;
    public GameObject[,] DungeonBoard;
    public int[] DungeonHallwaysX;
    public int[] DungeonHallwaysZ;
    public int GenerateDungeonRoomX;
    public int GenerateDungeonRoomZ;
    public int NumDungeons = 0;
    public int dungeonRoomNumber = 1;
    public GameObject Corner;
    public GameObject[] SideWall;
    public GameObject Pilar;
    public GameObject PilarOne;
    public GameObject StairCase;
    public GameObject TriplePilar;
    public int randomStairCase;
    public int randomSpawnPoint;
    public bool hasStairs = false;
    public bool hasSpawn = false;
    public GameObject player;
    public GameObject halfFloor;

    // Amount of time till the function SpawnEnemies() is called again
    private float timeEnemySpawn = 15f;


    // Start is called before the first frame update
    void Awake()
    {
        Time.timeScale = 1;
        NumDungeons = Random.Range(4, 12);
        randomStairCase = Random.Range(1, NumDungeons);
        randomSpawnPoint = Random.Range(1, NumDungeons);
        GenerageDungeonBoard();
        for (int i = 0; i < NumDungeons; i++)
        {
            GenerateDungeonRoomX = Random.Range(7, boardSize - 7);
            GenerateDungeonRoomZ = Random.Range(7, boardSize - 7);
            GenerateDungeonRoom(GenerateDungeonRoomX, GenerateDungeonRoomZ, dungeonRoomNumber);
            dungeonRoomNumber++;
        }
        Debug.Log("Generating Hallways...");
        for (int i = 1; i < NumDungeons; i++)
        {
            GenerateHallways(i, i - 1);
        }
        GenerateWallValue();
        GenerateWalls();
        RemoveBoardOutline();

        transform.position = new Vector3(0,0,0);
    }
    public void GenerageDungeonBoard()
    {
        int randomTile = Random.Range(0, Tile.Length);
        //Create Dungeon Board
        CheckRoomTile = Tile[randomTile].GetComponent<Tile>();
        DungeonBoard = new GameObject[boardSize, boardSize];
        DungeonHallwaysX = new int[NumDungeons];
        DungeonHallwaysZ = new int[NumDungeons];
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {

                randomTile = Random.Range(0, Tile.Length);
                CheckRoomTile.isRoom = false;
                GameObject newTile = Instantiate(Tile[randomTile], new Vector3(Tile[randomTile].transform.localPosition.x + groundOffsetX, 0, Tile[randomTile].transform.localPosition.x + groundOffsetZ), Quaternion.identity);
                newTile.transform.parent = this.transform;
                DungeonBoard[i, j] = newTile;
                groundOffsetX = groundOffsetX + 5;
            }
            groundOffsetZ = groundOffsetZ + 5;
            Tile[randomTile].transform.localPosition = new Vector3(0, 0, 0);
            groundOffsetX = 0;
        }
    }

    public void GenerateDungeonRoom(int start, int end, int dungeonRoomNumber)
    {
        dungeonRoomWidth = Random.Range(3, 7);
        dungeonRoomLength = Random.Range(3, 7);
        for (int i = 0; i < dungeonRoomWidth; i++)
        {
            for (int j = 0; j < dungeonRoomLength; j++)
            {
                //Sets the tile to wether its a dungeon room or not
                DungeonBoard[start + i, end + j].GetComponent<Tile>().isRoom = true;
                //Sets the tile to what dungeon room number it is
                DungeonBoard[start + i, end + j].GetComponent<Tile>().RoomNumber = dungeonRoomNumber;
                //Set the spawn point
                if (randomSpawnPoint == dungeonRoomNumber && hasSpawn == false)
                {
                    int RandomWidth = Random.Range(0, dungeonRoomWidth - 1);
                    int RandomLength = Random.Range(0, dungeonRoomLength - 1);
                    DungeonBoard[start + RandomWidth, end + RandomLength].GetComponent<Tile>().isSpawnPoint = true;
                    hasSpawn = true;
                    Transform spawnPoint = DungeonBoard[start + RandomWidth, end + RandomLength].transform;
                    player.transform.localPosition = new Vector3(spawnPoint.transform.localPosition.x, 0.1f, spawnPoint.transform.localPosition.z);
                }
                //Set the Stairs
                if (randomStairCase == dungeonRoomNumber && hasStairs == false)
                {
                    int RandomWidth = Random.Range(0, dungeonRoomWidth - 1);
                    int RandomLength = Random.Range(0, dungeonRoomLength - 1);
                    DungeonBoard[start + RandomWidth, end + RandomLength].GetComponent<Tile>().isStair = true;
                    hasStairs = true;
                    GameObject stairCase = Instantiate(StairCase, new Vector3(0, 0, 0), Quaternion.identity);
                    stairCase.transform.Rotate(0, 0, 0);
                    stairCase.transform.localPosition = new Vector3(DungeonBoard[start + RandomWidth, end + RandomLength].transform.localPosition.x, DungeonBoard[start + RandomWidth, end + RandomLength].transform.localPosition.y + 0.5f, DungeonBoard[start + RandomWidth, end + RandomLength].transform.localPosition.z);
                }
            }
        }
        //Add a Hallway
        int randomNumX = Random.Range(0, dungeonRoomWidth);
        int randomNumZ = Random.Range(0, dungeonRoomLength);
        DungeonBoard[start + randomNumX, end + randomNumZ].GetComponent<Tile>().isHallway = true;
        //Gets the X and Z position of each hallway enterance and stores them separetly in an array (Why separatly? because im stupid)
        DungeonHallwaysX[dungeonRoomNumber - 1] = start + randomNumX;
        DungeonHallwaysZ[dungeonRoomNumber - 1] = end + randomNumZ;
        //Using Math to calculate the rise over run between 2 hallways
    }

    public void GenerateHallways(int hallway2, int hallway1)
    {
        //Using Math to calculate the rise over run between 2 hallways
        int rise = 0;
        int run = 0;
        run = DungeonHallwaysX[hallway2] - DungeonHallwaysX[hallway1];
        rise = DungeonHallwaysZ[hallway2] - DungeonHallwaysZ[hallway1];
        for (int i = 0; i < Mathf.Abs(run); i++)
        {
            if (run > 0)
            {
                DungeonBoard[DungeonHallwaysX[hallway1] + i, DungeonHallwaysZ[hallway1]].GetComponent<Tile>().isHallway = true;
            }
            else if (run < 0)
            {
                DungeonBoard[DungeonHallwaysX[hallway1] - i, DungeonHallwaysZ[hallway1]].GetComponent<Tile>().isHallway = true;
            }

        }
        for (int i = 0; i < Mathf.Abs(rise); i++)
        {
            if (rise > 0)
            {
                DungeonBoard[DungeonHallwaysX[hallway1] + run, DungeonHallwaysZ[hallway1] + i].GetComponent<Tile>().isHallway = true;
            }
            else if (rise < 0)
            {
                DungeonBoard[DungeonHallwaysX[hallway1] + run, DungeonHallwaysZ[hallway1] - i].GetComponent<Tile>().isHallway = true;
            }
        }
    }

    public void GenerateWallValue()
    {
        for (int i = 1; i < boardSize - 1; i++)
        {
            for (int j = 1; j < boardSize - 1; j++)
            {
                //Checks if the tile is NOT a dungeon room or hallway
                if (DungeonBoard[i, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j].GetComponent<Tile>().isHallway == false)
                {
                    //Check if the Tile Has a hallway or a room to the right of it
                    if (DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true)
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isRight = true;
                        DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                    }
                    //Check if the Tile Has a hallway or a room to the left of it
                    if (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true)
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isLeft = true;
                        DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                    }
                    //Check if the Tile Has a hallway or a room to the Up of it
                    if (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true)
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isUp = true;
                        DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                    }
                    //Check if the Tile Has a hallway or a room to the Down of it
                    if (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true)
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isDown = true;
                        DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                    }
                    //Checks if [i+1,j+1] is a corner OuterCorner
                    if (DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false && (DungeonBoard[i + 1, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j + 1].GetComponent<Tile>().isHallway == true))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner = true;
                    }
                    //Checks if [i-1,j-1] is a corner OuterCorner
                    if (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false && (DungeonBoard[i - 1, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j - 1].GetComponent<Tile>().isHallway == true))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner = true;
                    }
                    //Checks if [i-1,j+1] is a corner OuterCorner
                    if (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false && (DungeonBoard[i - 1, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j + 1].GetComponent<Tile>().isHallway == true))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner = true;
                    }
                    //Checks if [i+1,j+1] is a corner InnerCorner
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                    }

                }
                if (DungeonBoard[i, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j].GetComponent<Tile>().isHallway == true)
                {
                    DungeonBoard[i, j].GetComponent<Tile>().isHalfFloor = false;
                }
                else if (DungeonBoard[i, j].GetComponent<Tile>().hasWall)
                {
                    DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                    DungeonBoard[i, j].SetActive(false);
                }
                else
                {
                    //Inactive Tiles
                    DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                    DungeonBoard[i, j].SetActive(false);
                }
            }
        }
    }

    public void GenerateWalls()
    {
        for (int i = 1; i < boardSize - 1; i++)
        {
            for (int j = 1; j < boardSize - 1; j++)
            {
                //Checks if the tile is NOT a dungeon room or hallway
                if (DungeonBoard[i, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j].GetComponent<Tile>().isHallway == false)
                {
                    //Check if the Tile Has a hallway or a room to the right of it
                    if (DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true)
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isRight = true;
                        DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                        GameObject halfFloorModel = Instantiate(halfFloor, DungeonBoard[i, j].transform.localPosition, Quaternion.identity);
                        halfFloorModel.GetComponent<Tile>().isHalfFloor = true;
                        halfFloorModel.transform.localPosition += new Vector3(0, 0, 1.25f);
                    }
                    //Check if the Tile Has a hallway or a room to the left of it
                    if (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true)
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isLeft = true;
                        DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                        GameObject halfFloorModel = Instantiate(halfFloor, DungeonBoard[i, j].transform.localPosition, Quaternion.identity);
                        halfFloorModel.GetComponent<Tile>().isHalfFloor = true;
                        halfFloorModel.transform.localPosition += new Vector3(0, 0, -1.25f);
                    }
                    //Check if the Tile Has a hallway or a room to the Up of it
                    if (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true)
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isUp = true;
                        DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                        GameObject halfFloorModel = Instantiate(halfFloor, DungeonBoard[i, j].transform.localPosition, Quaternion.identity);
                        halfFloorModel.GetComponent<Tile>().isHalfFloor = true;
                        halfFloorModel.transform.localPosition += new Vector3(1.25f, 0, 0);
                        halfFloorModel.transform.Rotate(0, 90f, 0);
                    }
                    //Check if the Tile Has a hallway or a room to the Down of it
                    if (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true)
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isDown = true;
                        DungeonBoard[i, j].GetComponent<Tile>().hasWall = true;
                        GameObject halfFloorModel = Instantiate(halfFloor, DungeonBoard[i, j].transform.localPosition, Quaternion.identity);
                        halfFloorModel.GetComponent<Tile>().isHalfFloor = true;
                        halfFloorModel.transform.localPosition += new Vector3(-1.25f, 0, 0);
                        halfFloorModel.transform.Rotate(0, 90f, 0);
                    }

                    //Checks if [i+1,j+1] is a corner OuterCorner
                    if (DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false && (DungeonBoard[i + 1, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j + 1].GetComponent<Tile>().isHallway == true))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner = true;
                        if ((DungeonBoard[i, j - 1].GetComponent<Tile>().isRight) || (DungeonBoard[i, j - 1].GetComponent<Tile>().isLeft) || (DungeonBoard[i, j - 1].GetComponent<Tile>().isOuterCorner))
                        {
                            //Checks for a triple pilar
                            GameObject TriplePilarClone = Instantiate(TriplePilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            TriplePilarClone.transform.Rotate(0, 180f, 0);
                        }
                        if ((DungeonBoard[i - 1, j].GetComponent<Tile>().isUp) || (DungeonBoard[i - 1, j].GetComponent<Tile>().isDown) || (DungeonBoard[i - 1, j].GetComponent<Tile>().isOuterCorner))
                        {
                            //Checks for a triple pilar
                            GameObject TriplePilarClone = Instantiate(TriplePilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            TriplePilarClone.transform.Rotate(0, 270f, 0);
                        }
                        else
                        {
                            GameObject SideWallClone = Instantiate(Corner, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            SideWallClone.transform.Rotate(0, 180f, 0);
                        }
                    }
                    //Checks if [i-1,j-1] is a corner OuterCorner
                    if (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false && (DungeonBoard[i - 1, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j - 1].GetComponent<Tile>().isHallway == true))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner = true;
                        if ((DungeonBoard[i, j + 1].GetComponent<Tile>().isRight) || (DungeonBoard[i, j + 1].GetComponent<Tile>().isLeft || (DungeonBoard[i, j + 1].GetComponent<Tile>().isOuterCorner)))// checked Once
                        {
                            //Checks for triple pilar
                            GameObject TriplePilarClone = Instantiate(TriplePilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            TriplePilarClone.transform.Rotate(0, 0, 0); // checked Once
                        }
                        if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isUp) || (DungeonBoard[i + 1, j].GetComponent<Tile>().isDown || (DungeonBoard[i + 1, j].GetComponent<Tile>().isOuterCorner)))
                        {
                            //Checks for triple pilar
                            GameObject TriplePilarClone = Instantiate(TriplePilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            TriplePilarClone.transform.Rotate(0, 90f, 0); // checked Once
                        }
                        else
                        {
                            GameObject SideWallClone = Instantiate(Corner, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            SideWallClone.transform.Rotate(0, 0, 0);
                        }
                    }
                    //Checks if [i+1,j-1] is a corner OuterCorner
                    if (DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false && (DungeonBoard[i + 1, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j - 1].GetComponent<Tile>().isHallway == true))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner = true;
                        if ((DungeonBoard[i, j + 1].GetComponent<Tile>().isRight) || (DungeonBoard[i, j + 1].GetComponent<Tile>().isLeft || (DungeonBoard[i, j + 1].GetComponent<Tile>().isOuterCorner))) //Checked Once
                        {
                            //Checks for triple pilar
                            GameObject TriplePilarClone = Instantiate(TriplePilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            TriplePilarClone.transform.Rotate(0, 180f, 0); // checked Once
                        }
                        if ((DungeonBoard[i - 1, j].GetComponent<Tile>().isUp) || (DungeonBoard[i - 1, j].GetComponent<Tile>().isDown || (DungeonBoard[i - 1, j].GetComponent<Tile>().isOuterCorner)))
                        {
                            //Checks for triple pilar
                            GameObject TriplePilarClone = Instantiate(TriplePilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            TriplePilarClone.transform.Rotate(0, 90f, 0);
                        }
                        else
                        {
                            GameObject SideWallClone = Instantiate(Corner, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            SideWallClone.transform.Rotate(0, 90f, 0);
                        }
                    }
                    //Checks if [i-1,j+1] is a corner OuterCorner
                    if (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false && (DungeonBoard[i - 1, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j + 1].GetComponent<Tile>().isHallway == true))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner = true;
                        if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isDown) || (DungeonBoard[i + 1, j].GetComponent<Tile>().isUp || (DungeonBoard[i + 1, j].GetComponent<Tile>().isOuterCorner)))//Checked Once
                        {
                            //Checks for triple pilar
                            GameObject TriplePilarClone = Instantiate(TriplePilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            TriplePilarClone.transform.Rotate(0, 270f, 0);
                        }
                        if ((DungeonBoard[i, j - 1].GetComponent<Tile>().isRight) || (DungeonBoard[i, j - 1].GetComponent<Tile>().isLeft || (DungeonBoard[i, j - 1].GetComponent<Tile>().isOuterCorner)))
                        {
                            //Checks for triple pilar
                            GameObject TriplePilarClone = Instantiate(TriplePilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            TriplePilarClone.transform.Rotate(0, 0, 0);
                        }
                        else
                        {
                            GameObject SideWallClone = Instantiate(Corner, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                            SideWallClone.transform.Rotate(0, 270f, 0);
                        }
                    }

                    //Checks if [i+1,j+1] is a corner InnerCorner
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                        GameObject SideWallClone = Instantiate(Corner, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.Rotate(0, 0, 0);
                    }
                    //Checks if [i-1,j-1] is a corner InnerCorner
                    if ((DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                        GameObject SideWallClone = Instantiate(Corner, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.Rotate(0, 180f, 0);
                    }
                    //Checks if [i-1,j+1] is a corner InnerCorner
                    if ((DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                        GameObject SideWallClone = Instantiate(Corner, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.Rotate(0, 90f, 0);
                    }
                    //Checks if [i+1,j-1] is a corner InnerCorner
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false && DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false && DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false))
                    {
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                        GameObject SideWallClone = Instantiate(Corner, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.Rotate(0, 270f, 0);
                    }


                    //Checks if there are 3 edges present
                    //Checks if right,down,up are present (Not Left)
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false))
                    {
                        GameObject SideWallClone = Instantiate(PilarOne, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.Rotate(0, 270f, 0);
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                    }
                    //Checks if right,down,left are present (Not Up)
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true))
                    {
                        GameObject SideWallClone = Instantiate(PilarOne, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.Rotate(0, 180f, 0);
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                    }
                    //Checks if right,down,left are present (Not Right)
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true))
                    {
                        GameObject SideWallClone = Instantiate(PilarOne, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.Rotate(0, 90f, 0);
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                    }
                    //Checks if right,down,left are present (Not Down)
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true))
                    {
                        GameObject SideWallClone = Instantiate(PilarOne, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.Rotate(0, 0f, 0);
                        DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner = true;
                    }

                    //Check if all 4 edges are connecting
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true && DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true && DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true))
                    {
                        Instantiate(Pilar, new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                    }

                    //Check if there is only one side aka a wall
                    //Checks if the up tile is a room or hallway
                    int randomWall = Random.Range(0, SideWall.Length);
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner == false && DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner == false && DungeonBoard[i, j].GetComponent<Tile>().duplicateWall == false))
                    {
                        GameObject SideWallClone = Instantiate(SideWall[randomWall], new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        DungeonBoard[i, j].GetComponent<Tile>().duplicateWall = true;
                        SideWallClone.transform.Rotate(0, 90f, 0);
                        SideWallClone.transform.localPosition = new Vector3(SideWallClone.transform.localPosition.x, SideWallClone.transform.localPosition.y + 2, SideWallClone.transform.localPosition.z);
                    }
                    //Checks if the down tile is a room or hallway
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == true || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner == false && DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner == false && DungeonBoard[i, j].GetComponent<Tile>().duplicateWall == false))
                    {
                        GameObject SideWallClone = Instantiate(SideWall[randomWall], new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        DungeonBoard[i, j].GetComponent<Tile>().duplicateWall = true;
                        SideWallClone.transform.Rotate(0, 270f, 0);
                        SideWallClone.transform.localPosition = new Vector3(SideWallClone.transform.localPosition.x, SideWallClone.transform.localPosition.y + 2, SideWallClone.transform.localPosition.z);
                    }
                    //Checks if the right tile is a room or hallway
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == false || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner == false && DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner == false && DungeonBoard[i, j].GetComponent<Tile>().duplicateWall == false))
                    {
                        GameObject SideWallClone = Instantiate(SideWall[randomWall], new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        DungeonBoard[i, j].GetComponent<Tile>().duplicateWall = true;
                        DungeonBoard[i, j].GetComponent<Tile>().duplicateWall = true;
                        SideWallClone.transform.localPosition = new Vector3(SideWallClone.transform.localPosition.x, SideWallClone.transform.localPosition.y + 2, SideWallClone.transform.localPosition.z);
                        SideWallClone.transform.Rotate(0, 180f, 0);
                    }
                    //Checks if the left tile is a room or hallway
                    if ((DungeonBoard[i + 1, j].GetComponent<Tile>().isRoom == false || DungeonBoard[i + 1, j].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j - 1].GetComponent<Tile>().isRoom == true || DungeonBoard[i, j - 1].GetComponent<Tile>().isHallway == true) && (DungeonBoard[i, j + 1].GetComponent<Tile>().isRoom == false || DungeonBoard[i, j + 1].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i - 1, j].GetComponent<Tile>().isRoom == false || DungeonBoard[i - 1, j].GetComponent<Tile>().isHallway == false) && (DungeonBoard[i, j].GetComponent<Tile>().isOuterCorner == false && DungeonBoard[i, j].GetComponent<Tile>().isInnerCorner == false && DungeonBoard[i, j].GetComponent<Tile>().duplicateWall == false))
                    {
                        GameObject SideWallClone = Instantiate(SideWall[randomWall], new Vector3(DungeonBoard[i, j].transform.localPosition.x, 0, DungeonBoard[i, j].transform.localPosition.z), Quaternion.identity);
                        SideWallClone.transform.localPosition = new Vector3(SideWallClone.transform.localPosition.x, SideWallClone.transform.localPosition.y + 2, SideWallClone.transform.localPosition.z);
                    }
                }
            }
        }
    }

    public void RemoveBoardOutline()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                DungeonBoard[0, i].SetActive(false);
                DungeonBoard[i, 0].SetActive(false);
                DungeonBoard[boardSize - 1, i].SetActive(false);
                DungeonBoard[i, boardSize - 1].SetActive(false);
            }
        }
    }

}

