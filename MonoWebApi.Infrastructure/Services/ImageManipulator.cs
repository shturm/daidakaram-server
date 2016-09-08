using System;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Domain.Infrastructure;

namespace MonoWebApi.Infrastructure.Services
{
	public class ImageManipulator : IImageManipulator
	{
		public Thumbnail ResizeToThumbnail (Image thumbnail)
		{
			// TODO implement image resizing
			return new Thumbnail() {Bytes = thumbnail.Bytes};
		}
	}
}

