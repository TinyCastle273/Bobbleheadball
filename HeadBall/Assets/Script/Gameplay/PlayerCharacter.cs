using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCharacter : MonoBehaviour
{
    [Header("Editor Only")]
    public bool isLeft;

    [Header("Display Datsa")]
    [SerializeField] SpriteRenderer headRenderer;
    [SerializeField] SpriteRenderer handRenderer;
    [SerializeField] SpriteRenderer feetRenderer;
    [SerializeField] Transform rotatorHead;
    [SerializeField] Transform rotatorFoot;
    [SerializeField] Collider2D shootTrigger;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundCheckRadius = 0.2f;

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] hitBallClips;
    [SerializeField] AudioClip[] punchHitsClips;

    [Header("Variables")]
    [SerializeField] Color hitColor = Color.red;
    [SerializeField] float smoothMovement = .05f;
    [SerializeField] float animMovement = 0.2f;
    [SerializeField] float headMove = 15f;
    [SerializeField] float feetMove = 10f;
    [SerializeField] float rotateKick = 40f;
    [SerializeField] float headJump = 0.1f;
    [SerializeField] float speedMod = 50f;
    [SerializeField] float jumpMod = 225000f;
    [SerializeField] float shootMod = 200000f;
    [SerializeField] Vector2 forceHighKickLeft = new Vector2(0.5f, 0.5f);
    [SerializeField] Vector2 forceLowKickLeft = new Vector2(1f, 0.05f);
    [SerializeField] Vector2 forceHeaderLeft = new Vector2(0.5f, 0.2f);

    public bool isOnGround = true;
    public bool isInHitMode;

    private CharacterSO character;
    private BaseController controller;
    private Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;
    private Vector2 forceHighKickRight, forceLowKickRight, forceHeaderRight;
    private bool isAnimatingAction;
    private bool isKickHigh;
    private bool isKickLow;
    
    private void Awake()
    {
        controller = GetComponent<BaseController>();
        rb = GetComponent<Rigidbody2D>();
        transform.localScale = new Vector3(isLeft ? 1f : -1f, 1f);

        forceHighKickRight = new Vector2(-forceHighKickLeft.x, forceHighKickLeft.y);
        forceLowKickRight = new Vector2(-forceLowKickLeft.x, forceLowKickLeft.y);
        forceHeaderRight = new Vector2(-forceHeaderLeft.x, forceHeaderRight.y);
        MessageBus.Instance.Subscribe(MessageBusType.HeadHit, OnHeadHit);
    }

    private void Start()
    {
        if (isLeft)
        {
            character = GameManager.Instance.GetUserData();
        }
        else
        {
            character = GameManager.Instance.GetEnemyData();
        }

        headRenderer.sprite = character.head;
        handRenderer.sprite = character.hand;
        feetRenderer.sprite = character.feet;
        shootTrigger.enabled = false;
    }

    private void FixedUpdate()
    {
        isOnGround = false;
        if (isOnGround) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                isOnGround = true;
                rotatorHead.localPosition = new Vector3(0f, -0.6f);
                break;
            }   
        }
    }

    private void Rotate(int i)
    {
        if (isAnimatingAction) return;

        // 1: right, -1: left, 0: reset
        if (i == 0)
        {
            rotatorFoot.DOKill();
            rotatorFoot.DORotate(Vector3.zero, animMovement);
        }
        else
        {
            rotatorFoot.DOKill();
            if (i == 1)
            {
                rotatorFoot.DORotate(new Vector3(0f, 0f, -feetMove), animMovement);
            }
            else
            {
                rotatorFoot.DORotate(new Vector3(0f, 0f, feetMove), animMovement);
            }
        }
    }

    public void MoveCharacter(float move, bool jump)
    {
        if (move > 0.01f)
        {
            Rotate(1);
        }
        else if (move < -0.01f)
        {
            Rotate(-1);
        }
        else
        {
            Rotate(0);
        }

        Vector3 targetVelocity = new Vector2(move * speedMod, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothMovement);

        if (isOnGround && jump)
        {
            DoJump(Vector2.zero);
        }
    }

    public float GetSpeed()
    {
        return character.speed;
    }

    public void DoJump(Vector2 dir)
    {
        isOnGround = false;
        rotatorHead.DOKill();
        rotatorHead.DOLocalMoveY(headJump, animMovement).SetRelative(true);
        if (dir == Vector2.zero)
        {
            rb.AddForce(new Vector2(0f, jumpMod * character.jumpPower));
        }
        else
        {
            rb.AddForce(dir*(jumpMod * character.jumpPower));
        }
    }

    public void DoAction(bool kickLow, bool kickHigh)
    {
        if (kickLow || kickHigh)
        {
            if (isInHitMode)
            {
                HeadMove();
                MessageBus.Annouce(new Message(MessageBusType.HeadHit, isLeft));
            }
            else
            {
                rotatorFoot.DOKill();
                isKickHigh = kickHigh;
                isKickLow = kickLow;
                isAnimatingAction = true;
                shootTrigger.enabled = true;
                rotatorFoot.DORotate(new Vector3(0f, 0f, isLeft ? rotateKick : -rotateKick), animMovement).OnComplete(AfterKick);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var collider = collision.collider;
        if (collider.CompareTag(Constant.TAG_BALL))
        {
            // is jumping, 
            if (!isOnGround)
            {
                //Debug.Log($"rgx: {rb.position.x}, colx: {collision.rigidbody.position.x}, rgy: {rb.position.y}, coly: {collision.rigidbody.position.y}");
                if (rb.position.x < collision.rigidbody.position.x && rb.position.y < collision.rigidbody.position.y)
                {
                    if (isLeft)
                        collision.rigidbody.AddForce(forceHeaderLeft * character.shootPower * shootMod);
                    else
                        collision.rigidbody.AddForce(forceHeaderRight * character.shootPower * shootMod);
                    AudioManager.Instance.PlayRandomSfx(hitBallClips);
                    HeadMove();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DoActionWhileOverlapping(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        DoActionWhileOverlapping(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isInHitMode && collision.CompareTag(Constant.TAG_PLAYER))
        {
            isInHitMode = false;
        }
    }

    private void DoActionWhileOverlapping(Collider2D collision)
    {
        if (collision.CompareTag(Constant.TAG_BALL))
        {
            if (isKickHigh)
            {
                isKickHigh = false;
                if (isLeft)
                    collision.GetComponent<Rigidbody2D>().AddForce(forceHighKickLeft * character.shootPower * shootMod);
                else
                    collision.GetComponent<Rigidbody2D>().AddForce(forceHighKickRight * character.shootPower * shootMod);
                AudioManager.Instance.PlayRandomSfx(hitBallClips);
            }
            else if (isKickLow)
            {
                isKickLow = false;
                if (isLeft)
                    collision.GetComponent<Rigidbody2D>().AddForce(forceLowKickLeft * character.shootPower * shootMod);
                else
                    collision.GetComponent<Rigidbody2D>().AddForce(forceLowKickRight * character.shootPower * shootMod);
                AudioManager.Instance.PlayRandomSfx(hitBallClips);
            }
            isInHitMode = false;
        }
        else if (collision.CompareTag(Constant.TAG_PLAYER))
        {
            isInHitMode = true;
        }
    }

    private void AfterKick()
    {
        isAnimatingAction = false;
        shootTrigger.enabled = isInHitMode || false;
        isKickLow = isKickHigh = false;
    }

    private void HeadMove()
    {
        var seq = DOTween.Sequence();
        seq.Append(rotatorHead.DORotate(new Vector3(0f, 0f, isLeft ? -headMove : headMove), animMovement));
        seq.Append(rotatorHead.DORotate(Vector3.zero, animMovement));
    }

    public void SetEnable(bool enable)
    {
        if (!enable)
        {
            headRenderer.DOKill();
            headRenderer.DOColor(Color.white, 0.1f);
            rotatorHead.DOKill();
            rotatorHead.DORotate(Vector3.zero, animMovement);
            rotatorFoot.DOKill();
            rotatorFoot.DORotate(Vector3.zero, animMovement);
        }
        controller.SetEnable(enable);
    }

    private void OnHeadHit(Message msg)
    {
        bool left = (bool)msg.data;
        // right person to recv msg
        if (left != isLeft)
        {
            headRenderer.DOKill();
            headRenderer.DOColor(hitColor, 0.125f);
            headRenderer.DOColor(Color.white, 0.1f).SetDelay(0.125f);
            AudioManager.Instance.PlayRandomSfx(punchHitsClips);
        }
    }
}
