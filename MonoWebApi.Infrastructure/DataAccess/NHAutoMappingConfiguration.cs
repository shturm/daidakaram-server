using System;
using FluentNHibernate.Automapping;

namespace MonoWebApi.Infrastructure.DataAccess
{

	// https://github.com/jagregory/fluent-nhibernate/blob/master/src/Examples.FirstAutomappedProject/ExampleAutomappingConfiguration.cs
	// https://github.com/jagregory/fluent-nhibernate/wiki/Auto-mapping
	class NHAutoMappingConfiguration : DefaultAutomappingConfiguration
	{
		
		public override bool ShouldMap (Type type)
		{
			// specify the criteria that types must meet in order to be mapped
			// any type for which this method returns false will not be mapped.
			return type.Namespace.EndsWith ("Entities", StringComparison.InvariantCulture)
				       && type.Name != "BaseEntity";
		}

	}
}

