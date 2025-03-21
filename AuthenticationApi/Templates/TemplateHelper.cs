namespace AuthenticationApi.Templates;

public class TemplateHelper
{
    private readonly IWebHostEnvironment _env;

    public TemplateHelper(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> replacements)
    {
        var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "EmailTemplates", templateName);
        
        if (!System.IO.File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Template file not found: {templatePath}");
        }

        var templateContent = await System.IO.File.ReadAllTextAsync(templatePath);

        foreach (var replacement in replacements)
        {
            templateContent = templateContent.Replace($"{{{replacement.Key}}}", replacement.Value);
        }

        return templateContent;
    }
}
