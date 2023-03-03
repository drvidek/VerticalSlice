using UnityEngine;

public class Enemy : Agent
{
    private Alarm nextStateAlarm;
    private Vector2 _moveDir;
    [Header("Enemy Behaviour")]
    [Header("Sight Checks")]
    [SerializeField] private Transform SightOrigin;
    [SerializeField] private float _sightDistance;
    [SerializeField] private LayerMask _playerLayer;
    [Header("Ground Checks")]
    [SerializeField] private float _dropCheckMaxDistance;
    [SerializeField] private LayerMask _dropCheckLayer;

    private float _animTime;
    private Weapon _weapon;


    private void Update()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    protected override void Start()
    {
        nextStateAlarm = Alarm.Get(0f, false, false);

        base.Start();
    }

    protected override void AttackHeavyEnter()
    {
        _animator.SetTrigger("AttackEnter");
        _animator.SetTrigger("AttackHeavy");
        nextStateAlarm.Stop();
    }
    protected override void AttackHeavyStay()
    {
        if (MathExt.Roll(2) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            ChangeStateTo(State.AttackLight);
    }
    protected override void AttackHeavyExit()
    {
        if (!Attacking)
            _animator.SetTrigger("AttackExit");

        _animator.ResetTrigger("AttackHeavy");
    }

    protected override void AttackJumpEnter()
    {
        //throw new System.NotImplementedException();
    }
    protected override void AttackJumpStay()
    {
        //throw new System.NotImplementedException();
    }
    protected override void AttackJumpExit()
    {
        if (!Attacking)
            _animator.SetTrigger("AttackExit");
    }


    protected override void AttackLightEnter()
    {
        _animator.SetTrigger("AttackEnter");
        _animator.SetTrigger("AttackLight");
        nextStateAlarm.Stop();
    }
    protected override void AttackLightStay()
    {
        switch (PlayerSeen())
        {
            case false:
                ChangeStateTo(State.Idle);
                break;
            case true:
                if (MathExt.Roll(4) && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                    ChangeStateTo(State.AttackHeavy);
                break;
        }
    }
    protected override void AttackLightExit()
    {
        if (!Attacking)
            _animator.SetTrigger("AttackExit");

        _animator.ResetTrigger("AttackLight");

    }


    protected override void DeadEnter()
    {
        //throw new System.NotImplementedException();
    }
    protected override void DeadStay()
    {
        //throw new System.NotImplementedException();
    }
    protected override void DeadExit()
    {
        //throw new System.NotImplementedException();
    }


    protected override void IdleEnter()
    {
        //Cleanup any attack overlaps
        _animator.ResetTrigger("AttackEnter");
        //trigger walking in 2 sec
        SetStateAlarm(2f, MathExt.Roll(4) ? State.Jump : State.Walk);
        //set the horizontal movement to 0
        _moveDir.x = 0;
    }

    protected override void IdleStay()
    {
        _moveDir.y = IsGrounded ? Mathf.Clamp(_moveDir.y, 0, float.PositiveInfinity) : _moveDir.y - Gravity;
        Move(_moveDir);
    }
    protected override void IdleExit()
    {

    }

    #region Jump
    protected override void JumpEnter()
    {
        //set the move direction's Y value to the jump power
        _moveDir.y = _jumpHeight;
    }
    protected override void JumpStay()
    {
        //move forward as you jump
        _moveDir.x = FacingDirection.x * _walkSpeed/2;
        //apply gravity
        _moveDir.y -= Gravity;
        //move the agent
        Move(_moveDir);
        //face the right direction
        transform.localScale = MathExt.ReplaceVectorValue(transform.localScale, VectorValue.x, _facingDirection);

        //exit jump state if grounded
        if (IsGrounded)
            ChangeStateTo(State.Idle);
    }

    protected override void JumpExit()
    {
        //throw new System.NotImplementedException();
    }
    #endregion

    protected override void ParryEnter()
    {
        //throw new System.NotImplementedException();
    }
    protected override void ParryStay()
    {
        //throw new System.NotImplementedException();
    }
    protected override void ParryExit()
    {
        //throw new System.NotImplementedException();
    }


    protected override void ProneEnter()
    {
        _animator.ResetTrigger("AttackHeavy");
        _animator.ResetTrigger("AttackLight");
        _animator.ResetTrigger("AttackEnter");
        _animator.ResetTrigger("AttackExit");
        _animator.SetTrigger("Stagger");
        SetStateAlarm(_proneTime, State.Idle);
    }
    protected override void ProneStay()
    {
        //throw new System.NotImplementedException();
    }
    protected override void ProneExit()
    {
        if (PlayerSeen())
            ChangeStateTo(MathExt.Roll(3) ? State.AttackHeavy : State.AttackLight);
        else
            _animator.SetTrigger("AttackExit");
    }


    protected override void WalkEnter()
    {
        SetStateAlarm(4f, State.Idle);

        if (MathExt.Roll(2))
            _facingDirection *= -1;

    }
    protected override void WalkStay()
    {
        _moveDir.x = FacingDirection.x * _walkSpeed;
        _moveDir.y = IsGrounded ? Mathf.Clamp(_moveDir.y, 0, float.PositiveInfinity) : _moveDir.y;
        _moveDir.y -= Gravity;
        if (!GroundFoundInFront())
            _facingDirection *= -1;
        Move(_moveDir);

        transform.localScale = MathExt.ReplaceVectorValue(transform.localScale, VectorValue.x, _facingDirection);

        if (PlayerSeen())
        {
            if (MathExt.Roll(3))
                ChangeStateTo(State.AttackHeavy);
            else
                ChangeStateTo(State.AttackLight);
        }
    }
    protected override void WalkExit()
    {

    }

    /// <summary>
    /// Returns true if a raycast detects a collider on _dropChecklayer
    /// </summary>
    /// <returns></returns>
    private bool GroundFoundInFront()
    {
        return Physics2D.Raycast(transform.position + (FacingDirection), Vector3.down, _dropCheckMaxDistance, _dropCheckLayer);
    }

    /// <summary>
    /// In t seconds, change the state to nextState
    /// </summary>
    /// <param name="t"></param>
    /// <param name="nextState"></param>
    private void SetStateAlarm(float t, State nextState)
    {
        nextStateAlarm.onComplete = () => _currentState = nextState;
        nextStateAlarm.ResetAndPlay(t);
    }

    private bool PlayerSeen()
    {
        return Physics2D.Raycast(SightOrigin.position, FacingDirection, _sightDistance, _playerLayer);
    }


}
