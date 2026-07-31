using UnityEngine;

public enum ExitDirection
{
    HorizontalRight,
    VerticalUp,
    StraightUpTower
}

public class RoomExit : MonoBehaviour
{
    [Header("Where does this exit doorway point?")]
    public ExitDirection facingDirection = ExitDirection.HorizontalRight;
}