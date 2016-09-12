using DaiDaKaram.Domain.Entities;

namespace DaiDaKaram.Domain.Infrastructure
{
	public interface IImageManipulator
	{
		Thumbnail ResizeToThumbnail (Image thumbnail);
		void Watermark (ref Photo photo);
	}
}