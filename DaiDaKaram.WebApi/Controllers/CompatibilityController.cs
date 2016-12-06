using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DaiDaKaram.Domain;

namespace DaiDaKaram.Infrastructure.WebApi.Controllers
{
	public class CompatibilityController : ApiController
	{
		readonly ICompatibilityService _compatibilityService;

		public CompatibilityController (ICompatibilityService cService)
		{
			_compatibilityService = cService;
		}

		public void CreateCompatibility(CreateCompatibilityCommand cmd)
		{
			_compatibilityService.CreateCompatibility (cmd.ProductId, cmd.Make, cmd.Model, cmd.Variants);
		}


		public IEnumerable<string> GetMakes ()
		{
			return _compatibilityService.GetMakes ();
		}

		public IEnumerable<string> GetModels (string make)
		{
			if (make == null || make.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			return _compatibilityService.GetModels (make);
		}

		public IEnumerable<string> GetVariants (string make, string model)
		{
			if (make == null || make.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			if (model == null || model.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			return _compatibilityService.GetVariants (make, model);
		}

		public IEnumerable<string> GetBodies (string make, string model)
		{
			if (make == null || make.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			if (model == null || model.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			return _compatibilityService.GetBodies (make, model);
		}

		public int GetYearFrom (string make, string model)
		{
			if (make == null || make.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			if (model == null || model.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			return _compatibilityService.GetYearFrom (make, model);
		}

		public int GetYearTo (string make, string model)
		{
			if (make == null || make.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			if (model == null || model.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			return _compatibilityService.GetYearTo (make, model);
		}

		public IEnumerable<string> GetTypes (string make, string model)
		{
			if (make == null || make.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			if (model == null || model.Length == 0)
				throw new HttpResponseException (System.Net.HttpStatusCode.BadRequest);

			return _compatibilityService.GetTypes (make, model);
		}

		public IEnumerable<CompatibilitySettingDto> GetProductCompatibilitySettings (int productId)
		{
			var resultDtos = _compatibilityService.GetSettings (productId)
			                     .Select (s => new CompatibilitySettingDto (s))
			                     .ToList();
			return resultDtos;
		}
	}
}

