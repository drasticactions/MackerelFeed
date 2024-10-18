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
        var createTablesResult = await this.database.CreateTablesAsync<AppSettings, FeedItem, FeedListItem, FeedFolder>(CreateFlags.None);
        this.isInitialized = createTablesResult.Results.Any(x => x.Value != CreateTableResult.Created && x.Value != CreateTableResult.Migrated) == false;
        return this.isInitialized;
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
    /// Update AppSettings.
    /// </summary>
    /// <param name="settings"><see cref="AppSettings"/>.</param>
    /// <returns>If the app settings were updated.</returns>
    public async Task<bool> UpdateAppSettingsAsync(AppSettings settings)
    {
        if (!this.isInitialized)
        {
            return false;
        }

        int rows = 0;

        try
        {
           rows = await this.database.UpdateAsync(settings, typeof(AppSettings));
        }
        catch (Exception ex)
        {
            this.errorHandler.HandleError(new DatabaseException("Error updating AppSettings.", ex));
        }

        return rows > 0;
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