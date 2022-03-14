//namespace Masa.Framework.Admin.RCL.RBAC;

//public class PlatformPage : ComponentPageBase
//{
//    public List<ObjectItemResponse> Datas { get; set; } = new();

//    public IEnumerable<ObjectItemResponse> SelectDatas { get; set; } = new List<ObjectItemResponse>();

//    public ObjectItemResponse CurrentData { get; set; } = new();

//    private AuthenticationCaller AuthenticationCaller { get; set; }

//    public string? _search;
//    public string? Search
//    {
//        get { return _search; }
//        set
//        {
//            _search = value;
//            QueryPageDatasAsync().ContinueWith(_ => Reload?.Invoke());
//        }
//    }

//    public int _pageIndex = 1;
//    public int PageIndex
//    {
//        get { return _pageIndex; }
//        set
//        {
//            _pageIndex = value;
//            QueryPageDatasAsync().ContinueWith(_ => Reload?.Invoke());
//        }
//    }

//    public int _pageSize = 10;
//    public int PageSize
//    {
//        get { return _pageSize; }
//        set
//        {
//            _pageSize = value;
//            QueryPageDatasAsync().ContinueWith(_ => Reload?.Invoke());
//        }
//    }

//    public int PageCount { get; set; }

//    public long TotalCount { get; set; }

//    public List<int> PageSizes = new() { 10, 25, 50, 100 };

//    public List<DataTableHeader<ObjectItemResponse>> Headers { get; set; }

//    public bool IsOpenObjectForm { get; set; }

//    public ObjectType? _objectTypeEnum;
//    public ObjectType? ObjectTypeEnum
//    {
//        get { return _objectTypeEnum; }
//        set
//        {
//            _objectTypeEnum = value;
//            QueryPageDatasAsync().ContinueWith(_ => Reload?.Invoke());
//        }
//    }

//    public List<(ObjectType,string)> ObjectTypeSelect => new List<(ObjectType, string)>
//    {
//        (ObjectType.Menu,I18n.T( ObjectType.Menu.ToString())),
//        (ObjectType.Operate,I18n.T( ObjectType.Operate.ToString()))
//    };

//    public bool IsAdd => CurrentData.Id == Guid.Empty;

//    public PlatformPage(AuthenticationCaller authenticationCaller, GlobalConfig globalConfig, I18n i18n) : base(globalConfig, i18n)
//    {
//        AuthenticationCaller = authenticationCaller;
//        Headers = new()
//        {
//            new() { Text = i18n.T("Object.Name"), Value = nameof(ObjectItemResponse.Name) },
//            new() { Text = i18n.T("Code"), Value = nameof(ObjectItemResponse.Code), Sortable = false },
//            new() { Text = i18n.T("State"), Value = nameof(ObjectItemResponse.State) },
//            new() { Text = i18n.T("Type"), Value = nameof(ObjectItemResponse.ObjectType), Sortable = false },
//            new() { Text = i18n.T("Action"), Value = "Action", Sortable = false }
//        };
//    }

//    public async Task QueryPageDatasAsync()
//    {
//        Loading = true;
//        var result = await AuthenticationCaller.GetObjectItemsAsync(PageIndex, PageSize, ObjectTypeEnum is null ? -1 : Convert.ToInt32(ObjectTypeEnum), Search);
//        if (result.Success)
//        {
//            var pageData = result.Data!;
//            PageCount = (int)pageData.TotalPages;
//            TotalCount = pageData.Count;
//            Datas = pageData.Items.ToList();
//        }
//        Loading = false;
//    }

//    public async Task<List<ObjectItemResponse>> QueryAllAsync()
//    {
//        var result = await AuthenticationCaller.GetAllAsync();
//        if (result.Success)
//        {
//            return result.Data ?? new();
//        }
//        else
//        {
//            OpenErrorMessage(result.Message);
//            return new();
//        }
//    }

//    public void OpenObjectForm(ObjectItemResponse? item = null)
//    {
//        CurrentData = item?.Copy() ?? new();
//        IsOpenObjectForm = true;
//    }

//    public async Task<bool> AddOrUpdateAsync()
//    {
//        Loading = true;
//        if(await CheckCodeAsync())
//        {
//            OpenWarningDialog(I18n.T("Code already exists, cannot be added repeatedly"));
//            Loading = false;
//            return false;
//        }
//        var result = default(ApiResultResponseBase);
//        if (IsAdd)
//        {
//            var request = new AddObjectRequest(CurrentData.Code, CurrentData.Name,CurrentData.State, CurrentData.ObjectType);
//            result = await AuthenticationCaller.AddObjectAsync(request);

//            await CheckApiResult(result, I18n.T("Added object successfully"), result.Message);
//        }
//        else
//        {
//            var request = new EditObjectRequest(CurrentData.Id, CurrentData.Name, CurrentData.State);
//            result = await AuthenticationCaller.EditObjectAsync(request);

//            await CheckApiResult(result, I18n.T("Edit object successfully"), result.Message);
//        }
//        Loading = false;

//        return result.Success;

//        async Task<bool> CheckCodeAsync()
//        {
//            var result = await AuthenticationCaller.ContainsObjectAsync(CurrentData.Id,CurrentData.Code);
//            return result.Success && result.Data;
//        }
//    }

//    public void OpenDeleteObjectDialog(ObjectItemResponse item)
//    {
//        CurrentData = item.Copy();
//        OpenDeleteConfirmDialog(DeleteAsync);
//    }

//    public async Task DeleteAsync(bool confirm)
//    {
//        if (confirm)
//        {
//            Loading = true;
//            var request = new DeleteObjectRequest { ObjectId = CurrentData.Id };
//            var result = await AuthenticationCaller.DeleteObjectAsync(request);
//            await CheckApiResult(result, I18n.T("Delete object successfully"), result.Message);
//            Loading = false;
//        }
//    }

//    public void OpenBatchDeleteObjectDialog()
//    {
//        OpenDeleteConfirmDialog(BatchDeleteAsync);
//    }

//    public async Task BatchDeleteAsync(bool confirm)
//    {
//        if (confirm)
//        {
//            Loading = true;
//            var request = new BatchDeleteObjectRequest(SelectDatas.Select(o => o.Id).ToList());
//            var result = await AuthenticationCaller.BatchDeleteObjectAsync(request);
//            await CheckApiResult(result, I18n.T("Delete object successfully"), result.Message);
//            SelectDatas = new List<ObjectItemResponse>();
//            Loading = false;
//        }
//    }

//    async Task CheckApiResult(ApiResultResponseBase result, string successMessage, string errorMessage)
//    {
//        if (result.Success is false) OpenErrorDialog(errorMessage);
//        else
//        {
//            OpenSuccessMessage(successMessage);
//            await QueryPageDatasAsync();
//        }
//    }
//}
