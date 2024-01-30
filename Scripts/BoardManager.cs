using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Using random in order for level generation to work.
using Random = UnityEngine.Random;
// Importing System namespace to use Mathf
using System;

// Importing UnityEngine.UIElements namespace for clarity
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    // Nested class to define count ranges
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // Public variables
    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count(5, 9);
    public Count keyCount = new Count(1, 1);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] keyTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    // Private variables
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    // Method to initialize grid positions list
    void InitialiseList()
    {
        gridPositions.Clear();

        // Loop through all inner positions
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // Method to setup the board
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        // Loop through all positions including outer walls
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // Method to get a random position from the grid positions list
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);

        // Exclude the bottom-left 2x2 corner
        while (IsInBottomLeftCorner(gridPositions[randomIndex]))
        {
            randomIndex = Random.Range(0, gridPositions.Count);
        }

        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // Method to check if a position is in the bottom-left corner
    bool IsInBottomLeftCorner(Vector3 position)
    {
        return position.x < 3 && position.y < 3;
    }

    // Method to lay out objects at random positions
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        // Loop through and instantiate objects
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // Method to set up the board for a given level
    public void Setup(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(keyTiles, keyCount.minimum, keyCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);
    }
}
