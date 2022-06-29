using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectBase : MonoBehaviourPunCallbacks
{
    public float hp;
    public float att;
    public float def;
    public float maxHp;
    protected float spawnTime;
    public float moveSpeed;
    public float runSpeed;
    public float jumpForce;
    public float gravity;

    protected int speedHash;
    protected int groundedHash;
    protected int jumpHash;
    //protected int attackHash;


    protected Animator _animator;
    protected CharacterController _controller;

    private bool _hasAnimator;


    void Start()
    {
        // retrieve relevant controller details
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>(); 

        if (_hasAnimator) 
            assignAnimationId();
    }

    void assignAnimationId()
    {
        speedHash = Animator.StringToHash("Speed");
        groundedHash = Animator.StringToHash("Grounded");
        jumpHash = Animator.StringToHash("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
