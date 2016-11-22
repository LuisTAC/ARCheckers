using UnityEngine;
using System.Collections;

public class CheckersBoard : MonoBehaviour {

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    // Use this for initialization
    void Start () {
        GenerateBoard();
    }
	
	// Update is called once per frame
	void Update () {
	
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
        p.transform.position = Vector3.back*(0.028f) + (Vector3.right * (-0.105f + 0.03f * x)) + (Vector3.up * (-0.105f + 0.03f * y));
    }
}
