using System.ComponentModel.DataAnnotations;

namespace DiscreteMath.Web.Models
{
    public class SetSimplificationModel
    {
        [Required(ErrorMessage = "The value is required.")] // localize
        public string Value { get; set; }
    }
}