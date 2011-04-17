// created on 28/06/2002 at 12:40 AM
using Import;


namespace Entities.Enemies
{

	class Ripper : Enemy
	{	
		const float speed=1.0f;
		public Ripper(Engine e,int startx, int starty) : base(e, (Sprites.ISprite)e.sprites.Load("ripper.txt"))//e.LoadSpriteImage("ripper.txt"))
		{
			x=startx;
			y=starty;
			
			hp=10;
			damage=8;
			
			vx=speed;
			direction=Dir.right;
			anim.Set(2,3,5,true);
			
			UpdateState=new StateHandler(DoTick);
		}
		
		// player states
		public void DoTick()
		{
			if (touchingleftwall && vx<0)
			{	
				vx=speed;
				direction=Dir.right;
				anim.Set(2,3,5,true);
				
			}
			if (touchingrightwall && vx>0)
			{	
				vx=-speed;
				direction=Dir.left;
				anim.Set(0,1,5,true);
			}
		}
		
	
		
	}

}
