// created on 01/07/2002 at 2:57 PM

namespace Entities
{
	
	class Door : Entity
	{	
		bool open=false;
		
		string map;
		int Newx, Newy;
		
		public Door(Engine e,int startx, int starty, Dir face, string mapname, int newx, int newy) : 
			base(e, e.DoorSprite)
		{
			engine=e;

			width=sprite.HotSpot.Width;
			height=sprite.HotSpot.Height;
			x=startx;
			y=starty;
			
			direction=face;
			open=false;
			
			width=16;
			
			map=mapname;
			Newx=newx;
			Newy=newy;
			
			//System.Console.WriteLine("Door {0}x{1}",width,height);	
			vx=0;
			vy=0;	
		}
	
		protected override void Update()
		{
//			base.Update();
			HandleGravity();
			CollisionThings();
			anim.Update();
		}
		
		// player states
		public void CollisionThings()
		{
			int px=engine.cameraTarget.X;
			int py=engine.cameraTarget.Y;
			int pw=engine.cameraTarget.Width;
			int ph=engine.cameraTarget.Height;			
			
			//door opening/closing stuff
			if(engine.DetectInYCoords(this) is Player) //make sure y coords match up
			{
				if(open==false) //door is closed, check if it should open
				{
					if(direction==Dir.left) 
					{
						if(x-px<64) //player is getting close
						{
							open = true;
							anim.Set(0,6, 8, false);
						
						}	
					}
					else
					{
						if(px-x<64) 
						{
							open = true;
							anim.Set(6,0, 8, false);				
						}
					}
				}
		
				if(open==true)
				{
					if(direction==Dir.left) 
					{
						if(x-px<8)
						{
							engine.MapSwitch(map, Newx, Newy);
						}	
						
						if(x-px>64) //too far, close!
						{
							open = false;
							anim.Set(6,0, 8, false);	
						}
					}
					else
					{
						if(px-x<24) 
						{
							engine.MapSwitch(map, Newx, Newy);
						}
						
						if(px-x>64) //too far, close!
						{
							open = false;
							anim.Set(0,6, 8, false);
						
						}				
					}
					
					
					
	
					
				}
	
			}
			//Entity temp=engine.DetectCollision(this);
			//if(temp is Player) //player is in the door
			//{
			//	engine.MapSwitch(map, Newx, Newy);
			//}
			
		}
	}
		
}
