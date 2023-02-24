using UnityEngine;

public class Enemy : Agent
{
    private Alarm nextStateAlarm;
    private float _facingDirection = 1;
    public float FacingDirection=> _facingDirection;
    private Vector2 _moveDir;
    [Header("Enemy Behaviour")]
    [SerializeField] private float _dropCheckMaxDistance;
    [SerializeField] private LayerMask _dropCheckLayer;


    private void Update()
    {
        Debug.Log($"Is grounded: {IsGrounded}");
    }

    protected override void Start()
    {
        nextStateAlarm = Alarm.Get(0f, false, false);
        
        base.Start();
    }

    protected override void AttackHeavyEnter()
    {
        throw new System.NotImplementedException();
    }
    protected override void AttackHeavyStay()
    {
        throw new System.NotImplementedException();
    }
    protected override void AttackHeavyExit()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackJumpEnter()
    {
        throw new System.NotImplementedException();
    }
    protected override void AttackJumpStay()
    {
        throw new System.NotImplementedException();
    }
    protected override void AttackJumpExit()
    {
        throw new System.NotImplementedException();
    }


    protected override void AttackLightEnter()
    {
        throw new System.NotImplementedException();
    }
    protected override void AttackLightStay()
    {
        throw new System.NotImplementedException();
    }
    protected override void AttackLightExit()
    {
        throw new System.NotImplementedException();
    }


    protected override void DeadEnter()
    {
        throw new System.NotImplementedException();
    }
    protected override void DeadStay()
    {
        throw new System.NotImplementedException();
    }
    protected override void DeadExit()
    {
        throw new System.NotImplementedException();
    }


    protected override void IdleEnter()
    {
        SetStateAlarm(2f, State.Walk);
        _moveDir.x = 0;
    }

    protected override void IdleStay()
    {
        _moveDir.y = IsGrounded ? Mathf.Clamp(_moveDir.y, 0, float.PositiveInfinity) : _moveDir.y;
        _moveDir.y -= Gravity;
        Move(_moveDir);
    }
    protected override void IdleExit()
    {

    }

    #region Jump
    protected override void JumpEnter()
    {
        throw new System.NotImplementedException();
    }
    protected override void JumpStay()
    {
        throw new System.NotImplementedException();
    }
    protected override void JumpExit()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    protected override void ParryEnter()
    {
        throw new System.NotImplementedException();
    }
    protected override void ParryStay()
    {
        throw new System.NotImplementedException();
    }
    protected override void ParryExit()
    {
        throw new System.NotImplementedException();
    }


    protected override void ProneEnter()
    {
        throw new System.NotImplementedException();
    }
    protected override void ProneStay()
    {
        throw new System.NotImplementedException();
    }
    protected override void ProneExit()
    {
        throw new System.NotImplementedException();
    }


    protected override void WalkEnter()
    {
        SetStateAlarm(4f, State.Idle);

        if (MathExt.Roll(2))
            _facingDirection *= -1;
    }
    protected override void WalkStay()
    {
        _moveDir.x = FacingDirection * _walkSpeed;
        _moveDir.y = IsGrounded ? Mathf.Clamp(_moveDir.y, 0, float.PositiveInfinity) : _moveDir.y;
        _moveDir.y -= Gravity;
        if (!GroundFoundInFront())
            _facingDirection *= -1;
        Move(_moveDir);
    }
    protected override void WalkExit()
    {
        
    }

    private bool GroundFoundInFront()
    {
        return Physics2D.Raycast(transform.position + (Vector3.right * FacingDirection), Vector3.down, _dropCheckMaxDistance, _dropCheckLayer);
    }

    private void SetStateAlarm(float time, State nextState)
    {
        nextStateAlarm.onComplete = () => _currentState = nextState;
        nextStateAlarm.ResetAndPlay(time);
    }

}
