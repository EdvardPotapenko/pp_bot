using System.Composition;

namespace pp_bot.Runtime.Composition;

public class ExportFactoryDependencyInjection<T>
{
	private readonly Func<IServiceProvider, Tuple<T, Action>> _exportCreator;
	
	public ExportFactoryDependencyInjection(Func<IServiceProvider, Tuple<T, Action>> exportCreator)
	{
		_exportCreator = exportCreator;
	}

	public Export<T> CreateExport(IServiceProvider serviceProvider)
	{
		Tuple<T, Action> untypedLifetimeContext = _exportCreator.Invoke(serviceProvider);
		return new Export<T>(untypedLifetimeContext.Item1, untypedLifetimeContext.Item2);
	}
}