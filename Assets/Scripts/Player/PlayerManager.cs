using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int playerNum;

    public KeyCode jumpKey;
    public KeyCode swingKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode changeServeAngleKey;


    private void Start()
    {
        if (playerNum == 1)
        {
            jumpKey = KeyCode.W;
            swingKey = KeyCode.S;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            changeServeAngleKey = KeyCode.W;
        }
        else
        {
            jumpKey = KeyCode.UpArrow;
            swingKey = KeyCode.DownArrow;
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            changeServeAngleKey = KeyCode.UpArrow;

        }

    }

}
