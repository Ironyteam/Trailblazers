using UnityEngine;
using System;
using System.Collections.Generic;

public class DiceScript : MonoBehaviour
{
	//public AudioSource DiceSound;

    public audioManager AudioManager;
	public Action<int> _onThrowComplete;

	private Rigidbody _rigidBody;
	private bool _active = false;
	private bool _checkValue = false;

	public void Throw (Action<int> onThrowComplete)
	{
		_onThrowComplete = onThrowComplete;
		_active = true;
	}

	void Awake ()
	{
		_rigidBody = GetComponent<Rigidbody>();
	}

	// Use this for initialization
	void Start ()
	{
		AudioManager = GameObject.Find("MUSIC").GetComponent<audioManager>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (_active)
		{
			transform.rotation = UnityEngine.Random.rotation;
		}
		else if (_checkValue)
		{
			if (_rigidBody.velocity.sqrMagnitude < 0.01f && _rigidBody.IsSleeping())
			{ 
				if (Number == 0)
				{
					transform.rotation = UnityEngine.Random.rotation;
					_active = true;
				}
				else
				{
					_checkValue = false;
					ShowNumber();
				}
			}
		}
	}

	void OnCollisionEnter (Collision collision)
	{
		if (collision.gameObject.CompareTag ("Finish"))
		{
			_active = false;
			_checkValue = true;

            AudioManager.playDiceSound();
			//DiceSound.Play ();
		}
	}

	public int Number
	{
		get
		{
			Vector3 global_show_direction = -Physics.gravity;

			var local_show_direction = transform.InverseTransformDirection(global_show_direction);
			local_show_direction.Normalize();

			int shownNumber = 0;
			if (local_show_direction == Vector3.up)
				shownNumber = 6;
			else if (local_show_direction == Vector3.down)
				shownNumber = 1;
			else if (local_show_direction == Vector3.forward)
				shownNumber = 2;
			else if (local_show_direction == Vector3.back)
				shownNumber = 5;
			else if (local_show_direction == Vector3.left)
				shownNumber = 4;
			else if (local_show_direction == Vector3.right)
				shownNumber = 3;

			return shownNumber;
		}
	}

	void ShowNumber()
	{
		if (_onThrowComplete != null)
			_onThrowComplete.Invoke(Number);
	}
}
