@helper ProcessToastrs()
{
    List<(DateTime Date, string SessionId, $rootnamespace$.Toastr Toastr)> toastrs = $rootnamespace$.ToastrService.ReadAndRemoveUserQueue();

    if (toastrs != null && toastrs.Count > 0)
    {
        foreach (var item in toastrs)
        {
            @ShowToastr(item.Toastr)
        }
    }
}

@helper ShowToastr($rootnamespace$.Toastr item)
{

    switch (item.Type)
    {
        case $rootnamespace$.ToastrType.Info:
            @ToastrInfo(item.Message, item.Title)
            break;
        case $rootnamespace$.ToastrType.Success:
            @ToastrSuccess(item.Message, item.Title)
            break;
        case $rootnamespace$.ToastrType.Warning:
            @ToastrWarning(item.Message, item.Title)
            break;
        case $rootnamespace$.ToastrType.Error:
            @ToastrError(item.Message, item.Title)
            break;
        default:
            @ToastrInfo(item.Message, item.Title)
            break;
    }
}

@helper ToastrInfo(string message, string title)
{
    <script>
        toastr.info('@message', '@title');
    </script>
}

@helper ToastrWarning(string message, string title)
{
    <script>
        toastr.warning('@message', '@title');
    </script>
}

@helper ToastrSuccess(string message, string title)
{
    <script>
        toastr.success('@message', '@title');
    </script>
}

@helper ToastrError(string message, string title)
{
    <script>
        toastr.error('@message', '@title');
    </script>
}
