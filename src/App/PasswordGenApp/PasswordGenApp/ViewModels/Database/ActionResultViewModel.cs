using System.Collections.Generic;
using Database.Models.Core;

namespace PasswordGenApp.ViewModels.Database;

public class ActionResultViewModel<T>
{
    public bool IsSuccess { get; set; }
    public IList<ActionResultInfo> ResultInfos { get; } = new List<ActionResultInfo>();
    public T? Value { get; set; }
}