using System.Composition;
using System.Composition.Hosting.Core;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace pp_bot.Runtime.Composition;

internal sealed class ExportFactoryDependencyInjectionDescriptorProvider : ExportDescriptorProvider
{
	private static readonly MethodInfo GetExportFactoryDefinitionsMethod =
        typeof(ExportFactoryDependencyInjectionDescriptorProvider).GetTypeInfo()
            .GetDeclaredMethod("GetExportFactoryDescriptors")!;

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(CompositionContract exportKey, DependencyAccessor definitionAccessor)
        {
            if (!exportKey.ContractType.IsConstructedGenericType || exportKey.ContractType.GetGenericTypeDefinition() != typeof(ExportFactory<>))
                return NoExportDescriptors;

            var gld = GetExportFactoryDefinitionsMethod.MakeGenericMethod(exportKey.ContractType.GenericTypeArguments[0]);
            var gldm = gld.CreateStaticDelegate<Func<CompositionContract, DependencyAccessor, object>>();
            return (ExportDescriptorPromise[])gldm(exportKey, definitionAccessor);
        }

        private static ExportDescriptorPromise[] GetExportFactoryDescriptors<TProduct>(CompositionContract exportFactoryContract, DependencyAccessor definitionAccessor)
        {
            var productContract = exportFactoryContract.ChangeType(typeof(TProduct));
            var boundaries = Array.Empty<string>();

            if (exportFactoryContract.TryUnwrapMetadataConstraint("SharingBoundaryNames", out IEnumerable<string> specifiedBoundaries, out var unwrapped))
            {
                productContract = unwrapped.ChangeType(typeof(TProduct));
                boundaries = (specifiedBoundaries ?? Array.Empty<string>()).ToArray();
            }

            return definitionAccessor.ResolveDependencies("product", productContract, false)
                .Select(d => new ExportDescriptorPromise(
                    exportFactoryContract,
                    Formatters.Format(typeof(ExportFactoryDependencyInjection<TProduct>)),
                    false,
                    () => new[] { d },
                    _ =>
                    {
                        var dsc = d.Target.GetDescriptor();
                        var da = dsc.Activator;
                        return ExportDescriptor.Create((c, o) =>
                            {
                                return new ExportFactoryDependencyInjection<TProduct>(serviceProvider =>
                                {
                                    IServiceScope scope = serviceProvider.CreateScope();
                                    TProduct product =
                                        ActivatorUtilities.CreateInstance<TProduct>(scope.ServiceProvider);
                                    return Tuple.Create<TProduct, Action>(product, scope.Dispose);
                                });
                            },
                            dsc.Metadata);
                    }))
                .ToArray();
        }
}