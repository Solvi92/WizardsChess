using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; set; }
    private bool[,] AllowedMoves { get; set; }

    public bool isWhiteTurn = true;
    public ChessPiece[,] ChessPieces { get; set; }
    private ChessPiece selectedChessPiece;

    public List<GameObject> chessPiecePrefabs;
    private List<GameObject> activeChessPiece;
    
    private Quaternion orientation = Quaternion.Euler(0, 90, 0);

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5F;

    private int selectionX = -1;
    private int selectionY = -1;
    
    private void Start()
    {
        Instance = this;
        SpawnAllChessPieces();
    }
    private void Update()
    {
        UpdateSelection();
        DrawChessboard();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessPiece == null)
                {
                    // Select piece
                    SelectChessPiece(selectionX, selectionY);
                }
                else
                {
                    // Move the piece
                    MoveChessPiece(selectionX, selectionY);
                }
            }
        }
    }
    private void SelectChessPiece(int x, int y)
    {
        if (ChessPieces[x, y] == null)
        {
            return;
        }

        if (ChessPieces[x, y].IsWhite != isWhiteTurn)
        {
            return;
        }
        AllowedMoves = ChessPieces[x, y].PossibleMove();
        selectedChessPiece = ChessPieces[x, y];
        BoardIndicators.Instance.IndicatorAllowedMoves(AllowedMoves);
    }
    private void MoveChessPiece(int x, int y)
    {
        if(AllowedMoves[x, y])
        {
            ChessPiece c = ChessPieces[x, y];
            if (c != null && c.IsWhite != isWhiteTurn)
            {
                // Captured piece
                // If it is a king then check/endgame
                if (c.GetType() == typeof(King))
                {
                    // End game
                    return;
                }

                activeChessPiece.Remove(c.gameObject);
                Destroy(c.gameObject);
            }

            ChessPieces[selectedChessPiece.CurrentX, selectedChessPiece.CurrentY] = null;
            selectedChessPiece.transform.position = GetTileCenter(x, y);
            ChessPieces[x, y] = selectedChessPiece;
            ChessPieces[x, y].SetPos(x, y);
            isWhiteTurn = !isWhiteTurn;
        }

        BoardIndicators.Instance.HideIndicators();

        selectedChessPiece = null;
    }
    private void UpdateSelection()
    {
        if (!Camera.main)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }
    private void DrawChessboard()
    {
        // Draw the board
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i < 9; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j < 9; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);
            }
        }

        //Draw the selection
        if(selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));

            Debug.DrawLine(
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }
    private void SpawnChessPieces(int index, int x, int y)
    {
        GameObject go = Instantiate(chessPiecePrefabs[index], GetTileCenter(x,y), orientation) as GameObject;
        go.transform.SetParent(transform);
        ChessPieces[x, y] = go.GetComponent<ChessPiece>();
        ChessPieces[x, y].SetPos(x, y);
        activeChessPiece.Add(go);
    }
    private void SpawnAllChessPieces()
    {
        activeChessPiece = new List<GameObject>();
        ChessPieces = new ChessPiece[8, 8];
        // White
        // King
        SpawnChessPieces(0, 4, 0);
        // Queen
        SpawnChessPieces(1, 3, 0);
        // Rook
        SpawnChessPieces(2, 0, 0);
        SpawnChessPieces(2, 7, 0);
        // Bishop
        SpawnChessPieces(3, 2, 0);
        SpawnChessPieces(3, 5, 0);
        // Knight
        SpawnChessPieces(4, 1, 0);
        SpawnChessPieces(4, 6, 0);
        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessPieces(5, i, 1);
        }

        // Black
        // King
        SpawnChessPieces(6, 4, 7);
        // Queen
        SpawnChessPieces(7, 3, 7);
        // Rook
        SpawnChessPieces(8, 0, 7);
        SpawnChessPieces(8, 7, 7);
        // Bishop
        SpawnChessPieces(9, 2, 7);
        SpawnChessPieces(9, 5, 7);
        // Knight
        SpawnChessPieces(10, 1, 7);
        SpawnChessPieces(10, 6, 7);
        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessPieces(11, i, 6);
        }
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }
}
