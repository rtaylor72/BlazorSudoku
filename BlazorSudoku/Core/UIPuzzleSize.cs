namespace BlazorSudoku.Core
{
    public class UIPuzzleSize
    {
        public UIPuzzleSize(byte puzzleSize, int minimumGivenCellsForUniqueAnswer)
        {
            PuzzleSize = puzzleSize;
            PuzzleSizeText = $"{puzzleSize}x{puzzleSize}";
            MinimumGivenCellsForUniqueAnswer = minimumGivenCellsForUniqueAnswer;
        }

        public byte PuzzleSize { get; internal set; }
        public string PuzzleSizeText { get; internal set; }
        public int MinimumGivenCellsForUniqueAnswer { get; internal set; }
    }
}