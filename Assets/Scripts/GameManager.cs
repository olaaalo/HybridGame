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
    // public int maxTurn;
    // public int turnCount;
    // public int playerPoint;
    // public int maxPlayerPoint;
    // public int scanCount;
    // public bool onMovingPhase;

    // public Text turnCountText;
    // public Text pointCountText;

    public Button scanButton;
    public Text scanButtonText;
    public Text movingPhaseText;

    public Image mazeImage;
    public Sprite[] mazeSprites;

    //public GridLayoutGroup gridLayout;
    //public GameObject outBoardPrefab;
    //public GameObject squarePrefab;
    //public int boardSize;
    //public float spaceBtwSquare;
    //public int wayCount;
    //[Range(1, 4)] public int playerCounts;
    public Square[] playersSquares;
    public Color[] playersColor;
    public Square minotaurSquare;
    public Color minotaurColor;

    public int currentTurn;
    public int mazeTurn;
    public Text mazeTurnText;

    public Transform FMODListenerTranform;
    public Transform FMODEventMinautorTranform;
    public FMODUnity.StudioEventEmitter FMODEventMinautor;

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

    [HideInInspector] public EventSystem eventSystem;

    private bool gameHasStarted;
    private IEnumerator Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        squares = FindObjectsOfType<Square>().ToList();

        yield return null;

        // GenerateBoard();
        // DOShowSquares();

        gameHasStarted = true;
        movingPhaseText.color = Color.clear;

        for (int i = 0; i < playersSquares.Length; ++i){
            playersSquares[i].image.color = playersColor[i];
        }

        FMODEventMinautorTranform.position = minotaurSquare.transform.position;

        movingPhaseText.color = playersColor[0];
        movingPhaseText.text = string.Format("PLAYER 0");

        OnClickMaze();

        // DOVirtual.DelayedCall(1f, DOScanningPhase);
    }

    private bool lockGeneration;
    public void OnToggleLockGenerationBoard()
    {
        lockGeneration = !lockGeneration;
        eventSystem.SetSelectedGameObject(null);
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

    int rdmMaze;
    public void OnClickMaze()
    {
        rdmMaze = Random.Range(0, mazeSprites.Length);
        while (mazeSprites[rdmMaze] == mazeImage.sprite)
        {
            rdmMaze = Random.Range(0, mazeSprites.Length);
        }

        mazeImage.sprite = mazeSprites[rdmMaze];

        mazeTurn = 0;
        mazeTurnText.text = mazeTurn.ToString();
    }

    public void OnShowMinautor()
    {
        minotaurSquare.image.color = Color.white;
        minotaurSquare.image.DOKill();
        minotaurSquare.image.DOColor(minotaurColor, 0.3f).SetDelay(0.5f).From();;
    }
}