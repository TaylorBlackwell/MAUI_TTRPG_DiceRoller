using MAUI_TTRPG_DiceRoller.Models;
using System.Text.Json;

namespace MAUI_TTRPG_DiceRoller.Data;

public class PresetRepository
{
    private readonly string _presetsFilePath;
    private List<RollPreset> _presets = [];

    public PresetRepository()
    {
        _presetsFilePath = Path.Combine(FileSystem.AppDataDirectory, "roll_presets.json");
        LoadPresets();
    }

    public List<RollPreset> GetAllPresets()
    {
        return [.. _presets.OrderBy(p => p.Name)];
    }

    public void SavePreset(RollPreset preset)
    {
        var existing = _presets.FirstOrDefault(p => p.Id == preset.Id);
        if (existing != null)
        {
            _presets.Remove(existing);
        }

        _presets.Add(preset);
        SaveToFile();
    }

    public void DeletePreset(Guid id)
    {
        var preset = _presets.FirstOrDefault(p => p.Id == id);
        if (preset != null)
        {
            _presets.Remove(preset);
            SaveToFile();
        }
    }

    private void LoadPresets()
    {
        try
        {
            if (File.Exists(_presetsFilePath))
            {
                var json = File.ReadAllText(_presetsFilePath);
                _presets = JsonSerializer.Deserialize<List<RollPreset>>(json) ?? [];
            }
        }
        catch
        {
            _presets = [];
        }
    }

    private void SaveToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_presets, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_presetsFilePath, json);
        }
        catch
        {
            // Handle save error silently for now
        }
    }
}
