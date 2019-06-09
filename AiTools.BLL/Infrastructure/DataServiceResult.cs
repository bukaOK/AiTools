using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiTools.BLL.Infrastructure
{
    public class DataServiceResult
    {
        public DataServiceResult(IEnumerable<string> errors)
        {
            Succeeded = false;
            Errors = errors;
        }
        public DataServiceResult(bool success, object data)
        {
            Succeeded = success;
            ResultData = data;
        }
        public DataServiceResult(bool success, object data, IEnumerable<string> errors)
        {
            Succeeded = success;
            ResultData = data;
            Errors = errors;
        }

        public object ResultData { get; }
        public bool Succeeded { get; }
        public IEnumerable<string> Errors { get; }

        public static DataServiceResult Success() => new DataServiceResult(true, null);

        public static DataServiceResult Success<TData>(TData data) => new DataServiceResult(true, data);

        public static DataServiceResult Failed(params string[] errors) => Failed(errors.ToList());

        public static DataServiceResult Failed(IEnumerable<string> errors) => new DataServiceResult(errors);

        public static DataServiceResult Failed(object data, params string[] errors) => new DataServiceResult(false, data, errors);

        public static DataServiceResult Failed(IEnumerable<IdentityError> errors) => new DataServiceResult(errors.Select(x => x.Description));

        public static DataServiceResult Failed(object data, IEnumerable<string> errors) => new DataServiceResult(false, data, errors);
    }
}
