
# Test Case 1 - Add a Resource

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the **Plus** button at the toolbar | A dialog pops up asking for the type of the created resource. No type is selected and the **Next** button is disabled. | |
| Select the *SimulatedAssemblyDriver* type | The **Next** button is enabled. | |
| Click **Next** and then click **CREATE** button | The dialog closes and the right side of the screen shows the properties component of a new Resource (Driver). The **add** button is disabled and the right side navbar shows the **cancel** and the **save** button. | |
| Fill in the name and some values for the driver properties | | |
| Click the **save** button | The right side of the screen returns to the index page and the driver appears in the tree in the left side navbar. | |
| Click on the **Plus** button at the toolbar again | A dialog pops up asking for the type of the created resource. No type is selected and the **Next** button is disabled. | |
| Select the *AssemblyCell* type | The **Next** button is enabled. | |
| Click **Next** | The dialog shows the properties component for the new AssemblyCell, including a **References** section. | |
| Under **References → Driver**, select the previously created **SimulatedAssemblyCellDriver** | The reference is set and required to proceed. | |
| Click the **CREATE** button | The dialog closes and the right side of the screen shows the properties component of the new AssemblyCell. The **add** button is disabled and the right side navbar shows the **cancel** and the **save** button. | |
| Fill in the name and some values for the cell properties | | |
| Click the **save** button | The right side of the screen returns to the index page and the cell appears in the tree in the left side navbar. | |
