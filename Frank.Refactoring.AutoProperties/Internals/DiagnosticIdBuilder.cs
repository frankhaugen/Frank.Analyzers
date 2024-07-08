namespace Frank.Refactoring.AutoProperties.Internals;

internal class DiagnosticIdBuilder
{
    private string _prefix = Constants.DiagnosticPrefix;
    private DiagnosticCategories? _category;
    private uint _id = 0;
    
    public DiagnosticIdBuilder WithPrefix(string prefix)
    {
        _prefix = prefix;
        return this;
    }
    
    public DiagnosticIdBuilder WithCategory(DiagnosticCategories category)
    {
        _category = category;
        return this;
    }
    
    public DiagnosticIdBuilder WithId(uint id)
    {
        _id = id;
        return this;
    }
    
    public string Build()
    {
        Validate();
        return $"{_prefix.ToUpperInvariant()}{GetCategoryValue():0}{_id:000}";
    }
    
    private uint GetCategoryValue() => (uint)_category!;

    private void Validate()
    {
        if (_category == null)
            throw new InvalidOperationException("Category must be set.");
        
        if (string.IsNullOrWhiteSpace(_prefix))
            throw new InvalidOperationException("Prefix must be set.");
        
        if (_id == 0)
            throw new InvalidOperationException("Id must be set.");
        
        if (_id > 999)
            throw new InvalidOperationException("Id must be less than 1000.");
    }
}