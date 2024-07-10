using Microsoft.CodeAnalysis;

namespace Frank.SourceGenerators.CppInteropts.Internals;

internal class DiagnosticDescriptorBuilder
{
    private DiagnosticIdBuilder? _idBuilder;
    
    private string? _title;
    
    private string? _messageFormat;
    
    private DiagnosticCategories? _category;
    
    private DiagnosticSeverity _defaultSeverity = DiagnosticSeverity.Warning;
    
    private bool _isEnabledByDefault = true;
    
    private string? _description;
    
    public DiagnosticDescriptorBuilder WithIdBuilder(DiagnosticIdBuilder idBuilder)
    {
        _idBuilder = idBuilder;
        return this;
    }
    
    public DiagnosticDescriptorBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public DiagnosticDescriptorBuilder WithMessageFormat(string messageFormat)
    {
        _messageFormat = messageFormat;
        return this;
    }
    
    public DiagnosticDescriptorBuilder WithCategory(DiagnosticCategories category)
    {
        _category = category;
        return this;
    }
    
    public DiagnosticDescriptorBuilder WithDefaultSeverity(DiagnosticSeverity defaultSeverity)
    {
        _defaultSeverity = defaultSeverity;
        return this;
    }
    
    public DiagnosticDescriptorBuilder WithIsEnabledByDefault(bool isEnabledByDefault)
    {
        _isEnabledByDefault = isEnabledByDefault;
        return this;
    }
    
    public DiagnosticDescriptorBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }
    
    public DiagnosticDescriptor Build()
    {
        Validate();
        return new DiagnosticDescriptor(_idBuilder!.Build(), _title!, _messageFormat!, _category?.ToString()!, _defaultSeverity, _isEnabledByDefault, _description);
    }
    
    private void Validate()
    {
        if (_idBuilder == null)
            throw new InvalidOperationException("IdBuilder must be set.");
        
        if (string.IsNullOrWhiteSpace(_title))
            throw new InvalidOperationException("Title must be set.");
        
        if (string.IsNullOrWhiteSpace(_messageFormat))
            throw new InvalidOperationException("MessageFormat must be set.");
        
        if (string.IsNullOrWhiteSpace(_description))
            throw new InvalidOperationException("Description must be set.");
        
        if (_category == null)
            throw new InvalidOperationException("Category must be set.");
    }
}