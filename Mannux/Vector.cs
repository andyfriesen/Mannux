
// Misc functions for handling velocity vectors and such.
// The name is a bit of a misnomer. ;P

class Vector
{
	// increases the vector's magnitude by quantity
	public static float Increase(float f,float quantity)
	{
		return f+quantity;
	}
	
	// Decreases the vector's magnitude
	public static float Decrease(float f,float quantity)
	{
		if (f>0)
		{
			if (f-quantity<=0)
				return 0;
			return f-quantity;
		}
		else
		{
			if (f+quantity>=0)
				return 0;
			return f+quantity;
		}
	}
	
	// makes sure the magnitude is less than that specified
	public static float Clamp(float f,float max)
	{
		if (f>0)
		{
			if (f>max)	return max;
		}
		else
		{
			if (f<-max)
				return -max;
		}
		return f;
	}
}
