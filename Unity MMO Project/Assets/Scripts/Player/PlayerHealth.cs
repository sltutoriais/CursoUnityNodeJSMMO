using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Player health.
/// </summary>
public class PlayerHealth : MonoBehaviour {

	
	
	public Image damageImage;

	public float health = 100;

	public float maxHealth = 100;

	public float damageValue = 10;

	public float flashSpeed = 5f;

	Color defaultColour;

	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

	ParticleSystem hitParticles;

	bool damaged;

    public AudioClip damageSound;

	public AudioClip deathSound;

	private AudioSource damageSoundSource;
	
	CapsuleCollider capsuleCollider;
	
	public bool isDead;
    

	void Awake()
	{
		isDead = false;



		capsuleCollider = GetComponent <CapsuleCollider> ();

		hitParticles = GetComponentInChildren <ParticleSystem> ();

		if (GetComponent<PlayerManager> ().isLocalPlayer && GameObject.Find ("DamageImage"))
		{
			damageImage = GameObject.Find ("DamageImage").GetComponent<Image> () as Image;
		}

	}

	
	// Update is called once per frame
	void Update () {


		if (GetComponent<PlayerManager> ().isLocalPlayer) {
			if (damaged) {
			
			//	damageImage.color = flashColour;

			} else {
				
				//damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);

			}

			damaged = false;
		}
		

	}

	public void TakeDamage ()
	{
		if (GetComponent<PlayerManager> ().isLocalPlayer) {
			damaged = true;

			if (health - damageValue > 0) {
				health = health - damageValue;

				GetComponent<PlayerManager> ().UpdateAnimator ("IsDamage");

				hitParticles.transform.position = transform.position + capsuleCollider.center;
				hitParticles.Play ();


			} else {
				Death ();
			}
		}
		else
		{
			
			hitParticles.transform.position = transform.position + capsuleCollider.center;
			hitParticles.Play ();
			GetComponent<PlayerManager> ().UpdateAnimator ("IsDamage");

		
		}




	}
	
	 public void Death ()
    {
        
	   health = 0;

	   PlayDeathSound();

	   GetComponent<PlayerManager> ().UpdateAnimator ("IsDead");

	   GetComponent <Rigidbody> ().isKinematic = true;

	   capsuleCollider.isTrigger = true;

	   capsuleCollider.direction = 2;
  
	   isDead = true;

	   Destroy (gameObject, 7f);
		
    }

	
	//---------------AUDIO METHODS--------
	public void PlayDeathSound()
	{
	   if (!GetComponent<AudioSource> ().isPlaying)
	    {
			
		  GetComponent<AudioSource>().PlayOneShot(deathSound);

		}
		

	}
		
	public void PlayDamageSound()
	{

	   if (!GetComponent<AudioSource> ().isPlaying )
		{
		
		  GetComponent<AudioSource>().PlayOneShot(damageSound);

		}


	}
		
}
