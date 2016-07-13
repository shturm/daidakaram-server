using System;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Domain.Infrastructure;

namespace MonoWebApi.Infrastructure.Services
{
	public class ImageService : IImageService
	{
		public Image ResizeToThumbnail (Image thumbnail)
		{
			// TODO implement image resizing
			return new Image() {Bytes = thumbnail.Bytes};
		}
	}
}

