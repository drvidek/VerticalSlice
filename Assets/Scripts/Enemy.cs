using UnityEngine;

public class Enemy : Agent
{
    private Alarm nextStateAlarm;
    private Vector2 _moveDir;
    [Header("Enemy Behaviour")]
    [SerializeField] private Transform SightOrigin;
    [SerializeField] private float _sightDistance;
    [SerializeField] private float _dropCheckMaxDistance;
    [SerializeField] private LayerMask _dropCheckLayer;
    [SerializeField] private LayerMask _playerLayer;


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
        //throw new System.NotImplementedException();
    }
    protected override void AttackHeavyStay()
    {
        //throw new System.NotImplementedException();
    }
    protected override void AttackHeavyExit()
    {
        //throw new System.NotImplementedException();
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
        //throw new System.NotImplementedException();
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
                break;
        }


    }
    protected override void AttackLightExit()
    {
        _animator.SetTrigger("AttackExit");

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
        SetStateAlarm(2f, State.Walk);
        _moveDir.x = 0;
    }

    protected override void IdleStay()
    {
        _moveDir.y = IsGrounded ? Mathf.Clamp(_moveDir.y, 0, float.PositiveInfinity) : _moveDir.y - Gravity;
        Move(_moveDir);
    }
    protected override void IdleExit()
    {
        _animator.ResetTrigger("Idle");
    }

    #region Jump
    protected override void JumpEnter()
    {
        //throw new System.NotImplementedException();
    }
    protected override void JumpStay()
    {
        //throw new System.NotImplementedException();
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
        //throw new System.NotImplementedException();
    }
    protected override void ProneStay()
    {
        //throw new System.NotImplementedException();
    }
    protected override void ProneExit()
    {
        //throw new System.NotImplementedException();
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(SightOrigin.position, SightOrigin.position + _sightDistance * FacingDirection);
    }

}
