// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;

namespace Moryx.Tools;

/// <summary>
/// Extension methods for the <see cref="IEditableObject"/>
/// </summary>
public static class EditableObjectExtensions
{
    /// <param name="editableObjects">Enumerable of editable objects</param>
    /// <typeparam name="T">Generic type of the object</typeparam>
    extension<T>(IEnumerable<T> editableObjects) where T : IEditableObject
    {
        /// <summary>
        /// Begins an edit on an enumerable of objects
        /// </summary>
        public void BeginEdit()
        {
            foreach (var editableObject in editableObjects)
                editableObject.BeginEdit();
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" /> call.
        /// </summary>
        public void CancelEdit()
        {
            foreach (var editableObject in editableObjects)
                editableObject.CancelEdit();
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit" />
        /// </summary>
        public void EndEdit()
        {
            foreach (var editableObject in editableObjects)
                editableObject.EndEdit();
        }
    }
}