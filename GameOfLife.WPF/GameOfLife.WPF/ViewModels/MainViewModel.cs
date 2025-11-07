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

        private GameBoard _gameBoard;
        private GameState _gameState;

        private const int CellSize = 5;
        public int BoardWidthPixels => _gameBoard.Width * CellSize;
        public int BoardHeightPixels => _gameBoard.Height * CellSize;

        public ObservableCollection<Point> CellsToDisplay { get; set; }

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

        public MainViewModel()
        {
            _gameBoard = new GameBoard();
            _gameState = new GameState();
            CellsToDisplay = new ObservableCollection<Point>();

            RulesText = "B3/S23";
            UpdateStatistics();

            StepCommand = new RelayCommand(ExecuteStep);

            _gameBoard.GenerateRandom(0.25);
            RefreshDisplay();
        }

        private void ExecuteStep()
        {
            _gameState.Update(_gameBoard.CalculateNextStep());

            UpdateStatistics(); 
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
    }
}