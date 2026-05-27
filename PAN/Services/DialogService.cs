namespace PAN.Services
{
    public partial class DialogService : IDialogService
    {
        public Task DisplayAlertAsync(string title, string message, string cancel)
        {
            var page = Application.Current?.Windows?[0]?.Page;

            return page switch
            {
                Shell => Shell.Current.DisplayAlertAsync(title, message, cancel),
                not null => page.DisplayAlertAsync(title, message, cancel),
                _ => throw new InvalidOperationException("Window's Page cannot be null.")
            };
        }

        public Task DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            var page = Application.Current?.Windows?[0]?.Page;

            return page switch
            {
                Shell => Shell.Current.DisplayAlertAsync(title, message, accept, cancel),
                not null => page.DisplayAlertAsync(title, message, accept, cancel),
                _ => throw new InvalidOperationException("Window's Page cannot be null.")
            };
        }
    }
}
