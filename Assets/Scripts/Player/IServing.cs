using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServeAngle
{
    High,
    Low
}


public interface IServing
{
    public bool isServing { get; set; }
    public bool isOpponentServing { get; set; }
    void ChangeServeAngle();
    Vector3 GetServeAngle();

}
