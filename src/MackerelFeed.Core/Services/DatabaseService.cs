// <copyright file="DatabaseService.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using DA.UI.Services;
using MackerelFeed.Exceptions;
using MackerelFeed.Models;
using SQLite;

namespace MackerelFeed.Services;

/// <summary>
/// Database Service.
/// </summary>
public class DatabaseService : IDisposable
{
    private const SQLite.SQLiteOpenFlags Flags =
        SQLite.SQLiteOpenFlags.ReadWrite |
        SQLite.SQLiteOpenFlags.Create |
        SQLite.SQLiteOpenFlags.SharedCache;

    private SQLiteAsyncConnection database;
    private IErrorHandler errorHandler;

    private bool isInitialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    /// <param name="connectionString">Connection String.</param>
    /// <param name="errorHandler">Error Handler.</param>
    public DatabaseService(string connectionString, IErrorHandler errorHandler)
    {
        SQLitePCL.Batteries.Init();
        this.database = new SQLiteAsyncConnection(connectionString, Flags);
        this.errorHandler = errorHandler;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DatabaseService"/> class.
    /// </summary>
    ~DatabaseService()
    {
        this.ReleaseUnmanagedResources();
    }

    /// <summary>
    /// Gets a value indicating whether the database is initialized.
    /// </summary>
    public bool IsInitialized => this.isInitialized;

    /// <summary>
    /// Initialize the database.
    /// </summary>
    /// <returns>Bool.</returns>
    public async Task<bool> InitializeAsync()
    {
        var createAppSettingsTable = this.database.CreateTableAsync<AppSettings>();
        var results = await Task.WhenAll(createAppSettingsTable);
        if (results.Any(n => n != CreateTableResult.Created && n != CreateTableResult.Migrated))
        {
            return this.isInitialized = false;
        }

        return this.isInitialized = true;
    }

    /// <summary>
    /// Get AppSettings.
    /// </summary>
    /// <returns><see cref="AppSettings"/>.</returns>
    public async Task<AppSettings?> GetAppSettingsAsync()
    {
        if (!this.isInitialized)
        {
            return null;
        }

        try
        {
            var settings = await this.database.Table<AppSettings>().FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new AppSettings();
                await this.database.InsertAsync(settings, typeof(AppSettings));
            }

            return settings;
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error getting AppSettings.", ex));
            return null;
        }
    }

    /// <summary>
    /// Dispose elements.
    /// </summary>
    public void Dispose()
    {
        this.ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        this.database.CloseAsync();
    }
}