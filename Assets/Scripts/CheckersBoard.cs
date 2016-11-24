using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CheckersBoard : MonoBehaviour {

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject cursorPrefab;

    private GameObject cursor;
    private GameObject winText;

    public Material grayL;
    public Material grayD;

    private bool isWhiteTurn = true;
    private Vector2 cursorCoords = new Vector2(0,0);

    private Piece selectedPiece = null;
    private Vector2 selectedPieceCoords = new Vector2(-1, -1);

    private bool isOver = false;

    // Use this for initialization
    void Start () {
        GenerateBoard();
        cursor = Instantiate(cursorPrefab, transform) as GameObject;
        winText = transform.Find("winText").gameObject;
    }
	
	// Update is called once per frame
	void Update () {

        if (!isOver)
        {
            UpdateCursor();
            if (Input.GetKeyDown(KeyCode.Space)) OnSpaceDown();
        }
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
        else if(selectedPiece!=null && p==null) //move
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
                if (!selectedPiece.IsKing() && ((selectedPiece.IsWhite() && cursorCoords.y == 7) || (!selectedPiece.IsWhite() && cursorCoords.y == 0)))
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
                    pieces[(int)(selectedPieceCoords.x + cursorCoords.x)/2, (int)(selectedPieceCoords.y + cursorCoords.y) /2] = null;
                    selectedPiece.transform.position = new Vector3(selectedPiece.transform.position.x, selectedPiece.transform.position.y, -0.031f);
                    MovePiece(selectedPiece, (int)cursorCoords.x, (int)cursorCoords.y);

                    //set king
                    if (!selectedPiece.IsKing() && ((selectedPiece.IsWhite() && cursorCoords.y == 7) || (!selectedPiece.IsWhite() && cursorCoords.y == 0)))
                    {
                        selectedPiece.setKing();
                        selectedPiece.transform.localScale = new Vector3(selectedPiece.transform.localScale.x, selectedPiece.transform.localScale.y * 2, selectedPiece.transform.localScale.z);
                    }

                    selectedPiece = null;

                    isWhiteTurn = !isWhiteTurn;

                    Destroy(enemy.gameObject);

                    int end = CheckEnd();
                    if (end!=0)
                    {

                        if (end==1) //white won
                        {
                            winText.GetComponent<TextMesh>().text = "White wins!";
                            winText.GetComponent<TextMesh>().color = new Color(255,255,255);
                            winText.transform.localPosition = new Vector3(-0.15f, -0.13f, 0.0f);
                            winText.transform.localRotation = new Quaternion(-180.0f, 0.0f, 0.0f, 0.0f);
                        }
                        else //black won
                        {
                            winText.GetComponent<TextMesh>().text = "Black wins!";
                            winText.GetComponent<TextMesh>().color = new Color(0, 0, 0);
                            winText.transform.localPosition = new Vector3(0.15f, -0.13f, 0.0f);
                            winText.transform.localRotation = new Quaternion(0.0f, 0.0f, -180.0f, 0.0f);
                        }
                        isOver = true;
                    }
                }
            }
        }
    }

    private int CheckEnd() //returns who (if game over) won; -1 for black; 1 for white
    {
        bool foundWhite = false;
        bool foundBlack = false;
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x ++)
            {
                Piece p = pieces[x, y];
                if (p != null && p.IsWhite()) foundWhite = true;
                else if (p != null && !p.IsWhite()) foundBlack = true;

                if (foundWhite && foundBlack) return 0;
            }
        }

        return (foundWhite ? 1 : -1);
    }
}
