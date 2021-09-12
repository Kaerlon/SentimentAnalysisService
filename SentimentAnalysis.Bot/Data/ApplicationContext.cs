using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SentimentAnalysis.Bot.Models;

using Telegram.Bot.Advanced.DbContexts;
using Telegram.Bot.Types;

namespace SentimentAnalysis.Bot.Data
{
	public partial class ApplicationContext : TelegramContext
	{
		private readonly IConfiguration _configuration;

		public ApplicationContext()
		{
		}

		public ApplicationContext(IConfiguration configuration, DbContextOptions options)
			: base(options)
		{
			_configuration = configuration;
		}

		public DbSet<Person> People { get; set; }
		public DbSet<RegisteredChat> RegisteredChats { get; set; }
		public DbSet<ChatSettings> ChatSettings { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlServer(_configuration.GetConnectionString("Default"), sqlOpt =>
				{

				});
			}

			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<RegisteredChat>()
				.HasOne<TelegramChat>()
				.WithMany()
				.HasForeignKey(x => x.ChatId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
