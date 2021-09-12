using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Telegram.Bot.Advanced.DbContexts;

namespace SentimentAnalysis.Bot.Models
{
	public class ChatSettings
	{
		[Key, ForeignKey("TelegramChat")]
		public long Id { get; set; }

		public bool ServantListNotifications { get; set; } = true;

		public bool SupportListNotifications { get; set; } = true;

		public virtual TelegramChat TelegramChat { get; set; }
	}
}
