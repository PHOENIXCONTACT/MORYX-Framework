// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Simulation.Tests;

public class Builder
{
    public static AssemblyTestCell CellBuilder(int id, SimulatedDummyTestDriver driver)
    {
        var cell = new AssemblyTestCell();
        cell.Id = id;
        cell.Driver = driver;
        driver.Cell = cell;

        return cell;
    }
}