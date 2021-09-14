using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;

using SentimentAnalysis.API.Data;
using SentimentAnalysis.API.Extensions;
using SentimentAnalysis.API.Models;
using SentimentAnalysis.API.Options;
using SentimentAnalysis.MlNet;
using SentimentAnalysis.MlNet.Model;

using System.Linq;

namespace SentimentAnalysis.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ModelTrainingController : ControllerBase
	{
		private readonly MLConfiguration _mlConfiguration;
		private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;
		private readonly TrainModelContext _context;

		public ModelTrainingController(IOptions<MLConfiguration> mlConfiguration,
									   PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool,
									   TrainModelContext context)
		{
			_mlConfiguration = mlConfiguration.Value;
			_predictionEnginePool = predictionEnginePool;
			_context = context;
		}

		[HttpPost]
		public IActionResult Training()
		{
			var mLContext = Predictor.GetMLContext();

			try
			{
				var predictedMessages = _context.StoredMessages
					.Where(x => x.Result > -1);

				var trainData = predictedMessages.Select(x => new TrainModel()
				{
					Message = x.Message,
					Result = x.Result,
				});

				_context.TrainData.AddRange(trainData);
				_context.StoredMessages.RemoveRange(predictedMessages);

				_context.SaveChanges();
			}
			catch (System.Exception ex)
			{

			}

			var elements = _context.TrainData
				.Select(v => new SentimentData { Message = v.Message.NormalizeString(), Label = v.Result })
				.ToList();

			var splitDataView = Predictor.LoadData(mLContext, elements);

			var model = Predictor.BuildAndTrainModel(mLContext, splitDataView.TrainSet);

			Predictor.SaveTrainModel(mLContext, model, splitDataView.TrainSet, _mlConfiguration.FilePath);

			var result = Predictor.Evaluate(mLContext, model, splitDataView.TestSet);

			return new JsonResult(result, new System.Text.Json.JsonSerializerOptions()
			{
				WriteIndented = true
			});
		}
	}
}
