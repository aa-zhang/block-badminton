using UnityEngine;

public static class Constants
{
    // Court position constants (Note: player 1 should use negative value of these constants)
    public static readonly float REAR_COURT_X_POS = 14.4f;
    public static readonly float FRONT_COURT_X_POS = 1f;
    public static readonly float SERVE_X_POS = 7f;
    public static readonly float GROUND_Y_POS = 2f;


    // Birdie serving position constants
    public static readonly Vector3 SERVING_OFFSET_PLAYER_ONE = new Vector3(2, -0.7f, 0);
    public static readonly Vector3 SERVING_OFFSET_PLAYER_TWO = new Vector3(-2, -0.7f, 0);
    public static readonly Vector3 BIRDIE_DEFAULT_POSITION = new Vector3(0, 6.5f, 0);

    // Serve arrow angle constants
    public static readonly Vector3 SERVE_ANGLE_HIGH = new Vector3(0, 0, 45);
    public static readonly Vector3 SERVE_ANGLE_LOW = new Vector3(0, 0, 15);

    // Score constants
    public static readonly int WINNING_SCORE = 11;
    public static readonly int MAX_SCORE = 15;

    // Physics constants
    public static readonly float GRAVITY = -40;
    public static readonly float PLAYER_X_DRAG = 5f;

    // Stamina constants
    public static readonly float DASH_STAMINA_COST = 34f;

    // Fast Fall contants
    public static readonly float FAST_FALL_TIME_FRAME = 0.1f;
    public static readonly float FAST_FALL_SPEED = -120;

}
