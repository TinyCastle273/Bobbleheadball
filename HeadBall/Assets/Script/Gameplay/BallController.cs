using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
	private float ballMaxSpeed = 20.0f;
	public AudioClip ballHitGround;

	public Vector2[] ballStartingPosition;
	[SerializeField] float checkRadius;
	[SerializeField] LayerMask mask;

	[SerializeField] GameObject hitEffect;
	[SerializeField] GameObject ballShadow;
	[SerializeField] float forceMod = 500f;
	[SerializeField] Rigidbody2D rb;

	private int countPlayer;

	void FixedUpdate()
	{
		manageBallShadow();
		//limit ball's maximum spped
		if (rb.velocity.magnitude > ballMaxSpeed)
        {
			rb.velocity = rb.velocity.normalized * ballMaxSpeed;
		}			
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, checkRadius, mask);
		for (int i = 0; i < colliders.Length; i++)
        {
			if (colliders[i].CompareTag(Constant.TAG_PLAYER))
            {
				++countPlayer;
				if (countPlayer >= 2)
                {
					rb.AddForce(Vector2.up * forceMod * 0.9f);
					break;
                }
            }
        }
		countPlayer = 0;

	}

	public void PrepareTheBall(Vector3 nextPoint)
    {
		//set ball starting position
		gameObject.SetActive(true);
		transform.position = ballStartingPosition[0];
		rb.velocity = Vector2.zero;
		if (nextPoint != Vector3.zero)
		{
			var direction = nextPoint - transform.position;
			rb.AddForce(direction.normalized * forceMod);
		}
	}

	public void StartNewRound(Vector3 nxtP)
    {
		StartCoroutine(IENewRound(nxtP));
    }

	private IEnumerator IENewRound(Vector3 nxtP)
    {
		rb.Sleep();
		rb.isKinematic = true;
		yield return new WaitForSeconds(2f);
		rb.WakeUp();
		rb.isKinematic = false;
		PrepareTheBall(nxtP);
	}

	void manageBallShadow()
	{
		if (!ballShadow)
			return;

		ballShadow.transform.position = new Vector3(transform.position.x, -1, 0);
		ballShadow.transform.localScale = new Vector3(1.5f, 0.75f, 0.001f);
	}

	public Rigidbody2D GetRigidbody2D()
    {
		return rb;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
		AudioManager.Instance.PlaySfx(ballHitGround, 0.3f);
        if (collision.collider.CompareTag(Constant.TAG_PLAYER))
        {
			collision.rigidbody.velocity = Vector3.zero;
		}
    }
}