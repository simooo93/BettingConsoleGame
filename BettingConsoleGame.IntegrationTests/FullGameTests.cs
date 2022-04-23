﻿using BettingConsoleGame.Domain.Entities;
using BettingConsoleGame.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace BettingConsoleGame.IntegrationTests;

using static Testing;

public class FullGameTests : TestBase
{
    [Test]
    public void DepositBetWithdrawExit()
    {
        var wallet = Wallet.Empty;

        //Act
        ExecuteAction(wallet, "deposit 15");
        SetupNextBetToWinTwiceBetSize();
        ExecuteAction(wallet, "bet 10");
        ExecuteAction(wallet, "withdraw 10");
        ExecuteAction(wallet, "exit");

        //Assert
        VerifyExitMessage(Money.Dollars(10));
        wallet.Balance.Should().Be(Money.Dollars(15));
    }

    [Test]
    public void DepositDepositBetBetWithdrawWithdrawExit()
    {
        var wallet = Wallet.Empty;

        //Act
        ExecuteAction(wallet, "deposit 15");
        ExecuteAction(wallet, "withdraw 5");

        SetupNextBetToWinTwiceBetSize();
        ExecuteAction(wallet, "bet 10");
        ExecuteAction(wallet, "bet 5");

        SetupNextBetToLose();
        ExecuteAction(wallet, "bet 5");
        ExecuteAction(wallet, "withdraw 10");
        ExecuteAction(wallet, "exit");

        //Assert
        VerifyExitMessage(Money.Dollars(10));
        wallet.Balance.Should().Be(Money.Dollars(10));
    }

    private static void SetupNextBetToLose()
    {
        NumberRandomizerServiceMock.SetupChanceWinner(75);
    }

    private static void SetupNextBetToWinTwiceBetSize()
    {
        NumberRandomizerServiceMock.SetupChanceWinner(5);
        NumberRandomizerServiceMock.SetupBigWinnerMultiplier(2);
    }

    private static void VerifyExitMessage(Money wonAmount)
    {
        WriteLineMock.Verify(x => x.WriteLine($"Thanks for playing! You won {wonAmount} today :) Come back soon!", ConsoleColor.White), Times.Once);
    }
}
