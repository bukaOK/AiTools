using System;
using System.Collections.Generic;
using System.Text;

namespace AiTools.DAL.Entities
{
    public class Shop
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Название организации/проекта
        /// </summary>
        public string OrganizationName { get; set; }
        /// <summary>
        /// Название магазина(для пользователя)
        /// </summary>
        public string ShopName { get; set; }
        /// <summary>
        /// Счет
        /// </summary>
        public double Cash { get; set; }
        /// <summary>
        /// Описание деятельности
        /// </summary>
        public string ActivityDesription { get; set; }
    }
}
