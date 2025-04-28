using UnityEngine;

public static class Constants
{
    // Court position constants (Note: player 1 should use negative value of these constants)
    public static readonly float REAR_COURT_X_POS = 6.15f;
    public static readonly float FRONT_COURT_X_POS = 0.35f;
    public static readonly float SERVE_X_POS = 3.25f;
    public static readonly float SERVING_LINE_X_POS = 2.5f;
    public static readonly float GROUND_Y_POS = 1.18f;


    // Birdie serving position constants
    public static readonly Vector3 SERVING_OFFSET_PLAYER_ONE = new Vector3(1, 0, -0.35f);
    public static readonly Vector3 SERVING_OFFSET_PLAYER_TWO = new Vector3(-1, 0, -0.35f);
    public static readonly Vector3 BIRDIE_DEFAULT_POSITION = new Vector3(0, 20f, 0);

    // Serve arrow angle constants
    public static readonly Vector3 SERVE_ANGLE_HIGH = new Vector3(0, 0, 45);
    public static readonly Vector3 SERVE_ANGLE_LOW = new Vector3(0, 0, 15);

    // Score constants
    public static readonly int WINNING_SCORE = 11;
    public static readonly int MAX_SCORE = 15;

    // Physics constants
    public static readonly float GRAVITY = -20f;
    public static readonly float PLAYER_X_DRAG = 5f;

    // Stamina constants
    public static readonly float DASH_STAMINA_COST = 34f;

    // Fast Fall contants
    public static readonly float FAST_FALL_TIME_FRAME = 0.1f;
    public static readonly float FAST_FALL_SPEED = -150f;

    // Referee flag constants (multiply x-pos by -1 for red flag)
    public static readonly Vector3 FLAG_DOWN_POSITION = new Vector3(0.3f, -0.75f, 0);
    public static readonly Vector3 FLAG_DOWN_ROTATION = new Vector3(180, 0, 0);
    public static readonly Vector3 FLAG_UP_POSITION = new Vector3(0.65f, 0.75f, 0);
    public static readonly Vector3 FLAG_UP_ROTATION = new Vector3(345, 90, 0);


}
