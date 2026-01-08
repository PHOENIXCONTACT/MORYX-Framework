// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ProcessData.SpreadsheetsListener;

internal class CsvRow
{
    private readonly IList<object> _row;
    public IList<object> Fields { get => _row; }

    public CsvRow(IList<object> row)
    {
        _row = row;
    }

    public void Add(object item)
    {
        _row.Add(item);
    }

    public object ElementAt(int index)
    {
        return _row.ElementAt(index);
    }
}