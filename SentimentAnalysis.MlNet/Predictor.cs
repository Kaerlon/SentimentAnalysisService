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
         var estimators = mlContext.Transforms.Text.NormalizeText(outputColumnName: "Features", inputColumnName: "Features", keepDiacritics: true)
                         .Append(mlContext.Transforms.Text.RemoveDefaultStopWords("Features", language: StopWordsRemovingEstimator.Language.Russian))
                         .Append(mlContext.Transforms.Text.ProduceNgrams("Features"))
                         .Append(mlContext.Transforms.NormalizeLpNorm("Features"))
                         .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features"));

         var model = estimators.Fit(splitTrainSet);
         return model;
      }

      public static string Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
      {
         IDataView predictions = model.Transform(splitTestSet);
         var metrics = mlContext.MulticlassClassification.Evaluate(predictions, "Label");

         return $"MicroAccuracy: {metrics.MicroAccuracy:P2} | MacroAccuracy: {metrics.MacroAccuracy:P2} | TopKAccuracy: {metrics.TopKAccuracy:P2}";
      }

      public static void SaveTrainModel(MLContext mlContext, ITransformer model, IDataView data, string path) => mlContext.Model.Save(model, data.Schema, path);
      public static MLContext GetMLContext()=>new MLContext();

   }
}
