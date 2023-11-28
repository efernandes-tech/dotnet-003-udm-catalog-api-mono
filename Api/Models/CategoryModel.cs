using Api.Validations;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class CategoryModel : BaseModel, IValidatableObject
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, ErrorMessage = "Name must be between 5 and 50 characters", MinimumLength = 5)]
    [FirstLetterCapitalizedAttibute]
    public string? Name { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }

    public ICollection<ProductModel>? Products { get; set; }

    public CategoryModel()
    {
        Products = new Collection<ProductModel>();
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(this.Description))
        {
            var firstLetter = this.Description[0].ToString();
            if (firstLetter != firstLetter.ToUpper())
            {
                yield return new ValidationResult(
                    "First letter must be capitalized",
                    new[]
                    {
                        nameof(this.Description)
                    }
                );
            }
        }
    }
}
