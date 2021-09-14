using CommandLine;

using Newtonsoft.Json;

using SentimentAnalysis.Test.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalysis.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				var command = Console.ReadLine().Split(' ');
				var httpClient = new HttpClient()
				{
					BaseAddress = new Uri("http://127.0.0.1:5000/")
				};
				Parser.Default.ParseArguments<CommandOptions>(command)
					   .WithParsed(async o =>
					   {
						   if (!string.IsNullOrEmpty(o.Predict))
						   {
							   var request = new HttpRequestMessage(HttpMethod.Post, "/api/Analyze/Predict")
							   {
								   Content = new StringContent($"\"{o.Predict}\"", Encoding.UTF8, "application/json"),
							   };
							   var response = await httpClient.SendAsync(request);
							   response.EnsureSuccessStatusCode();

							   var result = JsonConvert.DeserializeObject<ResponseModel>(await response.Content.ReadAsStringAsync());

							   var str = "По моему мнению, ваше сообщение:";

							   foreach (var pred in result.Scores)
							   {
								   var rusName = "";

								   switch (pred.Key)
								   {
									   case LabelEnums.Negative:
										   rusName = "Негативное";
										   break;
									   case LabelEnums.Positive:
										   rusName = "Позитивное";
										   break;
									   case LabelEnums.Neutral:
										   rusName = "Нейтральное";
										   break;
								   }

								   str += $"\n{rusName} на {pred.Value:P2}";
							   }

							   Console.WriteLine(str);
							   return;
						   }

						   if (o.Evaluate)
						   {
							   var request = new HttpRequestMessage(HttpMethod.Post, "api/Analyze/Evaluate") { };
							   var response = await httpClient.SendAsync(request);
							   response.EnsureSuccessStatusCode();

							   var result = await response.Content.ReadAsStringAsync();

							   Console.WriteLine(result);
							   return;
						   }

						   if (o.ModelTrain)
						   {
							   var request = new HttpRequestMessage(HttpMethod.Post, "api/ModelTraining") { };
							   var response = await httpClient.SendAsync(request);
							   response.EnsureSuccessStatusCode();

							   var result = await response.Content.ReadAsStringAsync();

							   Console.WriteLine(result);
							   return;
						   }
					   });
			}

		}
	}
}
