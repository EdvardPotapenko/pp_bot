using System.Reflection;
using System.Composition.Hosting.Core;
using Microsoft.Extensions.DependencyInjection;

namespace pp_bot.Runtime.Composition;

// TODO this implementation is based on class ExportFactoryWithMetadataExportDescriptorProvider and needs a lot of refactoring
internal sealed class ExportFactoryWithMetadataDependencyInjectionDescriptorProvider : ExportDescriptorProvider
{
    private static readonly MethodInfo GetLazyDefinitionsMethod =
        typeof(ExportFactoryWithMetadataDependencyInjectionDescriptorProvider).GetTypeInfo().GetDeclaredMethod("GetExportFactoryDescriptors")!;

    public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract contract, DependencyAccessor definitionAccessor)
    {
        if (!contract.ContractType.IsConstructedGenericType || contract.ContractType.GetGenericTypeDefinition() != typeof(ExportFactoryDependencyInjection<,>))
            return NoExportDescriptors;

        var ga = contract.ContractType.GenericTypeArguments;
        var gld = GetLazyDefinitionsMethod.MakeGenericMethod(ga[0], ga[1]);
        var gldm = gld.CreateStaticDelegate<Func<CompositionContract, DependencyAccessor, object>>();
        return (ExportDescriptorPromise[])gldm(contract, definitionAccessor);
    }

    private static ExportDescriptorPromise[] GetExportFactoryDescriptors<TProduct, TMetadata>(
        CompositionContract exportFactoryContract, DependencyAccessor definitionAccessor)
    {
        var productContract = exportFactoryContract.ChangeType(typeof(TProduct));
        var boundaries = Array.Empty<string>();

        if (exportFactoryContract.TryUnwrapMetadataConstraint("SharingBoundaryNames",
                out IEnumerable<string> specifiedBoundaries, out var unwrapped))
        {
            productContract = unwrapped.ChangeType(typeof(TProduct));
            boundaries = (specifiedBoundaries ?? Array.Empty<string>()).ToArray();
        }

        var metadataProvider = MetadataViewProvider.GetMetadataViewProvider<TMetadata>();

        return definitionAccessor.ResolveDependencies("product", productContract, false)
            .Select(d => new ExportDescriptorPromise(
                exportFactoryContract,
                typeof(ExportFactoryDependencyInjection<TProduct, TMetadata>).Name,
                false,
                () => new[] { d },
                _ =>
                {
                    var dsc = d.Target.GetDescriptor();
                    return ExportDescriptor.Create((_, _) =>
                        {
                            return new ExportFactoryDependencyInjection<TProduct, TMetadata>(serviceProvider =>
                                {
                                    IServiceScope scope = serviceProvider.CreateScope();
                                    var product = ActivatorUtilities.CreateInstance<TProduct>(scope.ServiceProvider);
                                    return Tuple.Create<TProduct, Action>(product, scope.Dispose);
                                },
                                metadataProvider(dsc.Metadata));
                        },
                        dsc.Metadata);
                }))
            .ToArray();
    }
}