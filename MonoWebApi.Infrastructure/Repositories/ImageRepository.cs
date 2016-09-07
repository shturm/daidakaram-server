using System;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Domain.Infrastructure;

namespace MonoWebApi.Infrastructure
{
	public class ImageRepository : IRepository<Image>, IDisposable
	{
		public ImageRepository ()
		{
		}

		public void Delete (System.Collections.Generic.IList<Image> entity)
		{
			throw new NotImplementedException ();
		}

		public void Delete (Image entity)
		{
			throw new NotImplementedException ();
		}

		public void Dispose ()
		{
			
		}

		public System.Collections.Generic.IEnumerable<Image> Get (long id)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<Image> Get (System.Linq.Expressions.Expression<Func<Image, bool>> expression)
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<Image> GetAll ()
		{
			throw new NotImplementedException ();
		}

		public System.Collections.Generic.IEnumerable<Image> GetAll (System.Linq.Expressions.Expression<Func<Image, bool>> expression)
		{
			throw new NotImplementedException ();
		}

		public void Insert (System.Collections.Generic.IList<Image> entity)
		{
			throw new NotImplementedException ();
		}

		public void Insert (Image entity)
		{
			throw new NotImplementedException ();
		}

		public void Update (System.Collections.Generic.IList<Image> entity)
		{
			throw new NotImplementedException ();
		}

		public void Update (Image entity)
		{
			throw new NotImplementedException ();
		}
	}
}

