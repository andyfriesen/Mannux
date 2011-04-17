
using Cataract;

using System;
using System.Drawing;
using System.Drawing.Imaging;

class TileSet : IUpdatable, IDisposable
{
	IImage[] tiles;
	IGraph graph;
	int width,height;
	
	public TileSet(IGraph g,Import.TileSet tileset)
	{
		graph=g;

		Update(tileset);
	}
	
	~TileSet()
	{
		Dispose();
	}
	
	public int Width	{	get	{	return width;	}	}	
	public int Height	{	get	{	return height;	}	}	
	public int NumTiles	{	get	{	return tiles.Length;	}	}
	
	public IImage this[int idx]
	{
		get	{	return	tiles[idx];	}
	}
	
	// IUpdatable	
	public void Update(object o)
	{
		Import.TileSet t=(Import.TileSet)o;
		
		tiles=new IImage[t.NumTiles];
		width=t.Width;
		height=t.Height;
		
		int i=0;
		foreach (Bitmap b in t)
		{
			BitmapData bd=b.LockBits(
				new Rectangle(0,0,b.Width,b.Height),
				ImageLockMode.ReadOnly,
				PixelFormat.Format32bppArgb);
			
			tiles[i]=graph.CreateImage(b.Width,b.Height,bd.Scan0);
			
			b.UnlockBits(bd);
			i++;
		}		
	}
	
	// IDisposable
	// 0 - undisposed.  1 - disposing. 2 - disposed
	int disposestate=0;
	public void Dispose()
	{
		if (disposestate!=0)
			return;
		
		disposestate=1;
		
		foreach (IImage i in tiles)
			i.Dispose();
		
		tiles=null;
		
		disposestate=2;
	}
}
