using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public abstract class Agent : MonoBehaviour
{
    public enum State { Idle, Prone, Walk, Jump, AttackLight, AttackHeavy, AttackJump, Parry, Dead }
    #region Variables
    [SerializeField] protected State _currentState;
    [SerializeField] protected Meter _healthMeter;
    [SerializeField] protected Meter _parryMeter;
    [SerializeField] protected float _walkSpeed, _jumpHeight, _gravity, _attackLightPower, _attackHeavyPower;
    [SerializeField] LayerMask _groundLayer;
    [Header("Components")]
    [SerializeField] protected Collider2D _collider;
    [SerializeField] protected Rigidbody2D _rigidbody;
    [SerializeField] protected Animator _animator;

    [SerializeField] private float _isGroundedDistance, _isGroundedRadius;

    [SerializeField] protected float _facingDirection = 1;

    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    #endregion

    /// <summary>
    /// Returns the current state of the agent
    /// </summary>
    public State CurrentState => _currentState;
    /// <summary>
    /// Returns the direction the agent is facing where 1 is right and -1 is left
    /// </summary>
    public Vector3 FacingDirection => _facingDirection * Vector3.right;
    /// <summary>
    /// Returns the agent's x coordinate
    /// </summary>
    public float X => transform.position.x;
    /// <summary>
    /// Returns the agent's y coordinate
    /// </summary>
    public float Y => transform.position.y;
    /// <summary>
    /// Returns gravity multiplied by fixed delta time
    /// </summary>
    public float Gravity => _gravity * Time.fixedDeltaTime;
    /// <summary>
    /// Returns true if a circlecast finds a collision underneath the rigidbody on Ground Layer
    /// </summary>
    public bool IsGrounded => Physics2D.CircleCast(transform.position, _isGroundedRadius, Vector2.down, _isGroundedDistance, _groundLayer);



    private void OnValidate()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
        if (_collider == null)
            _collider = GetComponent<Collider2D>();
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        //sets the state to dead if health reaches 0
        _healthMeter.onMin += () => _currentState = State.Dead;
        //sets the state to prone if parry power runs out
        _parryMeter.onMin += () => _currentState = State.Prone;

        _isGroundedDistance = _collider.bounds.extents.y / 2f + 0.01f;
        _isGroundedRadius = _collider.bounds.extents.x;
        NextState();
    }

    #region State Machine
    /// <summary>
    /// Triggers the next state behaviour based on _currentState
    /// </summary>
    private void NextState()
    {
        StartCoroutine(_currentState.ToString());
        Debug.Log($"Entered {_currentState.ToString()} state");
    }
    #region Coroutines
    IEnumerator Idle()
    {
        IdleEnter();
        //entry behaviour here
        while (_currentState == State.Idle)
        {
            IdleStay();
            yield return waitForFixedUpdate;
        }
        IdleExit();
        NextState();
    }
    IEnumerator Prone()
    {
        ProneEnter();
        while (_currentState == State.Prone)
        {
            ProneStay();
            yield return waitForFixedUpdate;
        }
        ProneExit();
        NextState();
    }
    IEnumerator Walk()
    {
        WalkEnter();
        while (_currentState == State.Walk)
        {
            WalkStay();
            yield return waitForFixedUpdate;
        }
        WalkExit();
        NextState();
    }
    IEnumerator Jump()
    {
        JumpEnter();
        while (_currentState == State.Jump)
        {
            JumpStay();
            yield return waitForFixedUpdate;
        }
        JumpExit();
        NextState();
    }
    IEnumerator AttackLight()
    {
        AttackLightEnter();
        while (_currentState == State.AttackLight)
        {
            AttackLightStay();
            yield return waitForFixedUpdate;
        }
        AttackLightExit();
        NextState();
    }
    IEnumerator AttackHeavy()
    {
        AttackHeavyEnter();
        while (_currentState == State.AttackHeavy)
        {
            AttackHeavyStay();
            yield return waitForFixedUpdate;
        }
        AttackHeavyExit();
        NextState();
    }
    IEnumerator AttackJump()
    {
        AttackJumpEnter();
        while (_currentState == State.AttackJump)
        {
            AttackJumpStay();
            yield return waitForFixedUpdate;
        }
        AttackJumpExit();
        NextState();
    }
    IEnumerator Parry()
    {
        ParryEnter();
        while (_currentState == State.Parry)
        {
            ParryStay();
            yield return waitForFixedUpdate;
        }
        ParryExit();
        NextState();
    }
    IEnumerator Dead()
    {
        DeadEnter();
        while (_currentState == State.Dead)
        {
            DeadStay();
            yield return waitForFixedUpdate;
        }
        DeadExit();
        NextState();
    }
    #endregion

    #region State Enter
    /// <summary>
    /// Runs once when entering Idle state
    /// </summary>
    protected abstract void IdleEnter();
    /// <summary>
    /// Runs once when entering Prone state
    /// </summary>
    protected abstract void ProneEnter();
    /// <summary>
    /// Runs once when entering Walk state
    /// </summary>
    protected abstract void WalkEnter();
    /// <summary>
    /// Runs once when entering Jump state
    /// </summary>
    protected abstract void JumpEnter();
    /// <summary>
    /// Runs once when entering AttackLight state
    /// </summary>
    protected abstract void AttackLightEnter();
    /// <summary>
    /// Runs once when entering AttackHeavy state
    /// </summary>
    protected abstract void AttackHeavyEnter();
    /// <summary>
    /// Runs once when entering AttackJump state
    /// </summary>
    protected abstract void AttackJumpEnter();
    /// <summary>
    /// Runs once when entering Parry state
    /// </summary>
    protected abstract void ParryEnter();
    /// <summary>
    /// Runs once when entering Dead state
    /// </summary>
    protected abstract void DeadEnter();
    #endregion
    #region State Stay
    /// <summary>
    /// Runs once when entering Idle state
    /// </summary>
    protected abstract void IdleStay();
    /// <summary>
    /// Runs every frame while in Prone state
    /// </summary>
    protected abstract void ProneStay();
    /// <summary>
    /// Runs every frame while in Walk state
    /// </summary>
    protected abstract void WalkStay();
    /// <summary>
    /// Runs every frame while in Jump state
    /// </summary>
    protected abstract void JumpStay();
    /// <summary>
    /// Runs every frame while in AttackLight state
    /// </summary>
    protected abstract void AttackLightStay();
    /// <summary>
    /// Runs every frame while in AttackHeavy state
    /// </summary>
    protected abstract void AttackHeavyStay();
    /// <summary>
    /// Runs every frame while in AttackJump state
    /// </summary>
    protected abstract void AttackJumpStay();
    /// <summary>
    /// Runs every frame while in Parry state
    /// </summary>
    protected abstract void ParryStay();
    /// <summary>
    /// Runs every frame while in Dead state
    /// </summary>
    protected abstract void DeadStay();
    #endregion
    #region State Exit
    /// <summary>
    /// Runs once when exiting Idle state
    /// </summary>
    protected abstract void IdleExit();
    /// <summary>
    /// Runs once when exiting Prone state
    /// </summary>
    protected abstract void ProneExit();
    /// <summary>
    /// Runs once when exiting Walk state
    /// </summary>
    protected abstract void WalkExit();
    /// <summary>
    /// Runs once when exiting Jump state
    /// </summary>
    protected abstract void JumpExit();
    /// <summary>
    /// Runs once when exiting AttackLight state
    /// </summary>
    protected abstract void AttackLightExit();
    /// <summary>
    /// Runs once when exiting AttackHeavy state
    /// </summary>
    protected abstract void AttackHeavyExit();
    /// <summary>
    /// Runs once when exiting AttackJump state
    /// </summary>
    protected abstract void AttackJumpExit();
    /// <summary>
    /// Runs once when exiting Parry state
    /// </summary>
    protected abstract void ParryExit();
    /// <summary>
    /// Runs once when exiting Dead state
    /// </summary>
    protected abstract void DeadExit();
    #endregion
    #endregion

    /// <summary>
    /// Moves the agent using a Vector2 based on fixedDeltaTime
    /// </summary>
    /// <param name="moveDirection"></param>
    protected virtual void Move(Vector2 moveDirection)
    {
        transform.Translate(moveDirection * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Reduce the health meter by f and trigger death if it reaches 0 
    /// </summary>
    /// <param name="f"></param>
    public virtual void TakeDamage(float f)
    {
        _healthMeter.Adjust(-f);
    }

    /// <summary>
    /// Increase the health by f
    /// </summary>
    /// <param name="f"></param>
    public virtual void Heal(float f)
    {
        _healthMeter.Adjust(f);
    }

    public virtual void ReduceStanima(float f)
    {

    }

    /// <summary>
    /// Returns true if this agent and otherAgent are facing opposite directions
    /// </summary>
    /// <param name="otherAgent"></param>
    /// <returns></returns>
    public bool IsFacing(Agent otherAgent)
    {
        return otherAgent.FacingDirection.x != FacingDirection.x &&   //agents are facing opposite directions
            Mathf.Sign(otherAgent.X - X) == FacingDirection.x;   //agent is on the left side of otherAgent if facing right, and vice-versa
    }

    /// <summary>
    /// Immediately change the agent's state
    /// </summary>
    /// <param name="state"></param>
    protected void ChangeStateTo(State state)
    {
        _currentState = state;
    }

}
