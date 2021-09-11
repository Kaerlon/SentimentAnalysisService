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
            var estimators = mlContext.Transforms.Text.FeaturizeText(Constants.FLabel, nameof(SentimentData.Message))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: Constants.Label, inputColumnName: Constants.Label))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy());

            //var estimators = mlContext.Transforms.Text.FeaturizeText("MessageF", nameof(SentimentData.Message))
            //    .Append(mlContext.Transforms.Concatenate(Constants.FLabel, "MessageF"))
            //    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy());

            var model = estimators.Fit(splitTrainSet);
            return model;
        }

        public static string Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            IDataView predictions = model.Transform(splitTestSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions);

            return $"Accuracy: {metrics.Accuracy:P2} | Auc: {metrics.AreaUnderRocCurve:P2} | F1Score: {metrics.F1Score:P2}";
        }

        public static void SaveTrainModel(MLContext mlContext, ITransformer model, IDataView data, string path) =>
            mlContext.Model.Save(model, data.Schema, path);

        public static MLContext GetMLContext() => new MLContext();

    }
}
