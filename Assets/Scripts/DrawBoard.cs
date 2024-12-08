using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawBoard : MonoBehaviour
{
    // Board properties.
    int boardHeight = 7;
    int boardWidth = 7;
    Tilemap gameBoard;
    public Tile[] tiles;

    // Audio properties.
    public AudioClip clearSound;
    public AudioClip shiftSound;
    public AudioClip refillSound;
    private AudioSource audioSource;

    // Tile dragging properties.
    private Vector3Int selectedCell = Vector3Int.zero;
    private bool isDragging = false;

    private void Start()
    {
        // Get the tilemap and audio source components.
        gameBoard = GetComponent<Tilemap>();
        audioSource = GetComponent<AudioSource>();

        // Draw the grid and start the game.
        DrawGrid();
        StartCoroutine(ClearMatchesAndShift());
    }

    void DrawGrid()
    {
        // Iterate through height and width and fill board with random tiles.
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, -1);
                int randomIndex = Random.Range(0, tiles.Length);
                gameBoard.SetTile(cell, tiles[randomIndex]);
            }
        }
    }

    private void Update()
    {
        // Handle input with mouse clicking and dragging.

        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int cell = GetMouseCellPosition();

            // Check if cell is within bounds and start dragging.
            if (IsWithinBounds(cell))
            {
                selectedCell = cell;
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            // Get the target cell and swap tiles if it is adjacent.
            Vector3Int targetCell = GetMouseCellPosition();
            if (IsWithinBounds(targetCell) && IsAdjacent(selectedCell, targetCell))
            {
                // Swap the two tiles.
                SwapTiles(selectedCell, targetCell, true); 
                StartCoroutine(ClearMatchesAndShift());
            }
            // Stop dragging after dragging is done.
            isDragging = false;
        }
    }

    void SwapTiles(Vector3Int cell1, Vector3Int cell2, bool isPlayerAction)
    {
        TileBase tile1 = gameBoard.GetTile(cell1);
        TileBase tile2 = gameBoard.GetTile(cell2);

        gameBoard.SetTile(cell1, tile2);
        gameBoard.SetTile(cell2, tile1);

        // Find matches and add to the matched cells hashset.
        HashSet<Vector3Int> matchedCells = FindMatches();

        if (matchedCells.Count == 0)
        {
            // No match found, swap back.
            gameBoard.SetTile(cell1, tile1);
            gameBoard.SetTile(cell2, tile2);
        }
    }

    IEnumerator ClearMatchesAndShift()
    {
        while (true)
        {
            // Allow clearing to be visible.
            yield return new WaitForSeconds(0.5f); 

            // Check for matches and clear them.
            HashSet<Vector3Int> matchedCells = FindMatches();
            if (matchedCells.Count > 0)
            {
                // If there's a match, play the clear sound.
                audioSource.PlayOneShot(clearSound);

                foreach (var cell in matchedCells)
                {
                    // Clear the matched tiles.
                    gameBoard.SetTile(cell, null); 
                }

                // Extra delay for visual feedback.
                yield return new WaitForSeconds(0.5f); 

                // Shift the cells down and play the shift sound.
                audioSource.PlayOneShot(shiftSound);
                ShiftCellsDown();

            }
            else
            {
                // Exits the loop if no matches are found.
                break; 
            }
        }
    }

    HashSet<Vector3Int> FindMatches()
    {
        HashSet<Vector3Int> matchedCells = new HashSet<Vector3Int>();

        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, -1);
                TileBase currentTile = gameBoard.GetTile(cell);

                if (currentTile != null)
                {
                    // Check matches in all directions.
                    CheckDirection(cell, Vector3Int.right, currentTile, matchedCells);
                    CheckDirection(cell, Vector3Int.up, currentTile, matchedCells);
                }
            }
        }

        // Return the matches cells to be cleared.
        return matchedCells;
    }

    void CheckDirection(Vector3Int startCell, Vector3Int direction, TileBase currentTile, HashSet<Vector3Int> matchedCells)
    {
        List<Vector3Int> matchCells = new List<Vector3Int> { startCell };

        for (int i = 1; i < 5; i++)
        {
            // Check the next cell in the direction, and add to match if it matches.
            Vector3Int nextCell = startCell + direction * i;
            if (!IsWithinBounds(nextCell)) break;

            TileBase nextTile = gameBoard.GetTile(nextCell);
            if (nextTile == currentTile)
            {
                matchCells.Add(nextCell);
            }
            else
            {
                // No match found, exit the loop.
                break;
            }
        }

        if (matchCells.Count >= 3)
        {
            foreach (var matchCell in matchCells)
            {
                // Add a match if the cell count is 3 or more.
                matchedCells.Add(matchCell);
            }
        }
    }

    void ShiftCellsDown()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, -1);
                if (gameBoard.GetTile(cell) == null)
                {
                    for (int ny = y + 1; ny < boardHeight; ny++)
                    {
                        // Check for a tile above the empty cell and shift it down.
                        Vector3Int aboveCell = new Vector3Int(x, ny, -1);
                        TileBase aboveTile = gameBoard.GetTile(aboveCell);

                        if (aboveTile != null)
                        {
                            // If there is a tile above, shift it down.
                            gameBoard.SetTile(cell, aboveTile);
                            gameBoard.SetTile(aboveCell, null);
                            break;
                        }
                    }

                    if (gameBoard.GetTile(cell) == null)
                    {
                        // If the cell is still empty, refill it with a random tile.
                        int randomIndex = Random.Range(0, tiles.Length);
                        gameBoard.SetTile(cell, tiles[randomIndex]);
                    }
                }
            }
        }
    }

    Vector3Int GetMouseCellPosition()
    {
        // Get the mouse position and convert it to a cell position.
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = gameBoard.WorldToCell(mousePosition);
        cellPosition.z = -1;
        return cellPosition;
    }

    bool IsWithinBounds(Vector3Int cell)
    {
        // Check if the cell is within the board bounds.
        return cell.x >= 0 && cell.x < boardWidth && cell.y >= 0 && cell.y < boardHeight;
    }

    bool IsAdjacent(Vector3Int cell1, Vector3Int cell2)
    {
        // Check if the two cells are adjacent.
        int dx = Mathf.Abs(cell1.x - cell2.x);
        int dy = Mathf.Abs(cell1.y - cell2.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }
}