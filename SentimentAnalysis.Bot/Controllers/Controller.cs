﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SentimentAnalysis.Bot.Data;
using SentimentAnalysis.Bot.Models;

using System;
using System.Linq;
using System.Threading.Tasks;

using Telegram.Bot.Advanced.Controller;
using Telegram.Bot.Advanced.Core.Dispatcher.Filters;
using Telegram.Bot.Advanced.Core.Tools;
using Telegram.Bot.Advanced.DbContexts;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SentimentAnalysis.Bot.Controllers
{
	public class Controller : TelegramController<ApplicationContext>
	{
		protected readonly ILogger<Controller> _logger;
		protected readonly IMemoryCache _cache;
		protected readonly IConfiguration _configuration;

		public Controller(ILogger<Controller> logger, IMemoryCache cache, IConfiguration configuration)
		{
			_logger = logger;
			_cache = cache;
			_configuration = configuration;
		}

		[CommandFilter("start"), ChatTypeFilter(ChatType.Private)]
		public async Task Start()
		{
			TelegramChat.State = ConversationState.Idle;
			await SaveChangesAsync();

			await ReplyTextMessageAsync("Привет", ParseMode.Html);
		}

		[CommandFilter("help"), ChatTypeFilter(ChatType.Private)]
		public async Task Help()
		{
			await ReplyTextMessageAsync("<b>Chaldea Bot</b>\n\n" +
										"Benvenuto nel bot dedicato a Fate/Grand Order.\n" +
										"Attualmente questo bot può permettervi di registrare un Master (in privato) " +
										"inviandomi le sue informazioni. Dopo che è stato registrato correttamente " +
										"potete collegarlo in una qualsasi chat in cui è presente questo bot in modo " +
										"da poter inviare velocemente il Master in qualsiasi momento.\n" +
										"\n" +
										"/help - Показать это сообщение\n" +
										"\n" +
										"Бот создан @lIllIIIllIl",
										ParseMode.Html);
		}

		[CommandFilter("reset")]
		public async Task ResetState()
		{
			if (Update.Message.Chat.Type == ChatType.Group || Update.Message.Chat.Type == ChatType.Supergroup)
			{
				if (!await IsSenderAdminAsync())
				{
					await ReplyTextMessageAsync("Только администраторы могут сбросить состояние бота в группе");
					return;
				}
			}

			TelegramChat.State = ConversationState.Idle;
			TelegramChat.Data.Clear();

			if (await SaveChangesAsync("Сброс невозможен, свяжитесь с администратором бота, чтобы получить прямую поддержку"))
			{
				await ReplyTextMessageAsync("Успешно завершен сброс");
			}
		}

		#region Helpers

		protected async Task<bool> SaveChangesAsync()
		{
			var error = false;
			try
			{
				await TelegramContext.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex.Message);
				error = true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}

			return !error;
		}

		protected async Task<bool> SaveChangesAsync(string text = "Ошибка сохранения данных, попробуйте отправить последнее сообщение")
		{
			var error = false;
			try
			{
				await TelegramContext.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				_logger.LogError(ex.Message);
				error = true;
				await ReplyTextMessageAsync(text);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				await ReplyTextMessageAsync(text);
				throw;
			}

			return !error;
		}

		protected async Task<Person> GetPersonFromCallbackData()
		{
			if (Update?.CallbackQuery?.Data == null || Update.CallbackQuery.Message?.From?.Id == null)
				return null;

			var idStr = InlineDataWrapper.ParseInlineData(Update.CallbackQuery.Data).Data["person"];
			if (idStr == null)
				return null;

			if (!long.TryParse(idStr, out long id))
				return null;

			var person = await TelegramContext.People.FirstOrDefaultAsync(m =>
				m.UserId == Update.CallbackQuery.Message.Chat.Id
			);

			return person;
		}

		protected async Task<bool> IsSenderAdminAsync()
		{
			return await IsUserAdminAsync(TelegramChat.Id, Update.Message.From.Id);
		}

		protected async Task<bool> IsUserAdminAsync(long chatId, long userId)
		{
			return (await BotData.Bot.GetChatAdministratorsAsync(chatId))
				.Any(ua => ua.User.Id == userId);
		}

		#endregion
	}

	public static class ConversationState
	{
		public const string Idle = null;
		public const string GoodBye = "ChaldeabotController_GoodBye";
		public const string SendingNewsletterState = "TBA_sendingNewsletter";
	}
}
