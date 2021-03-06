using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float shipSpeed;
	public Boundary boundary;
	public float tilt;
	private Rigidbody rb;

	public GameObject shot;
	public Transform[] shotSpawns;
	public Transform smoke, powerup;
	public float fireRate;
	public float nextFire;

	public GameObject[] groundExplo;

	private Transform crosshair;

	private int gunType;
	private bool alive, dead;

	private CameraShake camShake;

	private AudioSource[] sounds;
	private GameController controller;

	void Start () 
	{
		rb = GetComponent<Rigidbody> ();
		sounds = GetComponents<AudioSource> ();
		controller = GameObject.Find ("GameController").GetComponent<GameController> ();
		crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponentInChildren<Transform> ();
		gunType = 1;
		alive = true;
		dead = false;
		camShake = Camera.main.GetComponent<CameraShake> ();
	}

	void Update()
	{
		if(Input.GetKey(KeyCode.Space) && Time.time > nextFire && gunType == 1)
		{
			nextFire = Time.time + fireRate;
			sounds [1].Play ();
			Instantiate (shot, shotSpawns[0].position, shotSpawns[0].transform.rotation);
		}
		else if(Input.GetKey(KeyCode.Space) && Time.time > nextFire && gunType == 2)
		{
			nextFire = Time.time + fireRate;
			sounds [1].Play ();
			Instantiate (shot, shotSpawns[1].position, shotSpawns[1].transform.rotation);
			Instantiate (shot, shotSpawns[2].position, shotSpawns[2].transform.rotation);
		}
		else if(Input.GetKey(KeyCode.Space) && Time.time > nextFire && gunType == 3)
		{
			nextFire = Time.time + fireRate;
			sounds [1].Play ();
			Instantiate (shot, shotSpawns[0].position, shotSpawns[0].transform.rotation);
			Instantiate (shot, shotSpawns[3].position, shotSpawns[3].transform.rotation);
			Instantiate (shot, shotSpawns[4].position, shotSpawns[4].transform.rotation);
		}
		else if(Input.GetKey(KeyCode.Space) && Time.time > nextFire && gunType == 4)
		{
			nextFire = Time.time + fireRate;
			sounds [1].Play ();
			Instantiate (shot, shotSpawns[1].position, shotSpawns[1].transform.rotation);
			Instantiate (shot, shotSpawns[2].position, shotSpawns[2].transform.rotation);
			Instantiate (shot, shotSpawns[3].position, shotSpawns[3].transform.rotation);
			Instantiate (shot, shotSpawns[4].position, shotSpawns[4].transform.rotation);
		}
		else if(Input.GetKey(KeyCode.Space) && Time.time > nextFire && gunType == 5)
		{
			nextFire = Time.time + fireRate;
			sounds [1].Play ();
			Instantiate (shot, shotSpawns[0].position, shotSpawns[0].transform.rotation);
			Instantiate (shot, shotSpawns[1].position, shotSpawns[1].transform.rotation);
			Instantiate (shot, shotSpawns[2].position, shotSpawns[2].transform.rotation);
			Instantiate (shot, shotSpawns[3].position, shotSpawns[3].transform.rotation);
			Instantiate (shot, shotSpawns[4].position, shotSpawns[4].transform.rotation);
		}
		if (Input.GetKeyDown (KeyCode.P))
			takeDamage ();
		
	}


	void FixedUpdate()
	{
		if (alive) 
		{
			rb.position = new Vector3 (
				Mathf.Clamp (Mathf.Lerp (transform.position.x, crosshair.position.x, 0.1f), boundary.xMin, boundary.xMax), 
				Mathf.Clamp (Mathf.Lerp (transform.position.y, crosshair.position.y, 0.1f), boundary.yMin, boundary.yMax),
				0.0f
			);

			transform.LookAt (crosshair.position);
		}
		if (gunType <= 0 && !dead) 
		{
			rb.AddForce (Vector3.down);
			rb.position = new Vector3 (
				Mathf.Clamp (GetComponent<Rigidbody> ().position.x, boundary.xMin, boundary.xMax), 
				transform.position.y,
				0.0f
			);
			transform.Rotate (0, 0, 250 * Time.deltaTime);
			Camera.main.transform.Translate (new Vector3(0.0f, 6.0f, -6.0f)* Time.deltaTime);
			Camera.main.transform.LookAt (transform.position);
			sounds [0].Stop ();
		}
	}

	void upgradeGun()
	{
		if (gunType < 5) 
		{
			gunType++;
			controller.changeHealth (gunType);
			sounds [2].Play ();
			GameObject power = Instantiate (groundExplo [4], powerup.position, powerup.rotation) as GameObject;
			power.transform.SetParent (powerup);
		}
	}

	void takeDamage()
	{
		if (gunType > 0) 
		{
			gunType--;
			controller.changeHealth (gunType);
			sounds [3].Play ();
			GameObject sparks = Instantiate (groundExplo [3], smoke.position, groundExplo [3].transform.rotation) as GameObject;
			sparks.transform.SetParent (smoke);
			StartCoroutine (camShake.Shake (1f, 0.25f));
		}
		if (gunType <= 0)
			die ();
	}

	void die()
	{
		GameObject smokey = Instantiate (groundExplo [2], smoke.position, groundExplo [2].transform.rotation) as GameObject;
		controller.setGameAlive ();
		if(crosshair != null)
			Destroy (crosshair.gameObject);
		smokey.transform.SetParent (smoke);
		StartCoroutine (camShake.Shake (0.25f, 1f));
		alive = false;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Ground")) 
		{
			if (alive && gunType != 1) {
				Instantiate (groundExplo [0], transform.position, groundExplo [0].transform.rotation);
				rb.AddForce (Vector3.up * 5f);
				crosshair.transform.position = new Vector3 (0.0f, 0.0f, 3f);
				takeDamage ();
			} else if (alive) {
				dead = true;
				controller.setGameAlive ();
				sounds [5].Play ();

				GameObject explo = Instantiate (groundExplo[1], transform.position, groundExplo[1].transform.rotation) as GameObject;
				rb.AddExplosionForce (350f, transform.position, 5f, 5f, ForceMode.Impulse);
				StartCoroutine (camShake.Shake (1f, 1f));
				Destroy (gameObject, 10f);
				StartCoroutine (controller.gameOver ());
			}
			else 
			{
				dead = true;
				sounds [5].Play ();
				//GameObject.Find ("GameController").GetComponent<GameController> ().setGameAlive ();
				GameObject explo = Instantiate (groundExplo[1], transform.position, groundExplo[1].transform.rotation) as GameObject;
				rb.AddExplosionForce (350f, transform.position, 5f, 5f, ForceMode.Impulse);
				Destroy (gameObject, 10f);
				StartCoroutine (controller.gameOver ());
			}
		}
		if (other.CompareTag ("PowerUp")) 
		{
			Destroy (other.gameObject);
			upgradeGun ();
		}
		if (other.CompareTag ("Enemy") || other.CompareTag ("EnemyShot") && alive) 
		{
			takeDamage ();
		}
	}
}
