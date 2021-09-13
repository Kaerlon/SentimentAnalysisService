using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SentimentAnalysis.Bot.Controllers;
using SentimentAnalysis.Bot.Data;
using SentimentAnalysis.Bot.Options;

using System.Collections.Generic;

using Telegram.Bot.Advanced.Core.Dispatcher;
using Telegram.Bot.Advanced.Core.Holder;
using Telegram.Bot.Advanced.DbContexts;
using Telegram.Bot.Advanced.Extensions;
using Telegram.Bot.Advanced.Models;

namespace SentimentAnalysis.Bot.Extensions
{
	public static class ServiceCollectionExtensions
	{
		private static ILogger<Startup> _logger;

		public static IServiceCollection AddTelegramBots(this IServiceCollection services, AppSettings appSettings)
		{
			var serviceProvider = services.BuildServiceProvider();
			_logger = serviceProvider.GetService<ILogger<Startup>>();

			_logger.LogInformation("Bots count = {0}", appSettings.Telegram.Bots.Count);

			var bots = new List<TelegramBotData>();
			foreach (var item in appSettings.Telegram.Bots)
			{
				var bot = item.Value;
				bots.Add(new TelegramBotData(options =>
				{
					options.CreateTelegramBotClient(bot.BotToken);
					options.DispatcherBuilder = new DispatcherBuilder<ApplicationContext, Controller>()
						.AddControllers(typeof(PrivateController), typeof(AdminController));

					options.BasePath = bot.BasePath;
					options.GroupChatBehaviour = bot.GroupChatBehaviour;
					options.PrivateChatBehaviour = bot.PrivateChatBehaviour;
					options.UserUpdate = bot.UserUpdate;// UserUpdate.BotCommand | UserUpdate.PrivateMessage;

					var defaultUserRole = new List<UserRole>();
					foreach (var userId in bot.Administrators)
						defaultUserRole.Add(new UserRole(userId, ChatRole.Administrator));

					foreach (var userId in bot.Moderators)
						defaultUserRole.Add(new UserRole(userId, ChatRole.Moderator));

					foreach (var userId in bot.Blocked)
						defaultUserRole.Add(new UserRole(userId, ChatRole.Blocked));

					foreach (var userId in bot.Banned)
						defaultUserRole.Add(new UserRole(userId, ChatRole.Banned));

					options.DefaultUserRole = defaultUserRole;
				}));
			}
			services.AddTelegramHolder(bots.ToArray());

			return services;
		}
	}
}
