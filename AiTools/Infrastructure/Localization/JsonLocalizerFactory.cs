using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.IO;

namespace AiTools.Infrastructure.Localization
{
    public class JsonLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public JsonLocalizerFactory(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public IStringLocalizer Create(Type resourceSource)
        {
            throw new NotImplementedException("Not realized");
        }

        public IStringLocalizer Create(string resourceName, string location)
        {
            return new JsonLocalizer(Path.Combine(hostingEnvironment.WebRootPath, "/", location, resourceName + ".json"));
        }
    }
}
