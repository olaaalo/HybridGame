using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Square : MonoBehaviour
{
    public Image image;
    public Button button;
    public Text text;

    public GameManager.SquareStruct _squareStruct = new GameManager.SquareStruct();
    public GameManager.SquareStruct squareStruct { get { return _squareStruct; } set { _squareStruct = value; ChangeStruct(); } }
    public int spawnPlayer = -1;
    
    [HideInInspector] public bool isLock;

    public List<Square> neightbors;

    private RaycastHit2D hit2D;

    void ChangeStruct()
    {
        text.text = _squareStruct.name;

        if (!hide)
            image.color = _squareStruct.color;
    }

    [ContextMenu("Check Neightbors")]
    public void CheckNeighborSquare()
    {
        // LOOK UP
        neightbors = new List<Square>();

        /*
        hit2D = Physics2D.Raycast(transform.position, Vector2.up);
        if (hit2D.collider.gameObject.layer != 10)
            neightbors.Add(hit2D.collider.GetComponent<Square>());
            */

        // LOOK RIGHT
        hit2D = Physics2D.Raycast(transform.position, Vector2.right);
        if (hit2D.collider.gameObject.layer != 10)
            neightbors.Add(hit2D.collider.GetComponent<Square>());

        // LOOK DOWN
        hit2D = Physics2D.Raycast(transform.position, Vector2.down);
        if (hit2D.collider.gameObject.layer != 10)
            neightbors.Add(hit2D.collider.GetComponent<Square>());

        // LOOK LEFT
        hit2D = Physics2D.Raycast(transform.position, Vector2.left);
        if (hit2D.collider.gameObject.layer != 10)
            neightbors.Add(hit2D.collider.GetComponent<Square>());
    }

    public void ResetState()
    {
        isLock = false;

        if (spawnPlayer == -1)
            squareStruct = GameManager.instance.squareStructs[0];
    }

    public void DOAnimate()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        transform.DOScale(0.6f, 0.4f).From();
        transform.DORotate(Vector3.forward * 90, 0.4f).From();
    }

    bool hide;
    public void DOShowSquare()
    {
        hide = !hide;
        image.color = (hide) ? Color.black : squareStruct.color;
        text.enabled = !hide;
    }
}
