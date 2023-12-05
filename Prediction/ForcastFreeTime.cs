using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using Stressless_Service.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Stressless_Service.Prediction;

public class ForcastFreeTime : IDisposable
{
    // The context for the forcast in the root of the class (Globally)
    MLContext Context;

    // The data to be used to make a forcast 
    IDataView Data;

    // The parameters used during the forcast, such as what the data column is and such...
    SsaForecastingEstimator Pipeline;

    // The engine used to make the prediction
    SsaForecastingTransformer Model;

    private async Task prepareData(List<CalenderModel> events)
    {
        Context = new MLContext();
        Data = Context.Data.LoadFromEnumerable(events);
    }

    private async Task prepareModel()
    {
        Pipeline = Context.Transforms.Conversion

        var trainTestSplit = Context.Data.TrainTestSplit(Data, testFraction: 0.2);
        Model = Pipeline.Fit(trainTestSplit.TrainSet);
    }

    public async Task<CalendarPrediction> Forcast(CalenderModel[] events)
    {
        List<CalendarPrediction> forcastList = new List<CalendarPrediction>();
        List<CalenderModel> eList = events.ToList();

        await prepareData(eList);
        await prepareModel();

        var ForcastingEngine = Model.CreateTimeSeriesEngine<CalenderModel, CalendarPrediction>(Context);
        var forcast = ForcastingEngine.Predict();
         
        return forcast;
    }


    public void Dispose() => GC.Collect();

}
