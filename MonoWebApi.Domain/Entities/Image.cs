namespace MonoWebApi.Domain.Entities
{
	public class Image : BaseEntity
	{
		public virtual byte [] Bytes { get; set; }
		public virtual Product ProductOwningAsGallery { get; set; }
		public virtual Product ProductOwningAsThumbnail { get; set;}
	}
}