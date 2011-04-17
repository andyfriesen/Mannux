
using System;

namespace Cataract
{
	//! Represents an image
	public interface IImage : IDisposable
	{
		int Width	{	get;	}		//!< Returns the width of the image
		int Height	{	get;	}		//!< Returns the height of the image
	}
}
