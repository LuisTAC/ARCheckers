using UnityEngine;
using System.Collections;

public class CheckersBoard : MonoBehaviour {

	Piece[,] pieces = new Piece[8,8];
	public GameObject whitePre;
	public GameObject blackPre;

    private Piece selPiece;
    private Vector2 mouseOver;
    private Vector2 initDrag;
    private Vector2 finishDrag;
	public Vector3 boardBalance = new Vector3 (-4.0f, 0, -4.0f);
	public Vector3 pieceBalance = new Vector3 (0.5f, 0, 0.5f);

    // Use this for initialization
    public void Start()
    {
        InitBoard();
    }
    private void Update()
    {
        RefreshOver();

        {
            int i = (int)mouseOver.x;
            int j = (int)mouseOver.y;

            if(selPiece != null)
            {
                RefreshDrag(selPiece);
            }

            if (Input.GetMouseButtonDown(0))
            {
                PressPiece(i, j);
            }

            if (Input.GetMouseButtonUp(0))
            {
                AttemptMove((int)initDrag.x, (int)initDrag.y, i, j);
            }
        }

        Debug.Log(mouseOver);
    }
    private void RefreshOver()
    {
        RaycastHit h;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out h, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(h.point.x - boardBalance.x);
            mouseOver.y = (int)(h.point.z - boardBalance.z);
        }
        else { mouseOver.x = -1; mouseOver.y = -1; }
    }
    
    private void PressPiece(int i, int j)
    {
        if ((i < 0 || i >= 8) || (j < 0 || j >= 8)) return;

        Piece piece = pieces[i,j];
        if(piece != null)
        {
            selPiece = piece;
            initDrag = mouseOver;
            Debug.Log(selPiece.name);
        }
    }
    private void AttemptMove(int i, int j, int i1, int j1)
    {
        initDrag = new Vector2(i, j);
        finishDrag = new Vector2(i1, j1);

        selPiece = pieces[i, j];
        if(i1 < 0 || i1 >= 8 || j1 < 0 || j1 >= 8)
        {
            if(selPiece != null)
            {
                MovePiece(selPiece, i, j);
            }
            selPiece = null;
            initDrag = Vector2.zero;
            return;
        }
        if(selPiece != null)
        {
            if(initDrag == finishDrag)
            {
                MovePiece(selPiece, i, j);
                selPiece = null;
                initDrag = Vector2.zero;
                return;
            }
        }
    }
    private void RefreshDrag(Piece piece)
    {
        RaycastHit h;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out h, 25.0f, LayerMask.GetMask("Board")))
        {
            piece.transform.position = h.point + Vector3.up;
        }
    }

    private void InitBoard()
    {
        // White pieces
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 8; j += 2)
            {
                if ((i % 2) == 0)
                {
                    InitPiece(j, i);
                }
                else
                {
                    InitPiece(j + 1, i);
                }
            }
        }

        // Black pieces
        for (int i = 7; i > 4; i--)
        {
            for (int j = 0; j < 8; j += 2)
            {
                if ((i % 2) == 0)
                {
                    InitPiece(j, i);
                }
                else
                {
                    InitPiece(j + 1, i);
                }
            }
        }

    }
    private void InitPiece(int i, int j)
    {
        GameObject game_obj;
        if (j > 3)
        {
            game_obj = Instantiate(whitePre) as GameObject;
        }
        else
        {
            game_obj = Instantiate(blackPre) as GameObject;
        }
        game_obj.transform.SetParent(transform);
        Piece piece = game_obj.GetComponent<Piece>();
        pieces[i, j] = piece;
        MovePiece(piece, i, j);
    }
    private void MovePiece(Piece piece, int i, int j)
    {
        piece.transform.position = (Vector3.right * i) + (Vector3.forward * j) + boardBalance + pieceBalance;
    }
   
}
