// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData.SpreadsheetsListener;

internal class CsvRow
{
    public IList<object> Fields { get; }

    public CsvRow(IList<object> row)
    {
        Fields = row;
    }

    public void Add(object item)
    {
        Fields.Add(item);
    }

    public object ElementAt(int index)
    {
        return Fields.ElementAt(index);
    }
}