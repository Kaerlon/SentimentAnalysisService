using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentimentAnalysis.API.Extensions
{
	public static class StringExtension
	{

		public static string NormolaceString(this string str)
		{
			str = str.ToLower();
			str = str.Where(c => !char.IsPunctuation(c) && !char.IsNumber(c)).Aggregate("", (current, c) => current + c);
			return str;
		}
	}
}
