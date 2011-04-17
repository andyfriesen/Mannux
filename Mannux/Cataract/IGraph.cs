using System.Drawing;

namespace Cataract
{
	//! Generic interface representing a graphics viewport.
	public interface IGraph
	{
		/*!
		 * Draws an image onscreen.
		 * \param src The image to be blitted.
		 * \param x X coordinate at which the left edge of src will be blitted.
		 * \param y Y coordinate at which the top edge of src will be blitted.
		 * \param trans If true, alpha blending is performed according to src's alpha channel.  If false, the image is drawn without regard for translucency.
		 */
		void Blit(IImage src,int x,int y,bool trans);
		
		/*!
		 * Draws an image onscreen, stretched to the specified size.
		 * \param src The image to be blitted.
		 * \param x X coordinate of the left edge
		 * \param y Y coordinate of the top edge
		 * \param w width to stretch the image to
		 * \param h height to stretch the image to
		 * \param trans If true, alpha blending will be performed.
		 */
		void ScaleBlit(IImage src,int x,int y,int w,int h,bool trans);
		
		/*!
		 * Draws a particle onscreen.
		 * \param x X coordinate at which the particle will be centred.
		 * \param y Y coordinate at which the particle will be centred.
		 * \param size The radius, in pixels, of the particle.
		 * \param r Red channel of the particle's colour.
		 * \param g Green channel of the particle's colour.
		 * \param b Blue channel of the particle's colour.
		 */
		void DrawParticle(int x,int y,float size,byte r,byte g,byte b,byte a);
		
		/*!
		 * Draws a line on the screen.
		 * \param x1 X coord of an endpoint.
		 * \param y1 Y coord of an endpoint.
		 * \param x2 X coord of the other endpoint.
		 * \param y2 Y coord of the other endpoind.
		 * \param r Red component of the colour to draw in.
		 * \param g Green component of the colour to draw in.
		 * \param b Blue component of the colour to draw in.
		 * \param a Alpha component of the colour to draw in.
		 */
		void DrawLine(int x1,int x2,int y1,int y2,byte r,byte g,byte b,byte a);
		
		//! Swaps the backbuffer and the front buffer.
		void ShowPage();
		
		//! Fills the screen with blackness.
		void Clear();
		
		/*!
		 * Creates a new image, using the pixel data specified.
		 * \param w Width of the new image.
		 * \param h Height of the new image.
		 * \param ptr A pointer to a buffer of 32bpp, ARGB pixel data to use as the image's contents.
		 */
		IImage CreateImage(int w,int h,System.IntPtr ptr);
		
		/*!
		 * Creates a new image from the image file specified.
		 * \param filename File name of the image to load.
		 */
		IImage LoadImage(string filename);	// replace this with a stream interface? so images could be loaded from packfiles, or network streams (!!!)
		
		int XRes	{	get;	}				//!< Returns the width of the viewport.
		int YRes	{	get;	}				//!< Returns the height of the viewport.
	}
}
