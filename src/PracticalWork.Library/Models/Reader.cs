using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PracticalWork.Library.Models
{
    /// <summary>
    /// Читатель
    /// </summary>
    public sealed class Reader
    {
        /// <summary>ФИО</summary>
        /// <remarks>Запись идет через пробел</remarks>
        public string FullName { get; set; }

        /// <summary>Номер телефона</summary>
        public string PhoneNumber { get; set; }

        /// <summary>Дата окончания действия карточки</summary>
        public DateOnly ExpiryDate { get; set; }

        /// <summary>Активность карточки</summary>
        public bool IsActive { get; set; }

    }
}
