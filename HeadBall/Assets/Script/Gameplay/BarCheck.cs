using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarCheck : MonoBehaviour
{
    [SerializeField] bool isLeft;
    [SerializeField] Vector2 direction = new Vector2(1f, 0.15f);
    [SerializeField] Vector3 rotation = new Vector3(0f, 0f, 5f);
    [SerializeField] float punchDur = 0.15f;
    [SerializeField] float punchElastic = 1f;
    [SerializeField] float ForceMod = 115000;
    [SerializeField] int vibrato = 10;
    [SerializeField] Transform goal;
    [SerializeField] AudioClip barSfx;

    private Vector2 rightDir;
    private void Awake()
    {
        rightDir = new Vector2(-direction.x, direction.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(Constant.TAG_BALL))
        {
            if (isLeft)
                collision.rigidbody.AddForce(direction * ForceMod);
            else
                collision.rigidbody.AddForce(rightDir * ForceMod);
            goal.DOPunchRotation(rotation, punchDur, vibrato, punchElastic);
            AudioManager.Instance.PlaySfx(barSfx);
        }
    }
}
