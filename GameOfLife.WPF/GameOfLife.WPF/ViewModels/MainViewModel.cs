using Microsoft.Win32;
using System.IO;
using System.Text;
using GameOfLife.WPF.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GameOfLife.WPF.Commands;
using Point = System.Drawing.Point;

namespace GameOfLife.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // Commands
        public ICommand StepCommand { get; }
        public ICommand RandomizeCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ApplySizeCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand ToggleCellCommand { get; }
        public ICommand ClearPatternSelectionCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        private GameBoard _gameBoard;
        private GameState _gameState;

        public double BoardWidth => _gameBoard.Width;
        public double BoardHeight => _gameBoard.Height;
        public ObservableCollection<Point> CellsToDisplay { get; set; }
        public List<Pattern> AvailablePatterns { get; set; }

        private CancellationTokenSource _cts;

        // UI properties
        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        private string _rulesText;
        public string RulesText
        {
            get => _rulesText;
            set
            {
                SetProperty(ref _rulesText, value);
                _gameBoard.Rules = GameRule.Parse(_rulesText);
            }
        }

        private int _generationCount;
        public int GenerationCount
        {
            get => _generationCount;
            set => SetProperty(ref _generationCount, value);
        }

        private int _cellsBorn;
        public int CellsBorn
        {
            get => _cellsBorn;
            set => SetProperty(ref _cellsBorn, value);
        }

        private int _cellsDied;
        public int CellsDied
        {
            get => _cellsDied;
            set => SetProperty(ref _cellsDied, value);
        }

        private double _currentZoom;
        public double CurrentZoom
        {
            get => _currentZoom;
            set => SetProperty(ref _currentZoom, value);
        }

        private int _newBoardWidth;
        public int NewBoardWidth
        {
            get => _newBoardWidth;
            set => SetProperty(ref _newBoardWidth, value);
        }

        private int _newBoardHeight;
        public int NewBoardHeight
        {
            get => _newBoardHeight;
            set => SetProperty(ref _newBoardHeight, value);
        }

        private int _simulationSpeed;
        public int SimulationSpeed
        {
            get => _simulationSpeed;
            set => SetProperty(ref _simulationSpeed, value);
        }

        private Pattern _selectedPattern;
        public Pattern SelectedPattern
        {
            get => _selectedPattern;
            set => SetProperty(ref _selectedPattern, value);
        }

        private bool _useCircles;
        public bool UseCircles
        {
            get => _useCircles;
            set => SetProperty(ref _useCircles, value);
        }

        public MainViewModel()
        {
            _gameBoard = new GameBoard();
            _gameState = new GameState();
            CellsToDisplay = new ObservableCollection<Point>();
            AvailablePatterns = PatternLibrary.GetPatterns();

            RulesText = "B3/S23";
            _isRunning = false;
            _simulationSpeed = 100;
            _currentZoom = 1.0;
            UseCircles = false;

            StepCommand = new RelayCommand(ExecuteStep, () => !_isRunning);
            RandomizeCommand = new RelayCommand(ExecuteRandomize, () => !_isRunning);
            ClearCommand = new RelayCommand(ExecuteReset, () => !_isRunning); 
            ApplySizeCommand = new RelayCommand(ExecuteApplySize);
            StartCommand = new RelayCommand(ExecuteStart, () => !_isRunning);
            StopCommand = new RelayCommand(ExecuteStop, () => _isRunning);
            ToggleCellCommand = new RelayCommand<Point>(ExecuteToggleCell, () => !_isRunning);
            ClearPatternSelectionCommand = new RelayCommand(() => SelectedPattern = null);
            SaveCommand = new RelayCommand(ExecuteSave, () => !_isRunning);
            LoadCommand = new RelayCommand(ExecuteLoad, () => !_isRunning);

            NewBoardWidth = _gameBoard.Width;
            NewBoardHeight = _gameBoard.Height;

            _gameBoard.GenerateRandom();
            UpdateStatistics();
            RefreshDisplay();
        }

        private void ExecuteStep()
        {
            _gameState.Update(_gameBoard.CalculateNextStep());

            UpdateStatistics();
            RefreshDisplay();
        }

        private void ExecuteRandomize()
        {
            _gameBoard.GenerateRandom();
            ResetState();

            UpdateStatistics();
            RefreshDisplay();
        }

        private void ExecuteReset()
        {
            _gameBoard.ClearBoard();
            ResetState();

            UpdateStatistics();
            RefreshDisplay();
        }

        private void ExecuteApplySize()
        {
            _gameBoard.Resize(NewBoardWidth, NewBoardHeight);

            OnPropertyChanged(nameof(BoardWidth));
            OnPropertyChanged(nameof(BoardHeight));

            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            CellsToDisplay.Clear();
            foreach (var cell in _gameBoard.AliveCells)
            {
                CellsToDisplay.Add(cell);
            }
        }

        private void UpdateStatistics()
        {
            GenerationCount = _gameState.GenerationCount;
            CellsBorn = _gameState.TotalCellsBorn;
            CellsDied = _gameState.TotalCellsDied;
        }

        private void ExecuteStart()
        {
            IsRunning = true;
            _cts = new CancellationTokenSource();

            Task.Run(() => RunSimulation(_cts.Token));

            CommandManager.InvalidateRequerySuggested();
        }

        private void ExecuteStop()
        {
            IsRunning = false;
            _cts?.Cancel(); 

            CommandManager.InvalidateRequerySuggested();
        }

        private async Task RunSimulation(CancellationToken token)
        {
            while (IsRunning)
            {
                StepResult result = _gameBoard.CalculateNextStep();
                _gameState.Update(result);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateStatistics(); 
                    RefreshDisplay(); 
                });

                try
                {
                    int delay = 1000 - SimulationSpeed;
                    await Task.Delay(delay, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            IsRunning = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommandManager.InvalidateRequerySuggested();
            });
        }

        private void ExecuteToggleCell(Point position)
        {
            if (IsRunning) return;

            if (SelectedPattern != null)
            {
                _gameBoard.PastePattern(position, SelectedPattern);
            }
            else
            {
                _gameBoard.ToggleCell(position);
            }

            RefreshDisplay();
        }

        private void ResetState()
        {
            _gameState.Reset();
        }

        private void ExecuteSave()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Plik tekstowy (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
                Title = "Zapisz stan gry",
                FileName = "save.txt",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var sb = new StringBuilder();

                    sb.AppendLine(_gameBoard.Rules.ToString());

                    sb.AppendLine(_gameBoard.Width.ToString());
                    sb.AppendLine(_gameBoard.Height.ToString());

                    foreach (var cell in _gameBoard.AliveCells)
                    {
                        sb.AppendLine($"{cell.X};{cell.Y}");
                    }

                    File.WriteAllText(dialog.FileName, sb.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas zapisu pliku: {ex.Message}", "Błąd zapisu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteLoad()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Plik tekstowy (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*",
                Title = "Wczytaj stan gry",

                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var lines = File.ReadAllLines(dialog.FileName);

                    var rules = GameRule.Parse(lines[0]);

                    var width = int.Parse(lines[1]);
                    var height = int.Parse(lines[2]);

                    var aliveCells = new HashSet<Point>();
                    for (int i = 3; i < lines.Length; i++)
                    {
                        var parts = lines[i].Split(';');
                        if (parts.Length == 2)
                        {
                            int x = int.Parse(parts[0]);
                            int y = int.Parse(parts[1]);
                            aliveCells.Add(new Point(x, y));
                        }
                    }

                    _gameBoard.SetBoardState(width, height, rules, aliveCells);

                    _gameState.Reset();

                    RulesText = rules.ToString(); 
                    NewBoardWidth = width;      
                    NewBoardHeight = height;

                    OnPropertyChanged(nameof(BoardWidth));
                    OnPropertyChanged(nameof(BoardHeight));

                    UpdateStatistics();
                    RefreshDisplay();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas wczytywania pliku: {ex.Message}", "Błąd odczytu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}