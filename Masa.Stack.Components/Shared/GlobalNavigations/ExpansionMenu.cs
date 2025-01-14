namespace Masa.Stack.Components.Shared.GlobalNavigations;

public enum ExpansionMenuType
{
    Root = 0,
    Category = 1,
    App = 2,
    Nav = 3,
    Element = 4,
    Api = 5,
}

public enum ExpansionMenuState
{
    Normal = 0,
    Selected = 1,
    Indeterminate = 2,
    Impersonal = 3,
    Favorite = 4,
    Hidden = 5,
}

public enum ExpansionMenuSituation
{
    Preview = 1,
    Favorite = 2,
    Authorization = 3,
}

public enum ExpansionMenuStatePopDirection
{
    UnSet = 0,
    Parent = 1,
    Children = 2,
}

public record ExpansionMenuMetaData
{
    public ExpansionMenuMetaData()
    {
        TypeDeepStartDic = new Dictionary<ExpansionMenuType, int>() {
            {ExpansionMenuType.Root, -1 },
            {ExpansionMenuType.Category, -1 },
            {ExpansionMenuType.App, -1 },
            {ExpansionMenuType.Nav, -1 },
            {ExpansionMenuType.Element, -1 },
            {ExpansionMenuType.Api, -1}
        };
    }

    public ExpansionMenuMetaData(ExpansionMenuSituation situation) : this()
    {
        Situation = situation;
    }

    public IDictionary<ExpansionMenuType, int> TypeDeepStartDic { get; set; }

    public ExpansionMenuSituation Situation { get; set; } = ExpansionMenuSituation.Preview;
}

public class ExpansionMenu : ICloneable
{
    private const string VIEW_ELEMENT_NAME = "View";
    private readonly List<ExpansionMenu> _children;
    private ExpansionMenuState _state;

    public ExpansionMenu(string id, string name, ExpansionMenuType type, ExpansionMenuState state, ExpansionMenuMetaData? metaData = null, bool impersonal = false, bool disabled = false, ExpansionMenu? parent = null, List<ExpansionMenu>? children = null, Func<ExpansionMenu, Task>? stateChangedAsync = null)
    {
        Id = id;
        Name = name;
        Type = type;
        State = state;
        MetaData = metaData ?? new ExpansionMenuMetaData();
        Impersonal = impersonal;
        Deep = parent != null ? parent.Deep + 1 : 0;
        Parent = parent;
        _children = children ?? new List<ExpansionMenu>();
        Children = _children.AsReadOnly();
        StateChangedAsync = stateChangedAsync;
        Data = new Dictionary<string, string>();
        Disabled = disabled;

        SetTypeDeepStart();
    }

    private string GetViewElementId() => $"{Id}-{VIEW_ELEMENT_NAME}";

    public string Id { get; private set; }

    public string Name { get; private set; }

    public ExpansionMenuType Type { get; private set; }

    public ExpansionMenuState State
    {
        get
        {
            //if (_state != ExpansionMenuState.Selected)
            //{
            //    return _state;
            //}
            //else if (_children.Any(c => c.State != ExpansionMenuState.Selected))
            //{
            //    return ExpansionMenuState.Indeterminate;
            //}
            //else
            //{
            //    return _state;
            //}
            return _state;
        }
        private set
        {
            _state = value;
        }
    }

    public ExpansionMenuMetaData MetaData { get; private set; }

    public IDictionary<string, string> Data { get; set; }

    public bool Disabled { get; set; }

    public bool Impersonal { get; private set; }

    public int Deep { get; private set; }

    public bool Hidden { get; private set; } = false;

    public ExpansionMenu? Parent { get; private set; }

    public IReadOnlyList<ExpansionMenu> Children { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public Func<ExpansionMenu, Task>? StateChangedAsync { get; set; }

    public ExpansionMenu AddChild(ExpansionMenu child)
    {
        if (child.Type == ExpansionMenuType.Element && !(Children.Any(c => c.Name == VIEW_ELEMENT_NAME)))
        {
            _children.Add(new ExpansionMenu(GetViewElementId(), VIEW_ELEMENT_NAME, ExpansionMenuType.Element, State, MetaData, Impersonal, Disabled, this));
        }

        _children.Add(child);
        Children = _children.AsReadOnly();
        return this;
    }

    public void SetSituation(ExpansionMenuSituation situation)
    {
        MetaData.Situation = situation;
        foreach (var child in Children)
        {
            child.SetSituation(situation);
        }
    }

    public void Show()
    {
        foreach (var child in Children)
        {
            child.Hidden = false;
            child.Show();
        }
    }

    public void SetHiddenBySearch(string? search, DynamicTranslateProvider translateProvider)
    {
        if (Type == ExpansionMenuType.Nav)
        {
            Hidden = !Filter(translateProvider, search);
            // Show all children when parent is found
            if (!Hidden)
            {
                Show();
                return;
            }
        }

        foreach (var child in Children)
        {
            child.SetHiddenBySearch(search, translateProvider);
        }

        Hidden = Children.All(child => child.Hidden);
    }

    public void SetHiddenByPreview(bool preview)
    {
        if (!preview)
        {
            Hidden = preview;
            foreach (var child in Children)
            {
                child.SetHiddenByPreview(preview);
            }
            return;
        }
        if (Type == ExpansionMenuType.Element)
        {
            Hidden = true;
            return;
        }
        if (Type == ExpansionMenuType.Nav && State != ExpansionMenuState.Selected && State != ExpansionMenuState.Indeterminate)
        {
            Hidden = true;
        }

        if (Children.Count > 0)
        {
            foreach (var child in Children)
            {
                child.SetHiddenByPreview(preview);
            }
        }

        if (Type <= ExpansionMenuType.App)
        {
            Hidden = Children.Count == 0 || Children.All(child => child.Hidden);
        }
    }

    private bool Filter(DynamicTranslateProvider translateProvider, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return true;
        }
        return translateProvider.DT(Name).Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    public ExpansionMenu AddData(string key, string value)
    {
        if (Data.ContainsKey(key))
        {
            Data[key] = value;
        }
        else
        {
            Data.Add(key, value);
        }

        return this;
    }

    public string GetData(string key)
    {
        if (!Data.TryGetValue(key, out string? data))
        {
            return string.Empty;
        }

        return data;
    }

    public int GetNavDeep()
    {
        if (!MetaData.TypeDeepStartDic.TryGetValue(ExpansionMenuType.Nav, out int start))
        {
            return Deep;
        }

        return Deep - start;
    }

    public List<ExpansionMenu> GetMenusByStates(params ExpansionMenuState[] states)
    {
        return GetMenusByStateInternal(this, new List<ExpansionMenu>());

        List<ExpansionMenu> GetMenusByStateInternal(ExpansionMenu menu, List<ExpansionMenu> stateMenus)
        {
            if (Type == ExpansionMenuType.Element && Id == GetViewElementId())
            {
                return stateMenus;
            }

            if (states.Contains(menu.State))
            {
                stateMenus.Add(menu);
            }

            foreach (var stateMenu in menu.Children)
            {
                stateMenus = GetMenusByStateInternal(stateMenu, stateMenus);
            }

            return stateMenus;
        }
    }

    public async Task ChangeStateAsync()
    {
        var newState = CalcState();
        await ChangeStateWithViewElementAsync(newState);
        await PopStateAsync(newState);
    }

    private async Task ChangeStateWithViewElementAsync(ExpansionMenuState newState)
    {
        if (Type != ExpansionMenuType.Element || MetaData.Situation != ExpansionMenuSituation.Authorization || Parent == null)
        {
            return;
        }

        if (newState != ExpansionMenuState.Selected && newState != ExpansionMenuState.Normal && newState != ExpansionMenuState.Impersonal)
        {
            return;
        }

        var viewElementId = Parent.GetViewElementId();
        if (Id == viewElementId && (newState == ExpansionMenuState.Normal || newState == ExpansionMenuState.Impersonal))
        {
            foreach (var child in Parent.Children)
            {
                await child.PopStateAsync(newState, ExpansionMenuStatePopDirection.Children);
            }
        }

        if (Id != viewElementId && newState == ExpansionMenuState.Selected)
        {
            var viewElementMenu = Parent.Children.FirstOrDefault(child => child.Id == viewElementId);
            if (viewElementMenu != null)
            {
                await viewElementMenu.PopStateAsync(newState, ExpansionMenuStatePopDirection.Children);
            }
        }
    }

    private ExpansionMenuState CalcState()
    {
        var newState = State;
        switch (MetaData.Situation)
        {
            case ExpansionMenuSituation.Preview:
                newState = CalcNewStateForPreview();
                break;
            case ExpansionMenuSituation.Favorite:
                newState = CalcNewStateForFavorite();
                break;
            case ExpansionMenuSituation.Authorization:
                newState = CalcNewStateForAuthorization();
                break;
        }

        return newState;
    }

    private ExpansionMenuState CalcNewStateForPreview()
    {
        throw new UserFriendlyException("Preview situation cannot change state");
    }

    private ExpansionMenuState CalcNewStateForFavorite()
    {
        return State == ExpansionMenuState.Normal ? ExpansionMenuState.Favorite : ExpansionMenuState.Normal;
    }

    private ExpansionMenuState CalcNewStateForAuthorization()
    {
        if (State == ExpansionMenuState.Normal || State == ExpansionMenuState.Impersonal || State == ExpansionMenuState.Indeterminate)
        {
            return ExpansionMenuState.Selected;
        }
        if (State == ExpansionMenuState.Selected)
        {
            return Impersonal ? ExpansionMenuState.Impersonal : ExpansionMenuState.Normal;
        }
        return State;
    }

    private async Task PopStateAsync(ExpansionMenuState newState, ExpansionMenuStatePopDirection popDirection = ExpansionMenuStatePopDirection.UnSet)
    {
        var oldState = State;
        State = Impersonal && newState == ExpansionMenuState.Normal ? ExpansionMenuState.Impersonal : newState;

        if (oldState != newState && StateChangedAsync != null && Id != GetViewElementId())
        {
            await StateChangedAsync.Invoke(this);
        }

        if (popDirection == ExpansionMenuStatePopDirection.UnSet || popDirection == ExpansionMenuStatePopDirection.Children)
        {
            await PopChildrenStateAsync();
        }
        if (popDirection == ExpansionMenuStatePopDirection.UnSet || popDirection == ExpansionMenuStatePopDirection.Parent)
        {
            await PopParentStateAsync();
        }
    }

    private Task PopParentStateAsync()
    {
        if (Parent == null)
        {
            return Task.CompletedTask;
        }

        switch (MetaData.Situation)
        {
            case ExpansionMenuSituation.Authorization:
                return PopParentStateWithAuthorizationAsync(Parent);
            case ExpansionMenuSituation.Favorite:
                return PopParentStateWithFavoriteAsync(Parent);
            default:
                return Task.CompletedTask;
        }
    }

    private async Task PopChildrenStateAsync()
    {
        if (Children.Count == 0 || State == ExpansionMenuState.Indeterminate)
        {
            return;
        }

        foreach (var children in Children)
        {
            await children.PopStateAsync(State, ExpansionMenuStatePopDirection.Children);
        }
    }

    private Task PopParentStateWithAuthorizationAsync(ExpansionMenu parent)
    {
        var childrenAllSelected = parent.Children.All(children => children.State == ExpansionMenuState.Selected);
        if (childrenAllSelected)
        {
            return parent.PopStateAsync(ExpansionMenuState.Selected, ExpansionMenuStatePopDirection.Parent);
        }

        var childrenPartialSelected = parent.Children.Any(
            children => children.State == ExpansionMenuState.Selected ||
                        children.State == ExpansionMenuState.Indeterminate);
        if (childrenPartialSelected)
        {
            return parent.PopStateAsync(ExpansionMenuState.Indeterminate, ExpansionMenuStatePopDirection.Parent);
        }

        var childrenAllImpersonal = parent.Children.Any(child => child.State == ExpansionMenuState.Impersonal);
        if (childrenAllImpersonal)
        {
            return parent.PopStateAsync(ExpansionMenuState.Impersonal, ExpansionMenuStatePopDirection.Parent);
        }

        var childrenAllNormal = parent.Children.Any(child => child.State == ExpansionMenuState.Normal);
        if (childrenAllNormal)
        {
            return parent.PopStateAsync(ExpansionMenuState.Normal, ExpansionMenuStatePopDirection.Parent);
        }

        return Task.CompletedTask;
    }

    private Task PopParentStateWithFavoriteAsync(ExpansionMenu parent)
    {
        var childrenPartialFavorite = parent.Children.Any(children => children.State == ExpansionMenuState.Normal) &&
                                      parent.Children.Any(children => children.State == ExpansionMenuState.Favorite);
        if (childrenPartialFavorite)
        {
            return parent.PopStateAsync(ExpansionMenuState.Normal, ExpansionMenuStatePopDirection.Parent);
        }

        var childrenAllFavorite = parent.Children.All(children => children.State == ExpansionMenuState.Favorite);
        if (childrenAllFavorite)
        {
            return parent.PopStateAsync(ExpansionMenuState.Favorite, ExpansionMenuStatePopDirection.Parent);
        }

        return Task.CompletedTask;
    }

    private void SetTypeDeepStart()
    {
        if (MetaData.TypeDeepStartDic.ContainsKey(Type) && Parent != null && Parent.Type != Type)
        {
            MetaData.TypeDeepStartDic[Type] = Deep;
        }
    }

    public static ExpansionMenu CreateRootMenu(ExpansionMenuSituation situation)
    {
        var metaData = new ExpansionMenuMetaData() { Situation = situation };
        return new ExpansionMenu("root", "root", ExpansionMenuType.Root, ExpansionMenuState.Normal, metaData);
    }

    public object Clone()
    {
        return new ExpansionMenu(Id, Name, Type, State, MetaData with { }, Impersonal, Disabled, Parent, Children.Select(c => (ExpansionMenu)c.Clone()).ToList(), StateChangedAsync);
    }
}
