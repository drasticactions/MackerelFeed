// <copyright file="Utilities.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Reflection;

namespace MackerelFeed.Tools;

/// <summary>
/// Helper Utilities.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Get the Default Placeholder Icon.
    /// </summary>
    /// <returns>Image Byte Array.</returns>
    /// <exception cref="Exception">Thrown if can't get the image.</exception>
    public static byte[] GetPlaceholderIcon()
    {
        var resource = GetResourceFileContent("Icon.favicon.ico");
        if (resource is null)
        {
            throw new Exception("Failed to get placeholder icon.");
        }

        using MemoryStream ms = new MemoryStream();
        resource.CopyTo(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Get Resource File Content via FileName.
    /// </summary>
    /// <param name="fileName">Filename.</param>
    /// <returns>Stream.</returns>
    public static Stream? GetResourceFileContent(string fileName)
        => GetResourceFileContent(Assembly.GetExecutingAssembly(), fileName);

    /// <summary>
    /// Get Resource File Content via FileName.
    /// </summary>
    /// <param name="assembly">Assembly.</param>
    /// <param name="fileName">Filename.</param>
    /// <returns>Stream.</returns>
    public static Stream? GetResourceFileContent(Assembly? assembly, string fileName)
    {
        var resourceName = "MauiFeed." + fileName;
        if (assembly is null)
        {
            return null;
        }

        return assembly.GetManifestResourceStream(resourceName);
    }
}