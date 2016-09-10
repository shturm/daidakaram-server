using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Domain.Infrastructure
{
	public interface IImageManipulator
	{
		Thumbnail ResizeToThumbnail (Image thumbnail);
	}
}