using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    [Header("Player Controller")]
    [SerializeField] EffectButtonCtrl leftBtn;
    [SerializeField] EffectButtonCtrl rightBtn;
    [SerializeField] EffectButtonCtrl jumpBtn;
    [SerializeField] EffectButtonCtrl lowBtn;
    [SerializeField] EffectButtonCtrl highBtn;

    [Header("AI Controller")]
    [SerializeField] BallController ball;
    [SerializeField] PlayerCharacter other;
    [SerializeField] Transform goalLeft;
    [SerializeField] Transform goalRight;
    [SerializeField] float timeUpdateAi = 0.05f;
    [SerializeField] float radiusAware = 3.75f;
    [SerializeField] float radiusMiddle = 2.2f;
    [SerializeField] float radiusResponse = 1.3f;
    [SerializeField] float yBall = -1f;
    [SerializeField] float halfChar = 0.5f;
    [SerializeField] float belowChar = 1.26f;
    [SerializeField] float aboveChar = 0.55f;
    [SerializeField] int skipDecision = 2;
    [SerializeField] bool doNothing;

    private float shootTime;
    private int skipDecisionCounter = -1;
    private int tempMove;
    protected override void Awake()
    {
        base.Awake();
        if (!character.isLeft)
        {

        }
        else
        {
            leftBtn.OnDown = GoLeft;
            rightBtn.OnDown = GoRight;
            leftBtn.OnUp = LiftUp;
            rightBtn.OnUp = LiftUp;

            jumpBtn.OnDown = () => jump = true;
            lowBtn.OnDown = () => kicklow = true;
            highBtn.OnDown = () => kickhigh = true;
        }
        
    }

    private void GoLeft()
    {
        tempMove = -1;
    }

    private void GoRight()
    {
        tempMove = 1;
    }

    private void LiftUp()
    {
        tempMove = 0;
    }

    private void Update()
    {
        if (!character.isLeft)
        {
            if (doNothing)
            {
#if UNITY_EDITOR
                horizontalMove = Input.GetAxisRaw("Horizontal2") * character.GetSpeed();

                if (Input.GetButtonDown("Jump2"))
                {
                    jump = true;
                }

                if (Input.GetButtonDown("LowKick2"))
                {
                    kicklow = true;
                }

                if (Input.GetButtonUp("HighKick2"))
                {
                    kickhigh = true;
                }
#endif
            }
            else
            {
                shootTime += Time.deltaTime;
                horizontalMove = tempMove * character.GetSpeed();
            }
        }
        else
        {
#if UNITY_EDITOR
            CheckKeyboardInput();
#else
            CheckForTouch();
#endif
        }
    }

    private void CheckKeyboardInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * character.GetSpeed();

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("LowKick"))
        {
            kicklow = true;
        }

        if (Input.GetButtonUp("HighKick"))
        {
            kickhigh = true;
        }
    }

    private void CheckForTouch()
    {
        horizontalMove = tempMove * character.GetSpeed();
    }

#region code for AI
    private void OnEnable()
    {
        if (!character.isLeft)
        {
            tempMove = 0;
            kickhigh = kicklow = jump = false;
            StartCoroutine(IEUpdateAI());
        }
    }

    private void OnDisable()
    {
        if (!character.isLeft)
        {
            StopAllCoroutines();
        }
    }
    
    // the entire logic assume AI always on the right side
    IEnumerator IEUpdateAI()
    {
        while (true)
        {
            if (character.isInHitMode) DoShoot();

            // no skip decision signal, just update logic
            if (skipDecisionCounter == -1)
            {
                var ballPos = ball.transform.position;
                var aiPos = transform.position;
                var ballDistance = (ballPos - aiPos).magnitude;
                var vel = ball.GetRigidbody2D().velocity;

                bool ballFollow = false;
                if (ballDistance <= radiusResponse)
                {
                    ballFollow = true;
                    // ball behind ai
                    if (ballPos.x >= aiPos.x + halfChar)
                    {
                        if (character.isOnGround)
                        {
                            // ball on the ground??
                            // dont move much jump over
                            // try jump over
                            if (ballPos.y <= yBall && vel.x < 2.5f)
                            {
                                tempMove = 1;
                                character.DoJump(new Vector2(1f, 1f));
                                skipDecisionCounter = 0;
                            }
                            else
                            {
                                // ball falling down
                                if (ballPos.y < 0.1f && vel.x < 2.5f && vel.y < 0f)
                                {
                                    // stop for 1 decision
                                    tempMove = 0;
                                    skipDecisionCounter = skipDecision - 2;

                                }
                                else
                                {
                                    // just follow it
                                    tempMove = 1;
                                }
                            }
                        }
                        // character is jumping
                        else
                        {
                            if (ballPos.y < aiPos.y - belowChar || ballPos.y > aiPos.y + aboveChar)
                            {
                                tempMove = 1;
                            }
                            else
                            {
                                tempMove = 0;
                            }
                        }
                    }
                    // ball front
                    else
                    {
                        if (character.isOnGround)
                        {
                            // ball on ground shoot
                            if (ballPos.y <= yBall)
                            {
                                // should move or stand still
                                tempMove = Random.Range(0f, 1f) > 0.5f ? -1 : 0;
                                DoShoot();

                            }
                            else
                            {
                                // ball in the air, jump it
                                if (ballPos.y > aiPos.y + aboveChar)
                                {
                                    MakeJumpToTheBall();
                                    tempMove = 0;
                                }
                                else
                                {
                                    // ball fly
                                    if (vel.y > 2.5f)
                                    {
                                        MakeJumpToTheBall();
                                        tempMove = 0;
                                    }
                                    else if (ballPos.y < aiPos.y - 0.42f)
                                    {
                                        DoShoot();
                                    }
                                    else
                                    {
                                        tempMove = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (ballPos.y < aiPos.y - belowChar || ballPos.y > aiPos.y + aboveChar)
                            {
                                tempMove = 1;
                            }
                            else if (ballPos.y < aiPos.y - 0.42f)
                            {
                                DoShoot();
                            }
                            else
                            {
                                tempMove = 0;
                            }
                        }
                    }
                }
                else if (ballDistance <= radiusMiddle)
                {
                    ballFollow = true;
                    if (ballPos.x >= aiPos.x + halfChar)
                    {
                        if (character.isOnGround)
                        {
                            if (ballPos.x <= aiPos.x + halfChar * 2f)
                            {
                                MakeJumpToTheBall();
                                tempMove = 0;
                            }
                            else
                            {
                                tempMove = 1;
                            }
                        }
                        else
                        {
                            tempMove = 1;
                        }
                    }
                    else
                    {
                        if (character.isOnGround)
                        {
                            if (ballPos.y > aiPos.y + aboveChar)
                            {
                                if (vel.x > 10f && vel.y > 10f)
                                {
                                    //character.DoJump(Vector2.up);
                                    MakeJumpToTheBall();
                                    tempMove = 0;
                                    //skipDecisionCounter = 0;
                                }
                                else
                                {
                                    MakeJumpToTheBall();
                                    tempMove = 0;
                                }
                            }
                            else
                            {
                                tempMove = -1;
                            }
                        }
                        else
                        {
                            //ball falling, back a bit
                            if (vel.x < 5f && vel.y < 5f)
                            {
                                tempMove = 1;
                            }
                            // stand still for the ball
                            else if (ballPos.y > aiPos.y - belowChar && ballPos.y < aiPos.y + aboveChar)
                            {
                                tempMove = 0;
                            }
                            else
                            {
                                tempMove = -1;
                            }
                        }
                    }
                }
                else if (ballDistance <= radiusAware) {
                    ballFollow = true;
                    if (ballPos.x >= aiPos.x + halfChar)
                    {
                        tempMove = 1;
                    }
                    else
                    {
                        // ball on ground
                        if (ballPos.y < yBall)
                        {
                            tempMove = Random.Range(0f, 1f) > 0.5f ? -1 : 0;
                            skipDecisionCounter = 0;
                        }
                        else
                        {
                            if (character.isOnGround)
                            {
                                // shoot from afar
                                if (ballPos.y > aiPos.y + aboveChar && vel.x > 14f)
                                {
                                    MakeJumpToTheBall();
                                }
                                else
                                {
                                    tempMove = 0;
                                }
                            }
                            else
                            {
                                tempMove = 0;
                            }
                        }
                    } 
                }

                var charPos = other.transform.position;
                var charDistance = (charPos - aiPos).magnitude;
                if (charDistance <= radiusAware)
                {
                    if (character.isOnGround)
                    {
                        // jump above character
                        if (charPos.x > aiPos.x)
                        {
                            ballFollow = true;  // just for skip below
                            tempMove = 1;
                            character.DoJump(new Vector2(0.75f, 1.25f));
                            skipDecisionCounter = 0;
                        }
                        else
                        {
                            DoShoot();
                        }
                    }
                }

                if (!ballFollow)
                {
                    if (ballPos.x > aiPos.x)
                    {
                        tempMove = 1;
                    }
                    else
                    {
                        if (character.isOnGround)
                        {
                            if (tempMove == 1) tempMove = 0;

                            if (charDistance <= radiusMiddle)
                            {
                                tempMove = charPos.x > aiPos.x ? 1 : -1;
                            }

                            if (tempMove == 0)
                            {
                                tempMove = Random.Range(0f, 1f) > 0.7f ? -1 : 0;
                            }
                            else
                            {
                                tempMove = Random.Range(0f, 1f) > 0.5f ? -1 : 0;
                            }
                        }
                    }
                }
            }
            else
            {
                ++skipDecisionCounter;
                if (skipDecisionCounter >= skipDecision) skipDecisionCounter = -1;
            }
            yield return new WaitForSeconds(timeUpdateAi);
        }
    }

    private void DoShoot()
    {
        if (shootTime > 0.2f)
        {
            shootTime = 0f;
            kickhigh = Random.Range(0f, 1f) > 0.5f;
            kicklow = !kickhigh;
            skipDecisionCounter = skipDecision - 2;
        }
    }

    private void MakeJumpToTheBall()
    {
        //var dir = ball.transform.position - transform.position;
        //character.DoJump(dir.normalized*1.5f);
        jump = true;
        skipDecisionCounter = skipDecision - 2;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, radiusResponse);
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, radiusMiddle);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, radiusAware);
    //}
#endregion
}
