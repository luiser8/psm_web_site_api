using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace psm_web_site_api_project.Utils.JsonArrayModelBinder;

public class JsonArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        try
        {
            var jsonString = valueProviderResult.FirstValue;
            if (!string.IsNullOrEmpty(jsonString))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true
                };

                var result = JsonSerializer.Deserialize(jsonString, bindingContext.ModelType, options);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(null);
            }
        }
        catch (JsonException ex)
        {
            bindingContext.ModelState.TryAddModelError(modelName, $"Invalid JSON format: {ex.Message}");
        }
        catch (Exception ex)
        {
            bindingContext.ModelState.TryAddModelError(modelName, $"An error occurred: {ex.Message}");
        }

        return Task.CompletedTask;
    }
}

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class FromJsonAttribute : Attribute, IBinderTypeProviderMetadata
{
    public Type BinderType => typeof(JsonArrayModelBinder);
    public BindingSource? BindingSource { get; }
}

public class JsonArrayModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        // Aplicar solo a propiedades/parámetros con el atributo [FromJson]
        if (context.BindingInfo.BinderType == typeof(JsonArrayModelBinder))
        {
            return new JsonArrayModelBinder();
        }

        return null;
    }
}