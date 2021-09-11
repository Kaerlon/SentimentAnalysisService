using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using SentimentAnalysis.MlNet;
using SentimentAnalysis.MlNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentimentAnalysis.API.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class ModelTrainingController : ControllerBase
   {
      private readonly PredictionEnginePool<SentimentData, SentimentPrediction> _predictionEnginePool;

      public ModelTrainingController(PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool)
      {
         _predictionEnginePool = predictionEnginePool;
      }


      [HttpPost]
      public ActionResult<string> TrainModel()
      {
         var model = Predictor.GetMLContext();
         return Ok();
      }
   }
}
