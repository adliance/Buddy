using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Adliance.AspNetCore.Buddy.Bulma.Test.Pages;

public class IndexModel : PageModel
{
    [BindProperty] public string InputValue { get; set; } = "My initial input value";
    [BindProperty] public int NumberValue { get; set; } = 123;
    [BindProperty] public string PasswordValue { get; set; } = "My initial password";
    [BindProperty] public string TextareaValue { get; set; } = $"My initial{Environment.NewLine}textarea value";
    [BindProperty] public IFormFile? FileValue { get; set; }
    [BindProperty] public IFormFile[]? FileValues { get; set; }
    [BindProperty] public DateTime DateValue { get; set; } = DateTime.Now;
    [BindProperty] public bool CheckboxValue { get; set; }

    [BindProperty]
    public IList<PossibleValues> SelectedCheckboxListValues { get; set; } = new List<PossibleValues>
    {
        PossibleValues.Five,
        PossibleValues.Three,
        PossibleValues.One
    };

    [BindProperty] public PossibleValues SelectValue { get; set; }

    public enum PossibleValues
    {
        One,
        Two,
        Three,
        Four,
        Five
    }
}
