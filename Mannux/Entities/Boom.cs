// created on 02/07/2002 at 12:04 PM
using System;
using Entities.Enemies;

namespace Entities
{
	class Boom : Entity
	{	
		public Boom(Engine e, float startx,float starty) 
            : base(e, e.BoomSprite)
		{
			UpdateState=new StateHandler(CheckCollision);
			
			x=startx;
			y=starty;
	                     		
			anim.Set(0,6,4,false);
		}
		
		public void CheckCollision()
		{
			if(anim.frame==6)
			{
				engine.DestroyEntity(this);
			}
		}
			
		
	}

}







