using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Photon.Pun;

public class ControllableCharacter : ObjectBase
{
    // Start is called before the first frame update
    public Camera _camera;
    
    void Start()
    {
        if (!photonView.IsMine)
        {
            _camera.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
