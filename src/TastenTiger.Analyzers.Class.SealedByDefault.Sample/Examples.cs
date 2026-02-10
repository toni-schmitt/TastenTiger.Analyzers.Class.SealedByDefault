// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace TastenTiger.Analyzers.Class.SealedByDefault.Sample;

public class Example; // <-- Violates TA0002

public abstract class BaseClass;

public static class StaticClass;

public class NonSealedClass; // <-- Inherited 

public class NonSealedInheritor : NonSealedClass;

public class NonSealedInheritor2 : NonSealedInheritor;

public class NonSealedInheritor3 : NonSealedInheritor2; // <-- Violates TA0002

public abstract record AbstractRecordClass;

public sealed record SealedRecordClass : AbstractRecordClass;

public record NonSealedRecordClass : AbstractRecordClass; // <-- Violates TA0002