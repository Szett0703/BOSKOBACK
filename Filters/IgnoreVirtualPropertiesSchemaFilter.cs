using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace DBTest_BACK.Filters
{
    /// <summary>
    /// Filtro personalizado de Swagger para ignorar propiedades virtuales 
    /// y prevenir referencias circulares en la documentación de la API
    /// </summary>
    public class IgnoreVirtualPropertiesSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null || context?.Type == null)
                return;

            var excludedProperties = context.Type
                .GetProperties()
                .Where(p =>
                    // Propiedades virtuales (navigation properties de EF)
                    p.GetGetMethod()?.IsVirtual == true ||
                    // Colecciones (excepto strings)
                    (p.PropertyType.GetInterface(nameof(System.Collections.IEnumerable)) != null &&
                     p.PropertyType != typeof(string)) ||
                    // Propiedades con [JsonIgnore]
                    p.GetCustomAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>() != null
                )
                .Select(p =>
                    // Convertir a camelCase para coincidir con la serialización JSON
                    char.ToLowerInvariant(p.Name[0]) + p.Name[1..]
                );

            foreach (var excludedProperty in excludedProperties)
            {
                if (schema.Properties.ContainsKey(excludedProperty))
                {
                    schema.Properties.Remove(excludedProperty);
                }
            }
        }
    }
}
