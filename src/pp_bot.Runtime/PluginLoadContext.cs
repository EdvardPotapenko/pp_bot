using System.Reflection;
using System.Runtime.Loader;

namespace pp_bot.Runtime;

internal sealed class PluginLoadContext : AssemblyLoadContext
{
	private readonly AssemblyDependencyResolver _resolver;

	public PluginLoadContext(string pluginPath)
	{
		_resolver = new AssemblyDependencyResolver(pluginPath);
		Resolving += (context, name) =>
		{
			Assembly mainPluginAssembly = context.Assemblies.First();
			string location = Path.GetDirectoryName(mainPluginAssembly.Location)!;
			string path = Path.Combine(location, name.Name! + ".dll");
			Assembly loadedDependency = context.LoadFromAssemblyPath(path);
			return loadedDependency;
		};
	}

	protected override Assembly? Load(AssemblyName assemblyName)
	{
		string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
		if (assemblyPath != null)
		{
			return LoadFromAssemblyPath(assemblyPath);
		}

		return null;
	}

	protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
	{
		string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
		if (libraryPath != null)
		{
			return LoadUnmanagedDllFromPath(libraryPath);
		}

		return IntPtr.Zero;
	}
}