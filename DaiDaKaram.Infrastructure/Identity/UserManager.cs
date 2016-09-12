using System;
using Microsoft.AspNet.Identity;

namespace DaiDaKaram.Infrastructure
{
	public class UserManager : UserManager<IdentityUser, string>
	{
		public UserManager () : this(new UserStore())
		{

		}

		public UserManager (UserStore store)  : base(store)
		{

		}

	}
}