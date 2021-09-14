using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SentimentAnalysis.Bot.Models;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Telegram.Bot.Advanced.Core.Dispatcher.Filters;
using Telegram.Bot.Advanced.Core.Tools;
using Telegram.Bot.Advanced.DbContexts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SentimentAnalysis.Bot.Controllers
{
	[ChatTypeFilter(ChatType.Private)]
	public class PrivateController : Controller
	{
		private readonly ILogger<PrivateController> _logger;
		private readonly HttpClient _httpClient;

		public PrivateController(IHttpClientFactory clientFactory, IMemoryCache cache, IConfiguration configuration, ILogger<PrivateController> logger)
			: base(logger, cache, configuration)
		{
			_logger = logger;
			_httpClient = clientFactory.CreateClient("MLHttpClient");
		}

		[NoCommandFilter, MessageTypeFilter(MessageType.Text)]
		public async Task Communication()
		{
			var message = Update.Type == UpdateType.EditedMessage ? Update.EditedMessage.Text : Update.Message.Text;

			var response = await _httpClient.PostAsJsonAsync<string>("/api/Analyze/Predict", message);
			response.EnsureSuccessStatusCode();

			var result = await response.Content.ReadFromJsonAsync<ResponseModel>();

			var str = "По моему мнению, ваше сообщение:";

			foreach (var pred in result.Scores)
			{
				var rusName = "";

				switch (pred.Key)
				{
					case LabelEnums.Negative:
						rusName = "😡 Негативное";
						break;
					case LabelEnums.Positive:
						rusName = "😄 Позитивное";
						break;
					case LabelEnums.Neutral:
						rusName = "😐 Нейтральное";
						break;
				}

				str += $"\n{rusName} на {pred.Value:P2}";
			}

			await ReplyTextMessageAsync(str);

			if (result.Prediction == 10)
			{
				string.Concat(new[] { "Спасибо за Ваш комментарий, пожалуйста оцените приложение 🙏" });

				var data = new Dictionary<string, string>()
				{
					{ "id", TelegramChat.ToChatId().Identifier.ToString()}
				};
				var rateApp = new InlineKeyboardButton()
				{
					Text = "Оценить",
					Url = "https://play.google.com/store/apps/details?id=ru.letobank.Prometheus&hl=ru&gl=RU",
					CallbackData = new InlineDataWrapper(InlineKeyboardCommands.RateApp, data).ToString()
				};
				var keyboard = new InlineKeyboardMarkup(new[] {
					new [] {
						rateApp
					}
				});

				await ReplyTextMessageAsync(str, mode: ParseMode.MarkdownV2, replyToMessageId: 0, replyMarkup: keyboard);
			}
		}

		#region

		//[CommandFilter("add"), MessageTypeFilter(MessageType.Text)]
		//public async Task Add()
		//{
		//    _logger.LogInformation("Ricevuto comando /add");
		//    if (MessageCommand.Parameters.Count < 1)
		//    {
		//        TelegramChat.State = ConversationState.Home;
		//        if (await SaveChangesAsync())
		//        {
		//            await BotData.Bot.SendTextMessageAsync(TelegramChat.Id, "Ok, inviami il nome che vuoi usare");
		//        }
		//    }
		//    else
		//    {
		//        await SetMasterName(MessageCommand.Message);
		//    }
		//}

		//[ChatStateFilter(ConversationState.Home), NoCommandFilter, MessageTypeFilter(MessageType.Text)]
		//public async Task GetNome()
		//{
		//    _logger.LogInformation($"Nome ricevuto da @{TelegramChat?.Username}: {MessageCommand.Text}");
		//    if (TelegramChat != null)
		//    {
		//        await SetMasterName(MessageCommand.Text);
		//    }
		//}

		//private async Task SetMasterName(string name)
		//{
		//    if (CheckName(name))
		//    {
		//        TelegramChat.State = ConversationState.FriendCode;
		//        TelegramChat["nome"] = name;
		//        if (await SaveChangesAsync())
		//        {
		//            await BotData.Bot.SendTextMessageAsync(TelegramChat.Id, "Ok, inviami il friend code in formato 123456789");
		//        }
		//    }
		//    else
		//    {
		//        await BotData.Bot.SendTextMessageAsync(TelegramChat.Id, "Nome invalido o già in uso, sceglierne un altro");
		//    }
		//}

		private bool CheckName(string messageText)
		{
			if (string.IsNullOrEmpty(messageText))
				return false;

			return !TelegramContext.People.Any(m => m.Name == messageText);
		}

		#endregion

		#region Inline Callback

		[CallbackCommandFilter(InlineKeyboardCommands.RateApp)]
		public async Task InlineRateApp()
		{
			await BotData.Bot.AnswerCallbackQueryAsync(Update.CallbackQuery.Id);

			var id = await GetPersonFromCallbackData();
			if (id == null)
			{
				await ReplyTextMessageAsync("");
				return;
			}

			if (await SaveChangesAsync())
			{
				await ReplyTextMessageAsync("");
			}
		}

		#endregion

		private async Task<List<RegisteredChatSettings>> GetRegisteredChatWithSettings(Person person)
		{
			var chats = await TelegramContext.RegisteredChats
				.Where(c => c.PersonId == person.Id)
				.Join(TelegramContext.ChatSettings,
					chat => chat.ChatId,
					settings => settings.Id,
					(chat, settings) => new RegisteredChatSettings(chat, settings))
				.ToListAsync();

			return chats;
		}

		private static class InlineKeyboardCommands
		{
			public const string RateApp = "RateApp";
		}

		private class RegisteredChatSettings
		{
			public RegisteredChat Chat { get; }
			public ChatSettings Settings { get; }

			public RegisteredChatSettings(RegisteredChat chat, ChatSettings settings)
			{
				Chat = chat;
				Settings = settings;
			}
		}
	}
}
