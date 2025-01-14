using System.Web;

namespace Masa.Stack.Components.Layouts
{
    public partial class Breadcrumbs : MasaComponentBase
    {
        [Inject] private ILogger<Breadcrumbs> Logger { get; set; } = null!;
        
        [Parameter, EditorRequired] public List<Nav> FlattenedNavs { get; set; } = new();

        private List<Nav>? _previousFlattenedNavs;
        private string? _prevLocation;
        private Action? _updateLastBreadcrumb;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _prevLocation = NavigationManager.Uri;
            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (_previousFlattenedNavs != FlattenedNavs)
            {
                _previousFlattenedNavs = FlattenedNavs;
                UpdateItems();
            }
        }

        private void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            _prevLocation = e.Location;
            UpdateItems();

            InvokeAsync(StateHasChanged);
        }

        private void UpdateItems()
        {
            Items = new();

            var absolutePath = new Uri(NavigationManager.Uri).AbsolutePath;
            var matchedNavs = FlattenedNavs.Where(n =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(n.Url) || n.HasChildren)
                    {
                        return false;
                    }

                    if (!string.IsNullOrWhiteSpace(n.MatchPattern))
                    {
                        return Regex.IsMatch(absolutePath, n.MatchPattern, RegexOptions.IgnoreCase);
                    }

                    if (n.Exact)
                    {
                        return n.Url.Equals(absolutePath, StringComparison.OrdinalIgnoreCase);
                    }

                    return Regex.IsMatch(absolutePath, $"^/?{n.Url}($|/[^/]?)", RegexOptions.IgnoreCase);
                }
                catch (RegexParseException e)
                {
                    Logger.LogError(e, "Invalid regular expression pattern: {MatchPattern}", n.MatchPattern);
                    return false;
                }
            }).ToList();

            if (matchedNavs.Count == 0)
            {
                return;
            }

            var currentNav = matchedNavs.Count == 1
                ? matchedNavs[0]
                : matchedNavs.OrderByDescending(n => n.Url!.Split("/").Length).First();

            string? extra = null;

            if (currentNav.Url != absolutePath)
            {
                if (!string.IsNullOrWhiteSpace(currentNav.MatchPattern))
                {
                    var intersection = GetIntersection(currentNav.Url, absolutePath);
                    extra = absolutePath.Replace(intersection, "", StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    extra = absolutePath.Replace(currentNav.Url!, string.Empty, StringComparison.OrdinalIgnoreCase);
                    extra = extra.Trim('/');
                }
            }

            string GetIntersection(string left, string right)
            {
                var str1 = left.ToLower();
                var str2 = right.ToLower();
                int index = 0;

                var maxLength = Math.Max(str1.Length, str2.Length);

                for (int i = 0; i < maxLength; i++)
                {
                    if (str1[i] == str2[i])
                    {
                        continue;
                    }

                    index = i;
                    break;
                }

                return index == 0 ? string.Empty : left[..index];
            }

            if (currentNav.ParentCode != null)
            {
                var parents = GetParents(currentNav.ParentCode);

                Items.AddRange(parents.Select(n => new BreadcrumbItem()
                {
                    Exact = true,
                    Href = n.ChildUrl,
                    Text = n.Name
                }).ToList());
            }

            Items.Add(new BreadcrumbItem()
            {
                Exact = true,
                Href = currentNav.Url,
                Text = currentNav.Name
            });

            if (extra != null)
            {
                Items.Add(new BreadcrumbItem()
                {
                    Text = HttpUtility.UrlDecode(extra)
                });
            }

            if (_updateLastBreadcrumb != null)
            {
                _updateLastBreadcrumb();
                _updateLastBreadcrumb = null;
            }
        }

        internal void ReplaceLastBreadcrumb(string text)
        {
            if (_prevLocation == NavigationManager.Uri && Items.Count > 0)
            {
                Items.Last().Text = text;
                StateHasChanged();
            }
            else
            {
                _updateLastBreadcrumb = () => Items.Last().Text = text;
            }
        }

        internal void UpdateBreadcrumbs(Action<List<BreadcrumbItem>> configure)
        {
            if (_prevLocation == NavigationManager.Uri && Items.Count > 0)
            {
                configure(Items);
                StateHasChanged();
            }
            else
            {
                _updateLastBreadcrumb = () => configure(Items);
            }
        }

        private List<BreadcrumbItem> Items { get; set; } = new();

        private IList<Nav> GetParents(string parentCode)
        {
            var parents = new List<Nav>();

            var found = FlattenedNavs.FirstOrDefault(nav => nav.Code == parentCode);

            if (found is not null)
            {
                if (!string.IsNullOrEmpty(found.ParentCode))
                {
                    parents.AddRange(GetParents(found.ParentCode));
                }

                parents.Add(found);
            }

            return parents;
        }

        protected override ValueTask DisposeAsyncCore()
        {
            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
            return base.DisposeAsyncCore();
        }
    }
}