using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;

using SentimentAnalysis.API.Models;
using SentimentAnalysis.API.Options;
using SentimentAnalysis.MlNet;
using SentimentAnalysis.MlNet.Model;

using System.Linq;

namespace SentimentAnalysis.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AnalyzeController : ControllerBase
	{
		private readonly MLConfiguration _mlConfiguration;
		private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;
		private readonly TrainModelContext _context;

		public AnalyzeController(IOptions<MLConfiguration> mlConfiguration,
								 PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool,
								 TrainModelContext context)
		{
			_mlConfiguration = mlConfiguration.Value;
			_predictionEnginePool = predictionEnginePool;
			_context = context;
		}

		[HttpPost("Predict")]
		public IActionResult Predict([FromBody] string input)
		{
			if (string.IsNullOrEmpty(input))
				return BadRequest();

			var prediction = _predictionEnginePool.Predict(_mlConfiguration.ModelName, new SentimentData { Message = input });

			return Ok(new { Prediction = prediction.PredictLabel });
		}

		[HttpPost("Evaluate")]
		public IActionResult Evaluate()
		{
			var mLContext = Predictor.GetMLContext();

			var elements = _context.TrainData1
				   .Select(v => new SentimentData { Message = v.Message, Label = v.Result })
				   .ToList();

			var splitDataView = Predictor.LoadData(mLContext, elements);

			var model = _predictionEnginePool.GetModel(_mlConfiguration.ModelName);

			var result = Predictor.Evaluate(mLContext, model, splitDataView.TestSet);

			return Ok(result);
		}
	}
}
