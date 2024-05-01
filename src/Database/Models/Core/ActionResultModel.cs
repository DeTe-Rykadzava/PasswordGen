using System.Collections.Generic;

namespace Database.Models.Core;

public class ActionResultModel<T>
{
    public bool IsSuccess { get; set; } = false;
    public IList<ActionResultInfo> ResultInfos { get; } = new List<ActionResultInfo>();
    public T? Value { get; set; }
}