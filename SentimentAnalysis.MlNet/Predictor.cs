using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Text;
using SentimentAnalysis.MlNet.Model;
using System.Data.SqlClient;
using static Microsoft.ML.DataOperationsCatalog;
using System.Collections;

namespace SentimentAnalysis.MlNet
{
   public class Predictor
   {
      public static TrainTestData LoadData(MLContext mlContext, IEnumerable<SentimentData> elements)
      {
         IDataView data = mlContext.Data.LoadFromEnumerable(elements);

         TrainTestData splitDataView = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
         return splitDataView;
      }

      public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
      {
         var estimators = mlContext.Transforms.Text.NormalizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.Message), keepDiacritics: true)
                         .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Features", language: StopWordsRemovingEstimator.Language.Russian))
                         .Append(mlContext.Transforms.Conversion.MapValueToKey("Features"))
                         .Append(mlContext.Transforms.Text.ProduceNgrams("Features"))
                         .Append(mlContext.Transforms.NormalizeLpNorm("Features"))
                         .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features"));

         var model = estimators.Fit(splitTrainSet);
         return model;
      }

      public static void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
      {
         IDataView predictions = model.Transform(splitTestSet);
         CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");

      }

      private static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
      {
         PredictionEngine<SentimentData, SentimentPrediction> predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

         SentimentData sampleStatement = new SentimentData
         {
            Message = "This was a very bad steak"
         };

         var resultPrediction = predictionFunction.Predict(sampleStatement);

      }

      public static void UseModelWithBatchItems(MLContext mlContext, ITransformer model)
      {
         IEnumerable<SentimentData> sentiments = new[]
         {
                new SentimentData
                {
                    Message = "This was a horrible meal"
                },
                new SentimentData
                {
                    Message = "I love this spaghetti."
                }
            };

         IDataView batchComments = mlContext.Data.LoadFromEnumerable(sentiments);

         IDataView predictions = model.Transform(batchComments);
         IEnumerable<SentimentPrediction> predictedResults = mlContext.Data.CreateEnumerable<SentimentPrediction>(predictions, reuseRowObject: false);


         foreach (SentimentPrediction prediction in predictedResults)
         {
            Console.WriteLine($"Sentiment: {prediction.Message} | Prediction: {prediction.Prediction} | Probability: {prediction.Probability} ");
         }
      }

      public static void SaveTrainModel(MLContext mlContext, ITransformer model)
      {
         var gg = new DataViewSchema()
         mlContext.Model.Save(model, model)
      }
      public static MLContext GetMLContext()=>new MLContext();

   }
}
