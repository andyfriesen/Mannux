using System.Drawing;

namespace Cataract
{
#if false
	public class OpenGLFont : IFont
	{
		CsGl.OpenGl.WGLBitmapFont font;
		
		Point pos;
		float rot;
		Color col;
		
		internal OpenGLFont(OpenGLContext context,string fontname,float size)
		{
			font=new WGLBitmapFont(context,new Font(fontname,size));
			font.InitRange((char)0,(char)255);
			
			col=Color.FromArgb(unchecked((int)0xFFFFFFFF));
			rot=0;
			pos=new Point(0,0);
		}
		
		public void PrintString(int x,int y,string s)
		{
			pos=new Point(x,y);
			PrintString(s);
		}
		
		public void PrintString(string s)
		{
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();
			Gl.glTranslatef(pos.X,pos.Y,0);
			Gl.glRotatef(rot,1,0,0);
			
			Gl.glColor4ub(col.R,col.G,col.B,col.A);
			
			font.DrawString(s);
			
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();			
		}
		
		public Point Pos
		{
			get	{	return pos;	}
			set	{	pos=value;	}
		}
		
		public float Rotation
		{
			get	{	return rot;	}
			set	{	rot=value;	}
		}
		
		public Color Colour
		{
			get	{	return col;	}
			set	{	col=value;	}
		}
	}
#endif
}
