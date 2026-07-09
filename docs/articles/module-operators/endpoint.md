# Operators Endpoint

The Operators Endpoint provides a REST API for managing operators and their skills.

## Facades

This endpoint is based on the following facades:

- [`IOperatorManagement`](/src/Moryx.Operators/IOperatorManagement.cs) - for operator management
- [`ISkillManagement`](/src/Moryx.Operators/Skills/ISkillManagement.cs) - for skill management
- [`IAttendanceManagement`](/src/Moryx.Operators/Attendances/IAttendanceManagement.cs) - for attendance tracking

## Controllers

- **OperatorManagementController**: Manages operators and their assignments
- **SkillManagementController**: Manages skill types and skill assignments to operators

## Permissions

Permissions are defined in [`OperatorPermissions`](/src/Moryx.Operators.Endpoints/OperatorPermissions.cs) and [`SkillPermissions`](/src/Moryx.Operators.Endpoints/SkillPermissions.cs).

### Operator Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Operators.CanView` | Permission for all actions related to viewing operators |
| `Moryx.Operators.CanManage` | Permission for all actions related to managing operators (create, update, delete) |

### Skill Permissions

| Permission String | Description |
|-------------------|-------------|
| `Moryx.Skills.CanView` | Permission for all actions related to viewing skills |
| `Moryx.Skills.CanManage` | Permission for all actions related to managing skills (create, update, delete) |
