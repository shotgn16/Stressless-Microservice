using Microsoft.ML.Data;

namespace Stressless_Service.Models;

internal class CalendarPrediction
{
    [VectorType(5)] // Assuming you want to predict a window of 5 future values
    public float[] ForecastedLabel { get; set; }
}