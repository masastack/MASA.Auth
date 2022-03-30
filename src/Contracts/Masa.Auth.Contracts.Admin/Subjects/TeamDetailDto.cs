using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamDetailDto
{
    public TeamBaseInfoDto TeamBaseInfo { get; set; } = new();

    public TeamPersonnelDto TeamAdmin { get; set; } = new();

    public TeamPersonnelDto TeamMember { get; set; } = new();
}

public class TeamBaseInfoDto : INotifyPropertyChanged
{
    public Guid Id { get; set; }

    string _name = string.Empty;

    public string Name
    {
        get => _name;
        set
        {
            if (value.FirstOrDefault() != default(char))
            {
                Avatar.Name = value.FirstOrDefault().ToString();
            }
            SetProperty(ref _name, value);
        }
    }

    public AvatarValueDto Avatar { get; set; } = new AvatarValueDto();

    public string Description { get; set; } = string.Empty;

    public int Type { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
    {
        if (Equals(storage, value))
        {
            return false;
        }
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class TeamPersonnelDto
{
    public List<Guid> Staffs { get; set; } = new();

    public List<Guid> Permissions { get; set; } = new();

    public List<Guid> Roles { get; set; } = new();
}
