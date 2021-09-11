using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.ML;
using SentimentAnalysis.API.Models;
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
      private readonly TrainModelContext _context;

      public ModelTrainingController(PredictionEnginePool<SentimentData, SentimentPrediction> predictionEnginePool, TrainModelContext context)
      {
         _predictionEnginePool = predictionEnginePool;
         _context = context;
      }


      [HttpPost]
      public ActionResult<string> Training()
      {
         var mLContext = Predictor.GetMLContext();

         var elements = _context.TrainData.Select(v => new SentimentData { Message = v.Message, Result = v.Result }).ToList();

         var splitDataView = Predictor.LoadData(mLContext, elements);

         var model = Predictor.BuildAndTrainModel(mLContext, splitDataView.TrainSet);

         Predictor.SaveTrainModel(mLContext, model, splitDataView.TrainSet,"");

         var result = Predictor.Evaluate(mLContext, model, splitDataView.TestSet);

         return Ok();
      }
   }
}
