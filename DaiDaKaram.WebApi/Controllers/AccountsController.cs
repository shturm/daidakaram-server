using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace DaiDaKaram.Infrastructure.WebApi.Controllers
{
	using System.Linq;
	using System.Web.Http.Cors;
	using UserManager = DaiDaKaram.Infrastructure.UserManager;

	[EnableCors ("*", "*", "*")]
	public class AccountsController : ApiController
	{
		//UserManager _userManager;

		//public AccountsController (UserManager userManager)
		//{
		//	_userManager = userManager;
		//}

		[HttpGet]
		public IHttpActionResult WhoAmI ()
		{

			return Ok (new {
				UserName = User.Identity.Name,
				UserId = User.Identity.GetUserId (),
				IsAuthenticated = User.Identity.IsAuthenticated,
				AuthenticationType = User.Identity.AuthenticationType,
				IsAdmin = User.IsInRole ("Admin")
			});
		}

		[HttpPost]
		[Authorize]
		public IHttpActionResult ChangeEmail (ChangeEmailCommand command)
		{
			var userManager = new UserManager ();
			var user = userManager.FindByName (User.Identity.Name);

			user.UserName = command.Email;
			userManager.Update (user);

			return Ok ();
		}

		[HttpPost]
		//[Route("api/accounts/register")]
		public async Task<IHttpActionResult> Register ([FromBody]RegisterCommand registerCommand)
		{
			if (!ModelState.IsValid) {
				return BadRequest (ModelState);
			}

			var userManager = new UserManager ();
			var user = new IdentityUser (registerCommand.Email);
			var identityResult = await userManager.CreateAsync (user, registerCommand.Password);


			if (!identityResult.Succeeded) {
				return GetErrorResult (identityResult);
			}

			//var roleManager = new RoleManager<IdentityRole, string> (new RoleStore());
			//roleManager.Create (new IdentityRole ("Admin"));
			//userManager.AddToRole (user.Id, "Admin");

			return Ok (new { message = "Success" });
		}

		[HttpGet]
		[Authorize (Roles = "Admin")]
		public IHttpActionResult GetAll ()
		{
			var userManager = new UserManager ();

			var allUsers = userManager.Users.ToList ();

			var userDtos = allUsers.Select (u => new {
				Id = u.Id,
				Email = u.UserName,
				IsAdmin = userManager.IsInRole (u.Id, "Admin")
			});

			return Ok (userDtos);
		}

		[HttpPost]
		[Authorize (Roles = "Admin")]
		public IHttpActionResult SetAdmin (PromoteDemoteAdminUserCommand command)
		{
			var userManager = new UserManager ();
			var user = userManager.FindByName (command.Email);

			if (command.Flag) {
				userManager.AddToRole (user.Id, "Admin");
			} else {
				userManager.RemoveFromRole (user.Id, "Admin");
			}

			return Ok ();
		}

		private IHttpActionResult GetErrorResult (IdentityResult result)
		{
			if (result == null) {
				return InternalServerError ();
			}

			if (!result.Succeeded) {
				if (result.Errors != null) {
					foreach (string error in result.Errors) {
						ModelState.AddModelError ("", error);
					}
				}

				if (ModelState.IsValid) {
					// No ModelState errors are available to send, so just return an empty BadRequest.
					return BadRequest ();
				}

				return BadRequest (ModelState);
			}

			return null;
		}
	}
}