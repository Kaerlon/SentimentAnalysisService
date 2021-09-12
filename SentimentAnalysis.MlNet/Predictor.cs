using Microsoft.ML;
using Microsoft.ML.Data;
using SentimentAnalysis.MlNet.Model;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.ML.DataOperationsCatalog;

namespace SentimentAnalysis.MlNet
{
    public class Predictor
    {
        public static TrainTestData LoadData(MLContext mlContext, IEnumerable<SentimentData> elements)
        {
            var data = mlContext.Data.LoadFromEnumerable(elements);

            var splitDataView = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            return splitDataView;
        }

        public static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView splitTrainSet)
        {
            var estimators = mlContext.Transforms.Text.FeaturizeText(Constants.Features, Constants.Message)
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: Constants.Label, inputColumnName: Constants.Label))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(inputColumnName: Constants.PredictedLabel, outputColumnName: Constants.PredictLabel));

            //var estimators = mlContext.Transforms.Text.FeaturizeText("MessageF", nameof(SentimentData.Message))
            //    .Append(mlContext.Transforms.Concatenate(Constants.FLabel, "MessageF"))
            //    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy());

            var model = estimators.Fit(splitTrainSet);
            return model;
        }

        public static MulticlassClassificationMetrics Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            var predict = model.Transform(splitTestSet);
            var metrics = mlContext.MulticlassClassification.Evaluate(predict, labelColumnName: Constants.Label/*, scoreColumnName: Constants.Score*/);


            return metrics;
            //return $"MicroAccuracy: {metrics.MicroAccuracy:P2} | MacroAccuracy: {metrics.MacroAccuracy:P2} | TopKAccuracy: {metrics.TopKAccuracy:P2}";
        }

        public static void SaveTrainModel(MLContext mlContext, ITransformer model, IDataView data, string path) =>
            mlContext.Model.Save(model, data.Schema, path);

        public static MLContext GetMLContext() => new MLContext();

    }
}
