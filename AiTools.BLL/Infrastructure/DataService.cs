using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace AiTools.BLL.Infrastructure
{
    public abstract class DataService<TService> : IDataService
    {
        protected readonly ILogger<TService> logger;
        protected DataServiceResult Success => DataServiceResult.Success();

        public DataService(ILogger<TService> logger)
        {
            this.logger = logger;
        }

        protected DataServiceResult CommonError(string message, Exception e)
        {
            var msg = $"{message}: {e.Message}";
            if (e.InnerException != null)
                msg += $";{e.InnerException.Message}";
            logger.LogError(msg);
            return DataServiceResult.Failed(message);
        }
    }
}
