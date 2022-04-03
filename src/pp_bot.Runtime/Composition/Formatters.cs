namespace pp_bot.Runtime.Composition;

internal static class Formatters
{
	public static string Format(Type? type)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type));
		}

		return type.IsConstructedGenericType ? FormatClosedGeneric(type) : type.Name;
	}

	private static string FormatClosedGeneric(Type closedGenericType)
	{
		var name = closedGenericType.Name[..closedGenericType.Name.IndexOf('`')];
		var args = closedGenericType.GenericTypeArguments.Select(Format);
		return $"{name}<{string.Join(", ", args)}>";
	}
}