using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Context;
using Database.Core;
using Database.DatabaseModels;
using Database.Models;
using Database.Models.Core;
using Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class PasswordRepository : IPasswordRepository
{
    private readonly IDatabaseContext _context;

    public PasswordRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<ActionResultModel<IEnumerable<PasswordModel>>> GetAllPasswordsAsync()
    {
        var result = new ActionResultModel<IEnumerable<PasswordModel>>();
        try
        {
            var passwords = await _context.Passwords.ToListAsync();
            result.IsSuccess = true;
            result.ResultInfos.Add(ActionResultInfo.SuccessGet);
            result.Value = passwords.Select(s => new PasswordModel(s)).ToList();
        }
        catch (Exception e)
        {
            DatabaseLogger.LogError(
                "Error with get all passwords. Message: {Message}.\nInnerException: {InnerException}", e.Message,
                e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailGet);
            result.ResultInfos.Add(ActionResultInfo.InternalError);
        }

        return result;
    }

    public async Task<ActionResultModel<PasswordModel>> AddPasswordAsync(PasswordAddModel model)
    {
        var result = new ActionResultModel<PasswordModel>();
        try
        {
            if (string.IsNullOrWhiteSpace(model.EncryptedPassword))
            {
                result.ResultInfos.Add(ActionResultInfo.FailAdd);
            }
            else
            {
                var addModel = new Password
                {
                    Title = model.Title,
                    EncryptedPassword = model.EncryptedPassword,
                    PasswordHash = model.PasswordHash,
                    Iv = model.Iv
                };

                var password = await _context.Passwords.AddAsync(addModel);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.ResultInfos.Add(ActionResultInfo.SuccessAdd);
                result.ResultInfos.Add(ActionResultInfo.SuccessSave);
                result.Value = new PasswordModel(password.Entity);
            }
        }
        catch (Exception e)
        {
            DatabaseLogger.LogError("Error with add password. Message: {Message}.\nInnerException: {InnerException}",
                e.Message, e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailAdd);
            result.ResultInfos.Add(ActionResultInfo.FailSave);
        }

        return result;
    }

    public async Task<ActionResultModel<bool>> RemovePasswordAsync(int id)
    {
        var result = new ActionResultModel<bool>();
        try
        {
            var password = await _context.Passwords.FirstOrDefaultAsync(x => x.Id == id);
            if (password == null)
            {
                result.ResultInfos.Add(ActionResultInfo.FailGet);
            }
            else
            {
                _context.Passwords.Remove(password);
                await _context.SaveChangesAsync();

                result.IsSuccess = true;
                result.ResultInfos.Add(ActionResultInfo.SuccessRemove);
                result.ResultInfos.Add(ActionResultInfo.SuccessSave);
                result.Value = true;
            }
        }
        catch (Exception e)
        {
            DatabaseLogger.LogError(
                "Error with remove password by Id: {id}. Message: {Message}.\nInnerException: {InnerException}", id,
                e.Message, e.InnerException);
            result.ResultInfos.Add(ActionResultInfo.FailAdd);
            result.ResultInfos.Add(ActionResultInfo.FailSave);
        }

        return result;
    }
}