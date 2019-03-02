using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AiTools.Infrastructure.Localization
{
    public class JsonLocalizer : IStringLocalizer
    {
        private readonly List<Resource> resources;

        public JsonLocalizer(string resourcePath)
        {
            using(var reader = new StreamReader(resourcePath))
            {
                var serializer = new JsonSerializer();
                resources = (List<Resource>)serializer.Deserialize(reader, typeof(List<Resource>));
            }
        }
        public LocalizedString this[string name]
        {
            get
            {
                var val = GetVal(name);
                return new LocalizedString(name, val, string.IsNullOrEmpty(val));
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var val = GetVal(name);
                return new LocalizedString(name, string.Format(val, arguments), string.IsNullOrEmpty(val));
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var res = resources.FirstOrDefault(x => x.Locale == CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            if (res == null)
                return new List<LocalizedString>();
            return res.Pairs.Select(x => new LocalizedString(x.Key, x.Value));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }

        private string GetVal(string name)
        {
            var res = resources.FirstOrDefault(x => x.Locale == CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            if (res == null)
                return null;
            return res.Pairs.GetValueOrDefault(name);
        }
    }
}
