using System.Composition.Hosting.Core;

namespace pp_bot.Runtime.Composition;

internal sealed class ExportFactoryDependencyInjectionDescriptorProvider : ExportDescriptorProvider
{
	public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor descriptorAccessor)
	{
		throw new NotImplementedException();
	}
}