// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SentimentAnalysis.API.Models
{
	public partial class TrainModel
	{
		[Key]
		[Required]
		public int Id { get; set; }
		public string Message { get; set; }
		public int Result { get; set; }
	}
}
