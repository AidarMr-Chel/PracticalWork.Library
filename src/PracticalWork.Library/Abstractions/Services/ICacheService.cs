namespace PracticalWork.Library.Abstractions.Services
{
    /// <summary>
    /// Абстракция сервиса кэширования.
    /// Предоставляет операции чтения, записи, удаления и группового управления ключами.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Получает значение из кэша по указанному ключу.
        /// </summary>
        /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
        /// <param name="key">Ключ, по которому хранится значение.</param>
        /// <returns>Значение из кэша или значение по умолчанию, если ключ отсутствует.</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// Сохраняет значение в кэше по указанному ключу с заданным временем жизни.
        /// </summary>
        /// <typeparam name="T">Тип сохраняемого значения.</typeparam>
        /// <param name="key">Ключ, по которому будет сохранено значение.</param>
        /// <param name="value">Сохраняемое значение.</param>
        /// <param name="ttl">Время жизни значения в кэше.</param>
        Task SetAsync<T>(string key, T value, TimeSpan ttl);

        /// <summary>
        /// Удаляет значение из кэша по указанному ключу.
        /// </summary>
        /// <param name="key">Ключ удаляемого значения.</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Регистрирует ключ в указанном реестре для последующей групповой очистки.
        /// Используется для логического объединения связанных ключей.
        /// </summary>
        /// <param name="registryKey">Ключ реестра.</param>
        /// <param name="key">Ключ, который необходимо отслеживать.</param>
        Task TrackKeyAsync(string registryKey, string key);

        /// <summary>
        /// Очищает кэш, удаляя все ключи, зарегистрированные в указанном реестре.
        /// </summary>
        /// <param name="registryKey">Ключ реестра, содержащего список отслеживаемых ключей.</param>
        Task ClearByRegistryAsync(string registryKey);
    }
}
