using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScreenSize : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Text labelText;

    private readonly Vector2Int[] resolutions = new Vector2Int[]
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1280, 720),
        new Vector2Int(1024, 576)
    };

    private const string PrefKeyWidth = "ScreenWidth";
    private const string PrefKeyHeight = "ScreenHeight";

    void Start()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        foreach (var res in resolutions)
        {
            options.Add($"({res.x}, {res.y})");
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        int savedWidth = PlayerPrefs.GetInt(PrefKeyWidth, Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt(PrefKeyHeight, Screen.currentResolution.height);

        int index = System.Array.FindIndex(resolutions, r => r.x == savedWidth && r.y == savedHeight);
        if (index >= 0)
        {
            resolutionDropdown.value = index;
            resolutionDropdown.RefreshShownValue();
        }

        labelText.text = "変更";

        Screen.SetResolution(savedWidth, savedHeight, FullScreenMode.Windowed);
    }

    void OnResolutionChanged(int index)
    {
        var selected = resolutions[index];
        Screen.SetResolution(selected.x, selected.y, FullScreenMode.Windowed);

        PlayerPrefs.SetInt(PrefKeyWidth, selected.x);
        PlayerPrefs.SetInt(PrefKeyHeight, selected.y);
        PlayerPrefs.Save();

        labelText.text = "変更";
    }
}
