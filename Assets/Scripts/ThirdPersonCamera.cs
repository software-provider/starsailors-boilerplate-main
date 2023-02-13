using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : InstanceMonoBehaviour<ThirdPersonCamera> // for controlling the player look camera
{
    public Camera camera_ref;

    protected override void Awake()
    {
        base.Awake();

        camera_ref = GetComponent<Camera>();
    }
}
