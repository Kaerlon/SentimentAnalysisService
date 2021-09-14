using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;

using SentimentAnalysis.API.Extensions;
using SentimentAnalysis.API.Models;
using SentimentAnalysis.API.Options;
using SentimentAnalysis.MlNet;
using SentimentAnalysis.MlNet.Model;
using System.Linq;
using SentimentAnalysis.API.Data;

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
