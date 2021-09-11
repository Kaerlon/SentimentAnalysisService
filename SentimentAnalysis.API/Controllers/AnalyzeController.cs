using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using SentimentAnalysis.MlNet.Model;

namespace SentimentAnalysis.API.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class AnalyzeController : ControllerBase
   {
      private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;

      public AnalyzeController(PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool)
      {
         _predictionEnginePool = predictionEnginePool;
      }

      [HttpPost]
      public ActionResult<string> Predict([FromBody] string input)
      {
         if (string.IsNullOrEmpty(input))
            return BadRequest();
         
         SentimentPrediction prediction = _predictionEnginePool.Predict(modelName: "SentimentAnalysisModel", example: new SentimentData {Message = input });

         var sentiment = $"Sentiment: {prediction.Message} | Prediction: {prediction.Prediction} | Probability: {prediction.Probability}";

         return Ok(sentiment);
      }
   }
}
