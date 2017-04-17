using UnityEngine;
using System;
using System.Collections.Generic;

public class DiceRollerScript : MonoBehaviour
{
	public Camera DiceCamera;

	public int HowManyDice = 2;
	public DiceScript[] DiceArray = new DiceScript[9];
	public float RollTimeout = 10f;

	private float _timeout = 0f;
	private bool _rolling = false;
	private int _diceCompleted = 0;
	private Action<int> _onResult;
	private Dictionary<DiceScript, Vector3> _startingPositions;

	// Use this for initialization
	void Start ()
	{
		_startingPositions = new Dictionary<DiceScript, Vector3> ();

		foreach (var dice in DiceArray)
			_startingPositions[dice] = dice.transform.localPosition;

		ClearDice ();
	}

    public void HideDice()
    {
        DiceArray[0].gameObject.SetActive(false);
        DiceArray[1].gameObject.SetActive(false);
    }

    public void ShowDice()
    {
        DiceArray[0].gameObject.SetActive(true);
        DiceArray[1].gameObject.SetActive(true);
    }

    public void ClearDice ()
	{
		foreach (var dice in DiceArray)
		{
			dice.transform.localPosition = _startingPositions[dice];
			dice.gameObject.SetActive (false);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (_rolling)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				_timeout = RollTimeout;

				_rolling = false;

				RollDice (_onResult);
			}
		}
		else
		{
			//ClearDice();
		}
	}

	public void RollDice (Action<int> onResult)
	{
		ClearDice ();

		_timeout = RollTimeout;
		_diceCompleted = 0;

		_onResult = onResult;

		for (int i = 0; i < HowManyDice; ++i)
		{
			DiceArray[i].gameObject.SetActive (true);
			DiceArray[i].Throw (OnDiceResult);
		}

		_rolling = true;
	}

	public void OnDiceResult (int value)
	{
		++_diceCompleted;

		if (_diceCompleted >= HowManyDice)
		{
			_rolling = false;

			int totalValue = 0;
			for (int i = 0; i < HowManyDice; ++i)
				totalValue += DiceArray[i].Number;
			
			if (_onResult != null)
				_onResult (totalValue);
		}
	}
}
