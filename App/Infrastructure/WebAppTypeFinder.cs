using System;
using System.Collections.Generic;
using System.Reflection;

namespace App.Infrastructure
{
	public class WebAppTypeFinder : AppDomainTypeFinder
	{
		private bool _binFolderAssembliesLoaded;

		public bool EnsureBinFolderAssembliesLoaded
		{
			get;
			set;
		} = true;


		public WebAppTypeFinder(IAppFileProvider fileProvider)
			: base(fileProvider)
		{
		}

		public virtual string GetBinDirectory()
		{
			return AppContext.BaseDirectory;
		}

		public override IList<Assembly> GetAssemblies()
		{
			if (!EnsureBinFolderAssembliesLoaded || _binFolderAssembliesLoaded)
			{
				return base.GetAssemblies();
			}
			_binFolderAssembliesLoaded = true;
			string binPath = GetBinDirectory();
			LoadMatchingAssemblies(binPath);
			return base.GetAssemblies();
		}
	}
}
