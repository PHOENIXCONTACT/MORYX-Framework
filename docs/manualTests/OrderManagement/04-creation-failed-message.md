# Test Case 4 - Error messages shows up if the creation of an operation failed

## Preconditions

Setup a *failed* order, for example by adding a new order with a non-existent `productIdentifier`. You can use any endpoint calling tools like Swagger, Postman, etc.

```yaml
POST {baseUrl}/api/moryx/orders

{
  "order": {
    "number": "987654321",
    "type": "1-23",
    "materialParameters": [
    ],
    "operations": [
    ]
  },
  "totalAmount": 10,
  "name": "My order",
  "number": "4321",
  "productIdentifier": "0123456",
  "productRevision": 13,
  "productName": "TT-ST-M-SFP-24AC",
  "recipePreselection": 2
}
```

## Steps

| Step/Instruction | Expected Result | Comment |
|------------------|-----------------|---------|
| Click on the *Messages* button of an operation | Messages should show at least one error message. Log messages have a timestamp, a type indicator and message text |  |
