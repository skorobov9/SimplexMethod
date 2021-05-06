using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexMethod.Models
{
    public class SimplexDataViewModel
    {        
        [Required(ErrorMessage = "Пустые поля!")]
        [Validation(ErrorMessage ="Неверные данные")]
        public List<string> OriginalFunc { get; set; }

        [Required(ErrorMessage = "Пустые поля!")]
        [Validation(ErrorMessage = "Неверные данные")]
        public List<List<string>> OriginalLimits { get; set; }

        public int[,] canonLimits; /* канонические ограничения*/
        public int[] canonFunc;       /*каноническая функция*/
        public bool NumberRepresent { get; set;} /*способ представления чисел*/
        public int N { get; set; } /*количество ограничений*/
        public int M { get; set; } /*количество переменных ориг*/
        public int Type { get; set; } /*тип задачи*/
        public int Mode { get; set; }  /*метод решения*/
        public int СountOfExtraVar { get; private set; } = 0; /*дополнительные переменные*/
        public int СountofArtificalvar { get; private  set; } = 0; /*искусственные переменные*/
        int pivotRow = -1; /*опорные строка и столбец*/
        int pivotCol = -1;
        public Tuple<int, string>[] Basis { get; set; } /*базисные переменные*/
        public Fraction[,] Matrix { get; set; } /*симплекс таблица*/
        public enum Status { SOLVED, UNSOLVED, NO_SOLUTIONS, UNLIMITED };
        public Status status = Status.UNSOLVED; 
        public List<Iteration> Iterations { get; set; }
        public SimplexDataViewModel()
        {
            Iterations = new List<Iteration>();
        }
        //Оригинальная задача в канонической форме
        private void OriginalToCanon()
        {   
            int sign;
            M += 1;
            int i = 0, j = 0;
            //количество дополнительных переменных
            for (i = 0; i < N; i++)
            {
                if (OriginalLimits[i][M - 1] != "0")
                {
                    СountOfExtraVar++;
                }
            }


            canonLimits = new int[N, СountOfExtraVar + M];
            canonFunc = new int[OriginalFunc.Count + СountOfExtraVar];    
            Basis = new Tuple<int, string>[canonLimits.GetLength(0)];
            for (i = 0; i < OriginalFunc.Count - 1; i++)
            {
                canonFunc[i] = int.Parse(OriginalFunc[i]) * Type;   /*если задача на макс умножаем на -1*/
            }
            canonFunc[canonFunc.Length - 1] = int.Parse(OriginalFunc[i]);
        
            for (i = 0; i < N; i++)
            {
                sign = 1;
                /*убираем отрицательный свободный член*/
                if (int.Parse(OriginalLimits[i][M]) < 0)
                {
                    string k = OriginalLimits[i][M - 1];
                    if (k == "1")
                        OriginalLimits[i][M - 1] = "-1";  
                    if (k == "-1")
                        OriginalLimits[i][M - 1] = "1";
                    sign = -1;
                }
                for (j = 0; j < M - 1; j++)
                {
                    canonLimits[i, j] = int.Parse(OriginalLimits[i][j]) * sign;
                }
                canonLimits[i, canonLimits.GetLength(1) - 1] = int.Parse(OriginalLimits[i][j + 1]) * sign;
            }
            int c = 0;

            //знаки перед дполнительными переменными
            for (i = 0; i < canonLimits.GetLength(0); i++)
            {
                int k = int.Parse(OriginalLimits[i][M - 1]);
                if (k != 0)
                {
                    canonLimits[i, M - 1 + c] = k;
                    c++;
                    if (k == 1)
                        Basis[i] = new Tuple<int, string>(M - 1 + c, $"X{M - 1 + c}");

                }
            }

        }

        //Вывод  канонических ограничений
        public string PrintCanonLimits()
        {
            StringBuilder str = new StringBuilder();
            int j = 0;
            for (int i = 0; i < canonLimits.GetLength(0); i++)
            {
                j = 0;
                if (canonLimits[i, j] != 0)
                {
                    str.AppendFormat("{0:#;-#;0}X1", canonLimits[i, j]);
                }
                for (j = 1; j < canonLimits.GetLength(1) - 1; j++)
                {
                    if (canonLimits[i, j] != 0)
                    {
                        if (canonLimits[i, j] == 1)
                        {
                            str.AppendFormat("+X{0}", j + 1);
                            continue;
                        }
                        if(canonLimits[i, j] == -1)
                        {
                            str.AppendFormat("-X{0}", j + 1);
                            continue;
                        }
                        str.AppendFormat("{0:+#;-#;+0}X{1}", canonLimits[i, j], j + 1);
                    }

                }
                str.AppendFormat("={0:#;-#;0}", canonLimits[i, j]);
                str.AppendFormat("\n");
            }
            return str.ToString();
        }

        //Вывод  канонической функции
        public string PrintCanonF()
        {
            StringBuilder str = new StringBuilder();
            int i = 0;
            if (canonFunc[i] != 0)
            {
                str.AppendFormat("{0:#;-#;0}X1", canonFunc[i]);
            }
            for (i = 1; i < canonFunc.Length - 1; i++)
            {
                if (canonFunc[i] != 0)
                {
                    if (canonFunc[i] == 1)
                    {
                        str.AppendFormat("+X{0}", i + 1);
                        continue;
                    }
                    if (canonFunc[i] == -1)
                    {
                        str.AppendFormat("-X{0}", i + 1);
                        continue;
                    }
                    str.AppendFormat("{0:+#;-#;+0}X{1}", canonFunc[i], i + 1);
                }
            }
            if (canonFunc[i] != 0)
            {
                str.AppendFormat("{0:+#;-#;+0}", canonFunc[i]);
            }
            str.AppendFormat("->min\n");
            return str.ToString();
        }

        //Основной метод решения
        public void Solve()
        { 
            OriginalToCanon();
            CreateMatrix();
            if (Mode != 1)
            {
                SolveWithArtificialBasis();
                return;
            }

            SearchBase();
            Iterations.Add(new Iteration(Basis, Matrix,Type));
            while (CheckForUnsolvable()); /* убираем отрицательные свободные члены*/
                Iterations.Add(new Iteration(Basis, Matrix, Type));
                if (status == Status.NO_SOLUTIONS)
                {
                    return;
                }

            ExprF();
            while (FindPivotCol())
            {
                if (FindPivotRow())
                {
                    Iterations.Add(new Iteration(Basis, Matrix,Type, pivotRow, pivotCol));
                    CalcBasis(pivotRow, pivotCol);
                    ExprF();
                }
                else
                {
                    status = Status.UNLIMITED;
                    Iterations.Add(new Iteration(Basis, Matrix, Type));
                    return;
                }
            }
            Iterations.Add(new Iteration(Basis, Matrix,Type));
            status = Status.SOLVED;

        }
        //Решение методом искусственного базиса
        public void SolveWithArtificialBasis()
        {
            AddArtificalvar();
            Iterations.Add(new Iteration(Basis, Matrix, Type));
            if (status == Status.NO_SOLUTIONS)
            {
                Iterations.Add(new Iteration(Basis, Matrix, Type));
                return;
            }
            if (!CheckArtificialVar())
            {
                ExprFArtificBasis();
            }
            while (FindPivotCol())
            {
                if (FindPivotRow())
                {
                    Iterations.Add(new Iteration(Basis, Matrix, Type, pivotRow, pivotCol));
                    CalcBasis(pivotRow, pivotCol);
                    if (CheckArtificialVar())
                    {
                        ExprF();
                    }
                    else
                    {
                        ExprFArtificBasis();
                    }
                }
                else
                {
                    status = Status.UNLIMITED;
                    Iterations.Add(new Iteration(Basis, Matrix, Type));
                    return;

                }
               
            }
            Iterations.Add(new Iteration(Basis, Matrix, Type));
            status = Status.SOLVED;

        }

        //Создание симплекс таблицы
        private void CreateMatrix()
        {
            int k=0;
            //Определяем размерность таблицы
            if (Mode == 1)
            {
                Matrix = new Fraction[N + 1, СountOfExtraVar + M];
            }
            else
            {
                for (int i = 0; i < Basis.Length; i++)
                    if (Basis[i] == null)
                        k++;
                Matrix = new Fraction[N + 1, СountOfExtraVar + M + k];
            }

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                    Matrix[i, j] = new Fraction(0);
            }

            for (int i = 0; i < OriginalFunc.Count - 1; i++)
            {
                Matrix[N, i] = new Fraction(int.Parse(OriginalFunc[i]) * Type);
            }

            for (int i  = 0; i < canonLimits.GetLength(0); i++)
            {
                for (int j = 0; j < canonLimits.GetLength(1)-1; j++)
                {
                    Matrix[i, j] = new Fraction(canonLimits[i, j]);
                }
                Matrix[i, Matrix.GetLength(1) - 1] = new Fraction(canonLimits[i, canonLimits.GetLength(1) - 1]);
            }
        }

        //Проверка базиса на искусственные переменные
       private bool CheckArtificialVar()
        {
            for(int i = 0; i < Basis.Length; i++)
            {
                if (Basis[i] != null && Basis[i].Item2.First() == 'U')
                {
                    return false;
                }
            }
            return true;
        }
        //Поиск начального базиса
        private void SearchBase()
        {
    
            for (int i = 0; i < Basis.Length; i++)
            {
                if (Basis[i] == null)
                {
                    if (!CheckRow(i))
                    {
                        for (int j = 0; j <Matrix.GetLength(1)- 1; j++)
                        {
                            if (!IsBaseVariable(j + 1)&&Matrix[i,j]!=0)
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
        //Добавление исскуственных переменных
        private void AddArtificalvar()
        {
            for (int i = 0; i < Basis.Length; i++)
            {
                if (Basis[i] == null)
                {
                        Basis[i] = new Tuple<int, string>(M+СountOfExtraVar+ СountofArtificalvar, $"U{СountofArtificalvar + 1}");
                        СountofArtificalvar++;
                        Matrix[i, M-2+ СountOfExtraVar + СountofArtificalvar] = new Fraction(1);
                       
                }
            }
        }

        //Добавление переменной в базис
        public void CalcBasis(int row, int col)
        {
            Fraction k = new Fraction(Matrix[row, col].numerator, Matrix[row, col].denominator);
            for (int i = 0; i < Matrix.GetLength(1); i++)
            {
                Matrix[row, i] = Matrix[row, i] / k;            /*делим опорную строку на разрешающий элемент*/
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
                    Matrix[i, j] = Matrix[i, j] - (k * Matrix[row, j]); /*из  строк вычитается найденная строка, умноженная на значения, стоящие в этом же столбце соответствующей строки*/
                }

            }
            Basis[row] = new Tuple<int, string>(col + 1, $"X{col + 1}");

        }
        //Проверка строки на единичную переменную
        public bool CheckRow(int row)
        {
            for (int j = 0; j < Matrix.GetLength(1) - 1; j++)
            {
                if (Matrix[row, j] == 1 && CheckNull(row, j)) 
                {
                    CalcBasis(row, j);
                    Basis[row] = new Tuple<int, string>(j + 1, $"X{j + 1}");
                    return true;
                }

            }
            return false;
        }
        //Проверка на нули в столбце
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
        //Проверка является ли переменная базисной
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

     
        //Избавляемся от отрицательных свободных членов
        public bool CheckForUnsolvable()
        {
            int row = -1;
            int col = -1;
            bool result = false;
            //ищем строку с максимальным по модулю отрицательным свободным членом
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
            //ищем  максимальный по модулю отрицательный элемент в этой строке
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
                        if (Matrix[row, i] < 0 && Matrix[row, i] < Matrix[row, col])        
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

        //Пересчет коэффицентов функции
        private void ExprF()
        {
            for (int i = 0; i < canonLimits.GetLength(1)-1; i++)
            {      
                Matrix[Matrix.GetLength(0) - 1, i] = new Fraction(0);
                for (int j = 0; j < Matrix.GetLength(0) - 1; j++)
                {
                    if(Basis[j]!=null)
                    Matrix[Matrix.GetLength(0) - 1, i] += canonFunc[Basis[j].Item1 - 1] * Matrix[j, i];
                }
                Matrix[Matrix.GetLength(0) - 1, i] = canonFunc[i] - Matrix[Matrix.GetLength(0) - 1, i];
            }
            Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1)-1] = new Fraction(0);
            for (int j = 0; j < Matrix.GetLength(0) - 1; j++)
            {
                if (Basis[j] != null)
                    Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1] += canonFunc[Basis[j].Item1 - 1] * Matrix[j, Matrix.GetLength(1) - 1];
            }
            Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1]  = -canonFunc[canonFunc.Length-1] - Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1];
        }

        //расчет строки "оценка" в  методе искусственного базиса
        private void ExprFArtificBasis()
        {
            for (int i = 0; i < Matrix.GetLength(1); i++)
            {
                Matrix[Matrix.GetLength(0) - 1, i] = new Fraction(0);
                for (int j = 0; j < Matrix.GetLength(0) - 1; j++)
                {
                    if (Basis[j] != null && Basis[j].Item2.First()=='U' )
                    {
                        Matrix[Matrix.GetLength(0) - 1, i] += Matrix[j, i];
                    }
                }
                Matrix[Matrix.GetLength(0) - 1, i] = -Matrix[Matrix.GetLength(0) - 1, i];
            }
        }
        //поиск опорного столбца
        private bool FindPivotCol()
        {
            pivotCol = -1;
            bool result = false;
            for (int i = 0; i < canonLimits.GetLength(1)-1; i++)
            {
                
                if (Matrix[Matrix.GetLength(0) - 1, i] < 0 && pivotCol == -1)
                {
                    pivotCol = i;
                    result = true;
                }
                else
                {
                    if (Matrix[Matrix.GetLength(0) - 1, i] < 0 && Matrix[Matrix.GetLength(0) - 1, i]<Matrix[Matrix.GetLength(0) - 1,pivotCol])
                    {              
                        pivotCol = i;
                    }
                }

            }
            return result;
        }

        //поиск опорной строки
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
