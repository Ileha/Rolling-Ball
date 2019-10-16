using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level;
using GameData;
using Pool;
using LevelGenerator.Borders;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(LineRenderer))]
public class BallControll : LeveAccess
{
	public float speed = 1f;
	public bool onGround { get; private set; } 

	public float RealSpeed { 
		get {
			return level.Difficulty(speed);	
		} 
	}
	private CircleCollider2D circleCollider;
	private Rigidbody2D rigidBody;
	private Vector3 acceleration = new Vector3(0, -1);
	private bool underControll = false;
	private Vector3 startPosition;
	private LineRenderer trail;
	private Coroutine trailPainter;

	void Awake()
	{
		rigidBody = gameObject.GetComponent<Rigidbody2D>();
		rigidBody.gravityScale = 0;
		circleCollider = gameObject.GetComponent<CircleCollider2D>();

		trail = gameObject.GetComponent<LineRenderer>();
		trail.positionCount = 15;
		trail.useWorldSpace = false;

		level.OnStartGame += OnStartGame;
		level.OnEndGame += OnEndGame;
	}

	// Use this for initialization
	void Start()
	{
		startPosition = transform.position;
	}

	// Update is called once per frame
	void Update()
	{
		if (underControll)
		{

#if UNITY_STANDALONE
			if (underControll && Input.GetKeyUp(KeyCode.Space)) {
				ChangeDirection();
			}
#endif
#if UNITY_ANDROID || UNITY_IOS
			if (
			Input.touchCount > 0  
				&& Input.GetTouch(0).phase == TouchPhase.Began 
				&& !level.IsUI(Input.GetTouch(0).position)
			) {
				ChangeDirection();
			}
#endif
		}

	}

	private void ChangeDirection() {
		if (!onGround) { return; }
		acceleration *= -1;
	}

	public IEnumerator DestroyCycle() {
		gameObject.SetActive(false);
		Particles endEffect = Singleton.Instanse.GetOnEndParticles(transform.position);
		yield return new WaitForSeconds(endEffect.LifeTime);
		ObjectPool.ReturnToPool(endEffect);
	}

	private void OnStartGame() {
		underControll = true;
		trailPainter = StartCoroutine(DrawTrailRoutine(trail));
	}
	private void OnEndGame(RunData result) {
		underControll = false;

		StopCoroutine(trailPainter);
		trail.enabled = false;

		rigidBody.velocity = Vector2.zero;
		transform.position = startPosition;

		acceleration = new Vector3(0, -1);
		gameObject.SetActive(true);
	}

	private IEnumerator DrawTrailRoutine(LineRenderer trail) {
		trail.enabled = true;
		Vector3[] trailsPoints = new Vector3[trail.positionCount];
		Vector3 lastPos = transform.position;

		while (true) {
			Vector2 realVelocity = transform.position - lastPos;
			lastPos = transform.position;
			realVelocity.Set(-level.generator.RealSpeed* Time.fixedDeltaTime, -realVelocity.y);

			for (int i = trailsPoints.Length - 2; i >= 0; i--) {
				trailsPoints[i + 1] = trailsPoints[i] + (Vector3) realVelocity;
			}
			trailsPoints[0] = new Vector2(0, 0);
			trail.SetPositions(trailsPoints);

			yield return new WaitForFixedUpdate();
		}
	}

	//void OnDrawGizmosSelected() {}

	private void OnJump() {
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position + Vector3.right * (circleCollider.radius * 2f),
												circleCollider.radius,
							 					Vector3.right,
							 					Singleton.Instanse.screen.x / 4f);

		for (int i = 0; i < hits.Length; i++) {
			if (hits[i].collider.gameObject.GetComponent<Barrier>() != null)
			{
				level.NearBarrierScore(Physics2D.Distance(circleCollider, hits[i].collider).distance, hits[i].collider.transform);
				break;
			}
		}
	}
	private void OnLand() {
		RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position + Vector3.left * (circleCollider.radius * 2f),
												circleCollider.radius,
												Vector3.left,
												Singleton.Instanse.screen.x / 4f);

		for (int i = 0; i < hits.Length; i++) {
			if (hits[i].collider.gameObject.GetComponent<Barrier>() != null) {
				level.NearBarrierScore(Physics2D.Distance(circleCollider, hits[i].collider).distance, hits[i].collider.transform);
				break;
			}
		}
	}

	private bool onRow = false;

	void FixedUpdate () {
		if (underControll) {
			rigidBody.velocity = acceleration* RealSpeed;//set velocity

			Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, circleCollider.radius + 0.1f);

			bool onRowTemp = false;
			for (int i = 0; i < colls.Length; i++) {
				if (colls[i].gameObject == gameObject) { continue; }

				Border borderTemp = colls[i].GetComponent<Border>();

				if (borderTemp != null) {
					onRowTemp = true;
					break;
				}
			}
			onGround = onRowTemp;

			if (onRow != onRowTemp) {
				if (onRowTemp) {
                	OnLand();
				}
				else {
                	OnJump();
				}
				onRow = onRowTemp;
			}
		}
	}
}
