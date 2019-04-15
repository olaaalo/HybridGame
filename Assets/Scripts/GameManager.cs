using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using DG.Tweening;

public class GameManager : MonoSingleton<GameManager>
{
    public GridLayoutGroup gridLayout;
    public GameObject outBoardPrefab;
    public GameObject squarePrefab;
    public int boardSize;
    public float spaceBtwSquare;
    public int wayCount;
    [Range(1, 4)] public int playerCounts;
    public Square[] playerSquares;
    public int playerTurn;

    public enum SquareType { neutral, good, bad, exit }

    [System.Serializable]
    public struct SquareStruct
    {
        public string name;

        [Range(0, 3)] public int interest;
        // 0 = only one
        // 1 = rare
        // 2 = commun
        // 3 = only way
        public int fixeNumber;

        public Color color;
        public SquareType type;
    }

    public List<SquareStruct> squareStructs;

    public List<Square> squares;

    private List<Vector2> startPlayers;
    private List<int> playerSquareID;

    [HideInInspector] public EventSystem eventSystem;


    [ContextMenu("DOIT")]
    void DOIT()
    {
        if (gridLayout.transform.childCount > 0)
        {
            GameObject oldObject = new GameObject { name = "TO DELETE" };
            for (int i = gridLayout.transform.childCount - 1; i >= 0; --i)
                gridLayout.transform.GetChild(i).SetParent(oldObject.transform);
            oldObject.SetActive(false);
        }        


        startPlayers = new List<Vector2>
        {
            new Vector2(Mathf.CeilToInt(boardSize / 2f), 0),
            new Vector2(0, Mathf.CeilToInt(boardSize / 2f)),
            new Vector2(boardSize, Mathf.CeilToInt(boardSize / 2f)),
            new Vector2(Mathf.CeilToInt(boardSize / 2f), boardSize)
        };

        int size = boardSize + 2;

        gridLayout.constraintCount = size;
        gridLayout.spacing = Vector2.one * spaceBtwSquare;

        squares = new List<Square>();
        playerSquareID = new List<int>();

        for (int i = 0; i < size; ++i)
        {
            var g = PrefabUtility.InstantiatePrefab(outBoardPrefab) as GameObject;
            g.transform.SetParent(gridLayout.transform, false);
        }

        for (int i = 0; i < size - 2; ++i)
        {
            var g = PrefabUtility.InstantiatePrefab(outBoardPrefab) as GameObject;
            g.transform.SetParent(gridLayout.transform, false);

            for (int j = 0; j < size - 2; ++j)
            {
                var go = PrefabUtility.InstantiatePrefab(squarePrefab) as GameObject;
                go.transform.SetParent(gridLayout.transform, false);

                for (int p = 0; p < playerCounts; ++p)
                {
                    if (i == startPlayers[p].y && j + 1 == startPlayers[p].x)
                    {
                        go.GetComponent<Square>().spawnPlayer = p;
                        break;
                    }
                }

                squares.Add(go.GetComponent<Square>());

                if (go.GetComponent<Square>().spawnPlayer != -1)
                    playerSquareID.Add(squares.Count);
            }

            g = PrefabUtility.InstantiatePrefab(outBoardPrefab) as GameObject;
            g.transform.SetParent(gridLayout.transform, false);
        }

        for (int i = 0; i < size; ++i)
        {
            var g = PrefabUtility.InstantiatePrefab(outBoardPrefab) as GameObject;
            g.transform.SetParent(gridLayout.transform);
        }
    }

    private bool gameHasStarted;
    private IEnumerator Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        yield return null;

        GenerateBoard();
        DOShowSquares();

        playerSquares = new Square[playerCounts];
        foreach (var sqr in squares)
        {
            if (sqr.spawnPlayer > -1)
                playerSquares[sqr.spawnPlayer] = sqr;
        }

        gameHasStarted = true;

        playerSquares[playerTurn].DOPlayerTurn();
    }

    private bool lockGeneration;
    public void OnToggleLockGenerationBoard()
    {
        lockGeneration = !lockGeneration;
        eventSystem.SetSelectedGameObject(null);
    }


    private void Update()
    {
        if (!gameHasStarted) return;

        if (!lockGeneration)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                GenerateBoard();

            if (Input.GetKeyDown(KeyCode.Return))
                DOShowSquares();
        }
    }

    public void NextTurn(Square newPlayerSquare)
    {
        playerSquares[playerTurn].playersOnThis[playerTurn] = false;
        playerSquares[playerTurn].STOPPlayerTurn();

        playerSquares[playerTurn] = newPlayerSquare;
        playerSquares[playerTurn].playersOnThis[playerTurn] = true;

        playerTurn = (int)Mathf.Repeat(playerTurn + 1, playerCounts);

        playerSquares[playerTurn].DOPlayerTurn();
    }

    public void DOShowSquares()
    {
        for (int i = 0; i < squares.Count; ++i)
        {
            squares[i].DOShowSquare();
        }
    }

    Square currentWaySquare;
    bool canContinue;
    int randomSquare;
    int countLock;
    List<SquareStruct> waySquares;
    List<SquareStruct> otherCommunSquares;
    List<SquareStruct> otherRareSquares;
    SquareStruct exitSquare;
    int countReturn;
    [ContextMenu("Generate the Board")]
    void GenerateBoard()
    {
        for (int i = 0; i < squares.Count; ++i)
        {
            squares[i].CheckNeighborSquare();
            squares[i].ResetState();
            squares[i].DOAnimate();
        }

        waySquares = new List<SquareStruct>();
        otherCommunSquares = new List<SquareStruct>();
        otherRareSquares = new List<SquareStruct>();
        foreach (SquareStruct strct in squareStructs)
        {
            if (strct.interest == 3)
                waySquares.Add(strct);
            if (strct.interest == 2)
                otherCommunSquares.Add(strct);
            if (strct.interest == 1)
                otherRareSquares.Add(strct);
            else if (strct.interest == 0 && strct.type == SquareType.exit)
                exitSquare = strct;
        }

        foreach (Square sqr in squares)
        {
            if (sqr.spawnPlayer == 0)
            {
                currentWaySquare = sqr;
                break;
            }
        }

        for (int i = 0; i < wayCount; ++i)
        {
            currentWaySquare.isLock = true;
            currentWaySquare.squareStruct = waySquares[Random.Range(0, waySquares.Count)];

            canContinue = false;

            countLock = 0;
            countReturn = 0;

            while (!canContinue)
            {
                randomSquare = Random.Range(0, currentWaySquare.neightbors.Count);

                countReturn++;

                if (!currentWaySquare.neightbors[randomSquare].isLock)
                {
                    for (int j = 1; j < currentWaySquare.neightbors[randomSquare].neightbors.Count; ++j)
                    {
                        if (currentWaySquare.neightbors[randomSquare].neightbors[j].isLock)
                            countLock++;
                    }

                    if (countLock < 2 || countLock == currentWaySquare.neightbors[randomSquare].neightbors.Count)
                        canContinue = true;
                }
                if (countReturn > 1000)
                    canContinue = true;
            }

            currentWaySquare = currentWaySquare.neightbors[randomSquare];

            countLock = 0;
            countReturn = 0;
        }

        currentWaySquare.squareStruct = exitSquare;
        currentWaySquare.isLock = true;

        for (int i = 0; i < squares.Count; ++i)
        {
            if (!squares[i].isLock)
            {
                squares[i].squareStruct = (Random.value < 0.7f) ? otherCommunSquares[Random.Range(0, waySquares.Count)] : otherRareSquares[Random.Range(0, waySquares.Count)];
            }
        }

        int generationFixe;
        foreach (SquareStruct strct in squareStructs)
        {
            generationFixe = 0;
            countReturn = 0;


            if (strct.fixeNumber > 0)
            {
                randomSquare = Random.Range(0, squares.Count);
                canContinue = false;

                while (generationFixe != strct.fixeNumber)
                {
                    countReturn++;

                    if (!squares[randomSquare].isLock && squares[randomSquare].squareStruct.fixeNumber < 1)
                    {
                        for (int i = 1; i < squares[randomSquare].neightbors.Count; ++i)
                        {
                            if (squares[randomSquare].neightbors[i].isLock)
                            {
                                squares[randomSquare].squareStruct = strct;
                                generationFixe++;
                                canContinue = true;
                            }
                            else if (squares[randomSquare].neightbors[i].squareStruct.type != SquareType.bad)
                            {
                                for (int j = 1; j < squares[randomSquare].neightbors[i].neightbors.Count; ++j)
                                {
                                    if (squares[randomSquare].neightbors[i].neightbors[j].isLock)
                                    {
                                        squares[randomSquare].squareStruct = strct;
                                        generationFixe++;
                                        canContinue = true;
                                        break;
                                    }
                                }
                            }

                            if (canContinue)
                                break;
                        }
                    }

                    randomSquare = Random.Range(0, squares.Count);
                    canContinue = false;

                    if (countReturn > 1000)
                        return;
                }
            }
        }
    }
}
