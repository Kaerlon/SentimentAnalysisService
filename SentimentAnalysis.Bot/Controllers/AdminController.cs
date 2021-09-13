using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System.Net.Http;
using System.Threading.Tasks;

using Telegram.Bot.Advanced.Core.Dispatcher.Filters;
using Telegram.Bot.Advanced.DbContexts;

using Telegram.Bot.Types.Enums;

namespace SentimentAnalysis.Bot.Controllers
{
	public class AdminController : Controller
	{
		private readonly ILogger<AdminController> _logger;
		private readonly HttpClient _httpClient;

		public AdminController(IHttpClientFactory clientFactory, IMemoryCache cache, IConfiguration configuration, ILogger<AdminController> logger)
			: base(logger, cache, configuration)
		{
			_logger = logger;
			_httpClient = clientFactory.CreateClient("MLHttpClient");
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
	}
}
