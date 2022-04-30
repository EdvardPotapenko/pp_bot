using pp_bot.Commands;
using Telegram.Bot.Types;

namespace pp_bot.Runtime;

internal sealed class DefaultCommandsContext : ICommandsContext
{
	private readonly ICommandsLoader _commandsLoader;
	private readonly IServiceProvider _serviceProvider;

	public DefaultCommandsContext(ICommandsLoader commandsLoader, IServiceProvider serviceProvider)
	{
		_commandsLoader = commandsLoader;
		_serviceProvider = serviceProvider;
	}

	public IChatAction? GetCommand(Message m)
	{
		foreach (var commandFactory in _commandsLoader.CommandFactory)
		{
			var command = commandFactory.CreateExport(_serviceProvider).Value;
			if (command.Contains(m))
			{
				return command;
			}
		}

		return null;
	}
}