using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{
    private Vector2 _moveDir;
    [SerializeField] private Transform _sprite;

    public bool InputAttackPress => Input.GetKeyDown(KeyCode.Space);
    public bool InputAttackHold => Input.GetKey(KeyCode.Space);
    public bool InputAttackRelease => Input.GetKeyUp(KeyCode.Space);
    public bool InputJump => Input.GetKeyDown(KeyCode.W);
    public float InputHorizontal => Input.GetAxis("Horizontal");

    [SerializeField] private Alarm _proneAlarm;

    override public bool IsParrying => _currentState == State.Parry || _currentState == State.Idle;

    private void Update()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    protected override void AttackHeavyEnter()
    {
        _animator.SetBool("AttackHeavy", true);
    }

    protected override void AttackHeavyExit()
    {

    }

    protected override void AttackHeavyStay()
    {
        if (AnimationDone)
        {
            ChangeStateTo(State.Idle);
        }
    }

    protected override void AttackJumpEnter()
    {
        ChangeStateTo(State.Jump);
    }

    protected override void AttackJumpExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackJumpStay()
    {
        State nextState =
            IsGrounded ? State.Idle :
            _currentState;

        if (nextState != _currentState)
            ChangeStateTo(nextState);
    }

    protected override void AttackLightEnter()
    {
        _animator.SetTrigger("AttackEnter");
    }

    protected override void AttackLightExit()
    {
        _animator.SetBool("AttackLight", false);
    }

    protected override void AttackLightStay()
    {
        if (!_animator.GetBool("AttackLight"))
        {
            //if you're no longer holding attack, trigger a light attack animation
            if (InputAttackRelease)
            {
                _animator.SetBool("AttackLight", true);
            }
            else    //if you're holding attack and your windup animation is done
                if (InputAttackHold && AnimationDone)
            {
                ChangeStateTo(State.AttackHeavy);
            }
        }

        //if your light attack animation is done
        if (AnimationDone && _animator.GetBool("AttackLight"))
        {
            ChangeStateTo(State.Idle);
        }
    }

    protected override void DeadEnter()
    {
        //throw new System.NotImplementedException();
    }

    protected override void DeadExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void DeadStay()
    {
        //throw new System.NotImplementedException();
    }

    protected override void IdleEnter()
    {
        _animator.SetTrigger("IdleEnter");
        _animator.SetBool("AttackHeavy", false);

    }

    protected override void IdleExit()
    {
        _animator.ResetTrigger("IdleEnter");
    }

    protected override void IdleStay()
    {
        State nextState =
            InputAttackPress || InputAttackHold ? State.AttackLight :
            InputJump ? State.Jump :
            InputHorizontal != 0 ? State.Walk :
            _currentState;

        if (nextState != _currentState)
            ChangeStateTo(nextState);
    }

    protected override void JumpEnter()
    {
        _moveDir.y = _jumpHeight;
    }

    protected override void JumpExit()
    {

    }

    protected override void JumpStay()
    {
        _moveDir.x = InputHorizontal * _walkSpeed * 2 / 3;
        _moveDir.y -= Gravity;

        Move(_moveDir);
        FlipWithHorizontalInput();

        State nextState =
            IsGrounded ? State.Idle :
            _currentState;

        if (nextState != _currentState)
            ChangeStateTo(nextState);
    }

    protected override void ParryEnter()
    {
        //throw new System.NotImplementedException();
    }

    protected override void ParryExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void ParryStay()
    {
        //throw new System.NotImplementedException();
    }

    protected override void ProneEnter()
    {
        _proneAlarm.onComplete = () => ChangeStateTo(State.Idle);
        _proneAlarm.ResetAndPlay(_proneTime);
    }

    protected override void ProneExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void ProneStay()
    {
        //throw new System.NotImplementedException();
    }

    protected override void WalkEnter()
    {
        _animator.SetTrigger("WalkEnter");
    }

    protected override void WalkExit()
    {

    }

    protected override void WalkStay()
    {
        _moveDir.x = InputHorizontal * _walkSpeed;
        _moveDir.y = IsGrounded ? Mathf.Clamp(_moveDir.y, 0, float.PositiveInfinity) : _moveDir.y;
        _moveDir.y -= Gravity;
        Move(_moveDir);
        FlipWithHorizontalInput();

        State nextState =
            InputAttackPress ? State.AttackLight :
            InputJump ? State.Jump :
            InputHorizontal == 0 ? State.Idle :
            _currentState;
        if (nextState != _currentState)
            ChangeStateTo(nextState);
    }

    private void FlipWithHorizontalInput()
    {
        if (InputHorizontal != 0)
        {
            _facingDirection = Mathf.Sign(InputHorizontal);
            _sprite.localScale = MathExt.ReplaceVectorValue(_sprite.transform.localScale, VectorValue.x, _facingDirection);
            _sprite.localPosition = MathExt.ReplaceVectorValue(_sprite.transform.localPosition, VectorValue.x, _facingDirection < 0 ? .35f : 0);
        }
    }

    public override void ReduceStanima(float f)
    {
        base.ReduceStanima(f);
        _animator.SetTrigger("Parry");
    }

}
