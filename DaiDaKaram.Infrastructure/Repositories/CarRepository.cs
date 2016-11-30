using System;
using System.Linq;
using System.Collections.Generic;
using DaiDaKaram.Domain;
using DaiDaKaram.Domain.Infrastructure;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Criterion;
using NHibernate.Transform;
using DaiDaKaram.Domain.Entities;

namespace DaiDaKaram.Infrastructure
{
	public class CarRepository : Repository<Car>, ICarRepository
	{
		public CarRepository (ISession s) : base (s)
		{
		}

		public IEnumerable<string> GetBodies (string make, string model)
		{
			List<string> result;
			using (var tx = _session.BeginTransaction ()) {
				result = _session.Query<Car> ()
								 .Where (c => c.Make == make)
								 .Where (c => c.Model == model)
								 .Select (c => c.Body)
								 .Distinct ().ToList ();
				result.Sort ();
				tx.Commit ();
			}
			return result;
		}

		public IEnumerable<string> GetMakes ()
		{
			List<string> makes;
			using (var tx = _session.BeginTransaction ()) {
				//var projections = Projections.Distinct (Projections.ProjectionList ()
				//                      .Add (Projections.Property ("Make")));
				//makes = _session.CreateCriteria <Car>().SetProjection (projections)
				//                .SetResultTransformer (new AliasToBeanResultTransformer(typeof(string)))
				//                .List<string> ();

				makes = _session.Query<Car> ().Select (c => c.Make).Distinct ().ToList ();
				makes.Sort ();
				//var carMakes = _session.QueryOver<Car> ().List ();
				tx.Commit ();
			}
			return makes;
		}

		public IEnumerable<string> GetModels (string make)
		{
			List<string> models;
			using (var tx = _session.BeginTransaction ()) {
				models = _session.Query<Car> ()
								.Where (c => c.Make == make)
								.Select (c => c.Model)
								.Distinct ().ToList ();
				models.Sort ();
				tx.Commit ();
			}
			return models;
		}

		public IEnumerable<string> GetTypes (string make, string model)
		{
			List<string> result;
			using (var tx = _session.BeginTransaction ()) {
				result = _session.Query<Car> ()
									.Where (c => c.Make == make)
									.Where (c => c.Model == model)
					                 .Select (c => c.Type)
									.Distinct ().ToList ();
				result.Sort ();
				tx.Commit ();
			}
			return result;
		}

		public IEnumerable<string> GetVariants (string make, string model)
		{
			List<string> variants;
			using (var tx = _session.BeginTransaction ()) {
				variants = _session.Query<Car> ()
									.Where (c => c.Make == make)
									.Where (c => c.Model == model)
								 .Select (c => c.Variant)
									.Distinct ().ToList ();
				variants.Sort ();
				tx.Commit ();
			}
			return variants;
		}

		public int GetYearFrom (string make, string model)
		{
			int result;
			using (var tx = _session.BeginTransaction ()) {
				result = _session.Query<Car> ()
								 .Where (c => c.Make == make)
								 .Where (c => c.Model == model)
				                 .Select (c => c.YearFrom)
				                 .Min ();
				
				tx.Commit ();
			}
			return result;
		}

		public int GetYearTo (string make, string model)
		{
			int result;
			using (var tx = _session.BeginTransaction ()) {
				result = _session.Query<Car> ()
								 .Where (c => c.Make == make)
								 .Where (c => c.Model == model)
								 .Select (c => c.YearTo)
				                 .Max ();

				tx.Commit ();
			}
			return result;
		}
	}
}

