using System.Reflection;

namespace pp_bot.Runtime.Composition;

internal static class MethodInfoExtensions
{
	public static T CreateStaticDelegate<T>(this MethodInfo methodInfo)
	{
		return (T)(object)methodInfo.CreateDelegate(typeof(T));
	}
}