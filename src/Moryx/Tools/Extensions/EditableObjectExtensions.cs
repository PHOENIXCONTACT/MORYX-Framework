// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace Moryx.Tools;

/// <summary>
/// Extension methods for the <see cref="IEditableObject"/>
/// </summary>
public static class EditableObjectExtensions
{
    /// <summary>
    /// Begins an edit on an enumerable of objects
    /// </summary>
    /// <typeparam name="T">Generic type of the object</typeparam>
    /// <param name="editableObjects">Enumerable of editable objects</param>
    public static void BeginEdit<T>(this IEnumerable<T> editableObjects)
        where T : IEditableObject
    {
        foreach (var editableObject in editableObjects)
            editableObject.BeginEdit();
    }

    /// <summary>
    /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
    /// </summary>
    /// <typeparam name="T">Generic type of the object</typeparam>
    /// <param name="editableObjects">Enumerable of editable objects</param>
    public static void CancelEdit<T>(this IEnumerable<T> editableObjects)
        where T : IEditableObject
    {
        foreach (var editableObject in editableObjects)
            editableObject.CancelEdit();
    }

    /// <summary>
    /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" />
    /// </summary>
    /// <typeparam name="T">Generic type of the object</typeparam>
    /// <param name="editableObjects">Enumerable of editable objects</param>
    public static void EndEdit<T>(this IEnumerable<T> editableObjects)
        where T : IEditableObject
    {
        foreach (var editableObject in editableObjects)
            editableObject.EndEdit();
    }
}