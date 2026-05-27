namespace PAN.Models
{
    public partial class Event : ObservableObject
    {
        public Event() => Initialize();

        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Location { get; set; } = string.Empty;

        [ObservableProperty]
        public partial DateTime StartsAt { get; set; }

        [ObservableProperty]
        public partial DateTime EndsAt { get; set; }

        [ObservableProperty]
        public partial string Notes { get; set; } = string.Empty;

        private void Initialize()
        {
            var now = DateTime.Now;
            var today = new DateOnly(now.Year, now.Month, now.Day);

            StartsAt = (now.Hour, now.Minute) switch
            {
                (23, >= 30) => today.AddDays(1).ToDateTime(new TimeOnly()),
                (_, >= 30) => today.ToDateTime(new TimeOnly(now.Hour + 1, 0)),
                _ => today.ToDateTime(new TimeOnly(now.Hour, 30)),
            };

            EndsAt = StartsAt.AddMinutes(30); // interval
        }
    }
}
