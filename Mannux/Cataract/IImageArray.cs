using System;

namespace Cataract
{   
	public interface IImageArray : IDisposable
	{
		int Width	{	get;	}
		int Height	{	get;	}
		int Count	{	get;	}
	}
}
