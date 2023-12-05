using System.Collections;
using Microsoft.ML.Data;
using Stressless_Service.Prediction;

namespace Stressless_Service.Models;

public class CalendarPrediction
{
    [VectorType(5)] // Assuming you want to predict a window of 5 future values
    public string[] ForecastedLabel { get; set; }
}