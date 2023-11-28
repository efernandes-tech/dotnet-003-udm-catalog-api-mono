using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Api.Models;

public class ProductModel : BaseModel
{
    [Required]
    public string? Name { get; set; }

    public string? Description { get; set; }

    [Required]
    public int Quantity { get; set; }

    public int? CategoryId { get; set; }

    [JsonIgnore]
    public CategoryModel? Category { get; set; }
}
