using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Telegram.Bot.Advanced.DbContexts;

namespace SentimentAnalysis.Bot.Models
{
	public class Person
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		public long UserId { get; set; }

		public string Name { get; set; }

		public PersonStatus Status { get; set; }

		public ICollection<RegisteredChat> RegisteredChats { get; set; }

		[ForeignKey("UserId")]
		public TelegramChat User { get; set; }

		public Person() { }

		public Person(TelegramChat user, PersonStatus status = PersonStatus.Enabled)
		{
			User = user;
			UserId = user.Id;
			Status = status;
		}
	}

	public enum PersonStatus
	{
		Enabled = 0,
		Disabled = 1
	}
}
