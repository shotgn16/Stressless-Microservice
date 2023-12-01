using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Stressless_Service.Models;

namespace Stressless_Service.Prediction;

public class ForcastFreeTime : IDisposable
{
    public async Task Forcast(CalenderModel[] events)
    {
        List<CalenderModel> CalendarEvents = events.ToList();

        MLContext context = new MLContext();
        var data = context.Data.LoadFromEnumerable(CalendarEvents);

        SsaForecastingEstimator Pipeline = context.Forecasting.ForecastBySsa(outputColumnName: "ForecastedLabel", inputColumnName: "Label", windowSize: 5, seriesLength: 20, trainSize: 100, horizon: 5);
        SsaForecastingTransformer Model = Pipeline.Fit(data);

        var ForcastingEngine = Model.CreateTimeSeriesEngine<CalenderModel, CalendarPrediction>(context);
        var forecast = ForcastingEngine.Predict();

        foreach (var item in forecast.ForecastedLabel)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(item);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public void Dispose() => GC.Collect();

}
