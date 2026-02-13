using MAUI_TTRPG_DiceRoller.Models;

namespace MAUI_TTRPG_DiceRoller.Services;

public class DiceRollerService
{
    private readonly Random _random = new();

    public DiceRoll RollDice(DiceType diceType, int numberOfDice, int modifier, bool explodingDice = false, bool advantage = false, bool disadvantage = false, int keepHighest = 0, int keepLowest = 0)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(numberOfDice, 1);

        var individualRolls = new List<int>();
        var removedSixes = new List<int>();
        var droppedDice = new List<int>();
        int? advantageRoll1 = null;
        int? advantageRoll2 = null;
        bool hasAdvantage = false;
        bool hasDisadvantage = false;

        if ((advantage || disadvantage) && diceType == DiceType.D20 && numberOfDice == 1)
        {
            advantageRoll1 = _random.Next(1, 21);
            advantageRoll2 = _random.Next(1, 21);

            if (advantage)
            {
                hasAdvantage = true;
                individualRolls.Add(Math.Max(advantageRoll1.Value, advantageRoll2.Value));
            }
            else
            {
                hasDisadvantage = true;
                individualRolls.Add(Math.Min(advantageRoll1.Value, advantageRoll2.Value));
            }
        }
        else if (explodingDice && diceType == DiceType.D6)
        {
            var dicesToRoll = numberOfDice;
            var iterations = 0;
            const int maxIterations = 100;

            while (dicesToRoll > 0 && iterations < maxIterations)
            {
                var newRolls = new List<int>();
                for (int i = 0; i < dicesToRoll; i++)
                {
                    newRolls.Add(_random.Next(1, 7));
                }

                var sixes = newRolls.Count(r => r == 6);
                removedSixes.AddRange(newRolls.Where(r => r == 6));
                individualRolls.AddRange(newRolls.Where(r => r != 6));
                dicesToRoll = sixes * 2;
                iterations++;
            }
        }
        else
        {
            for (int i = 0; i < numberOfDice; i++)
            {
                individualRolls.Add(_random.Next(1, (int)diceType + 1));
            }
        }

        if (keepHighest > 0 && keepHighest < individualRolls.Count)
        {
            var sorted = individualRolls.OrderByDescending(x => x).ToList();
            var kept = sorted.Take(keepHighest).ToList();
            droppedDice = sorted.Skip(keepHighest).ToList();
            individualRolls = kept;
        }
        else if (keepLowest > 0 && keepLowest < individualRolls.Count)
        {
            var sorted = individualRolls.OrderBy(x => x).ToList();
            var kept = sorted.Take(keepLowest).ToList();
            droppedDice = sorted.Skip(keepLowest).ToList();
            individualRolls = kept;
        }

        var sum = individualRolls.Sum();
        var total = sum + modifier;

        return new DiceRoll
        {
            DiceType = diceType,
            NumberOfDice = numberOfDice,
            Modifier = modifier,
            IndividualRolls = individualRolls,
            RemovedSixes = removedSixes,
            DroppedDice = droppedDice,
            KeepHighest = keepHighest,
            KeepLowest = keepLowest,
            HasAdvantage = hasAdvantage,
            HasDisadvantage = hasDisadvantage,
            AdvantageRoll1 = advantageRoll1,
            AdvantageRoll2 = advantageRoll2,
            Total = total,
            RolledAt = DateTime.Now
        };
    }
}
