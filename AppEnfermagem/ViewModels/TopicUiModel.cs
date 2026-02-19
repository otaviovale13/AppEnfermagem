using AppEnfermagem.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography;

namespace AppEnfermagem.ViewModels;

public partial class TopicUiModel : ObservableObject
{
    public Topic TopicData { get; private set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BackgroundColor))]
    private bool isSelected;

    public TopicUiModel(Topic topic)
    {
        TopicData = topic;
    }

    public Color BackgroundColor => IsSelected ? Color.FromArgb("#8EDDDE") : Color.FromArgb("#004AAD");

    public ImageSource IconSource
    {
        get
        {
            //if (string.IsNullOrEmpty(TopicData.IconPath)) return "heart.png";
            //return TopicData.IconPath;

            return "heart.png";
        }
    }
}