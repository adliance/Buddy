using System;
using System.Collections.Generic;
using System.Linq;
using Adliance.AspNetCore.Buddy.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Adliance.AspNetCore.Buddy.Bulma;

public static class SelectList
{
    public static IList<SelectListItem> For(Enum enumValue)
    {
        return (from Enum e in Enum.GetValues(enumValue.GetType()) select new SelectListItem(e.GetDisplayName(), e.ToString())).ToList();
    }

    public static IList<SelectListItem> For<T>() where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum) throw new Exception("T must be an enum.");
        if (!typeof(T).IsEnum) throw new Exception("T must be an enum.");
        return (from Enum e in Enum.GetValues(typeof(T)) select new SelectListItem(e.GetDisplayName(), e.ToString())).ToList();
    }

    public static IList<SelectListItem> For(string trueText = "Ja", string falseText = "Nein")
    {
        return new List<SelectListItem>
        {
            new(trueText, "true"),
            new(falseText, "false")
        };
    }

    public static IList<SelectListItem> For(IDictionary<Guid, string> dict)
    {
        return dict.Select(x => new SelectListItem(x.Value, x.Key.ToString())).ToList();
    }
}