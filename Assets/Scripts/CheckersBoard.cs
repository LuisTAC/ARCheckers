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

    public bool whiteTurn = true;
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
                GeneratePiece((y%2==0)?x : x+1, y);
            }
        }

        //black
        for (int y = 7; y > 4; y--)
        {
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((y % 2 == 0) ? x : x + 1, y);
            }
        }
    }

    private void GeneratePiece(int x, int y)
    {
        GameObject go = Instantiate((y<=3) ? whitePiecePrefab : blackPiecePrefab, Vector3.zero, Quaternion.Euler(90,0,0), transform) as GameObject;
        go.transform.localScale = (Vector3.right * 0.05f) + (Vector3.up * 0.005f) + (Vector3.forward * 0.05f);
        Piece p = go.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    private void MovePiece(Piece p, int x, int y)
    {
        p.transform.position = Vector3.back*(0.025f) + (Vector3.right * (-0.105f + 0.03f * x)) + (Vector3.up * (-0.105f + 0.03f * y));
    }

    private void UpdateCursor()
    {
        cursor.transform.position = (Vector3.back * 0.026f) + (Vector3.right * (-0.1044f + 0.03f * cursorCoords.x)) + (Vector3.up * (-0.1044f + 0.03f * cursorCoords.y));

        Material[] mats = cursor.GetComponent<Renderer>().materials;
        mats[0] = (whiteTurn ? grayL : grayD);
        cursor.GetComponent<Renderer>().materials = mats;

        if (Input.GetKeyDown(KeyCode.UpArrow) && cursorCoords.y < 7) cursorCoords.y++;
        if (Input.GetKeyDown(KeyCode.DownArrow) && cursorCoords.y > 0) cursorCoords.y--;
        if (Input.GetKeyDown(KeyCode.RightArrow) && cursorCoords.x < 7) cursorCoords.x++;
        if (Input.GetKeyDown(KeyCode.LeftArrow) && cursorCoords.x > 0) cursorCoords.x--;
    }

    private void OnSpaceDown()
    {
        Piece p = pieces[(int)cursorCoords.x, (int)cursorCoords.y];
        if (p != null)
        {
            if (selectedPiece != null)
            {
                selectedPiece.transform.position = new Vector3(selectedPiece.transform.position.x, selectedPiece.transform.position.y, selectedPiece.transform.position.z + 0.025f);
            }
            selectedPiece = p;
            selectedPieceCoords = new Vector2(cursorCoords.x, cursorCoords.y);
            selectedPiece.transform.position = new Vector3(selectedPiece.transform.position.x, selectedPiece.transform.position.y, selectedPiece.transform.position.z - 0.025f);
        }
        else
        {
            pieces[(int)cursorCoords.x, (int)cursorCoords.y] = selectedPiece;
            pieces[(int)selectedPieceCoords.x, (int)selectedPieceCoords.y] = null;
            MovePiece(selectedPiece, (int)cursorCoords.x, (int)cursorCoords.y);
            selectedPiece = null;
        }

        Debug.Log(selectedPiece);

        /*
        if (whiteTurn)
        {

        }
        else
        {

        }
        */
    }
}
