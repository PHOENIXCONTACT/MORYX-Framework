# Test Case 1 - Create notifications and check, if it shows up

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
|**Create notifications** as described below |||
| Go to the **Notifications** module | The previously created notifications should all show up and be sorted by their severity |||
| Check their data | **Title**, **message** and **severity** should match the provided data. ||


### Create notifications

1. Go to **Resources** and select any of them
2. Enter the resources **Methods** tab
3. There must be an option **PublishNotification**, select it
4. Enter a title, message and select a severity
5. **INVOKE** it

Repeat this test for all types of `Severity`:

* `Info`
* `Warning`
* `Error`
* `Fatal`