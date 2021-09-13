using Microsoft.ML;
using Microsoft.ML.Data;

using SentimentAnalysis.MlNet.Model;

using System;
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

			var estimators = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: Constants.Label, inputColumnName: Constants.Label)
				.Append(mlContext.Transforms.Text.NormalizeText(outputColumnName: Constants.Message, inputColumnName: Constants.Message, keepDiacritics: true))
				.Append(mlContext.Transforms.Text.TokenizeIntoWords(outputColumnName: Constants.Message, inputColumnName: Constants.Message, new char[] { ' ' }))
				.Append(mlContext.Transforms.Text.FeaturizeText(Constants.Features, Constants.Message))
				//.Append(mlContext.Transforms.Text.RemoveDefaultStopWords(outputColumnName: Constants.Features, inputColumnName: Constants.Features, Language.Russian))
				.Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
				.Append(mlContext.Transforms.Conversion.MapKeyToValue(inputColumnName: Constants.PredictedLabel, outputColumnName: Constants.PredictLabel));

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

		public static Dictionary<string, float> GetScoresWithLabelsSorted(DataViewSchema schema, string name, float[] scores)
		{
			var result = new Dictionary<string, float>();

			var column = schema.GetColumnOrNull(name);

			var slotNames = new VBuffer<ReadOnlyMemory<char>>();
			column.Value.GetSlotNames(ref slotNames);
			var names = new string[slotNames.Length];
			var num = 0;
			foreach (var denseValue in slotNames.DenseValues())
			{
				result.Add(denseValue.ToString(), scores[num++]);
			}

			return result.OrderByDescending(c => c.Value).ToDictionary(i => i.Key, i => i.Value);
		}

	}
}
