using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {

    private bool isWhite = false;
    private bool isKing = false;

    public bool IsWhite()
    {
        return isWhite;
    }
    public bool IsKing()
    {
        return isKing;
    }

    public void setWhite()
    {
        isWhite = true;
    }
    public void setKing()
    {
        isKing = true;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
