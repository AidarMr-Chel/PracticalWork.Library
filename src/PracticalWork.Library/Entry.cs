using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Services;

namespace PracticalWork.Library;

/// <summary>
/// Точка входа для регистрации сервисов доменного уровня.
/// Содержит расширение для добавления бизнес-логики в DI-контейнер.
/// </summary>
public static class Entry
{
    /// <summary>
    /// Регистрирует сервисы доменного уровня в контейнере зависимостей.
    /// </summary>
    /// <param name="services">Коллекция сервисов.</param>
    /// <returns>Обновлённая коллекция сервисов.</returns>
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IReaderService, ReaderService>();
        services.AddScoped<IBorrowService, BorrowService>();

        return services;
    }
}
