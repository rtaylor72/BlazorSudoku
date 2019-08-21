using System;

namespace BlazorSudoku.Solvers {
    public class SolverSolution {
        public string SolverName { get; set; }
        public bool IsSolution { get; set; }
        public TimeSpan TimeToSolve { get; set; }
        public byte[,] Board { get; set; }
        public int SolutionDepth { get; set; }
        public int BackTracks { get; set; }
        public int StepsToSolve { get; set; }
        public string SolverNotes { get; set; }

        public string SolverComboBoxDesc {
            get {
                if (IsSolution) {
                    return SolverName + " TimeToSolve=" + TimeToSolve.ToString().Substring(0, 13) + " Depth=" + SolutionDepth + " BackTracks=" + BackTracks + " StepsToSolve=" + StepsToSolve;
                } else {
                    return SolverName;
                }
            }
        }

        public string TestLogger {
            get {
                return SolverName + ";" + Math.Round(TimeToSolve.TotalMilliseconds, 0, MidpointRounding.AwayFromZero).ToString() + ";" + SolutionDepth + ";" + BackTracks + ";" + StepsToSolve + ";\r\n";
            }
        }
    }
}
