using System;
using System.Web.Http;
using MonoWebApi.Domain;

namespace MonoWebApi.Infrastructure.WebApi.Controllers
{
	public class DefaultController : ApiController
	{
		public IHttpActionResult Get()
		{
			return Ok (new { message = "Hello WebApi" });
		}
	}
}