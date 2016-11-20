using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

    public bool isWhitePiece;

    public bool ValidMove(Piece[,] board, int i, int j, int i1, int j1)
    {
        if (board[i1, j1] != null) return false;

    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
