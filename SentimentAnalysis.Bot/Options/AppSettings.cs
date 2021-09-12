using System;
using System.Collections.Generic;

using Telegram.Bot.Advanced.Models;

namespace SentimentAnalysis.Bot.Options
{
	public class AppSettings
	{
		public MLOptions MLOptions { get; set; }
		public TelegramSettings Telegram { get; set; }
	}

	public class MLOptions
	{
		public Uri ServiceUri { get; set; }
	}

	public class TelegramSettings
	{
		public Uri WebhookBaseUrl { get; set; }
		public IDictionary<string, TelegramBot> Bots { get; set; }
	}

	public class TelegramBot
	{
		public Version BotVersion { get; set; }
		public string BotToken { get; set; }
		public string BasePath { get; set; } = "/telegram";
		public IgnoreBehaviour GroupChatBehaviour { get; set; } = IgnoreBehaviour.IgnoreAllMessagesAndCommandsWithoutTarget;
		public IgnoreBehaviour PrivateChatBehaviour { get; set; } = IgnoreBehaviour.IgnoreNothing;
		public UserUpdate UserUpdate { get; set; } = UserUpdate.BotCommand | UserUpdate.PrivateMessage;
		public IList<long> Administrators { get; set; } = new List<long>();
		public IList<long> Moderators { get; set; } = new List<long>();
		public IList<long> Blocked { get; set; } = new List<long>();
		public IList<long> Banned { get; set; } = new List<long>();
	}
}
