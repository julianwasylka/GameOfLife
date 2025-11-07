using GameOfLife.WPF.Models;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using GameOfLife.WPF.Commands;

namespace GameOfLife.WPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // Commands
        public ICommand StepCommand { get; }
        public ICommand RandomizeCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand ApplySizeCommand { get; }

        private GameBoard _gameBoard;
        private GameState _gameState;

        public double BoardWidthPixels => _gameBoard.Width * AppConfig.CellSize;
        public double BoardHeightPixels => _gameBoard.Height * AppConfig.CellSize;
        public ObservableCollection<Point> CellsToDisplay { get; set; }

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

        private double _currentZoom = 1.0;
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

        public MainViewModel()
        {
            _gameBoard = new GameBoard();
            _gameState = new GameState();
            CellsToDisplay = new ObservableCollection<Point>();

            RulesText = "B3/S23";

            StepCommand = new RelayCommand(ExecuteStep);
            RandomizeCommand = new RelayCommand(ExecuteRandomize);
            ClearCommand = new RelayCommand(ExecuteReset); 
            ApplySizeCommand = new RelayCommand(ExecuteApplySize);

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

            OnPropertyChanged(nameof(BoardWidthPixels));
            OnPropertyChanged(nameof(BoardHeightPixels));

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

        private void ResetState()
        {
            _gameState.Reset();
        }
    }
}