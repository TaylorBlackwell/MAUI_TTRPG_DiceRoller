using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUI_TTRPG_DiceRoller.Data;
using MAUI_TTRPG_DiceRoller.Models;
using MAUI_TTRPG_DiceRoller.Services;
using System.Collections.ObjectModel;

namespace MAUI_TTRPG_DiceRoller.PageModels;

public partial class DiceRollerPageModel : ObservableObject
{
    private readonly DiceRollerService _diceRollerService;
    private readonly PresetRepository _presetRepository;
    private readonly DiceNotationParser _notationParser;

    [ObservableProperty]
    private DiceType _selectedDiceType = DiceType.D20;

    [ObservableProperty]
    private int _numberOfDice = 1;

    [ObservableProperty]
    private int _modifier = 0;

    [ObservableProperty]
    private int _keepHighest = 0;

    [ObservableProperty]
    private int _keepLowest = 0;

    [ObservableProperty]
    private bool _explodingDiceEnabled = false;

    [ObservableProperty]
    private bool _advantageEnabled = false;

    [ObservableProperty]
    private bool _disadvantageEnabled = false;

    [ObservableProperty]
    private string _notationInput = string.Empty;

    [ObservableProperty]
    private DiceRoll? _lastRoll;

    [ObservableProperty]
    private ObservableCollection<DiceRoll> _rollHistory = [];

    [ObservableProperty]
    private ObservableCollection<RollPreset> _savedPresets = [];

    [ObservableProperty]
    private ObservableCollection<DicePoolEntry> _dicePool = [];

    [ObservableProperty]
    private bool _showPresetDialog = false;

    [ObservableProperty]
    private string _newPresetName = string.Empty;

    [ObservableProperty]
    private bool _showAboutDialog = false;

    public List<DiceType> AvailableDiceTypes { get; } =
    [
        DiceType.D2,
        DiceType.D4,
        DiceType.D6,
        DiceType.D8,
        DiceType.D10,
        DiceType.D12,
        DiceType.D20,
        DiceType.D100,
        DiceType.D1000
    ];

    public bool IsD6Selected => SelectedDiceType == DiceType.D6;
    public bool IsD20Selected => SelectedDiceType == DiceType.D20;
    public bool HasKeepDice => KeepHighest > 0 || KeepLowest > 0;
    public bool HasDiceInPool => DicePool.Count > 0;

    public DiceRollerPageModel(DiceRollerService diceRollerService, PresetRepository presetRepository, DiceNotationParser notationParser)
    {
        _diceRollerService = diceRollerService;
        _presetRepository = presetRepository;
        _notationParser = notationParser;
        LoadPresets();
    }

    [RelayCommand]
    private void Roll()
    {
        HapticFeedback.Default.Perform(HapticFeedbackType.Click);

        var roll = _diceRollerService.RollDice(SelectedDiceType, NumberOfDice, Modifier, ExplodingDiceEnabled, AdvantageEnabled, DisadvantageEnabled, KeepHighest, KeepLowest);
        LastRoll = roll;
        RollHistory.Insert(0, roll);

        if (RollHistory.Count > 50)
        {
            RollHistory.RemoveAt(RollHistory.Count - 1);
        }
    }

    [RelayCommand]
    private void RollPool()
    {
        if (DicePool.Count == 0)
        {
            return;
        }

        HapticFeedback.Default.Perform(HapticFeedbackType.Click);

        var roll = _diceRollerService.RollDicePool(DicePool);
        LastRoll = roll;
        RollHistory.Insert(0, roll);

        if (RollHistory.Count > 50)
        {
            RollHistory.RemoveAt(RollHistory.Count - 1);
        }
    }

    [RelayCommand]
    private void AddToDicePool()
    {
        var entry = new DicePoolEntry
        {
            DiceType = SelectedDiceType,
            NumberOfDice = NumberOfDice,
            Modifier = Modifier
        };

        DicePool.Add(entry);
        OnPropertyChanged(nameof(HasDiceInPool));
    }

    [RelayCommand]
    private void RemoveFromDicePool(DicePoolEntry entry)
    {
        DicePool.Remove(entry);
        OnPropertyChanged(nameof(HasDiceInPool));
    }

    [RelayCommand]
    private void ClearDicePool()
    {
        DicePool.Clear();
        OnPropertyChanged(nameof(HasDiceInPool));
    }

    [RelayCommand]
    private void ParseAndRoll()
    {
        if (_notationParser.TryParse(NotationInput, out var diceType, out var numDice, out var mod, out var kh, out var kl))
        {
            SelectedDiceType = diceType;
            NumberOfDice = numDice;
            Modifier = mod;
            KeepHighest = kh;
            KeepLowest = kl;
            Roll();
            NotationInput = string.Empty;
        }
    }

    [RelayCommand]
    private void LoadPreset(RollPreset preset)
    {
        SelectedDiceType = preset.DiceType;
        NumberOfDice = preset.NumberOfDice;
        Modifier = preset.Modifier;
        ExplodingDiceEnabled = preset.ExplodingDice;
        AdvantageEnabled = preset.Advantage;
        DisadvantageEnabled = preset.Disadvantage;
        KeepHighest = preset.KeepHighest;
        KeepLowest = preset.KeepLowest;
    }

    [RelayCommand]
    private void SaveCurrentAsPreset()
    {
        ShowPresetDialog = true;
    }

    [RelayCommand]
    private void ConfirmSavePreset()
    {
        if (!string.IsNullOrWhiteSpace(NewPresetName))
        {
            var preset = new RollPreset
            {
                Name = NewPresetName.Trim(),
                DiceType = SelectedDiceType,
                NumberOfDice = NumberOfDice,
                Modifier = Modifier,
                ExplodingDice = ExplodingDiceEnabled,
                Advantage = AdvantageEnabled,
                Disadvantage = DisadvantageEnabled,
                KeepHighest = KeepHighest,
                KeepLowest = KeepLowest
            };

            _presetRepository.SavePreset(preset);
            LoadPresets();
            NewPresetName = string.Empty;
            ShowPresetDialog = false;
        }
    }

    [RelayCommand]
    private void CancelSavePreset()
    {
        NewPresetName = string.Empty;
        ShowPresetDialog = false;
    }

    [RelayCommand]
    private void ShowAbout()
    {
        ShowAboutDialog = true;
    }

    [RelayCommand]
    private void CloseAbout()
    {
        ShowAboutDialog = false;
    }

    [RelayCommand]
    private async Task CopyEmailAsync()
    {
        try
        {
            await Clipboard.Default.SetTextAsync("ScaleTech_Industries@proton.me");
        }
        catch (Exception)
        {
            // If copying fails, ignore silently
        }
    }

    [RelayCommand]
    private void DeletePreset(RollPreset preset)
    {
        _presetRepository.DeletePreset(preset.Id);
        LoadPresets();
    }

    [RelayCommand]
    private void ClearHistory()
    {
        RollHistory.Clear();
        LastRoll = null;
    }

    [RelayCommand]
    private void IncrementDice()
    {
        if (NumberOfDice < 99)
        {
            NumberOfDice++;
        }
    }

    [RelayCommand]
    private void DecrementDice()
    {
        if (NumberOfDice > 1)
        {
            NumberOfDice--;
        }
    }

    [RelayCommand]
    private void IncrementModifier()
    {
        if (Modifier < 999)
        {
            Modifier++;
        }
    }

    [RelayCommand]
    private void DecrementModifier()
    {
        if (Modifier > -999)
        {
            Modifier--;
        }
    }

    [RelayCommand]
    private void IncrementKeepHighest()
    {
        if (KeepHighest < NumberOfDice)
        {
            KeepHighest++;
        }
    }

    [RelayCommand]
    private void DecrementKeepHighest()
    {
        if (KeepHighest > 0)
        {
            KeepHighest--;
        }
    }

    [RelayCommand]
    private void IncrementKeepLowest()
    {
        if (KeepLowest < NumberOfDice)
        {
            KeepLowest++;
        }
    }

    [RelayCommand]
    private void DecrementKeepLowest()
    {
        if (KeepLowest > 0)
        {
            KeepLowest--;
        }
    }

    private void LoadPresets()
    {
        SavedPresets.Clear();
        foreach (var preset in _presetRepository.GetAllPresets())
        {
            SavedPresets.Add(preset);
        }
    }

    partial void OnSelectedDiceTypeChanged(DiceType value)
    {
        OnPropertyChanged(nameof(IsD6Selected));
        OnPropertyChanged(nameof(IsD20Selected));
        
        if (value != DiceType.D6)
        {
            ExplodingDiceEnabled = false;
        }
        
        if (value != DiceType.D20 || NumberOfDice != 1)
        {
            AdvantageEnabled = false;
            DisadvantageEnabled = false;
        }
    }

    partial void OnKeepHighestChanged(int value)
    {
        if (value > 0 && KeepLowest > 0)
        {
            KeepLowest = 0;
        }
        OnPropertyChanged(nameof(HasKeepDice));
    }

    partial void OnKeepLowestChanged(int value)
    {
        if (value > 0 && KeepHighest > 0)
        {
            KeepHighest = 0;
        }
        OnPropertyChanged(nameof(HasKeepDice));
    }

    partial void OnAdvantageEnabledChanged(bool value)
    {
        if (value && DisadvantageEnabled)
        {
            DisadvantageEnabled = false;
        }
    }

    partial void OnDisadvantageEnabledChanged(bool value)
    {
        if (value && AdvantageEnabled)
        {
            AdvantageEnabled = false;
        }
    }
}
