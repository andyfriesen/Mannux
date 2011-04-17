using System.Threading;
using System.Runtime.InteropServices;

/*
 * Operator int is overloaded here so you can just say if (mytimer>sometimevalue) .....
 */

class Timer
{
	int rate;
	
	public Timer(int r)
	{
		rate=r;
	}
	
	public int Time
	{
		get
		{
			float f=timeGetTime();
			return ((int)f*rate/1000);
		}
	}

	public static implicit operator int(Timer t)
	{
		return t.Time;
	}
	
	[DllImport("winmm.dll")]
	static extern int timeGetTime();
}
