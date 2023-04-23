using System;
using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int horizontalSize;
    public int verticalSize;
    public float mineRatio;
    public int loopLimit;
    int numMines;
    bool freeSpace = false;
    int centerx;
    int centery;
    public static sbyte[,] gameGrid;
    public static sbyte[,] visibleGrid;
    public static sbyte gameReady = 0;
    bool endGenMessageSent = false;
    GameObject TileBase;
    GameObject GridParent;
    public Sprite[] localSprites;
    public static Sprite[] sprites;
    public static TileHandler[,] tiles;
    CubeHandler cube;
    public static sbyte[] cubeSides;
    public sbyte[] cubeSidesOverride;
    public bool cubeSidesOverrideActive;
    public Rigidbody cubeBody;
    public static bool gameOver;
    public static int revealedTiles;
    int totalTiles;
    void Start()
    {
        gameOver = false;
        revealedTiles = 0;
        TileBase = GameObject.Find("TileBase");
        GridParent = GameObject.Find("GridParent");
        cube = GameObject.Find("PlayerCube").GetComponent<CubeHandler>();
        numMines = (int)Mathf.Floor(horizontalSize * verticalSize * mineRatio);
        if (numMines == 0) numMines = 1;
        totalTiles = horizontalSize * verticalSize;
        if (totalTiles - numMines >= 8 && horizontalSize>=3&&verticalSize>=3) freeSpace = true;
        centerx = horizontalSize / 2;
        centery = verticalSize / 2;
        sprites = new Sprite[localSprites.Length];
        for (int i=0;i<localSprites.Length;i++)
        {
            sprites[i] = localSprites[i];
        }
        //Debug.Log(numMines);
        GenerateGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!endGenMessageSent)
        {
            if (gameReady == 0) Debug.Log("Game Loading...");
            else if (gameReady == -1)
            {
                Debug.Log("Error in Field Generation");
                endGenMessageSent = true;
            }
            else
            {
                /*
                Debug.Log(horizontalSize + " x " + verticalSize + " game with " + numMines + " mines at a " + mineRatio + " ratio.");
                for (int r = 0; r < verticalSize; r++)
                {
                    string line = "";
                    for (int c = 0; c < horizontalSize; c++)
                    {
                        line += gameGrid[r, c] + " ";
                    }
                    Debug.Log(line);
                }
                */
                if (cubeSidesOverrideActive) cubeSides = cubeSidesOverride;
                GameObject.Find("Directional Light").transform.position = tiles[tiles.GetLength(0) - 1, tiles.GetLength(1) - 1].transform.position + new Vector3(0, 10, 0);
                tiles[centery, centerx].Reveal();
                cube.PlayerSetup();
                endGenMessageSent = true;
            }
        }
        if (revealedTiles >= totalTiles - numMines && !gameOver) GameWon();
    }
    void GenerateGame()
    {
        //Visible: true or false
        //-1: Mine
        //0: Nothing
        //1-8: Number of Adjacent Mines
        //
        //Also this is bad so all arrays are in the form [y,x]
        int currentMines = 0;
        int i = 0;
        gameGrid = new sbyte[verticalSize,horizontalSize];
        visibleGrid = new sbyte[verticalSize, horizontalSize];
        tiles = new TileHandler[verticalSize, horizontalSize];
        while (currentMines < numMines)
        {
            for (int r = 0; r < verticalSize; r++)
            {
                for (int c = 0; c < horizontalSize; c++)
                {
                    if (UnityEngine.Random.Range(0.0f, 1.0f) < mineRatio / 2)
                    {
                        if (gameGrid[r, c] != -1 && !(centery==r&&centerx==c))
                        {
                            if (!(freeSpace&&Math.Abs(centery-r)<2 && Math.Abs(centerx - c) < 2))
                            {
                                //Debug.Log("Loop 1");
                                gameGrid[r, c] = -1;
                                currentMines++;
                            }
                        }
                    }
                    if (currentMines >= numMines) break;
                }
                if (currentMines >= numMines) break;
            }
            i++;
            if (i>=loopLimit)
            {
                Debug.Log("Loop Limit Reached! Error in game parameters?");
                gameReady = -1;
                return;
            }
        }
        for (int r = 0; r < verticalSize; r++)
        {
            for (int c = 0; c < horizontalSize; c++)
            {
                //Debug.Log("Loop 2");
                visibleGrid[r, c] = 0;
                if (gameGrid[r, c] != -1)
                {
                    sbyte adjacentMines = 0;
                    if (r > 0 && c > 0)
                    {
                        if (gameGrid[r-1, c-1] == -1) adjacentMines++;
                    }
                    if (r > 0)
                    {
                        if (gameGrid[r-1, c] == -1) adjacentMines++;
                    }
                    if (r > 0 && c < horizontalSize - 1)
                    {
                        if (gameGrid[r-1, c+1] == -1) adjacentMines++;
                    }
                    if (c > 0)
                    {
                        if (gameGrid[r, c-1] == -1) adjacentMines++;
                    }
                    if (c < horizontalSize - 1)
                    {
                        if (gameGrid[r, c+1] == -1) adjacentMines++;
                    }
                    if (r < verticalSize - 1 && c > 0)
                    {
                        if (gameGrid[r+1, c-1] == -1) adjacentMines++;
                    }
                    if (r < verticalSize - 1)
                    {
                        if (gameGrid[r+1, c] == -1) adjacentMines++;
                    }
                    if (r < verticalSize - 1 && c < horizontalSize - 1)
                    {
                        if (gameGrid[r+1, c+1] == -1) adjacentMines++;
                    }
                    gameGrid[r, c] = adjacentMines;                    
                }
                GameObject currentTile = Instantiate(TileBase, new Vector3(1f * c, 0, -1f * r), Quaternion.Euler(90,0,0), GridParent.transform);
                tiles[r, c] = currentTile.GetComponent<TileHandler>();
                tiles[r,c].Setup(r, c);
            }
        }
        gameReady = 1;
    }
    public void GameOver()
    {
        gameOver = true;
        cubeBody.useGravity = true;
        cubeBody.AddForce(new Vector3(UnityEngine.Random.Range(-1000f, 1000f), 3000, UnityEngine.Random.Range(-1000f, 1000f)));
        cubeBody.AddTorque(UnityEngine.Random.Range(-1000f, 1000f), UnityEngine.Random.Range(-1000f, 1000f), UnityEngine.Random.Range(-1000f, 1000f));
        for (int r = 0; r < verticalSize; r++)
        {
            for (int c = 0; c < horizontalSize; c++)
            {
                tiles[r, c].RevealMine();
            }
        }
        //TODO: Restart Menu
    }
    public void GameWon()
    {
        gameOver = true;
        Debug.Log("Winner!");
    }
}

