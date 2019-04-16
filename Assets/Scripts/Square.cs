using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    public Image image;
    public Button button;
    public Text text;

    public GameManager.SquareStruct _squareStruct = new GameManager.SquareStruct();
    public GameManager.SquareStruct squareStruct { get { return _squareStruct; } set { _squareStruct = value; ChangeStruct(); } }

    private bool saveSpawnPlayer;
    public bool playersOnThis;
    public bool isMovePosition;

    [HideInInspector] public bool isLock;

    public Transform overlapSphereOrigin;
    public Collider2D selfCollider;
    public List<Square> neightbors;

    private List<Collider2D> col2D;

    void ChangeStruct()
    {
        text.text = _squareStruct.name;

        if (!hide)
            image.color = _squareStruct.color;
    }

    private Quaternion baseRot;
    private void Start()
    {
        baseRot = transform.rotation;
        saveSpawnPlayer = playersOnThis;
    }

    [ContextMenu("Check Neightbors")]
    public void CheckNeighborSquare()
    {
        // LOOK UP
        neightbors = new List<Square>();

        col2D = Physics2D.OverlapCircleAll(transform.position, 1f).ToList();
        if (col2D.Contains(selfCollider))
            col2D.Remove(selfCollider);

        col2D.Sort((v,w)=> v.transform.position.x.CompareTo(w.transform.position.x));
        col2D.Sort((v,w)=> v.transform.position.y.CompareTo(w.transform.position.y));

        for (int i = 0; i < col2D.Count; ++i)
            neightbors.Add(col2D[i].transform.parent.GetComponent<Square>());
    }

    public void DOVisualPlayer()
    {
        image.DOKill();
        image.DOColor(Color.Lerp(Color.black, Color.green, 0.8f), 0.5f);
    }

    public void DOEnableVisualScan()
    {
        image.DOKill();
        image.DOColor(Color.Lerp(Color.black, Color.white, 0.3f), 1f);
    }

    public void DOSoundScan()
    {
        PlayCurrentSound();

        image.DOKill();
        image.DOColor(Color.black, 1f);
    }

    public void DOVisualNeightborMove()
    {
        for (int i = 0; i < neightbors.Count; ++i)
        {
            neightbors[i].isMovePosition = true;
            neightbors[i].image.DOKill();
            neightbors[i].image.DOColor(Color.Lerp(Color.black, Color.yellow, 0.3f), 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void DOLastPlayerPlosition()
    {
        playersOnThis = false;

        image.DOKill();
        image.DOColor(Color.black, 0.5f);

        for (int i = 0; i < neightbors.Count; ++i)
        {
            neightbors[i].isMovePosition = false;
            neightbors[i].image.DOKill();
            neightbors[i].image.DOColor(Color.black, 0.1f);
        }
    }

    public void DONewPlayerPlosition()
    {
        playersOnThis = true;
    }

    public void DOVisualSuccess()
    {
        playersOnThis = false;
        isMovePosition = false;

        image.DOKill();
        image.color = Color.black;
        image.DOColor(Color.Lerp(Color.black, Color.blue, 0.3f), 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void DOVisualGameOver()
    {
        playersOnThis = false;
        isMovePosition = false;

        image.DOKill();
        image.color = Color.black;
        image.DOColor(Color.Lerp(Color.black, Color.red, 0.3f), 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void ResetState()
    {
        isLock = saveSpawnPlayer;

        if (!saveSpawnPlayer)
            squareStruct = GameManager.instance.squareStructs[0];

        playersOnThis = saveSpawnPlayer;
    }

    public void DOAnimate()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.rotation = baseRot;
        transform.DOScale(0.6f, 0.4f).From();
        transform.DORotate(Vector3.forward * 90, 0.4f).From().SetRelative();
    }

    bool hide;
    public void DOShowSquare()
    {
        if (playersOnThis)
            return;

        hide = !hide;
        image.color = (hide) ? Color.black : squareStruct.color;
        text.enabled = !hide;
    }

    public void OnClick()
    {
        if(GameManager.instance.currentTurn < 4)
        {
            GameManager.instance.playersSquares[GameManager.instance.currentTurn].image.color = Color.white;
            GameManager.instance.playersSquares[GameManager.instance.currentTurn] = this;
            GameManager.instance.playersSquares[GameManager.instance.currentTurn].image.color =
                GameManager.instance.playersColor[GameManager.instance.currentTurn];
                
            GameManager.instance.FMODListenerTranform.position = transform.position;
            GameManager.instance.FMODEventMinautor.Play();
        }
        else
        {
            GameManager.instance.FMODEventMinautorTranform.position = GameManager.instance.minotaurSquare.transform.position;
            GameManager.instance.minotaurSquare = this;
        }

        GameManager.instance.currentTurn = (int)Mathf.Repeat(GameManager.instance.currentTurn + 1, 5);

        if(GameManager.instance.currentTurn < 4)
        {
            GameManager.instance.movingPhaseText.color = GameManager.instance.playersColor[GameManager.instance.currentTurn];
            GameManager.instance.movingPhaseText.text = string.Format("PLAYER {0}", GameManager.instance.currentTurn);
        }
        else
        {
            GameManager.instance.movingPhaseText.color = Color.white;
            GameManager.instance.movingPhaseText.text = string.Format("MINAUTOR");
            GameManager.instance.mazeTurn++;
            GameManager.instance.mazeTurnText.text = GameManager.instance.mazeTurn.ToString();
        }

        GameManager.instance.eventSystem.SetSelectedGameObject(null);
    }

    public void PlayCurrentSound()
    {
        switch (squareStruct.name)
        {
        case "TRAP":
            SoundManager.PlayTrapSound();
            break;

        case "":
            SoundManager.PlayEmptySound();
            break;

        case "GOOD":
            SoundManager.PlayMoneySound();
            break;
        }
    }
}