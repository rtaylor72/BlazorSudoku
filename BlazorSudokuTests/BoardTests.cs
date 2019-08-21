using BlazorSudoku.Core;
using Xunit;

namespace BlazorSudokuTests
{
    public class BoardTests
    {
        [Fact]
        public void CreateNewBoard()
        {
            Board gameBoard = Utility.CreateNewBoard(4, 10, true);
            var x = gameBoard.ToArray();
            Assert.Equal(4, x.GetLength(0));
            Assert.Equal(4, x.GetLength(1));
        }
    }
}