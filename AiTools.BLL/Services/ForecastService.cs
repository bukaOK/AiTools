using AiTools.BLL.DataStructures;
using AiTools.BLL.Infrastructure;
using AiTools.BLL.Providers;
using AiTools.BLL.Services.Interfaces;
using AiTools.DAL.Entities;
using AiTools.DAL.Repositories;
using AiTools.Models.ForecastModels;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AiTools.BLL.Services
{
    public class ForecastService : DataService, IForecastService
    {
        private readonly FileProvider fileProvider;
        private readonly ForecastRepository forecastRepository;
        private readonly IMapper mapper;

        public ForecastService(ILogger<ForecastService> logger, FileProvider fileProvider, ForecastRepository forecastRepository,
            IMapper mapper) : base(logger)
        {
            this.fileProvider = fileProvider;
            this.forecastRepository = forecastRepository;
            this.mapper = mapper;
        }

        public async Task<IList<SelectListItem>> GetByUserAsync(string userId)
        {
            var entities = await forecastRepository.GetByUserAsync(userId);
            return mapper.Map<IList<SelectListItem>>(entities);
        }

        public async Task<ForecastEditModel> GetForEditAsync(Guid id)
        {
            var entity = await forecastRepository.GetAsync(id);
            return mapper.Map<ForecastEditModel>(entity);
        }

        public async Task<DataServiceResult> PredictAsync(ForecastEditModel model)
        {
            var results = new List<PredictResult>();
            if (model.Id == null)
                return DataServiceResult.Failed("Заполните Id");
            using (var stream = model.File.OpenReadStream())
            {
                using(var reader = new StreamReader(stream))
                {
                    using(var csvReader = new CsvReader(reader, new Configuration
                    {
                        Delimiter = ","
                    }))
                    {
                        await csvReader.ReadAsync();
                        csvReader.ReadHeader();
                        var entity = await forecastRepository.GetAsync(model.Id);
                        if (entity == null)
                            return DataServiceResult.Failed("Модель не найдена");
                        var mlContext = new MLContext(seed: 0);
                        var trainModel = mlContext.Model.Load(entity.ModelPath, out var modelInputSchema);

                        var predEngine = mlContext.Model.CreatePredictionEngine<PredictRow, PredictResult>(trainModel);

                        while (await csvReader.ReadAsync())
                        {
                            var canGetDate = csvReader.TryGetField("date", out string dateStr);
                            var canGetStore = csvReader.TryGetField("store", out string store);
                            var canGetItem = csvReader.TryGetField("item", out string item);
                            var canGetId = csvReader.TryGetField("id", out string id);

                            if (!canGetDate || !canGetItem)
                                return DataServiceResult.Failed("Не все поля найдены");

                            var date = DateTime.Parse(dateStr);

                            var row = new PredictRow
                            {
                                Item = item,
                                Store = canGetStore ? store : "0",
                                Month = date.Month,
                                Week = (float)date.DayOfWeek,
                                Year = date.Year,
                                Id = id
                            };

                            var result = predEngine.Predict(row);
                            results.Add(result);
                        }

                        return DataServiceResult.Success(results);
                    }
                }
            }
        }

        public async Task<DataServiceResult> TrainAsync(ForecastEditModel model)
        {
            try
            {
                var newTrainFilePath = fileProvider.GetFullPath($@"Data\{Guid.NewGuid()}.csv");

                using (var stream = model.File.OpenReadStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var csvReader = new CsvReader(reader, new Configuration
                        {
                            Delimiter = model.Delimiter
                        }))
                        {
                            using (var writer = new StreamWriter(newTrainFilePath))
                            {
                                using (var csvWriter = new CsvWriter(writer, new Configuration
                                {
                                    Delimiter = ";"
                                }))
                                {
                                    await csvReader.ReadAsync();
                                    csvReader.ReadHeader();
                                    csvWriter.WriteHeader<TrainRow>();
                                    await csvWriter.NextRecordAsync();
                                    await csvReader.ReadAsync();

                                    while (await csvReader.ReadAsync())
                                    {
                                        var canGetDate = csvReader.TryGetField("date", out string dateStr);
                                        var canGetStore = csvReader.TryGetField("store", out string store);
                                        var canGetItem = csvReader.TryGetField("item", out string item);
                                        var canGetSales = csvReader.TryGetField("sales", out int sales);

                                        if (!canGetDate || !canGetItem || !canGetSales)
                                            return DataServiceResult.Failed("Не все поля найдены");

                                        var date = DateTime.Parse(dateStr);

                                        var row = new TrainRow
                                        {
                                            Item = item,
                                            Store = canGetStore ? store : "0",
                                            Month = date.Month,
                                            Week = (float)date.DayOfWeek,
                                            Year = date.Year,
                                            Sales = sales
                                        };
                                        csvWriter.WriteRecord(row);
                                        await csvWriter.NextRecordAsync();
                                    }
                                }
                            }
                        }
                    }
                }
                var mlContext = new MLContext(seed: 0);

                var trainDataView = mlContext.Data.LoadFromTextFile<TrainRow>(newTrainFilePath, separatorChar: ';');
                var dataPipeline = mlContext.Transforms.Categorical.OneHotEncoding("ItemFeature", nameof(TrainRow.Item))
                    .Append(mlContext.Transforms.Categorical.OneHotEncoding("StoreFeature", nameof(TrainRow.Store)))
                    .Append(mlContext.Transforms.Concatenate("Features", nameof(TrainRow.Week), nameof(TrainRow.Month), nameof(TrainRow.Year), 
                        "ItemFeature", "StoreFeature"));

                var trainPipeline = dataPipeline.Append(mlContext.Regression.Trainers.LightGbm("Sales"));
                var trainModel = trainPipeline.Fit(trainDataView);
                var trainModelPath = fileProvider.GetFullPath($@"Data\Models\{Guid.NewGuid()}.zip");

                mlContext.Model.Save(trainModel, trainDataView.Schema, trainModelPath);
                File.Delete(newTrainFilePath);

                var entity = new ForecastResult
                {
                    ModelPath = trainModelPath,
                    Id = Guid.NewGuid(),
                    UserId = model.UserId,
                    Name = model.Name
                };
                await forecastRepository.AddAsync(entity);
                return DataServiceResult.Success(entity.Id);
            }
            catch(Exception e)
            {
                return CommonError("Ошибка при обучении модели", e);
            }
        }
    }
}
