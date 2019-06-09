using AiTools.BLL.Infrastructure;
using AiTools.BLL.Providers;
using AiTools.BLL.Services.Interfaces;
using AiTools.DAL.Entities;
using AiTools.DAL.Repositories;
using AiTools.Models.AnalyzeModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AiTools.BLL.Services
{
    public class AnalyzeService : DataService, IAnalyzeService
    {
        private readonly AnalyzeRepository analyzeRepository;
        private readonly FileProvider fileProvider;
        private readonly IMapper mapper;

        public AnalyzeService(ILogger<AnalyzeService> logger, AnalyzeRepository analyzeRepository, FileProvider fileProvider,
            IMapper mapper) : base(logger)
        {
            this.analyzeRepository = analyzeRepository;
            this.fileProvider = fileProvider;
            this.mapper = mapper;
        }

        public async Task<DataServiceResult> AddAsync(EditAnalyzeModel model)
        {
            try
            {
                var entity = mapper.Map<AnalyzeResult>(model);
                var savePath = $"Files/AnalyzeResults/{entity.Id}.json.gz";

                entity.DataFilePath = await fileProvider.SaveFileCompressedAsync(savePath, model.Data);
                await analyzeRepository.AddAsync(entity);
                return Success;
            }
            catch(Exception e)
            {
                return CommonError("Ошибка при добавлении результата анализа", e);
            }
        }

        public async Task<EditAnalyzeModel> GetDataAsync(Guid id, string userId)
        {
            var entity = await analyzeRepository.GetAsync(id);
            if (entity.UserId != userId)
                return null;
            var model = mapper.Map<EditAnalyzeModel>(entity);
            model.Data = await fileProvider.GetCompressedDataAsync(entity.DataFilePath);
            return model;
        }

        public async Task<IList<SelectListItem>> GetSavedAsync(string userId)
        {
            var entities = await analyzeRepository.GetByUserAsync(userId);
            return mapper.Map<IList<SelectListItem>>(entities);
        }

        public async Task<DataServiceResult> RemoveAsync(Guid id)
        {
            try
            {
                var entity = await analyzeRepository.GetAsync(id);
                if (entity == null)
                    return DataServiceResult.Failed("Анализ не найден");
                await analyzeRepository.RemoveAsync(entity);
                return Success;
            }
            catch(Exception e)
            {
                return CommonError("Ошибка при удалении анализа", e);
            }
        }

        public async Task<DataServiceResult> UpdateAsync(EditAnalyzeModel model)
        {
            try
            {
                if (model.Id == null)
                    return DataServiceResult.Failed("Не найден анализ");
                var entity = await analyzeRepository.GetAsync(model.Id);
                if(entity == null)
                    return DataServiceResult.Failed("Не найден анализ");

                entity.DataFilePath = await fileProvider.SaveFileCompressedAsync(entity.DataFilePath, model.Data, false);
                await analyzeRepository.AddAsync(entity);
                return Success;
            }
            catch (Exception e)
            {
                return CommonError("Ошибка при добавлении результата анализа", e);
            }
        }
    }
}
