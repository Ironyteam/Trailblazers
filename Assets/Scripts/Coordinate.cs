using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinate 
{
	private int _x; // The representation of the X coordinate on the game map.
	private int _y; // The representation of the X coordinate on the game map.
	
	// Constructors to instantiate Player classes
	#region Public constructors
	
	// Default constructor.
	public Coordinate(int X, int Y)
	{
		_x = X;
		_y = Y;
	}
	
	#endregion Public constructors
	
	// Public accessors. Includes data validation.
	#region Public accessors
	
	public int X
	{
		get
		{
			return _x;
		}
		set
		{
			if (value >= Constants.MinXCoordinate)
			{
				if (value <= Constants.MaxXCoordinate)
				{
					_x = value;
				}
				else
					throw new System.ArgumentOutOfRangeException("The X coordinate cannot be more than the maximum X coordinate." + value.ToString());
			}
			else
				throw new System.ArgumentOutOfRangeException("The X coordinate cannot be less than the minimum X coordinate.");
		}

	}
	
	public int Y
	{
		get
		{
			return _y;
		}
		set
		{
			if (value >= Constants.MinYCoordinate)
			{
				if (value <= Constants.MaxYCoordinate)
				{
					_y = value;
				}
				else
					throw new System.ArgumentOutOfRangeException("The Y coordinate cannot be more than the maximum Y coordinate." + value.ToString());
			}
			else
				throw new System.ArgumentOutOfRangeException("The Y coordinate cannot be less than the minimum Y coordinate.");
		}

	}
	
	#endregion Public accessors

}