using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplexMethod.Models
{
    public class Fraction
    {
        public int numerator;              // Числитель
        public int denominator;            // Знаменатель
        public Fraction(int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new DivideByZeroException("В знаменателе не может быть нуля");
            }
            this.numerator = numerator;
            this.denominator = denominator;

        }
        // Возвращает наибольший общий делитель (Алгоритм Евклида)
        private static int getGreatestCommonDivisor(int a, int b)
        {
  
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public Fraction Reduce()
        {
            Fraction result = this;
            int greatestCommonDivisor = getGreatestCommonDivisor(Math.Abs(this.numerator), this.denominator);
            if (greatestCommonDivisor != 1)
            {
             result.numerator /= greatestCommonDivisor;
            result.denominator /= greatestCommonDivisor;
            }
            
            return result;
        }
        private static Fraction Add(Fraction a, Fraction b)
        {
            Fraction r = new Fraction(0);
            r.numerator = (a.numerator * b.denominator )+ (a.denominator * b.numerator);
            r.denominator = a.denominator * b.denominator;
            r.Reduce();
            return r;
        }
        // Возвращает дробь, обратную данной
        private Fraction GetReverse()
        {
            return new Fraction(this.denominator , this.numerator);
        }
        // Возвращает дробь с противоположным знаком
        private Fraction GetWithChangedSign()
        {
            return new Fraction(-this.numerator, this.denominator);
        }
        // Вызов первого конструктора со знаменателем равным единице
        public Fraction(int number) : this(number, 1) { }

        // Перегрузка оператора "+" для случая сложения двух дробей
        public static Fraction operator +(Fraction a, Fraction b)
        {
            return Add(a,b); 
        }
        // Перегрузка оператора "+" для случая сложения дроби с числом
        public static Fraction operator +(Fraction a, int b)
        {
            return a + new Fraction(b);
        }
        // Перегрузка оператора "+" для случая сложения числа с дробью
        public static Fraction operator +(int a, Fraction b)
        {
            return b + a;
        }
        // Перегрузка оператора "-" для случая вычитания двух дробей
        public static Fraction operator -(Fraction a, Fraction b)
        {
            return Add(a, -b);
        }
        // Перегрузка оператора "-" для случая вычитания из дроби числа
        public static Fraction operator -(Fraction a, int b)
        {
            return a - new Fraction(b);
        }
        // Перегрузка оператора "-" для случая вычитания из числа дроби
        public static Fraction operator -(int a, Fraction b)
        {
            return new Fraction(a) - b;
        }
        // Перегрузка оператора "*" для случая произведения двух дробей
        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(a.numerator *  b.numerator , a.denominator * b.denominator).Reduce();
        }
        // Перегрузка оператора "*" для случая произведения дроби и числа
        public static Fraction operator *(Fraction a, int b)
        {
            return a * new Fraction(b);
        }
        // Перегрузка оператора "*" для случая произведения числа и дроби
        public static Fraction operator *(int a, Fraction b)
        {
            return b * a;
        }
        // Перегрузка оператора "/" для случая деления двух дробей
        public static Fraction operator /(Fraction a, Fraction b)
        {
            return new Fraction(a.numerator * b.denominator, a.denominator * b.numerator).Reduce();
        }
        // Перегрузка оператора "/" для случая деления дроби на число
        public static Fraction operator /(Fraction a, int b)
        {
            return a / new Fraction(b);
        }
        // Перегрузка оператора "/" для случая деления числа на дробь
        public static Fraction operator /(int a, Fraction b)
        {
            return new Fraction(a) / b;
        }
        // Перегрузка оператора "унарный минус"
        public static Fraction operator -(Fraction a)
        {
            if (a.numerator == 0)
                return a;
            return a.GetWithChangedSign();
        }

        // Метод сравнения двух дробей
        // Возвращает	 0, если дроби равны
        //				 1, если this больше that
        //				-1, если this меньше that
        private int CompareTo(Fraction that)
        {
            if (this.Equals(that))
            {
                return 0;
            }
            Fraction a = this.Reduce();
            Fraction b = that.Reduce();
            if (a.numerator  * b.denominator > b.numerator *  a.denominator)
            {
                return 1;
            }
            return -1;
        }
        // Мой метод Equals
        public bool Equals(Fraction that)
        {
         
            Fraction a = this.Reduce();
            Fraction b = that.Reduce();
            return a.numerator == b.numerator &&
            a.denominator == b.denominator;
            
        }
        // Переопределение метода Equals
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Fraction)
            {
                result = this.Equals(obj as Fraction);
            }
            return result;
        }
        // Переопределение метода GetHashCode
        public override int GetHashCode()
        {
            return (this.numerator * this.numerator + this.denominator * this.denominator);
        }
        // Перегрузка оператора ">" для двух дробей
        public static bool operator >(Fraction a, Fraction b)
        {
            return a.CompareTo(b) > 0;
        }
        // Перегрузка оператора ">" для дроби и числа
        public static bool operator >(Fraction a, double b)
        {
            return a.AsDouble>b;
        }
 
        // Перегрузка оператора "<" для двух дробей
        public static bool operator <(Fraction a, Fraction b)
        {
            
            return a.AsDouble<b.AsDouble;
        }
        // Перегрузка оператора "<" для дроби и числа
        public static bool operator <(Fraction a, double b)
        {
            return a.AsDouble < b;
        }

        // Переопределение метода ToString
        public override string ToString()
        {
            if (this.numerator == 0)
            {
                return "0";
            }
            string result;
            if (this.numerator < 0 ^ this.denominator<0)
            {
                result = "-";
            }
            else
            {
                result = "";
            }
            if (this.numerator == this.denominator)
            {
                return result + "1";
            }
            if (Math.Abs( this.denominator) == 1)
            {
                return result + Math.Abs(this.numerator);
            }
            return result + Math.Abs(this.numerator) + "/" + Math.Abs( this.denominator);
        }
        // Перегрузка оператора "Равенство" для двух дробей
        public static bool operator ==(Fraction a, Fraction b)
        {
            // Приведение к Object необходимо для того, чтобы
            // можно было сравнивать дроби с null.
            // Обычное сравнение a.Equals(b) в данном случае не подходит,
            // так как если a есть null, то у него нет метода Equals,
            // следовательно будет выдано исключение, а если
            // b окажется равным null, то исключение будет вызвано в
            // методе this.Equals
            Object aAsObj = a as Object;
            Object bAsObj = b as Object;
            if (aAsObj == null || bAsObj == null)
            {
                return aAsObj == bAsObj;
            }
            return a.Equals(b);
        }
        // Перегрузка оператора "Равенство" для дроби и числа
        public static bool operator ==(Fraction a, int b)
        {
            return a == new Fraction(b);
        }
        // Перегрузка оператора "Равенство" для числа и дроби
        public static bool operator ==(int a, Fraction b)
        {
            return new Fraction(a) == b;
        }
        // Перегрузка оператора "Неравенство" для двух дробей
        public static bool operator !=(Fraction a, Fraction b)
        {
            return !(a == b);
        }
        // Перегрузка оператора "Неравенство" для дроби и числа
        public static bool operator !=(Fraction a, int b)
        {
            return a != new Fraction(b);
        }
        // Перегрузка оператора "Неравенство" для числа и дроби
        public static bool operator !=(int a, Fraction b)
        {
            return new Fraction(a) != b;
        }
        public double AsDouble
        {
            get =>  1.0 * this.numerator / this.denominator;
            set
            {
              
            }
        }
        public double AsRoundDouble
        {
            get => Math.Round(1.0 * this.numerator / this.denominator,3);
            set
            {

            }
        }

    }

}
