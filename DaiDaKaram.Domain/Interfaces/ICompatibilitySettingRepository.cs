using System;
using DaiDaKaram.Domain.Entities;
using DaiDaKaram.Domain.Infrastructure;

namespace DaiDaKaram.Domain
{
	public interface ICompatibilitySettingRepository : IRepository<CompatibilitySetting>
	{
		void CreateCompatibility (int productId, string make, string model, string [] variants);
	}
}

