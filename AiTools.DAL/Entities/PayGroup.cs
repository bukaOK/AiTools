using System;
using System.Collections.Generic;
using System.Text;

namespace AiTools.DAL.Entities
{
    public class PayGroup
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Сумма всех пользователей в группе
        /// </summary>
        public double Cash { get; set; }
        public List<User> Users { get; set; }
    }
}
