using Microsoft.CodeAnalysis;

namespace Frank.SourceGenerator.AdditionalFiles.Internals;

internal class DiagnosticBuilder
{
    private DiagnosticDescriptor? _descriptor;
    private Location? _location;
    private object?[]? _arguments;
    
    public DiagnosticBuilder WithDescriptor(DiagnosticDescriptor descriptor)
    {
        _descriptor = descriptor;
        return this;
    }
    
    public DiagnosticBuilder WithLocation(Location location)
    {
        _location = location;
        return this;
    }
    
    public DiagnosticBuilder WithArguments(params object[] arguments)
    {
        _arguments = arguments;
        return this;
    }
    
    public Diagnostic Build()
    {
        if (_descriptor == null)
            throw new InvalidOperationException("DescriptorBuilder must be set.");
        
        if (_location == null)
            throw new InvalidOperationException("Location must be set.");
        
        if (GetParameterCount() != (_arguments?.Length ?? 0))
            throw new InvalidOperationException("Argument count does not match parameter count.");
        
        return Diagnostic.Create(_descriptor, _location, _arguments);
    }
    
    private int GetParameterCount() => _descriptor?.MessageFormat.ToString().Count(c => c == '{') ?? 0;
}