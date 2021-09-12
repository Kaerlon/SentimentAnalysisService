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
using Telegram.Bot.Advanced.Models;
using Telegram.Bot.Exceptions;
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
			var response = await _httpClient.PostAsJsonAsync<string>("/api/Analyze/Predict", this.Update.Message.Text);
			response.EnsureSuccessStatusCode();

			var result = await response.Content.ReadFromJsonAsync<ResponseModel>();

			string str="По моему мнению, ваше сообщение - ";
			switch (result.Prediction)
			{
				case 0:
					str += "Отрицательное";
					break;
				case 1:
					str += "Положительное";
					break;
				case 2:
					str += "Нейтральное";
					break;
				default:
					break;
			}

			await ReplyTextMessageAsync(str);
		}

		[CommandFilter("ModelTraining"), ChatRoleFilter(ChatRole.Administrator), MessageTypeFilter(MessageType.Text)]
		public async Task ModelTraining()
		{
			var response = await _httpClient.PostAsync("/api/ModelTraining", null);
			response.EnsureSuccessStatusCode();

			var result = await response.Content.ReadAsStringAsync();

			await ReplyTextMessageAsync(result);
		}

		[CommandFilter("Evaluate"), ChatRoleFilter(ChatRole.Administrator), MessageTypeFilter(MessageType.Text)]
		public async Task Evaluate()
		{
			var response = await _httpClient.PostAsync("/api/Analyze/Evaluate", null);
			response.EnsureSuccessStatusCode();

			var result = await response.Content.ReadAsStringAsync();

			await ReplyTextMessageAsync(result);
		}

		#region Creazione Master

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

		[CommandFilter("master"), ChatTypeFilter(ChatType.Private)]
		public async Task ShowMasterPrivate()
		{
			if (MessageCommand.Parameters.Count < 1)
			{
				_logger.LogDebug("Ricevuto comando /master senza parametri");
				await BotData.Bot.SendTextMessageAsync(TelegramChat.Id,
					"Devi passarmi il nome del master che vuoi mostrare");
			}
			else
			{
				//var master = TelegramContext.People
				//    .Include(m => m.User)
				//    .SingleOrDefault(m => m.Name == MessageCommand.Parameters.JoinStrings(" ") &&
				//                          m.UserId == Update.Message.Chat.Id);
				//if (master == null)
				//{
				//    await BotData.Bot.SendTextMessageAsync(TelegramChat.Id,
				//        "Nessun Master trovato con il nome " + MessageCommand.Parameters.JoinStrings(" "));
				//}
				//else
				//{
				//    await SendMaster(master);

				//    var settingsText = "Impostazioni:\n\n" +
				//                       $"Rayshift: {(master.UseRayshift ? "abilitato" : "disabilitato")}";
				//    var settingsKeyboard = BuildSettingsKeyboard(master);
				//    await BotData.Bot.SendTextMessageAsync(TelegramChat.Id, settingsText,
				//        replyMarkup: settingsKeyboard);
				//}
			}
		}

		#region Inline Callback

		[CallbackCommandFilter(InlineKeyboardCommands.UpdateServantList)]
		public async Task InlineUpdateServantList()
		{
			await BotData.Bot.AnswerCallbackQueryAsync(Update.CallbackQuery.Id);

			var master = await GetPersonFromCallbackData();
			if (master == null)
			{
				await ReplyTextMessageAsync("Il master è errato o non è disponibile");
				return;
			}

			//await GotoEditServantList(master);
		}

		[CallbackCommandFilter(InlineKeyboardCommands.DisableRayshift)]
		public async Task InlineDisableRayshift()
		{
			await BotData.Bot.AnswerCallbackQueryAsync(Update.CallbackQuery.Id);

			var master = await GetPersonFromCallbackData();
			if (master == null)
			{
				await ReplyTextMessageAsync("Il master è errato o non è disponibile");
				return;
			}

			if (await SaveChangesAsync())
			{
				await ReplyTextMessageAsync(
					"Rayshift è stato disabilitato.\n" +
					$"Ora sei senza support list, se vuoi impostare un immagine come support list usa il comando /support_list {master.Name}\n");
			}
		}

		[CallbackCommandFilter(InlineKeyboardCommands.DeleteMaster)]
		public async Task InlineDeleteMaster()
		{
			await BotData.Bot.AnswerCallbackQueryAsync(Update.CallbackQuery.Id);

			var master = await GetPersonFromCallbackData();
			if (master == null)
			{
				await ReplyTextMessageAsync("Il master è errato o non è disponibile");
				return;
			}

			TelegramContext.People.Remove(master);
			if (await SaveChangesAsync())
			{
				await ReplyTextMessageAsync($"Il Master {master.Name} è stato cancellato correttamente");
			}
		}

		#endregion

		private async Task SendSupportListUpdateNotifications(Person person, string text)
		{
			var chats = await GetRegisteredChatWithSettings(person);

			foreach (var chat in chats.Where(chat => chat.Settings.SupportListNotifications))
			{
				try
				{
					await BotData.Bot.SendTextMessageAsync(chat.Chat.ChatId, text, ParseMode.Html);
				}
				catch (ApiRequestException e)
				{
					_logger.LogWarning(e.Message);
				}
			}
		}

		private async Task SendServantListUpdateNotifications(Person person, string text)
		{
			var chats = await GetRegisteredChatWithSettings(person);

			foreach (var chat in chats.Where(chat => chat.Settings.ServantListNotifications))
			{
				try
				{
					await BotData.Bot.SendTextMessageAsync(chat.Chat.ChatId, text, ParseMode.Html);
				}
				catch (ApiRequestException e)
				{
					_logger.LogWarning(e.Message);
				}
			}
		}

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

		private IReplyMarkup BuildSettingsKeyboard(Person person)
		{
			var data = new Dictionary<string, string>() {
				{"master", person.Name}
			};

			var updateSupportList = new InlineKeyboardButton
			{
				Text = "Cambia Support List",
				CallbackData = new InlineDataWrapper(InlineKeyboardCommands.UpdateSupportList, data).ToString()
			};

			var updateServantList = new InlineKeyboardButton
			{
				Text = "Cambia Servant List",
				CallbackData = new InlineDataWrapper(InlineKeyboardCommands.UpdateServantList, data).ToString()
			};

			var toggleRayshift = new InlineKeyboardButton
			{
				Text = "Abilita Rayshift",
				CallbackData =
				new InlineDataWrapper(InlineKeyboardCommands.EnableRayshift, data).ToString()
			};

			var deleteMaster = new InlineKeyboardButton()
			{
				Text = "Elimina Master",
				CallbackData = new InlineDataWrapper(InlineKeyboardCommands.DeleteMaster, data).ToString()
			};

			var keyboard = new InlineKeyboardMarkup(new[] {
				new [] {
					updateSupportList, updateServantList
				},
				new [] {
					toggleRayshift
				},
				new [] {
					deleteMaster
				}
			});

			return keyboard;
		}

		//private async Task GotoEditSupportList(Person person)
		//{
		//    TelegramChat["edit_support_list"] = person.Id.ToString();
		//    TelegramChat.State = ConversationState.UpdatingSupportList;
		//    if (await SaveChangesAsync())
		//    {
		//        await ReplyTextMessageAsync(
		//            "Inviami la nuova foto, /rayshift se vuoi impostare Rayshift.io come provider o /skip se vuoi rimuoverla");
		//    }
		//}

		//private async Task GotoEditServantList(Person person)
		//{
		//    TelegramChat["edit_servant_list"] = person.Id.ToString();
		//    TelegramChat.State = ConversationState.UpdatingServantList;
		//    if (await SaveChangesAsync())
		//    {
		//        await ReplyTextMessageAsync("Inviami la nuova foto o /skip che rimuoverla");
		//    }
		//}

		private static class InlineKeyboardCommands
		{
			public const string UpdateSupportList = "UpdateSupportList";
			public const string UpdateServantList = "UpdateServantList";
			public const string DeleteMaster = "DeleteMaster";
			public const string EnableRayshift = "EnableRayshift";
			public const string DisableRayshift = "DisableRayshift";
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
