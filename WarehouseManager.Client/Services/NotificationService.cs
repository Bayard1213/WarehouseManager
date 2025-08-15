namespace WarehouseManager.Client.Services
{
    public class NotificationService
    {
        public event Action? OnChange;
        
        public string Message { get; private set; } = string.Empty;
        public string CssClass { get; private set; } = string.Empty;
        public bool IsVisible { get; set; }
        public bool IsMouseOver { get; set; }
        private CancellationTokenSource? _cts;

        public void Show(string message, bool isSuccess = true, int timeoutMs = 3000)
        {
            Message = message;
            CssClass = isSuccess ? "bg-success text-white" : "bg-danger text-white";
            IsVisible = true;
            NotifyStateChanged();

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            _ = AutoHide(timeoutMs, _cts.Token);

        }

        private async Task AutoHide(int timeoutMs, CancellationToken token)
        {
            int elapsed = 0;
            const int step = 200; 

            while (elapsed < timeoutMs && !token.IsCancellationRequested)
            {
                await Task.Delay(step, token);
                if (!IsMouseOver) elapsed += step;
            }

            if (!token.IsCancellationRequested)
            {
                IsVisible = false;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged()
            => OnChange?.Invoke();
        public void Hide()
        {
            _cts?.Cancel();
            IsVisible = false;
            NotifyStateChanged();
        }
    }
}
