using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nameEntryScript : MonoBehaviour {

	public Text inGameName;
	public Text invalidText;
	public Text noName;
	public Text inappropriateText;

	void Start()
	{
		invalidText.enabled = false;
		inappropriateText.enabled = false;
		noName.enabled = false;
	}

	public void checkName()
	{
		// Checks for wrong ASCII or Empty string
		bool invalidAscii = false;
		if(String.IsNullOrEmpty(inGameName.text))
		{
			noName.enabled = true;
		}
		else
		{
			foreach(char letter in inGameName.text)
			{
				if(letter == 32)
					continue;
	
				char lowerLetter = char.ToLower(letter);
				if(lowerLetter  < 97 || lowerLetter  > 122)
					invalidAscii = true;
			}
	
			if(invalidAscii == true)
			{
				noName.enabled = false;
				inappropriateText.enabled = false;
				invalidText.enabled = true;
			}
		}

		// Reserved for expletive checker
	}

}
