using Microsoft.Extensions.Logging;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Setups;

namespace Moryx.ControlSystem.SetupProvider;

internal partial class SetupManager
{
    private static partial bool LogTargetCapsProvided(
        SetupEvaluation.Change change,
        IEnumerable<ICell> targetCells,
        ILogger logger)
    {
        logger.LogTrace(
         "Target already provides required capabilities {capabilities} (cells=[{cells}])",
         change.TargetCapabilities,
         string.Join(", ", targetCells.Select(c => c.Name)));

        return true;
    }

    private static partial bool LogCurrentCapsAlreadyRemoved(
        SetupEvaluation.Change change,
        IEnumerable<ICell> currentCells,
        ILogger logger)
    {
        logger.LogTrace(
            "Current capabilities were already removed from target cell {capabilities} (cells=[{cells}])",
            change.CurrentCapabilities,
            string.Join(", ", currentCells.Select(c => c.Name)));

        return true;
    }

}
