using System;
using Unity.VisualScripting;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Prefabs && Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;



    // Logic
    private ChessPiece[,] chessPieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake()
    {
        generateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray,out info, 100, LayerMask.GetMask("Tile","Hover")))
        {
            // Get the indexes of the tile ive hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);

            // if we are hovering a tile after not hovering any tiles
            if(currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // if we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

        }
        else
        {
            if(currentHover == -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }

    // Generate the Board
    private void generateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for(int x = 0; x < tileCountX; x++)
        {
            for(int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize) - bounds;
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x+1) * tileSize, yOffset, (y+1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        tileObject.layer = LayerMask.NameToLayer("Tile");
        mesh.RecalculateNormals();

        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

   // Spawning of the pieces
   private void SpawnAllPieces()
    {
            chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

            int whiteTeam = 1, blackTeam = 0;

            // White Team
            chessPieces[0, 0] = SpawningSinglePiece(ChessPieceType.Rook, whiteTeam);
            chessPieces[1, 0] = SpawningSinglePiece(ChessPieceType.Knight, whiteTeam);
            chessPieces[2, 0] = SpawningSinglePiece(ChessPieceType.Bishop, whiteTeam);
            chessPieces[3, 0] = SpawningSinglePiece(ChessPieceType.King, whiteTeam);
            chessPieces[4, 0] = SpawningSinglePiece(ChessPieceType.Queen, whiteTeam);
            chessPieces[5, 0] = SpawningSinglePiece(ChessPieceType.Bishop, whiteTeam);
            chessPieces[6, 0] = SpawningSinglePiece(ChessPieceType.Knight, whiteTeam);
            chessPieces[7, 0] = SpawningSinglePiece(ChessPieceType.Rook, whiteTeam);
            for (int i = 0; i < TILE_COUNT_X; i++)
            {
                chessPieces[i, 1] = SpawningSinglePiece(ChessPieceType.Pawn, whiteTeam);
            }


        // Black Team
        chessPieces[0, 7] = SpawningSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawningSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawningSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawningSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[4, 7] = SpawningSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[5, 7] = SpawningSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawningSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawningSinglePiece(ChessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawningSinglePiece(ChessPieceType.Pawn, blackTeam);
        }

    }
    private ChessPiece SpawningSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type-1],transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];
        
        return cp;
    }

    // Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x,y] != null)
                {
                    // **Single
                    PositionSiglePieces(x, y, true);
                }
            }
        }
    }

    // PositionSingle***
    private void PositionSiglePieces(int x,int y, bool force=false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x, y].transform.position = GetTileCenter(x,y);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x*tileSize,yOffset, y*tileSize)-bounds + new Vector3(tileSize/2,0,tileSize/2);
    }
    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for(int x = 0; x < TILE_COUNT_X; x++) 
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);
            }
        }

        return -Vector2Int.one; // -1 -1
    }
}
