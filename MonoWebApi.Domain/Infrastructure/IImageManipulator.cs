using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Domain.Infrastructure
{
	public interface IImageManipulator
	{
		Image ResizeToThumbnail (Image thumbnail);
	}
}