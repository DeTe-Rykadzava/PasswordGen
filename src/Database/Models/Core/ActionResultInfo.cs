namespace Database.Models.Core;

public enum ActionResultInfo
{
    SuccessAdd,
    SuccessRemove,
    SuccessSave,
    SuccessGet,
    FailAdd,
    FailRemove,
    FailSave,
    FailGet,
    InternalError
}