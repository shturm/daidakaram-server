﻿using System;
using System.Collections.Generic;

namespace DaiDaKaram.Domain
{
	public interface ICompatibilityService
	{
		IEnumerable<string> GetMakes ();
		IEnumerable<string> GetModels (string make);
		IEnumerable<string> GetVariants (string make, string model);
		IEnumerable<string> GetBodies (string make, string model);
		int GetYearFrom (string make, string model);
		int GetYearTo (string make, string model);

		IEnumerable<string> GetTypes (string make, string model);
}
}