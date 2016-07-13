namespace MonoWebApi.Domain.Entities
{
	public class Image : BaseEntity
	{
		public virtual byte [] Bytes { get; set; }
		public virtual Product PhotoOfProduct { get; set; }
	}
}