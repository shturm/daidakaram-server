using System.Collections.Generic;
using MonoWebApi.Domain.Entities;

namespace MonoWebApi.Domain
{
	public interface IProductService
	{
		Product Create (string name, string description = null, List<Image> photos = null, Image thumb = null);
	}
}