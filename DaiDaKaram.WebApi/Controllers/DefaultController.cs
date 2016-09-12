using System;
using System.Web.Http;
using DaiDaKaram.Domain;

namespace DaiDaKaram.Infrastructure.WebApi.Controllers
{
	public class DefaultController : ApiController
	{
		public IHttpActionResult Get()
		{
			return Ok (new { message = "Hello WebApi" });
		}
	}
}