using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public class ChessBoard : MonoBehaviour
{
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private float deathSize = 0.3f;
    [SerializeField] private float deathSpacing = 0.3f;
    [SerializeField] private float dragOffset = 0.75f;
    [SerializeField] private GameObject VictoryScreen;


    [Header("Prefabs && Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;



    // Logic
    private ChessPiece[,] chessPieces;
    private ChessPiece currentlyDragging;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> deadWhites = new List<ChessPiece>();
    private List<ChessPiece> deadBlacks = new List<ChessPiece>();
    
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private bool isWhiteTurn;

    private SpecialMove specialMove;
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    private void Awake()
    {
        isWhiteTurn = true;

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
        if(Physics.Raycast(ray,out info, 100, LayerMask.GetMask("Tile","Hover", "Highlight")))
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
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // seleciona uma peça e a caracteriza como currently dragging
            //If we press down on the mouse
            if (Input.GetMouseButtonDown(0)) 
            {
                if (chessPieces[hitPosition.x, hitPosition.y] != null) {
                    // is it our turn ????
                    if ((chessPieces[hitPosition.x, hitPosition.y].team == 0 && isWhiteTurn) || (chessPieces[hitPosition.x, hitPosition.y].team == 1 && !isWhiteTurn)) { 
                        currentlyDragging = chessPieces[hitPosition.x, hitPosition.y];

                        //Get a list of where I can go, highlight tiles as well
                        availableMoves = currentlyDragging.GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                        // Get a list of special moves as well
                        specialMove = currentlyDragging.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);
                        HighlightTiles();

                    }
                }

            }
            //If we are releasing the mouse button
            if (currentlyDragging !=null && Input.GetMouseButtonUp(0) )
            {
                Vector2Int previousPosition = new Vector2Int(currentlyDragging.currentX, currentlyDragging.currentY);

                bool validMove = MoveTo(currentlyDragging, hitPosition.x, hitPosition.y);
                
                if (!validMove)
                    currentlyDragging.SetPosition(GetTileCenter(previousPosition.x, previousPosition.y));

                currentlyDragging = null;
                RemoveHighlightTiles();
            }
        }
        else
        {
            if(currentHover == -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = (ContainsValidMove(ref availableMoves, currentHover)) ? LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            if (currentlyDragging && Input.GetMouseButtonUp(0)) {
                currentlyDragging.SetPosition(GetTileCenter(currentlyDragging.currentX, currentlyDragging.currentY));
                currentlyDragging = null;
                RemoveHighlightTiles();
            }

        }

        //Dragging a piece
        if (currentlyDragging) 
        {
            Plane horizontalPlane = new Plane(Vector3.up, Vector3.up * yOffset);
            float distance = 0.0f;
            if (horizontalPlane.Raycast(ray, out distance))
                currentlyDragging.SetPosition(ray.GetPoint(distance) + Vector3.up * dragOffset);
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
            chessPieces[i, 1] = SpawningSinglePiece(ChessPieceType.Pawn, whiteTeam);


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
            chessPieces[i, 6] = SpawningSinglePiece(ChessPieceType.Pawn, blackTeam);

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
                    PositionSiglePieces(x, y, true);
                }
            }
        }
    }

    private void PositionSiglePieces(int x,int y, bool force=false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x, y].SetPosition( GetTileCenter(x,y), force);
    }

    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x*tileSize,yOffset, y*tileSize)-bounds + new Vector3(tileSize/2,0,tileSize/2);
    }

    //Highlight Tiles
    private void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }
    private void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }

    //checkMate
    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }

    private void DisplayVictory(int winningTeam)
    {
        VictoryScreen.SetActive(true);
        VictoryScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }

    public void onResetButton()
    {
        //UI
        VictoryScreen.transform.GetChild(0).gameObject.SetActive(false);
        VictoryScreen.transform.GetChild(1).gameObject.SetActive(false);
        VictoryScreen.SetActive(false);

        // Fields Reset
        currentlyDragging = null;
        availableMoves.Clear();
        moveList.Clear();

        // Limpar tabuleiro
        for(int x = 0; x < TILE_COUNT_X; x++)
        {
            for(int y = 0; y < TILE_COUNT_Y; y++)
            {
                if(chessPieces[x,y] != null)
                {
                    Destroy(chessPieces[x,y].gameObject);
                }

                chessPieces[x,y] = null;
            }
        }

        for(int i = 0; i < deadWhites.Count; i++)
        {
            Destroy(deadWhites[i].gameObject);
        }
        for(int i = 0; i < deadBlacks.Count; i++)
        {
            Destroy(deadBlacks[i].gameObject);
        }

        deadWhites.Clear();
        deadBlacks.Clear();

        SpawnAllPieces();
        PositionAllPieces();
        isWhiteTurn = true;
    }

    public void onExitButton()
    {
        Application.Quit();
    }

    // Special Moves
    private void ProcessSpecialMove()
    {
        if(specialMove == SpecialMove.EnPassant)
        {
            var newMove = moveList[moveList.Count-1];
            ChessPiece myPawn = chessPieces[newMove[1].x, newMove[1].y];
            var targetPawnPosition = moveList[moveList.Count - 2];
            ChessPiece enemyPawn = chessPieces[targetPawnPosition[1].x,targetPawnPosition[1].y];

            if(myPawn.currentX == enemyPawn.currentX)
            {
                if(myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1 )
                {
                    if(enemyPawn.team == 0)
                    {
                        deadWhites.Add(enemyPawn);
                        enemyPawn.SetScale(Vector3.one * deathSize);
                        enemyPawn.SetPosition(
                            new Vector3(8 * tileSize, yOffset, -1 * tileSize) 
                            - bounds 
                            + new Vector3(tileSize/2, 0, tileSize/2) 
                            + (Vector3.forward * deathSpacing) * deadWhites.Count);
                        
                    }else
                    {
                        deadBlacks.Add(enemyPawn);
                        enemyPawn.SetScale(Vector3.one * deathSize);
                        enemyPawn.SetPosition(
                            new Vector3(-1 * tileSize, yOffset, 8 * tileSize) 
                            - bounds 
                            + new Vector3(tileSize/2, 0, tileSize/2) 
                            + (Vector3.forward * deathSpacing) * deadBlacks.Count);
                        
                    }

                    chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
                }
            }
        }

        // MUDANÇA
        // Esse codigo é chamado nos scripts do rei e do Peão
        // PROMOÇÃO
        if (specialMove == SpecialMove.Promotion)
        {
            //Verifica o ultimo movimento feito pelo peão para verificar se ele esta na casa certae qual o peão que realizou o movimento
            Vector2Int[] lastMove = moveList[moveList.Count - 1];  
            ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

            // Caso a peça seja um peão
            if (targetPawn.type == ChessPieceType.Pawn)
            {   
                // time Branco
                if (targetPawn.team == 1 && lastMove[1].y == 7)
                {
                    ChessPiece newQueen = SpawningSinglePiece(ChessPieceType.Queen, 1); // Cria uma rainha do time Branco
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position; // Põe a rainha criada na posiçao que o peão estava
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject); // Destroi o peão que estava na casa 
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen; // Adiciona a nova rainha na lista de peças
                    PositionSiglePieces(lastMove[1].x, lastMove[1].y); // Adiciona a nova posição da rainha
                }
                // Time Preto
                if (targetPawn.team == 0 && lastMove[1].y == 0)
                {
                    ChessPiece newQueen = SpawningSinglePiece(ChessPieceType.Queen, 0); // Cria uma rainha do time Branco
                    newQueen.transform.position = chessPieces[lastMove[1].x, lastMove[1].y].transform.position; // Põe a rainha criada na posiçao que o peão estava
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject); // Destroi o peão que estava na casa 
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen; // Adiciona a nova rainha na lista de peças
                    PositionSiglePieces(lastMove[1].x, lastMove[1].y); // Adiciona a nova posição da rainha
                }
            }

        }

        // MOVIMENTA A TORRE PARA O MOVIMENTO ESPECIAL DO REI
        // verifica se o movimento do especial é o do rei
        if (specialMove == SpecialMove.Castling)
        {
            // Pega o ultimo movimento feito
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            // Right Rook
            if (lastMove[1].x == 1)
            {
                // Black team
                if (lastMove[1].y == 0)
                {
                    // Move a torre para o Local relativo da jogada especial
                    ChessPiece rook = chessPieces[0, 0];
                    chessPieces[2, 0] = rook;
                    PositionSiglePieces(2, 0);
                    chessPieces[0, 0] = null;
                
                // White team
                } else if (lastMove[1].y == 7) 
                {
                    // Move a torre para o Local relativo da jogada especial
                    ChessPiece rook = chessPieces[0, 7];
                    chessPieces[2, 7] = rook;
                    PositionSiglePieces(2, 7);
                    chessPieces[0, 7] = null;
                }
            } else if (lastMove[1].x == 5)
            {
                // Black team
                if (lastMove[1].y == 0)
                {
                    // Move a torre para o Local relativo da jogada especial
                    ChessPiece rook = chessPieces[7, 0];
                    chessPieces[4, 0] = rook;
                    PositionSiglePieces(4, 0);
                    chessPieces[7, 0] = null;

                    // White team
                }
                else if (lastMove[1].y == 7)
                {
                    // Move a torre para o Local relativo da jogada especial
                    ChessPiece rook = chessPieces[7, 7];
                    chessPieces[4, 7] = rook;
                    PositionSiglePieces(4, 7);
                    chessPieces[7, 7] = null;
                }
            }
        }

    }



    // Operations
    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2Int pos)
    {
        for (int i = 0; i < moves.Count;i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }
    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        if (!ContainsValidMove(ref availableMoves, new Vector2Int(x, y)))
            return false;


        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        if (chessPieces[x, y] != null) {
            ChessPiece ocp = chessPieces[x, y];

            if (cp.team == ocp.team) { 
                return false;
            }

            // If ITS THE ENEMY TEAM 
            if (ocp.team == 0)
            {
                if(ocp.type == ChessPieceType.King){
                    CheckMate(1);
                }

                deadWhites.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(8 * tileSize, yOffset, -1 * tileSize) -  bounds 
                    + new Vector3(tileSize/2,0, tileSize/2)
                    + (Vector3.forward * deathSpacing) *  deadWhites.Count);
            }
            else {
                if(ocp.type == ChessPieceType.King){
                    CheckMate(0);
                }
                deadBlacks.Add(ocp);
                ocp.SetScale(Vector3.one * deathSize);
                ocp.SetPosition(new Vector3(-1 * tileSize, yOffset, 8 * tileSize) - bounds
                    + new Vector3(tileSize / 2, 0, tileSize / 2)
                    + (Vector3.back * deathSpacing) * deadBlacks.Count);
            }

        }


        chessPieces[x, y] = cp;
        chessPieces[previousPosition.x, previousPosition.y] = null;

        PositionSiglePieces(x, y);

        isWhiteTurn = !isWhiteTurn;
        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x,y)});

        ProcessSpecialMove();

        return true;
    }
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
