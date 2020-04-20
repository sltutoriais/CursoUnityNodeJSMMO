using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

	public bool isLocalPlayer;

	Animator myAnim;
	
	public bool isAttack;

	public float attackTime;


	void Awake()
	{
		myAnim = GetComponent<Animator> ();
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

		if (isLocalPlayer) {

			Attack ();
			
			var h = Input.GetAxis ("Horizontal") * Time.deltaTime * 150.0f;
			var v = Input.GetAxis ("Vertical") * Time.deltaTime * 3.0f;

			transform.Rotate (0, h, 0);
			transform.Translate (0, 0, v);

			if (h != 0 || v != 0) {
				
				UpdateAnimator ("IsWalk");
				UpdateStatustoServer ();
			}

			if (h == 0 && v == 0) 
			{
				UpdateAnimator ("IsIdle");
				NetworkManager.instance.EmitAnimation ("IsIdle");
			}
				


		}
    }


	public void Attack()
	{
		 isAttack = Input.GetKey (KeyCode.A);

		if (isAttack)
		{
			UpdateAnimator ("IsAttack");

			NetworkManager.instance.EmitAnimation ("IsAttack");
		}
	}

	public void UpdateStatustoServer()
	{
		NetworkManager.instance.EmitPosAndRot (transform.position, transform.rotation);

		NetworkManager.instance.EmitAnimation ("IsWalk");


	}


	public void UpdatePosAndRot(Vector3 _pos, Quaternion _rot)
	{
		transform.position = _pos;

		transform.rotation = _rot;
	}


	IEnumerator StopAttack()
	{
		if (isAttack)
		{
			yield break; // if already attack... exit and wait attack is finished
		}

		isAttack = true; // we are now attack


		yield return new WaitForSeconds(attackTime); // wait for set attack animation time
		isAttack = false;


	}

	public void UpdateAnimator(string _animation)
	{
		switch (_animation) 
		{
		    case"IsIdle":
			if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Idle")) 
			{
				myAnim.SetTrigger ("IsIdle");
			}
			break;
		    case"IsWalk":
			if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Walk")) 
			{
				myAnim.SetTrigger ("IsWalk");
			}
			break;
		    case"IsDamage":
			if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Damage")) 
			{
				myAnim.SetTrigger ("IsDamage");
			}
			break;
		    case"IsAttack":
			if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Attack")) 
			{
				myAnim.SetTrigger ("IsAttack");

				if (!isLocalPlayer)
				{

					StartCoroutine ("StopAttack");
				}
			}
			break;
		    case"IsDeath":
			if (!myAnim.GetCurrentAnimatorStateInfo (0).IsName ("Death")) 
			{
				myAnim.SetTrigger ("IsDeath");
			}
			break;
		}
	}
	



}
