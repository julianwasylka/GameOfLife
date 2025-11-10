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

        private bool _isRunning;
        private CancellationTokenSource _cts;

        // UI properties
        private string _rulesText;
        public string RulesText
        {
            get { return _rulesText; }
            set
            {
                _rulesText = value;
                OnPropertyChanged();
                _gameBoard.Rules = GameRule.Parse(_rulesText);
            }
        }

        private int _generationCount;
        public int GenerationCount
        {
            get { return _generationCount; }
            set
            {
                _generationCount = value;
                OnPropertyChanged();
            }
        }

        private int _cellsBorn;
        public int CellsBorn
        {
            get { return _cellsBorn; }
            set
            {
                _cellsBorn = value;
                OnPropertyChanged(); 
            }
        }

        private int _cellsDied;
        public int CellsDied
        {
            get { return _cellsDied; }
            set
            {
                _cellsDied = value;
                OnPropertyChanged();
            }
        }

        private double _currentZoom;
        public double CurrentZoom
        {
            get { return _currentZoom; }
            set
            {
                _currentZoom = value;
                OnPropertyChanged();
            }
        }

        private int _newBoardWidth;
        public int NewBoardWidth
        {
            get { return _newBoardWidth; }
            set
            {
                _newBoardWidth = value;
                OnPropertyChanged();
            }
        }

        private int _newBoardHeight;
        public int NewBoardHeight
        {
            get { return _newBoardHeight; }
            set
            {
                _newBoardHeight = value;
                OnPropertyChanged();
            }
        }

        private int _simulationSpeed;
        public int SimulationSpeed
        {
            get { return _simulationSpeed; }
            set
            {
                _simulationSpeed = value;
                OnPropertyChanged();
            }
        }

        private Pattern _selectedPattern;
        public Pattern SelectedPattern
        {
            get { return _selectedPattern; }
            set
            {
                _selectedPattern = value;
                OnPropertyChanged();
            }
        }

        private bool _useCircles = false;
        public bool UseCircles
        {
            get { return _useCircles; }
            set
            {
                _useCircles = value;
                OnPropertyChanged();
            }
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
            _isRunning = true;
            _cts = new CancellationTokenSource();

            Task.Run(() => RunSimulation(_cts.Token));

            CommandManager.InvalidateRequerySuggested();
        }

        private void ExecuteStop()
        {
            _isRunning = false;
            _cts?.Cancel(); 

            CommandManager.InvalidateRequerySuggested();
        }

        private async Task RunSimulation(CancellationToken token)
        {
            while (_isRunning)
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
                    int delay = 1050 - SimulationSpeed;
                    await Task.Delay(delay, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            _isRunning = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommandManager.InvalidateRequerySuggested();
            });
        }

        private void ExecuteToggleCell(Point position)
        {
            if (_isRunning) return;

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