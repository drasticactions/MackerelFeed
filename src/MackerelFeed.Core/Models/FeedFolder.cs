// <copyright file="FeedFolder.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using SQLite;

namespace MackerelFeed.Models;

/// <summary>
/// Feed Folder.
/// </summary>
public class FeedFolder
{
    /// <summary>
    /// Gets or sets the Id of the folder.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement]
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the folder.
    /// </summary>
    public string? Name { get; set; }
}