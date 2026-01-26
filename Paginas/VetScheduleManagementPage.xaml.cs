using FrontEndHealthPets.Helpers;
using FrontEndHealthPets.Services;

namespace FrontEndHealthPets.Paginas
{
    public partial class VetScheduleManagementPage : ContentPage
    {
        private readonly IVeterinarianService _veterinarianService;
        private int _vetId;
        private int _selectedDayIndex = 1; // Default to Monday (0=Sunday, 1=Monday)
        
        // Store hour selections per day: Dictionary<dayIndex, HashSet<hour>>
        private Dictionary<int, HashSet<int>> _schedulesByDay = new();
        
        // Day buttons for selection
        private Dictionary<int, Frame> _dayButtons = new();
        
        // Hour blocks for toggling
        private Dictionary<int, Frame> _hourBlocks = new();

        private readonly string[] _dayNames = { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
        private readonly string[] _fullDayNames = { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };

        public VetScheduleManagementPage() : this(Settings.VetId)
        {
        }

        public VetScheduleManagementPage(int vetId)
        {
            InitializeComponent();
            _veterinarianService = ServiceHelper.GetService<IVeterinarianService>() ?? new VeterinarianService();
            
            // Use passed ID or fallback to Settings
            _vetId = vetId > 0 ? vetId : Settings.VetId;
            
            // Initialize schedules dictionary for all days
            for (int i = 0; i < 7; i++)
            {
                _schedulesByDay[i] = new HashSet<int>();
            }
            
            BuildDaySelector();
            BuildHourGrids();
            SelectDay(1); // Select Monday by default
            
            if (_vetId > 0)
            {
                LoadSchedules();
            }
        }

        public void Initialize(int vetId)
        {
            _vetId = vetId;
            LoadSchedules();
        }

        private void BuildDaySelector()
        {
            DaysContainer.Children.Clear();
            _dayButtons.Clear();

            for (int i = 0; i < 7; i++)
            {
                int dayIndex = i;
                var frame = new Frame
                {
                    Padding = new Thickness(16, 12),
                    BackgroundColor = Colors.White,
                    BorderColor = Color.FromArgb("#DDD"),
                    CornerRadius = 10,
                    HasShadow = false,
                    Content = new Label
                    {
                        Text = _dayNames[i],
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Color.FromArgb("#666"),
                        HorizontalOptions = LayoutOptions.Center
                    }
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => SelectDay(dayIndex);
                frame.GestureRecognizers.Add(tapGesture);

                _dayButtons[i] = frame;
                DaysContainer.Children.Add(frame);
            }
        }

        private void BuildHourGrids()
        {
            _hourBlocks.Clear();

            // Morning hours: 6 AM to 11 AM (6 hours)
            int row = 0;
            int col = 0;
            for (int hour = 6; hour < 12; hour++)
            {
                var block = CreateHourBlock(hour);
                Grid.SetRow(block, row);
                Grid.SetColumn(block, col);
                MorningHoursGrid.Children.Add(block);
                _hourBlocks[hour] = block;

                col++;
                if (col >= 3) { col = 0; row++; }
            }

            // Add rows to morning grid
            MorningHoursGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MorningHoursGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Afternoon hours: 12 PM to 5 PM (6 hours)
            row = 0;
            col = 0;
            for (int hour = 12; hour < 18; hour++)
            {
                var block = CreateHourBlock(hour);
                Grid.SetRow(block, row);
                Grid.SetColumn(block, col);
                AfternoonHoursGrid.Children.Add(block);
                _hourBlocks[hour] = block;

                col++;
                if (col >= 3) { col = 0; row++; }
            }

            // Add rows to afternoon grid
            AfternoonHoursGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AfternoonHoursGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Evening hours: 6 PM to 11 PM (6 hours - until midnight)
            row = 0;
            col = 0;
            for (int hour = 18; hour < 24; hour++)
            {
                var block = CreateHourBlock(hour);
                Grid.SetRow(block, row);
                Grid.SetColumn(block, col);
                EveningHoursGrid.Children.Add(block);
                _hourBlocks[hour] = block;

                col++;
                if (col >= 3) { col = 0; row++; }
            }

            // Add rows to evening grid
            EveningHoursGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            EveningHoursGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        private Frame CreateHourBlock(int hour)
        {
            string timeLabel = FormatHour(hour);
            string endLabel = FormatHour(hour + 1);

            var frame = new Frame
            {
                Padding = new Thickness(10, 15),
                BackgroundColor = Colors.White,
                BorderColor = Color.FromArgb("#DDD"),
                CornerRadius = 10,
                HasShadow = false,
                Content = new VerticalStackLayout
                {
                    Spacing = 2,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = timeLabel,
                            FontSize = 14,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = Color.FromArgb("#333"),
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            Text = $"a {endLabel}",
                            FontSize = 11,
                            TextColor = Color.FromArgb("#888"),
                            HorizontalOptions = LayoutOptions.Center
                        }
                    }
                }
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => ToggleHour(hour);
            frame.GestureRecognizers.Add(tapGesture);

            return frame;
        }

        private void SelectDay(int dayIndex)
        {
            _selectedDayIndex = dayIndex;

            // Update day button styles
            foreach (var kvp in _dayButtons)
            {
                bool isSelected = kvp.Key == dayIndex;
                kvp.Value.BackgroundColor = isSelected ? Color.FromArgb("#6B4EE6") : Colors.White;
                kvp.Value.BorderColor = isSelected ? Color.FromArgb("#6B4EE6") : Color.FromArgb("#DDD");
                
                if (kvp.Value.Content is Label label)
                {
                    label.TextColor = isSelected ? Colors.White : Color.FromArgb("#666");
                }
            }

            // Update hour blocks to reflect current day's schedule
            UpdateHourBlocksDisplay();
        }

        private void ToggleHour(int hour)
        {
            var daySchedule = _schedulesByDay[_selectedDayIndex];
            
            if (daySchedule.Contains(hour))
            {
                daySchedule.Remove(hour);
            }
            else
            {
                daySchedule.Add(hour);
            }

            UpdateHourBlockDisplay(hour);
        }

        private void UpdateHourBlocksDisplay()
        {
            foreach (var kvp in _hourBlocks)
            {
                UpdateHourBlockDisplay(kvp.Key);
            }
        }

        private void UpdateHourBlockDisplay(int hour)
        {
            if (!_hourBlocks.TryGetValue(hour, out var frame)) return;

            var daySchedule = _schedulesByDay[_selectedDayIndex];
            bool isActive = daySchedule.Contains(hour);

            frame.BackgroundColor = isActive ? Color.FromArgb("#E8F5E9") : Colors.White;
            frame.BorderColor = isActive ? Color.FromArgb("#4CAF50") : Color.FromArgb("#DDD");

            if (frame.Content is VerticalStackLayout stack && stack.Children.Count > 0)
            {
                if (stack.Children[0] is Label label)
                {
                    label.TextColor = isActive ? Color.FromArgb("#2E7D32") : Color.FromArgb("#333");
                }
            }
        }

        private async void LoadSchedules()
        {
            if (_vetId <= 0) return;

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var schedules = await _veterinarianService.GetSchedulesAsync(_vetId);
                
                // Clear existing
                foreach (var day in _schedulesByDay.Values)
                {
                    day.Clear();
                }

                // Populate from API
                foreach (var schedule in schedules)
                {
                    int startHour = schedule.StartTime.Hours;
                    int endHour = schedule.EndTime.Hours;
                    
                    for (int h = startHour; h < endHour; h++)
                    {
                        _schedulesByDay[schedule.DayOfWeek].Add(h);
                    }
                }

                UpdateHourBlocksDisplay();
                UpdateScheduleSummary();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadSchedules] Error: {ex.Message}");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async void OnSaveSchedule(object sender, EventArgs e)
        {
            if (_vetId <= 0)
            {
                await DisplayAlert("Error", "ID de veterinario no válido", "OK");
                return;
            }

            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                SaveButton.IsEnabled = false;

                // For each day, create schedules from the selected hours
                foreach (var kvp in _schedulesByDay)
                {
                    int dayOfWeek = kvp.Key;
                    var hours = kvp.Value.OrderBy(h => h).ToList();

                    if (hours.Count == 0) continue;

                    // Group consecutive hours into time ranges
                    var ranges = GroupConsecutiveHours(hours);

                    foreach (var (startHour, endHour) in ranges)
                    {
                        var startTime = new TimeSpan(startHour, 0, 0);
                        var endTime = new TimeSpan(endHour + 1, 0, 0); // +1 because each block is 1 hour

                        await _veterinarianService.CreateScheduleAsync(
                            _vetId, dayOfWeek, startTime, endTime, 60); // 60 min slots
                    }
                }

                await DisplayAlert("Éxito", "Horarios guardados correctamente", "OK");
                UpdateScheduleSummary();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron guardar los horarios: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                SaveButton.IsEnabled = true;
            }
        }

        private List<(int start, int end)> GroupConsecutiveHours(List<int> hours)
        {
            var ranges = new List<(int, int)>();
            if (hours.Count == 0) return ranges;

            int start = hours[0];
            int prev = hours[0];

            for (int i = 1; i < hours.Count; i++)
            {
                if (hours[i] != prev + 1)
                {
                    ranges.Add((start, prev));
                    start = hours[i];
                }
                prev = hours[i];
            }
            ranges.Add((start, prev));

            return ranges;
        }

        private void UpdateScheduleSummary()
        {
            var summaryLines = new List<string>();

            for (int day = 0; day < 7; day++)
            {
                var hours = _schedulesByDay[day].OrderBy(h => h).ToList();
                if (hours.Count == 0) continue;

                var ranges = GroupConsecutiveHours(hours);
                var rangeStrings = ranges.Select(r => 
                    $"{FormatHour(r.start)} - {FormatHour(r.end + 1)}");
                
                summaryLines.Add($"• {_fullDayNames[day]}: {string.Join(", ", rangeStrings)}");
            }

            if (summaryLines.Count > 0)
            {
                ScheduleSummaryLabel.Text = string.Join("\n", summaryLines);
                ScheduleSummaryFrame.IsVisible = true;
            }
            else
            {
                ScheduleSummaryFrame.IsVisible = false;
            }
        }

        private string FormatHour(int hour)
        {
            if (hour == 0 || hour == 24) return "12:00 AM";
            if (hour < 12) return $"{hour}:00 AM";
            if (hour == 12) return "12:00 PM";
            return $"{hour - 12}:00 PM";
        }
    }
}
