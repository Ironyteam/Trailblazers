using UnityEngine;
using UnityEngine.UI;

public class ExampleScript : MonoBehaviour
{
	public DiceRollerScript DiceRoller;
	public Text RollValue;

	public void OnRollButton ()
	{
		RollValue.text = "Rolling...";

		DiceRoller.RollDice ((value) => {

			RollValue.text = value.ToString ();
		});
	}
}
