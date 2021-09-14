﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;

using SentimentAnalysis.API.Extensions;
using SentimentAnalysis.API.Models;
using SentimentAnalysis.API.Options;
using SentimentAnalysis.MlNet;
using SentimentAnalysis.MlNet.Model;

using System.Collections.Generic;
using System.Linq;
using SentimentAnalysis.API.Data;

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

			var prediction = _predictionEnginePool.Predict(_mlConfiguration.ModelName, new SentimentData { Message = input.NormalizeString() });

			var scheme = _predictionEnginePool.GetPredictionEngine(_mlConfiguration.ModelName).OutputSchema;

			//var test = Predictor.GetScoresWithLabelsSorted(scheme, "Score", prediction.Score.GetValues().ToArray());

			var result = new
			{
				Prediction = prediction.PredictLabel,
				Scores = new Dictionary<string, float>()
				{
					{ "Positive", prediction.Score.GetValues()[2] },
					{ "Neutral", prediction.Score.GetValues()[0] },
					{ "Negative", prediction.Score.GetValues()[1] },
				}
			};

			return Ok(result);
		}

		[HttpPost("Evaluate")]
		public IActionResult Evaluate()
		{
			var mLContext = Predictor.GetMLContext();

			var elements = _context.TrainData
				   .Select(v => new SentimentData { Message = v.Message.NormalizeString(), Label = v.Result })
				   .ToList();

			var splitDataView = Predictor.LoadData(mLContext, elements);

			var model = _predictionEnginePool.GetModel(_mlConfiguration.ModelName);

			var result = Predictor.Evaluate(mLContext, model, splitDataView.TestSet);

			return new JsonResult(result, new System.Text.Json.JsonSerializerOptions()
			{
				WriteIndented = true
			});
		}
	}
}
