using System;
using System.Linq;


namespace Cranberries
{
    namespace Interval
    {
        public struct Interval
        {
            private double lower;
            private double upper;

            public double Lower
            {
                get => lower;
                set => lower = value <= upper ? value : throw new ArgumentException("Set value that greater than upper field!");
            }
            public double Upper
            {
                get => upper;
                set => upper = lower <= value ? value : throw new ArgumentException("Set value that less than lower field!");
            }

            public void Deconstruct(out double x, out double y) => (x, y) = (lower, upper);

            public Interval(double l, double u) => (lower, upper) = u < l ? throw new ArgumentException() : (l, u);

            static private double Max(params double[] nums) => nums.Max();

            static private double Min(params double[] nums) => nums.Min();

            public Interval Inverse()
                => lower < 0 && 0 < upper ? throw new DivideByZeroException()
                : new Interval(1.0 / upper, 1.0 / lower);

            public static Interval operator +(Interval l, Interval r)
                => new Interval(l.Lower + r.Lower, l.Upper + r.Upper);

            public static Interval operator +(double l, Interval r)
                => new Interval(l + r.Lower, l + r.Upper);

            public static Interval operator +(Interval l, double r)
                => new Interval(l.Lower + r, l.Upper + r);

            public static Interval operator -(Interval l, Interval r)
                => new Interval(l.Lower - r.Upper, l.Upper - r.Lower);

            public static Interval operator -(double l, Interval r)
                => new Interval(l - r.Upper, l - r.Lower);

            public static Interval operator -(Interval l, double r)
                => new Interval(l.Lower - r, l.Upper - r);

            public static Interval operator *(Interval l, Interval r)
                => new Interval(Min(l.Lower * r.Lower, l.Lower * r.Upper, l.Upper * r.Lower, l.Upper * r.Upper),
                            Max(l.Lower * r.Lower, l.Lower * r.Upper, l.Upper * r.Lower, l.Upper * r.Upper));

            public static Interval operator *(double l, Interval r)
                => (l < 0.0) ? new Interval(l * r.Upper, l * r.Lower) : new Interval(l * r.Lower, l * r.Upper);

            public static Interval operator *(Interval l, double r)
                => (r < 0.0) ? new Interval(l.Upper * r, l.Lower * r) : new Interval(l.Lower * r, l.Upper * r);

            public static Interval operator /(Interval l, Interval r) => l * r.Inverse();

            public static Interval operator /(double l, Interval r) => l * r.Inverse();

            public static Interval operator /(Interval l, double r)
                => (r < 0.0) ? new Interval(l.Upper / r, l.Lower / r) : new Interval(l.Lower / r, l.Upper / r);

            public override bool Equals(object obj) => base.Equals(obj);

            public bool Equals(Interval x)
                => (object)x == null ? false : (Lower == x.Lower) && (Upper == x.Upper);

            public override int GetHashCode() => base.GetHashCode();

            public static bool operator !=(Interval l, Interval r) => !(l.Lower == r.Lower && l.Upper == r.Upper);

            public static bool operator ==(Interval l, Interval r) => l.Lower == r.Lower && l.Upper == r.Upper;

            public static bool operator <(Interval l, Interval r) => l.Upper < r.Lower;

            public static bool operator >(Interval l, Interval r) => r.Upper < l.Lower;

            public static bool operator <=(Interval l, Interval r) => l.Upper <= r.Lower;

            public static bool operator >=(Interval l, Interval r) => r.Upper <= l.Lower;

            public bool IsSingleton => Lower == Upper;

            public double Width => Upper - Lower;

            public double Middle => (Lower + Upper) / 2.0;

            public double Rad => (Upper - Lower) / 2.0;

            public override string ToString() => $"[{Lower}, {Upper}]";

            // Constants
            public static Interval PI => new Interval(Math.PI, Math.PI);

            public static Interval E => new Interval(Math.E, Math.E);

            public static Interval Whole => new Interval(double.NegativeInfinity, double.PositiveInfinity);

            public Interval Sin()
            {
                if (Lower - Lower >= 2.0 * Math.PI) return new Interval(-1, 1);
                var PI2 = 2.0 * Math.PI;

                /*  base point reset  */
                var (base1, base2) = (Math.Ceiling(Lower / PI2 - 0.25), Math.Ceiling(Lower / PI2 - 0.75));
                var (x1, x2, y1, y2) = (Lower / PI2 - 0.25, Lower / PI2 - 0.25, Lower / PI2 - 0.75, Lower / PI2 - 0.75);

                /*  checking phase  */
                return x1 < base1 && base1 < x2 ?
                            y1 < base2 && base2 < y2 ?
                                new Interval(-1, 1)
                                : new Interval(Math.Min(Math.Sin(Lower), Math.Sin(Lower)), 1)
                        : y1 < base2 && base2 < y2 ?
                            new Interval(-1, Math.Max(Math.Sin(Lower), Math.Sin(Lower)))
                        : Math.Sin(Lower) < Math.Sin(Lower) ?
                            new Interval(Math.Sin(Lower), Math.Sin(Lower))
                        : new Interval(Math.Sin(Lower), Math.Sin(Lower));
            }

            public Interval Cos()
            {
                if (Upper - Lower >= 2.0 * Math.PI) return new Interval(-1, 1);
                var PI2 = 2.0 * Math.PI;

                /*  base point reset  */
                var (base1, base2) = (Math.Ceiling(Lower / PI2), Math.Ceiling(Lower / PI2 - 0.5));
                var (x1, x2, y1, y2) = (Lower / PI2, Upper / PI2, Lower / PI2 - 0.5, Upper / PI2 - 0.5);

                /*  checking phase  */
                return x1 < base1 && base1 < x2 ?
                            y1 < base2 && base2 < y2 ?
                                new Interval(-1, 1)
                                : new Interval(Math.Min(Math.Cos(Lower), Math.Cos(Upper)), 1)
                        : y1 < base2 && base2 < y2 ?
                            new Interval(-1, Math.Max(Math.Cos(Lower), Math.Cos(Upper)))
                        : Math.Cos(Lower) < Math.Cos(Upper) ?
                            new Interval(Math.Cos(Lower), Math.Cos(Upper))
                        : new Interval(Math.Cos(Upper), Math.Cos(Lower));
            }

            public Interval Tan()
                => Math.Ceiling(Upper * 2.0 / Math.PI) - Math.Ceiling(Lower * 2.0 / Math.PI) != 0 ? throw new OverflowException()
                : new Interval(Math.Tan(Lower), Math.Tan(Upper));

            public Interval Asin()
                => (Lower < -1.0 || 1.0 < Upper) ? throw new ArgumentOutOfRangeException()
                : new Interval(Math.Asin(Lower), Math.Asin(Upper));

            public Interval Acos()
                => Lower < -1.0 || 1.0 < Upper ? throw new ArgumentOutOfRangeException()
                : new Interval(Math.Acos(Upper), Math.Acos(Lower));

            public Interval Atan() => new Interval(Math.Atan(Lower), Math.Atan(Upper));

            public Interval Sinh() => new Interval(Math.Sinh(Lower), Math.Sinh(Upper));

            public Interval Cosh()
                => Lower < 0.0 && 0.0 < Upper ? new Interval(1, Math.Max(Math.Cosh(Lower), Math.Cosh(Upper)))
                : Upper < 0.0 ? new Interval(Math.Cosh(Upper), Math.Cosh(Lower))
                : new Interval(Math.Cosh(Lower), Math.Cosh(Upper));

            public Interval Tanh() => new Interval(Math.Tanh(Lower), Math.Tanh(Upper));

            public Interval Asinh() => new Interval(Math.Log(Lower + Math.Sqrt(Lower * Lower + 1.0)), Math.Log(Upper + Math.Sqrt(Upper * Upper + 1.0)));

            public Interval Acosh()
                => Lower < 1.0 ? throw new ArgumentOutOfRangeException()
                : new Interval(Math.Log(Lower + Math.Sqrt(Lower * Lower - 1.0)), Math.Log(Upper + Math.Sqrt(Upper * Upper - 1.0)));

            public Interval Atanh()
                => Lower < -1.0 || 1.0 < Upper ? throw new ArgumentOutOfRangeException()
                : new Interval(Math.Log((1.0 + Lower) / (1.0 - Lower)) / 2.0, Math.Log((1.0 + Upper) / (1.0 - Upper)) / 2.0);

            public Interval Sec() => Cos().Inverse();

            public Interval Csc() => Sin().Inverse();

            public Interval Cot() => Tan().Inverse();

            public Interval Acsc()
                => -1.0 < Lower || 1.0 < Upper ? throw new ArgumentOutOfRangeException()
                : Inverse().Asin();

            public Interval Asec()
                => -1.0 < Lower || 1.0 < Upper ? throw new ArgumentOutOfRangeException()
                : Inverse().Acos();

            public Interval Acot() => Inverse().Atan();

            public Interval Pow(double n)
                => n < 0 ? Inverse().Pow(-n) :
                    n % 1 == 0 ?
                        n == 0.0 ? new Interval(1, 1) :
                        Lower <= 0.0 && 0.0 <= Upper ? new Interval(0, Math.Max(Math.Pow(Lower, n), Math.Pow(Upper, n)))
                        : new Interval(Math.Min(Math.Pow(Lower, n), Math.Pow(Upper, n)), Math.Max(Math.Pow(Lower, n), Math.Pow(Upper, n))) :
                    Lower < 0.0 ? throw new ArgumentOutOfRangeException() :
                    Lower <= 0.0 && 0.0 <= Upper ? new Interval(0, Math.Max(Math.Pow(Lower, n), Math.Pow(Upper, n)))
                    : new Interval(Math.Min(Math.Pow(Lower, n), Math.Pow(Upper, n)), Math.Max(Math.Pow(Lower, n), Math.Pow(Upper, n)));

            public Interval Sqrt()
                => Lower < 0.0 ? throw new ArgumentOutOfRangeException()
                    : new Interval(Math.Sqrt(Lower), Math.Sqrt(Upper));

            public Interval Cbrt() => new Interval(Math.Pow(Lower, 0.3333333333333333), Math.Pow(Upper, 0.333333333333333333));
        }
    }
}
