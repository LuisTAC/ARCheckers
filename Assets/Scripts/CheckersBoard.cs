using UnityEngine;
using System.Collections;

public class CheckersBoard : MonoBehaviour {

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject cursorPrefab;

    private GameObject cursor;

    public Material grayL;
    public Material grayD;

    private bool isWhiteTurn = true;
    private Vector2 cursorCoords = new Vector2(0,0);

    private Piece selectedPiece = null;
    private Vector2 selectedPieceCoords = new Vector2(-1, -1);

    // Use this for initialization
    void Start () {
        GenerateBoard();
        cursor = Instantiate(cursorPrefab, transform) as GameObject;
    }
	
	// Update is called once per frame
	void Update () {

        UpdateCursor();

        if (Input.GetKeyDown(KeyCode.Space)) OnSpaceDown();
    }

    private void GenerateBoard()
    {
        //white
        for(int y=0;y<3;y++)
        {
            for(int x=0;x<8;x+=2)
            {
                GeneratePiece((y%2==0)?x : x+1, y, true);
            }
        }

        //black
        for (int y = 7; y > 4; y--)
        {
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((y % 2 == 0) ? x : x + 1, y, false);
            }
        }
    }

    private void GeneratePiece(int x, int y, bool isWhite)
    {
        GameObject go = Instantiate((y<=3) ? whitePiecePrefab : blackPiecePrefab, new Vector3(0,0,-0.031f), Quaternion.Euler(90,0,0), transform) as GameObject;
        go.transform.localScale = (Vector3.right * 0.05f) + (Vector3.up * 0.01f) + (Vector3.forward * 0.05f);
        Piece p = go.GetComponent<Piece>();
        if(isWhite) p.setWhite();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    private void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = new Vector3((-0.105f + 0.03f * x), (-0.105f + 0.03f * y), p.transform.position.z);
    }

    private void UpdateCursor()
    {
        cursor.transform.position = (Vector3.back * 0.026f) + (Vector3.right * (-0.1044f + 0.03f * cursorCoords.x)) + (Vector3.up * (-0.1044f + 0.03f * cursorCoords.y));

        Material[] mats = cursor.GetComponent<Renderer>().materials;
        mats[0] = (isWhiteTurn ? grayL : grayD);
        cursor.GetComponent<Renderer>().materials = mats;

        if (Input.GetKeyDown(KeyCode.UpArrow) && cursorCoords.y < 7) cursorCoords.y++;
        if (Input.GetKeyDown(KeyCode.DownArrow) && cursorCoords.y > 0) cursorCoords.y--;
        if (Input.GetKeyDown(KeyCode.RightArrow) && cursorCoords.x < 7) cursorCoords.x++;
        if (Input.GetKeyDown(KeyCode.LeftArrow) && cursorCoords.x > 0) cursorCoords.x--;
    }

    private void OnSpaceDown()
    {
        Piece p = pieces[(int)cursorCoords.x, (int)cursorCoords.y];
        if (p != null && ((p.IsWhite() && isWhiteTurn) || (!p.IsWhite() && !isWhiteTurn)))
        {
            bool same = false;
            if (selectedPiece != null) //deselect
            {
                selectedPiece.transform.position = new Vector3(selectedPiece.transform.position.x, selectedPiece.transform.position.y, selectedPiece.transform.position.z + 0.025f);
                if(selectedPiece==p)
                {
                    same = true;
                    selectedPiece = null;
                    selectedPieceCoords = new Vector2(-1, -1);
                }
            }
            if(!same) // select other
            {
                selectedPiece = p;
                selectedPieceCoords = new Vector2(cursorCoords.x, cursorCoords.y);
                selectedPiece.transform.position = new Vector3(selectedPiece.transform.position.x, selectedPiece.transform.position.y, selectedPiece.transform.position.z - 0.025f);
            }
        }
        else if(selectedPiece!=null && p==null)
        {
            if((selectedPiece.IsWhite() && //move diagonally
                (cursorCoords.x==selectedPieceCoords.x+1 || cursorCoords.x == selectedPieceCoords.x - 1) && (cursorCoords.y == selectedPieceCoords.y + 1)) ||
                (!selectedPiece.IsWhite() &&
                (cursorCoords.x == selectedPieceCoords.x + 1 || cursorCoords.x == selectedPieceCoords.x - 1) && (cursorCoords.y == selectedPieceCoords.y - 1)) ||
                (selectedPiece.IsKing() && (cursorCoords.y == selectedPieceCoords.y - 1 || cursorCoords.y == selectedPieceCoords.y + 1) &&
                        (cursorCoords.x == selectedPieceCoords.x + 1 || cursorCoords.x == selectedPieceCoords.x - 1)))
            {
                pieces[(int)cursorCoords.x, (int)cursorCoords.y] = selectedPiece;
                pieces[(int)selectedPieceCoords.x, (int)selectedPieceCoords.y] = null;
                selectedPiece.transform.position = new Vector3(selectedPiece.transform.position.x, selectedPiece.transform.position.y, -0.031f);
                MovePiece(selectedPiece, (int)cursorCoords.x, (int)cursorCoords.y);

                //set king
                if ((selectedPiece.IsWhite() && cursorCoords.y == 7) || (!selectedPiece.IsWhite() && cursorCoords.y == 0))
                {
                    selectedPiece.setKing();
                    selectedPiece.transform.localScale = new Vector3(selectedPiece.transform.localScale.x, selectedPiece.transform.localScale.y * 2, selectedPiece.transform.localScale.z);
                }

                selectedPiece = null;

                isWhiteTurn = !isWhiteTurn;
            }
            else //eat enemy piece and move
            {
                Piece enemy = pieces[(int)(selectedPieceCoords.x + cursorCoords.x)/2, (int)(selectedPieceCoords.y + cursorCoords.y) / 2];
                if (enemy!=null && enemy.IsWhite() != selectedPiece.IsWhite() &&
                    ((selectedPiece.IsWhite() && cursorCoords.y==selectedPieceCoords.y+2 &&
                        (cursorCoords.x == selectedPieceCoords.x + 2 || cursorCoords.x == selectedPieceCoords.x - 2) ) ||
                    (!selectedPiece.IsWhite() && cursorCoords.y == selectedPieceCoords.y - 2 &&
                        (cursorCoords.x == selectedPieceCoords.x + 2 || cursorCoords.x == selectedPieceCoords.x - 2) ) ||
                    (selectedPiece.IsKing() && (cursorCoords.y == selectedPieceCoords.y - 2 || cursorCoords.y == selectedPieceCoords.y + 2) &&
                        (cursorCoords.x == selectedPieceCoords.x + 2 || cursorCoords.x == selectedPieceCoords.x - 2))))
                {
                    pieces[(int)cursorCoords.x, (int)cursorCoords.y] = selectedPiece;
                    pieces[(int)selectedPieceCoords.x, (int)selectedPieceCoords.y] = null;
                    selectedPiece.transform.position = new Vector3(selectedPiece.transform.position.x, selectedPiece.transform.position.y, -0.031f);
                    MovePiece(selectedPiece, (int)cursorCoords.x, (int)cursorCoords.y);

                    //set king
                    if ((selectedPiece.IsWhite() && cursorCoords.y == 7) || (!selectedPiece.IsWhite() && cursorCoords.y == 0))
                    {
                        selectedPiece.setKing();
                        selectedPiece.transform.localScale = new Vector3(selectedPiece.transform.localScale.x, selectedPiece.transform.localScale.y * 2, selectedPiece.transform.localScale.z);
                    }

                    selectedPiece = null;

                    isWhiteTurn = !isWhiteTurn;

                    Destroy(enemy.gameObject);
                }
                    
            }

        }
    }
}
