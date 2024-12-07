using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawBoard : MonoBehaviour
{
    int boardHeight = 7;
    int boardWidth = 7;
    float spacing = 1.5f;
    Tilemap gameBoard;
    public Tile[] tiles;

    private void Start()
    {
        gameBoard = GetComponent<Tilemap>();
        DrawGrid();
    }


    void DrawGrid()
    {
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                // Create an offset for the grid.
                Vector3Int offset = new Vector3Int(-8, -3, -1);
                // Calculate the position with spacing.
                float posY = y * spacing;
                float posX = x * spacing;

                // Convert to Vector3Int for tilemap.
                Vector3Int cell = new Vector3Int(x, y, -1) + offset;

                int randomIndex = Random.Range(0, tiles.Length);
                // Set the tile.
                gameBoard.SetTile(cell, tiles[randomIndex]);
            }
        }
    }
}
