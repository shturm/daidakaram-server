namespace MonoWebApi.Infrastructure.WebApi.Controllers
{
	public class PromoteDemoteAdminUserCommand
	{
		public string Email { get; set;}
		public bool Flag { get; set; }
	}
}