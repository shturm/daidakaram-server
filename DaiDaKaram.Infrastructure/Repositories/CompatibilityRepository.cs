using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DaiDaKaram.Domain;
using DaiDaKaram.Domain.Entities;
using NHibernate;
using NHibernate.Linq;

namespace DaiDaKaram.Infrastructure
{
	public class CompatibilityRepository : Repository<CompatibilitySetting>, ICompatibilitySettingRepository
	{
		public CompatibilityRepository (ISession s) : base (s)
		{
		}

		public void CreateCompatibility (int productId, string make, string model, string [] variants)
		{

			using (var tx = _session.BeginTransaction ()) {
				int countAllVariants = _session.Query<Car> ()
						.Where (c => c.Make == make)
						.Where (c => c.Model == model)
						.Select (c => c.Variant)
						.Distinct ().ToList ()
						.Count ();
				if (countAllVariants == 0)
					throw new Exception (string.Format ("0 variants found for {0} {1}", make, model));

				var product = _session.Get<Product> (productId);
				var existingSettings = _session.Query<CompatibilitySetting> ()
												   .Where (c => c.Make == make)
												   .Where (c => c.Model == model)
					                               .Where (c=> c.Product == product)
												   .ToList ();
				if (existingSettings.Count() == 1 && string.IsNullOrEmpty (existingSettings[0].Variant))
				{
					return;
				}

				if (countAllVariants == variants.Length || variants.Length == 0) {
					foreach (var es in existingSettings) {
						product.CompatibilitySettings.Remove (es);
						es.Product = null;
						_session.Delete (es);
					}
					_session.Save (new CompatibilitySetting () {
						Make = make,
						Model = model,
						Product = product
					});
				} else {
					
					IEnumerable<CompatibilitySetting> settings =
						variants.Where (v => existingSettings.Where (es => es.Variant == v).Count () == 0)
								.Select (v => new CompatibilitySetting () {
									Make = make,
									Model = model,
									Variant = v,
									Product = product
								}).ToArray ();
					foreach (var s in settings) {
						_session.Save (s);
					}
				}
				tx.Commit ();
			}
		}
	}
}

