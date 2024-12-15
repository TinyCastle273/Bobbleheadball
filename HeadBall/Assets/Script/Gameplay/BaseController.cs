using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected PlayerCharacter character;
    protected float horizontalMove = 0f;
    protected bool jump = false;
    protected bool kicklow = false;
    protected bool kickhigh = false;

    protected virtual void Awake()
    {
        character = GetComponent<PlayerCharacter>();
    }

    public void SetEnable(bool enable)
    {
        this.enabled = enable;
    }

    protected virtual void FixedUpdate()
    {
        // actually calculate physics
        character.MoveCharacter(horizontalMove * Time.fixedDeltaTime, jump);
        character.DoAction(kicklow, kickhigh);
        jump = kicklow = kickhigh = false;
    }
}
