﻿using Identity.Contracts;
using Identity.Service.Entities;
using Identity.Service.Exceptions;
using MassTransit;
using Microsoft.AspNetCore.Identity;

namespace Identity.Service.Consumers;

public class DebitGilConsumer : IConsumer<DebitGil>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DebitGilConsumer(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Consume(ConsumeContext<DebitGil> context)
    {
        var message = context.Message;

        var user = await _userManager.FindByIdAsync(message.UserId.ToString());

        if (user is null)
        {
            throw new UnknownUserException(message.UserId);
        }

        if (user.MessageIds.Contains(context.MessageId!.Value))
        {
            await context.Publish(new GilDebited(message.CorrelationId));

            return;
        }

        user.Gil -= message.Gil;

        if (user.Gil < 0)
        {
            throw new InsufficientFundsException(message.UserId, message.Gil);
        }

        user.MessageIds.Add(context.MessageId.Value);

        await _userManager.UpdateAsync(user);

        var gitDebitedTask = context.Publish(new GilDebited(message.CorrelationId));

        var userUpdatedTask = context.Publish(new UserUpdated(user.Id, user.Email!, user.Gil));

        await Task.WhenAll(userUpdatedTask, gitDebitedTask);
    }
}
