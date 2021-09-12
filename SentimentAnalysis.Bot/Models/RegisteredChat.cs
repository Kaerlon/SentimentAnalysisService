using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Telegram.Bot.Advanced.DbContexts;

namespace SentimentAnalysis.Bot.Models
{
	public class RegisteredChat
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public int PersonId { get; set; }

		public long ChatId { get; set; }

		[ForeignKey(nameof(PersonId))]
		public Person Person { get; set; }

		[ForeignKey(nameof(ChatId))]
		public TelegramChat Chat { get; set; }

		public RegisteredChat() { }

		public RegisteredChat(int personId, long chatId)
		{
			PersonId = personId;
			ChatId = chatId;
		}
	}
}
