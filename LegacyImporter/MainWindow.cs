using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using Gtk;

using Autofac;
using DaiDaKaram.Domain;
using DaiDaKaram.Infrastructure;

using NHibernate;
using NHibernate.Linq;
using FluentNHibernate;
using LegacyImporter;
using NHibernate.Criterion;

public partial class MainWindow : Gtk.Window
{
	ISession _session;
	IProductService _productService;

	//public MainWindow () : base (Gtk.WindowType.Toplevel)
	//{
	//	Build ();
	//}

	public MainWindow (ISession session) : base (WindowType.Toplevel)
	{
		this._session = session;

		ContainerBuilder builder = new ContainerBuilder ();
		AutofacDomainConfiguration.Configure (builder);
		AutofacInfrastructureConfiguration.Configure (builder);
		var container = builder.Build ();
		var scope = container.BeginLifetimeScope ();

		_productService = scope.Resolve<IProductService> ();
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnBtnBrowseClicked (object sender, EventArgs a)
	{
		var d = new FileChooserDialog ("Choose folder", this, FileChooserAction.SelectFolder,
									   "Accept", ResponseType.Accept,
									   "Cancel", ResponseType.Cancel);

		if (d.Run () == (int)ResponseType.Accept) {
			entry1.Text = d.Filename;
			Log ("Folder chosen: " + d.Filename);
		}

		d.Destroy ();
	}

	void Log (string text)
	{
		var endIter = tvLog.Buffer.EndIter;
		tvLog.Buffer.Insert (endIter, text + "\n");
	}

	protected void OnBtnImportClicked (object sender, EventArgs a)
	{
		Log ("===== Starting import ======");

		var files = Directory.EnumerateFiles (entry1.Text, "*", SearchOption.AllDirectories);
		int totalFilesLookedup, totalImported = 0;
		totalFilesLookedup = files.Count ();

		Task.Run (() => {
			foreach (var file in files) {
				Match match = Regex.Match (file, @"(\d+)/([a-z0-9]+)\.(jpg|png)$", RegexOptions.IgnoreCase);
				if (!match.Success) continue;
				string sku = match.Groups [1].Value;
				byte [] fileBytes = File.ReadAllBytes (file);

				_productService.ImportPhoto (sku, fileBytes);

				Log (string.Format ("#{0} {1}", sku, file));
				totalImported++;
			}
		}).ContinueWith ((t) => {
			Log (string.Format ("======= Done. {0} photos imported ======", totalImported));
		});

	}

	protected void OnBtnRefreshClicked (object sender, EventArgs a)
	{
		lblCount.Text = "Querying...";
		Task.Run (() => {
			var count = _session.QueryOver<LegacyProduct> ()
						   .Select (Projections.RowCount ())
						   .FutureValue<int> ().Value;
			lblCount.Text = count.ToString ();
		});

	}

	protected void OnBtnImportProductsClick(object sender, EventArgs a)
	{
		int imported = 0;

		Task.Run (() => {
			try {
				var legacyProducts = _session.QueryOver<LegacyProduct> ().List ();
				Log (string.Format ("==== Importing {0} legacy products =====", legacyProducts.Count ()));
				foreach (var product in legacyProducts) {
					_productService.ImportProduct (product.TypeName,
					                               product.GroupName,
					                               product.ItemName,
					                               product.SKU,
					                               product.OEM);
					Log (string.Format ("#{0} {1} {2} {3}",
									   product.SKU, product.TypeName, product.GroupName, product.ItemName));
					imported++;
				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
				throw ex;
			}


		}).ContinueWith ((t)=>{
			Log (string.Format ("==== Done. {0} imported ====", imported));
		});
	}
}