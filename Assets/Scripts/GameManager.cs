using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoSingleton<GameManager>
{
    public int maxTurn;
    public int turnCount;
    public int playerPoint;
    public int maxPlayerPoint;
    public int scanCount;
    public bool onMovingPhase;

    public Text turnCountText;
    public Text pointCountText;

    public Button scanButton;
    public Text scanButtonText;
    public Text movingPhaseText;

    //public GridLayoutGroup gridLayout;
    //public GameObject outBoardPrefab;
    //public GameObject squarePrefab;
    //public int boardSize;
    //public float spaceBtwSquare;
    //public int wayCount;
    //[Range(1, 4)] public int playerCounts;
    public Square playersSquare;

    public enum SquareType { neutral, good, bad }

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

    private bool gameHasStarted;
    private IEnumerator Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        squares = FindObjectsOfType<Square>().ToList();

        yield return null;
        yield return null;
        yield return null;

        GenerateBoard();
        DOShowSquares();

        gameHasStarted = true;
        movingPhaseText.color = Color.clear;

        turnCountText.text = string.Format(turnCountText.text, turnCount, maxTurn);
        pointCountText.text = string.Format(pointCountText.text, playerPoint, maxPlayerPoint);

        DOVirtual.DelayedCall(1f, DOScanningPhase);
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

    public void DOScanningPhase()
    {
        scanButtonText.text = "SCAN";
        scanButton.interactable = true;
        movingPhaseText.DOKill();
        movingPhaseText.color = Color.clear;

        playersSquare.DOVisualPlayer();

        playersSquare.neightbors[scanCount].DOEnableVisualScan();
    }

    public void OnScanButton()
    {
        if (!gameHasStarted) return;

        playersSquare.neightbors[scanCount].DOSoundScan();
        scanCount++;

        scanButtonText.text = "SCANNING";
        scanButton.interactable = false;

        DOVirtual.DelayedCall(2f, () =>
        {
            if (scanCount == playersSquare.neightbors.Count)
                DOMovingPhase();
            else
                DOScanningPhase();
        });
    }

    public void DOMovingPhase()
    {
        scanButtonText.text = "MOVING PHASE";

        movingPhaseText.DOKill();
        movingPhaseText.DOColor(Color.yellow, 0.5f).SetLoops(-1, LoopType.Yoyo);

        onMovingPhase = true;

        playersSquare.DOVisualNeightborMove();
    }

    public void MovingPlayer(Square newPosition)
    {
        onMovingPhase = false;

        playersSquare.DOLastPlayerPlosition();
        playersSquare = newPosition;
        playersSquare.DONewPlayerPlosition();
        
        if (newPosition.squareStruct.type == SquareType.good)
        {
            playerPoint++;
            pointCountText.text = string.Format("POINTS : {0}/{1}", playerPoint, maxPlayerPoint);
            pointCountText.transform.DOScale(1.5f, 0.3f).From();

            if (playerPoint == maxPlayerPoint)
            {
                Success();
                return;
            }
        }
        else
        {
            turnCount++;
            turnCountText.text = string.Format("MISS : {0}/{1}", turnCount, maxTurn);
            
            if (turnCount == maxTurn - 1)
                turnCountText.DOColor(Color.Lerp(Color.black, Color.red, 0.3f), 0.5f).SetLoops(-1, LoopType.Yoyo);

            if (turnCount == maxTurn || newPosition.squareStruct.type == SquareType.bad)
            {
                GameOver();
                return;
            }
        }

        newPosition.squareStruct = squareStructs[0];

        scanCount = 0;
        DOScanningPhase();
    }

    public void Success()
    {
        if (!gameHasStarted) return;

        gameHasStarted = false;

        for (int i = 0; i < squares.Count; ++i)
            squares[i].DOVisualSuccess();

        scanButtonText.text = "SUCCESS";
        movingPhaseText.DOKill();
        movingPhaseText.color = Color.blue;
        movingPhaseText.text = "Deminer win !";
    }

    public void GameOver()
    {
        if (!gameHasStarted) return;

        gameHasStarted = false;

        for (int i = 0; i < squares.Count; ++i)
            squares[i].DOVisualGameOver();

        scanButtonText.text = "GAME OVER";
        movingPhaseText.DOKill();
        movingPhaseText.color = Color.red;
        movingPhaseText.text = "Terrorist win !";
    }

    #region Generation TOOL
    public void DOShowSquares()
    {
        for (int i = 0; i < squares.Count; ++i)
        {
            squares[i].DOShowSquare();
        }
    }

    List<Square> emptySquares;
    List<Square> goodSquares;
    List<Square> badSquares;
    int countLock;
    int random;
    [ContextMenu("Generate the Board")]
    void GenerateBoard()
    {
        for (int i = 0; i < squares.Count; ++i)
        {
            squares[i].CheckNeighborSquare();
            squares[i].ResetState();
            squares[i].DOAnimate();
        }

        emptySquares = new List<Square>();
        goodSquares = new List<Square>();
        badSquares = new List<Square>();

        foreach (var sqr in squares)
        {
            if (sqr.playersOnThis)
            {
                playersSquare = sqr;
                playersSquare.isLock = true;
            }
        }

        countLock = 0;
        while (countLock != squareStructs[1].fixeNumber)
        {
            random = Random.Range(0, squares.Count);
            
            if (!squares[random].isLock
            && squares[random] != playersSquare.neightbors[0]
            && squares[random] != playersSquare.neightbors[1]
            && squares[random] != playersSquare.neightbors[2])
            {
                badSquares.Add(squares[random]);
                squares[random].squareStruct = squareStructs[1];
                squares[random].isLock = true;
                countLock++;
            }
        }

        countLock = 0;
        while (countLock != squareStructs[2].fixeNumber)
        {
            random = Random.Range(0, squares.Count);
            if (!squares[random].isLock)
            {
                goodSquares.Add(squares[random]);
                squares[random].squareStruct = squareStructs[2];
                squares[random].isLock = true;
                countLock++;
            }
        }

        for (int i = 0; i < squares.Count; ++i)
        {
            if (!squares[i].isLock)
                emptySquares.Add(squares[i]);
        }
    }
    #endregion
}