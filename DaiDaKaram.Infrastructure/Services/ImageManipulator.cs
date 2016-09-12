using System;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Domain.Infrastructure;

namespace DaiDaKaram.Infrastructure.Services
{
	public class ImageManipulator : IImageManipulator
	{
		public Thumbnail ResizeToThumbnail (Image thumbnail)
		{
			// TODO implement image resizing
			return new Thumbnail() {Bytes = thumbnail.Bytes};
		}

		public void Watermark (ref Photo photo)
		{
			// TODO implement image watermark
		}
	}
}

