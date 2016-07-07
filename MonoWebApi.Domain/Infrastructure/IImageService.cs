using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Domain.Infrastructure
{
	public interface IImageService
	{
		
		Image ResizeToThumbnail (Image thumbnail);
	}
}