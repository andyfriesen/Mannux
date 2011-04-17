using System;

namespace Import.Geo
{
	[Serializable]
	public class Point
	{
		int x,y;
		
		public Point() : this (0,0)
		{}
		
		public Point(int a,int b)
		{
			x=a;
			y=b;
		}
		
		public int X
		{
			get	{	return x;	}
			set	{	x=value;	}
		}
		
		public int Y
		{
			get	{	return y;	}
			set	{	y=value;	}
		}
	}
}
