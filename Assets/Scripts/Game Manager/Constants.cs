using UnityEngine;

public static class Constants
{
    // Court position constants (Note: player 1 should use negative value of these constants)
    public static readonly float rearCourtXPos = 11.4f;
    public static readonly float frontCourtXPos = 0.8f;
    public static readonly float servingLineXPos = 5f;
    public static readonly float startingXPos = 6.14f;


    // Birdie serving position constants
    public static readonly Vector3 servingOffsetOne = new Vector3(2, -0.7f, 0);
    public static readonly Vector3 servingOffsetTwo = new Vector3(-2, -0.7f, 0);


    // Score constants
    public static readonly int winningScore = 11;
    public static readonly int maxScore = 15;

    // Gravity constant
    public static readonly Vector3 gravity = new Vector3(0, -4, 0);
}
