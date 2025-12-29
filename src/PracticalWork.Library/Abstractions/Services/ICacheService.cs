using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Abstractions.Services
{
    public interface ICacheService
    {
        /// <summary>
        /// Получение значения из кэша по ключу
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);
        /// <summary>
        /// Сохранение значения в кэш по ключу с временем жизни
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T value, TimeSpan ttl);
        /// <summary>
        /// Удаление значения из кэша по ключу
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(string key);
        /// <summary>
        /// Отслеживание ключа в реестре для последующего удаления
        /// </summary>
        /// <param name="registryKey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task TrackKeyAsync(string registryKey, string key);
        /// <summary>
        /// Очистка кэша по реестру ключей
        /// </summary>
        /// <param name="registryKey"></param>
        /// <returns></returns>
        Task ClearByRegistryAsync(string registryKey);
    }

}
