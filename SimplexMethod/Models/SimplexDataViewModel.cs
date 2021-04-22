using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexMethod.Models
{
    public class SimplexDataViewModel
    {
        [Validation(ErrorMessage ="Неверные данные")]
        public List<string> originalFunc { get; set; }

        [Validation(ErrorMessage = "Неверные данные")]
        public List<List<string>> originalLimits { get; set; }

        public int[,] canonLimits;
        public int[] canonFunc;
        public bool NumberRepresent { get; set; }
        public List<string> k { get; set; }
        public int N { get; set; }
        public int M { get; set; }
        public int type { get; set; }
        int countOfExtraVar = 0; /*дополнительные переменные*/
        int pivotRow = -1;
        int pivotCol = -1;
        public Tuple<int, string>[] Basis { get; set; }
        public List<Tuple<int, string>> AllVariables { get; set; }
        public Fraction[,] Matrix { get; set; } /*симплекс таблица*/
        public enum Status { UNSOLVED, SOLVED, NO_SOLUTIONS, UNLIMITED };
        public Status status = Status.UNSOLVED;
        public List<Iteration> Iterations { get; set; }
        //public SimplexDataViewModel(string[] items, string[] k, string[] originalF, int mode, int n, int m)
        //{
        //    originalLimits = new string[n, m + 2];
        //    originalFunc = new string[m + 1];
        //    int j, c = 0;
        //    for (int i = 0; i < n; i++)
        //    {
        //        for (j = 0; j < m; j++)
        //        {
        //            originalLimits[i, j] = items[j + i * m];
                    

        //        }
        //        originalLimits[i, j] = k[c];
        //        originalLimits[i, j + 1] = k[c + 1];
        //        c += 2;
        //    }
        //    for (int i = 0; i < m + 1; i++)
        //    {
        //        originalFunc[i] = originalF[i];
        //    }
        //    OriginalToCanon(mode);
        //}
        public SimplexDataViewModel()
        {
            Iterations = new List<Iteration>();
        }
        private void OriginalToCanon(int mode)
        {
            M += 2;
            int i = 0, j = 0;
            //количество дополнительных переменных
            for (i = 0; i < N; i++)
            {
                if (originalLimits[i][M - 2] != "0")
                {
                    countOfExtraVar++;
                }
            }


            canonLimits = new int[N, countOfExtraVar + M - 1];
            canonFunc = new int[originalFunc.Count + countOfExtraVar];
            Matrix = new Fraction[N + 1, countOfExtraVar + M - 1];
            Basis = new Tuple<int, string>[canonLimits.GetLength(0)];
            for (i = 0; i < Matrix.GetLength(0); i++)
            {
                for (j = 0; j < Matrix.GetLength(1); j++)
                    Matrix[i, j] = new Fraction(0);
            }
            for (i = 0; i < originalFunc.Count - 1; i++)
            {
                canonFunc[i] = int.Parse(originalFunc[i]) * mode;
                Matrix[N, i] = new Fraction(int.Parse(originalFunc[i]) * mode);
            }
            canonFunc[canonFunc.Length - 1] = int.Parse(originalFunc[i]);
            int sign;

            for (i = 0; i < N; i++)
            {
                sign = 1;
                if (int.Parse(originalLimits[i][M - 1]) < 0)
                {
                    string k = originalLimits[i][M - 2];
                    if (k == "1")
                        originalLimits[i][M - 2] = "-1";
                    if (k == "-1")
                        originalLimits[i][M - 2] = "1";
                    sign = -1;
                }
                for (j = 0; j < M - 2; j++)
                {
                    Matrix[i, j] = new Fraction(int.Parse(originalLimits[i][ j]) * sign);
                    canonLimits[i, j] = int.Parse(originalLimits[i][ j]) * sign;
                }
                canonLimits[i, canonLimits.GetLength(1) - 1] = int.Parse(originalLimits[i][j + 1]) * sign;
                Matrix[i, canonLimits.GetLength(1) - 1] = new Fraction(int.Parse(originalLimits[i][j + 1]) * sign);
            }
            int c = 0;

            for (i = 0; i < canonLimits.GetLength(0); i++)
            {
                int k = int.Parse(originalLimits[i][M - 2]);
                if (k != 0)
                {
                    canonLimits[i, M - 2 + c] = k;
                    Matrix[i, M - 2 + c] = new Fraction(k);
                    //if (k < 0) {
                    //    for (j = 0; j < canonLimits.GetLength(1); j++)
                    //    {
                    //        canonLimits[i, j] *= k;
                    //        Matrix[i, j] *= k;
                    //    }
                    //}
                    c++;
                    if (k == 1)
                        Basis[i] = new Tuple<int, string>(M - 2 + c, $"X{M - 2 + c}");

                }
            }

        }
        public string PrintCanonLimits()
        {
            StringBuilder str = new StringBuilder();
            int j = 0;
            for (int i = 0; i < canonLimits.GetLength(0); i++)
            {
                j = 0;
                str.AppendFormat("{0:#;-#;+0}X1", canonLimits[i, j]);
                for (j = 1; j < canonLimits.GetLength(1) - 1; j++)
                {
                    if (canonLimits[i, j] != 0)
                    {
                        str.AppendFormat("{0:+#;-#;+0}X{1}", canonLimits[i, j], j + 1);
                    }

                }
                str.AppendFormat("={0:#;-#;+0}", canonLimits[i, j]);
                str.AppendFormat("\n");
            }
            return str.ToString();
        }
        public string PrintCanonF()
        {
            M += 2;
            StringBuilder str = new StringBuilder();
            int i = 0;
            str.AppendFormat("{0:#;-#;+0}X1", canonFunc[i]);
            for (i = 1; i < canonFunc.Length - 1; i++)
            {
                if (i >= M- 2)
                {
                    str.AppendFormat("{0:+#;-#;+0}U{1}", canonFunc[i], i + 1);
                }
                else
                    str.AppendFormat("{0:+#;-#;+0}X{1}", canonFunc[i], i + 1);
            }
            str.AppendFormat("{0:+#;-#;+0}", canonFunc[i]);
            str.AppendFormat("->min\n");
            return str.ToString();
        }
        public void Solve()
        {
            OriginalToCanon(type);
            SearchBase();
            Iterations.Add(new Iteration(Basis, Matrix,type));
            while (CheckForUnsolvable()) ;
            if (status == Status.NO_SOLUTIONS)
            {
                Iterations.Add(new Iteration(Basis, Matrix,type));
                return;
            }
            ExprF();
            while (FindPivotCol())
            {
                if (FindPivotRow())
                {
                    Iterations.Add(new Iteration(Basis, Matrix,type, pivotRow, pivotCol));
                    CalcBasis(pivotRow, pivotCol);
                    ExprF();
                }
                else
                {
                    status = Status.UNLIMITED;
                }
            }
            Iterations.Add(new Iteration(Basis, Matrix,type));
            status = Status.SOLVED;

        }
        private void SearchBase()
        {
    
            for (int i = 0; i < Basis.Length; i++)
            {
                if (Basis[i] == null)
                {
                    if (!CheckRow(i))
                    {
                        for (int j = 0; j < M - 2; j++)
                        {
                            if (!IsBaseVariable(j + 1))
                            {
                                CalcBasis(i, j);
                                Basis[i] = new Tuple<int, string>(j + 1, $"X{j + 1}");
                                break;
                            }
                        }
                    }
                }
            }
        }
        public void CalcBasis(int row, int col)
        {
            Fraction k = new Fraction(Matrix[row, col].numerator, Matrix[row, col].denominator);
            for (int i = 0; i < Matrix.GetLength(1); i++)
            {
                Matrix[row, i] = Matrix[row, i] / k;
            }
            for (int i = 0; i < Matrix.GetLength(0) - 1; i++)
            {
                if (i == row)
                {
                    continue;
                }
                k = new Fraction(Matrix[i, col].numerator, Matrix[i, col].denominator);

                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Matrix[i, j] = Matrix[i, j] - (k * Matrix[row, j]);
                }

            }
            Basis[row] = new Tuple<int, string>(col + 1, $"X{col + 1}");

        }
        public bool CheckRow(int row)
        {
            for (int j = 0; j < Matrix.GetLength(1) - 1; j++)
            {
                if (Matrix[row, j] != 0 && CheckNull(row, j)) /*Matrix?*/
                {
                    CalcBasis(row, j);
                    Basis[row] = new Tuple<int, string>(j + 1, $"X{j + 1}");
                    return true;
                }

            }
            return false;
        }
        //проверка на нули в столбце
        private bool CheckNull(int _numRow, int _numCol)
        {
            for (int i = 0; i < Matrix.GetLength(0) - 1; i++)
            {
                if (Matrix[i, _numCol] == 0 || _numRow == i)
                    continue;
                else
                    return false;
            }
            return true;
        }
        public bool IsBaseVariable(int variable)
        {
            for (int i = 0; i < Basis.Length; i++)
            {
                if (Basis[i] != null)
                    if (Basis[i].Item1 == variable)
                        return true;
            }
            return false;
        }
        public bool CheckForUnsolvable()
        {
            int row = -1;
            int col = -1;
            bool result = false;
            for (int i = 0; i < Matrix.GetLength(0) - 1; i++)
            {
                if (Matrix[i, Matrix.GetLength(1) - 1] < 0 && row == -1)
                {
                    row = i;
                    result = true;
                }
                else
                    if (Matrix[i, Matrix.GetLength(1) - 1] < 0 && Matrix[i, Matrix.GetLength(1) - 1] < Matrix[row, Matrix.GetLength(1) - 1])
                {
                    row = i;
                   
                }
            }
            if (result)
            {
                result = false;
                for (int i = 0; i < Matrix.GetLength(1) - 1; i++)
                {
                    if (Matrix[row, i] < 0 && col == -1)
                    {
                        col = i;
                        result = true;
                    }
                    else
                        if (Matrix[row, i] < 0 && Matrix[row, i] < Matrix[row, col])             /*где то ошибка неправльно выбирает максимальный минус*//* ищет первый отрицательный?*/
                    {
                        col = i;
                      
                    }
                   
                }
                if (result)
                {
                    CalcBasis(row, col);
                }
                else { status = Status.NO_SOLUTIONS; }
            }

            return result;

        }


        private void ExprF()
        {
            for (int i = 0; i < Matrix.GetLength(1); i++)
            {
                Matrix[Matrix.GetLength(0) - 1, i] = new Fraction(0);
                for (int j = 0; j < Matrix.GetLength(0) - 1; j++)
                {
                    Matrix[Matrix.GetLength(0) - 1, i] += canonFunc[Basis[j].Item1 - 1] * Matrix[j, i];
                }
                Matrix[Matrix.GetLength(0) - 1, i] = canonFunc[i] - Matrix[Matrix.GetLength(0) - 1, i];
            }
        }
        private bool FindPivotCol()
        {
            bool result = false;
            Fraction max = new Fraction(0);
            for (int i = 0; i < Matrix.GetLength(1)-1; i++)
            {
                if (Matrix[Matrix.GetLength(0) - 1,i] < 0)
                {

                    if (-Matrix[Matrix.GetLength(0) - 1,i] > max)
                    {
                        max = -Matrix[Matrix.GetLength(0) - 1, i];
                        pivotCol = i;
                        result = true;
                    }
                }

            }
            return result;
        }
        private bool FindPivotRow()
        {
            Fraction min = new Fraction(int.MaxValue);
            bool result = false;
            for(int i = 0; i < Matrix.GetLength(0) - 1; i++)
            {
                if (Matrix[i, pivotCol] > 0)
                {
                    Fraction r = Matrix[i, Matrix.GetLength(1) - 1] / Matrix[i, pivotCol];
                    if (r<min)
                    {
                        min = r;
                        pivotRow = i;
                        result = true;
                    }
                }
            }
            return result;
        }


    }
}
