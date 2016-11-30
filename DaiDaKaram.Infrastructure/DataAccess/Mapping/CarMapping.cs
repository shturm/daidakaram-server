using System;
using DaiDaKaram.Domain.Entities;
using FluentNHibernate.Mapping;

namespace DaiDaKaram.Infrastructure.DataAccess
{
	public class CarMapping : ClassMap<Car>
	{
		public CarMapping ()
		{
			//Table ("Car");

			Id (c => c.Id).GeneratedBy.Assigned ();
			Map (c => c.Make);
			Map (c => c.Model);
			Map (c => c.Variant);
			Map (c => c.Type);
			Map (c => c.Body);
			Map (c => c.YearFrom);
			Map (c => c.YearTo);
			Map (c => c.EngineCcm);
			Map (c => c.EngineHp);
			Map (c => c.EngineKw);
			Map (c => c.EngineFuel);
		}
	}
}

