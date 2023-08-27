using UnityEngine;

public enum ChessPieceType
{
    None = 0, Pawn = 1, Rook = 2, Knight = 3, Bishop = 4, Queen = 5, King = 6
}
public class ChessPiece : MonoBehaviour
{
    public int team;
    public int currentX;
    public int currentY;
    public ChessPieceType type;

    private Vector3 desiredPostion;
    private Vector3 desiredScale = Vector3.one;

    private void Awake(){
        desiredScale = transform.localScale;
    }
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPostion, Time.deltaTime * 5);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 5);
    }
    public virtual void SetPosition(Vector3 position, bool force = false) { 
        desiredPostion = position;
        if (force) {
            transform.position = desiredPostion;
        }
    
    }

    public virtual void SetScale(Vector3 scale, bool force = false)
    {
        desiredScale = scale;
        if (force)
        {
            transform.localScale = desiredScale;
        }
    }
}
