// <copyright file="AppSettings.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using SQLite;

namespace MackerelFeed.Models;

/// <summary>
/// App Settings.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the last updated time of the app.
    /// </summary>
    public DateTime? LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the app theme.
    /// </summary>
    public AppTheme AppTheme { get; set; }

    /// <summary>
    /// Gets or sets the language setting.
    /// </summary>
    public LanguageSetting LanguageSetting { get; set; }
}