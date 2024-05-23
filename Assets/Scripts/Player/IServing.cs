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
    void SetServeAngle(ServeAngle serveAngle);
    Vector3 GetServeAngle();

}
