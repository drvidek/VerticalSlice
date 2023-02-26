using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Agent
{


    protected override void Start()
    {
        base.Start();
        _healthMeter.onMin = () => Destroy(this.gameObject);
    }

    protected override void AttackHeavyEnter()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackHeavyExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackHeavyStay()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackJumpEnter()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackJumpExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackJumpStay()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackLightEnter()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackLightExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void AttackLightStay()
    {
        //throw new System.NotImplementedException();
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
        //throw new System.NotImplementedException();
    }

    protected override void IdleExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void IdleStay()
    {
        //throw new System.NotImplementedException();
    }

    protected override void JumpEnter()
    {
        //throw new System.NotImplementedException();
    }

    protected override void JumpExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void JumpStay()
    {
        //throw new System.NotImplementedException();
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
        //throw new System.NotImplementedException();
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
        //throw new System.NotImplementedException();
    }

    protected override void WalkExit()
    {
        //throw new System.NotImplementedException();
    }

    protected override void WalkStay()
    {
        //throw new System.NotImplementedException();
    }

}
