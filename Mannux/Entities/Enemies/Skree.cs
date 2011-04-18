// created on 19/07/2002 at 11:50 PM
namespace Entities.Enemies
{

	class Skree : Enemy
	{	
		bool flying=false;
		
		public Skree(Engine e,int startx, int starty)
            : base(e, e.RipperSprite)
		{
			x=startx;
			y=starty;
			
			hp=10;
			
			direction=Dir.right;
			anim.Set(2,3,5,true);
			
			UpdateState=new StateHandler(DoTick);
		}
	
		// player states
		public void DoTick()
		{
	    	if(flying==false && engine.DetectInXCoords(this) is Player)
	    	{
	    		flying=true;
	    	}
	     	
	     	if(flying==true)
	     	{
	     		HandleGravity();
	     	}
	     	
	     	if(touchingground)
	     	{
	     		Boom Bo=new Boom(engine, x, y);
				engine.SpawnEntity(Bo);	
		
				engine.DestroyEntity(this);
	     	}
	     


		}
			
	}

}
