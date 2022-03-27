namespace pp_bot.Runtime.Composition;

public sealed class ExportFactoryDependencyInjection<T, TMetadata> : ExportFactoryDependencyInjection<T>
{
	public TMetadata Metadata { get; }
	
	public ExportFactoryDependencyInjection(Func<IServiceProvider, Tuple<T, Action>> exportCreator, TMetadata metadata)
		: base(exportCreator)
	{
		Metadata = metadata;
	}
}