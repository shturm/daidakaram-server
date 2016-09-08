using System;
using System.Collections.Generic;
using MonoWebApi.Domain.Entities;
using MonoWebApi.Domain.Infrastructure;
using MonoWebApi.Infrastructure;

namespace MonoWebApi.Infrastructure
{
	public class ImageRepository : IRepository<Image>, IDisposable
	{
		private readonly MySQLDatabase Db;

		public ImageRepository (MySQLDatabase db)
		{
			Db = db;
		}

		public void Delete (IList<Image> entities)
		{
			foreach (var entity in entities) {
				string commandText = "DELETE FROM Image where Id = @id";
				Dictionary<string, object> parameters = new Dictionary<string, object> ();
				parameters.Add ("@id", entity.Id);

				Db.Execute (commandText, parameters);
			}
		}

		public void Delete (Image entity)
		{
			string commandText = "DELETE FROM Image where Id = @id";
			Dictionary<string, object> parameters = new Dictionary<string, object> ();
			parameters.Add ("@id", entity.Id);

			Db.Execute (commandText, parameters);
		}

		public void Dispose ()
		{
			Db.Dispose ();
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

		public void Insert (IList<Image> entities)
		{
			foreach (var entity in entities) {
				string commandText = "Insert into Image (Bytes, ProductId, IsThumbnail, Created, Updated) values " +
				"(@bytes, @productId, @isThumbnail, @created, @updated)";
				Dictionary<string, object> parameters = new Dictionary<string, object> ();

				int productId = 0;
				if (entity.Product != null && entity.Product.Id > 0)
				{
					productId = entity.Product.Id;
				}

				parameters.Add ("@bytes", entity.Bytes);
				parameters.Add ("@productId", productId);
				parameters.Add ("@isThumbnail", entity.IsThumbnail);
				parameters.Add ("@created", entity.Created);
				parameters.Add ("@updated", entity.Created);

				Db.Execute (commandText, parameters);
			}
		}

		public void Insert (Image entity)
		{
			string commandText = "Insert into Image (Bytes, ProductId, IsThumbnail, Created, Updated) values " +
				"(@bytes, @productId, @isThumbnail, @created, @updated)";
			Dictionary<string, object> parameters = new Dictionary<string, object> ();

			int productId = 0;
			if (entity.Product != null && entity.Product.Id > 0) {
				productId = entity.Product.Id;
			}

			parameters.Add ("@bytes", entity.Bytes);
			parameters.Add ("@productId", productId);
			parameters.Add ("@isThumbnail", entity.IsThumbnail);
			parameters.Add ("@created", entity.Created);
			parameters.Add ("@updated", entity.Created);

			Db.Execute (commandText, parameters);
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

