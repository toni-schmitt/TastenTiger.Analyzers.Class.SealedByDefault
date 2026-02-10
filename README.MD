# TastenTiger.Analyzers.Class.SealedByDefault

A Roslyn analyzer that ensures that classes and records which are not inherited anywhere are marked as `sealed`.
This promotes clearer design intent, better performance, and safer code.

## Why enforce sealed classes?

- To prevent unintended inheritance
- To improve runtime performance
- To clarify design intent
- To encourage deliberate API design

## What this analyzer does

- Produces the `TA0002` compilation hint when a class or record:
    - is not declared `sealed`
    - and is not inherited anywhere in the solution
- Provides a bulk quick fix to automatically add the `sealed` keyword to the class or record declaration.

## Example

```csharp
public class NotSealedClass // <-- triggers TA0002
{
}

public sealed class SealedClass // <-- compliant
{
}
```

For more examples, see:  
`TastenTiger.Analyzers.Class.SealedByDefault.Sample.Examples.cs`
