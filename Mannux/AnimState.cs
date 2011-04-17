//! Sprite animation state.
class AnimState
{
	public int frame;
	int count;
	
	int first;
	int last;
	int delay;
	bool loop;
	public bool dead=false;
	
	public AnimState()
	{
		frame=0;
		count=0;

		first=last=delay=0;
		delay=0;
		dead=true;
	}
	
	public AnimState(int f,int l,int d,bool L)
	{
		first=f;
		last=l;
		delay=d;
		loop=L;
		
		count=0;
		frame=first;
	}
	
	public void Set(AnimState s)
	{
		Set(s.first,s.last,s.delay,s.loop);
	}
	
	public void Set(int f,int l,int d,bool L)
	{
		first=f;
		last=l;
		delay=d;
		loop=L;
		
		frame=first;
		count=delay;
		dead=false;
	}
	
	public void Update()
	{
		if (dead)
			return;

		count--;
		if (count>0)
			return;
		
		count=delay;
		
		if(first<last) //forward
		{	
			frame++;
			if (frame>last)
			{
	 			if (loop)
				{
					frame=first;
				}
				else
				{
					frame=last;
					dead=true;
					return;
				}
			}
		}
		
		if(first>last) //reverse
		{	
			frame--;
			if (frame<last)
			{
				if (loop)
				{
					frame=first;
				}
				else
				{
					frame=last;
					dead=true;
					return;
				}
			}
		}
		
		
	}
	
	// Common states
	public static AnimState[] playerstand =
	{
		new AnimState(0,0,0,false),	// left
		new AnimState(8,8,0,false)	// right
	};
	
	public static AnimState[] playerwalk =
	{
		new AnimState(16,23,20,true),
		new AnimState(24,31,20,true)
	};


	public static AnimState[] playerwalkshootingangleup =
	{
		new AnimState(48,55,20,true),
		new AnimState(56,63,20,true)
	};

	public static AnimState[] playerwalkshootingangledown =
	{
		new AnimState(64,71,20,true),
		new AnimState(72,79,20,true)
	};

	public static AnimState[] playerwalkshooting =
	{
		new AnimState(32,39,20,true),
		new AnimState(40,47,20,true)
	};

	public static AnimState[] playershooting =
	{
		new AnimState(4,4,0,false),
		new AnimState(12,12,0,false)
	};
		
	public static AnimState[] playershootup =
	{
		new AnimState(10,10,0,false),
		new AnimState(10,10,0,false)
	};
	
	public static AnimState[] playerjump =
	{
		new AnimState(82,85,12,true),
		new AnimState(90,93,12,true)
	};
	
	public static AnimState[] playerfall =
	{
		new AnimState(80,80,0,false),
		new AnimState(88,88,0,false)
	};

	public static AnimState[] playerfallshooting =
	{
		new AnimState(98,98,0,false),
		new AnimState(106,106,0,false)
	};

	public static AnimState[] playerfallshootingangleup =
	{
		new AnimState(97,97,0,false),
		new AnimState(105,105,0,false)
	};

	public static AnimState[] playerfallshootingangledown =
	{
		new AnimState(99,99,0,false),
		new AnimState(107,107,0,false)
	};

	public static AnimState[] playerfallshootingup =
	{
		new AnimState(96,96,0,false),
		new AnimState(104,104,0,false)
	};

	public static AnimState[] playerfallshootingdown =
	{
		new AnimState(100,100,0,false),
		new AnimState(108,108,0,false)
	};



	public static AnimState[] playercrouch =
	{
		new AnimState(116,116,0,false),
		new AnimState(112,112,0,false)
	};

	public static AnimState[] playercrouchshooting =
	{
		new AnimState(118,118,0,false),
		new AnimState(114,114,0,false)
	};

	public static AnimState[] playercrouchshootingangleup =
	{
		new AnimState(117,117,0,false),
		new AnimState(113,113,0,false)
	};

	public static AnimState[] playercrouchshootingangledown =
	{
		new AnimState(119,119,0,false),
		new AnimState(115,115,0,false)
	};

	public static AnimState[] hurt =
	{
		new AnimState(7,7,0,false),
		new AnimState(15,15,0,false)
	};
}
