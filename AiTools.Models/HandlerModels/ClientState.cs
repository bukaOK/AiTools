using System;
using System.Collections.Generic;
using System.Text;

namespace AiTools.Models.HandlerModels
{
    public class ClientState
    {
        public bool UserAuthenticated { get; set; }
        /// <summary>
        /// Имя Фамилия
        /// </summary>
        public string UserName { get; set; }
        public IList<string> UserRoles { get; set; }
    }
}
