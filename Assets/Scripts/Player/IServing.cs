using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IServing
{
    public bool isServing { get; set; }
    void ChangeServeAngle();
    Vector3 GetServeAngle();

}
