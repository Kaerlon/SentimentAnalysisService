using CommandLine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SentimentAnalysis.Test
{
	class CommandOptions
	{
		[Option('p', "predicted", HelpText ="Введите текст для анализа",Required =false)]
		public string Predict { get; set; }
		[Option('e', "evaluate", HelpText ="Введите для получения данных",Required =false)]
		public bool Evaluate { get; set; }
		[Option('t', "train", HelpText ="Введите для обучения модели",Required =false)]
		public bool ModelTrain { get; set; }
	}
}
