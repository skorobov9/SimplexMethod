using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplexMethod.Models
{
    public class Iteration
    {
        public Tuple<int, string>[] Basis { get; set; } 
        public Fraction[] AllVariables { get; set; }
        public Fraction[,] Matrix { get; set; } /*симплекс таблица*/
        public int pivotRow;  
        public int pivotCol;
        public Fraction F;
        public Iteration(Tuple<int,string>[] basis,Fraction[,] matrix,int type, int pivotRow=-1, int pivotCol=-1,string str="")
        {
            this.Basis = (Tuple<int, string>[])basis.Clone();
            this.Matrix =(Fraction[,]) matrix.Clone();
            this.pivotCol = pivotCol;
            this.pivotRow = pivotRow;
            AllVariables = new Fraction[Matrix.GetLength(1)-1];
            for (int i = 0; i < AllVariables.Length; i++)
                AllVariables[i] = new Fraction(0);
            for(int i=0;i<Matrix.GetLength(0)-1;i++)
            {
                if (Basis[i] != null)
                {
                    int k = Basis[i].Item1 - 1;
                    AllVariables[k] = Matrix[i, Matrix.GetLength(1) - 1];
                }
            }
            if(type==1)
            F = -Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1];
            else F = Matrix[Matrix.GetLength(0) - 1, Matrix.GetLength(1) - 1];
        }
        public string AllVar()
        {
            StringBuilder str = new StringBuilder();
            str.Append("(");
            int i;
            for ( i = 0; i < AllVariables.Length-1; i++)
                str.Append($"{AllVariables[i]},");
            str.Append($"{AllVariables[i]})");
            return str.ToString();

        }
        public string AllVarAsDouble()
        {
            StringBuilder str = new StringBuilder();
            str.Append("(");
            int i;
            for (i = 0; i < AllVariables.Length - 1; i++)
                str.Append($"{AllVariables[i].AsRoundDouble},");
            str.Append($"{AllVariables[i].AsRoundDouble})");
            return str.ToString();

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
    }
}
