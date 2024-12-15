using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : BaseController
{
    enum State
    {
        IDLE,
        MOVING
    }

    [SerializeField] BallController ball;
    [SerializeField] float timeUpdateAi = 0.25f;
    [SerializeField] bool doNothing;
    private void OnEnable()
    {
        StartCoroutine(IEUpdateAI());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

#if UNITY_EDITOR
    private void Update()
    {
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
    }
#endif

    IEnumerator IEUpdateAI()
    {
        while (true)
        {
            ChooseAction();
            yield return new WaitForSeconds(timeUpdateAi);
        }
    }

    private void ChooseAction()
    {
        if (doNothing) return;
        var rand = Random.Range(0f, 1f);
        if (rand > 0.5f)
            jump = true;
    }
}
