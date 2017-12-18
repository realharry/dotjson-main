using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Core
{
    // Represents a "number".
    public struct Number
    {
        // ??? Int32 type?
        public static readonly Number Zero = new Number(0);
        public static readonly Number Invalid = new Number(Double.MinValue);   // ?????

        // private Type type;  // --> Just use numeral's type... ????
        private object numeral;

        // ...

        public Number(object numeral)
        {
            if (IsNumber(numeral)) {
                this.numeral = numeral;
            } else {
                throw new ArgumentException("Arg numeral is not a number.");
            }
        }


        public object Value
        {
            get
            {
                return numeral;
            }
            //private set
            //{
            //    // Validate ??
            //    numeral = value;
            //}
        }


        public static bool IsNumber(object obj)
        {
            // TBD:
            // Is byte considred a number????
            // What about char?
            var type = obj.GetType();
            if (type == typeof(Int16)
                || type == typeof(Int32)
                || type == typeof(Int64)
                || type == typeof(UInt16)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(Single)
                || type == typeof(Double)
                || type == typeof(Decimal)
                || type == typeof(Byte)
                || type == typeof(SByte)
                ) {
                return true;
            } else {
                return false;
            }
        }


        public override string ToString()
        {
            // ???
            return numeral.ToString();
        }
    }


    public static class NumberExtensions
    {
        // TBD:
        // arbitrary algo.
        // Note that we prefer int -> long -> decimal then double.
        // .....
        public static Number ToNumber(this string me)
        {
            // TBD:
            // Check first if the string contains "." ????
            // ...
            Number number;
            try {
                var l1 = Convert.ToInt64(me);
                if (l1 > Int32.MaxValue || l1 < Int32.MinValue) {
                    number = new Number(l1);
                } else {
                    number = new Number((int) l1);
                }
            } catch (Exception ex1) {
                System.Diagnostics.Debug.WriteLine("ToInt64() Failed: " + ex1.Message);
                try {
                    var d1 = Convert.ToDouble(me);
                    if (d1 > (double) Decimal.MaxValue || d1 < (double) Decimal.MinValue) {
                        number = new Number(d1);
                    } else {
                        var d2 = Convert.ToDecimal(me);
                        number = new Number(d2);
                    }
                } catch (Exception ex2) {
                    System.Diagnostics.Debug.WriteLine("ToDouble() | ToDecimal() Failed: " + ex2.Message);
                    // ????
                    // number = Number.Zero;   // ???
                    throw new ArgumentException("Cannot be converted to a number.");
                }
            }
            return number;
        }

        // TBD:
        // Note: Number.IsNumber(obj) checks the obj's type.
        // This method tries to parse the string...
        public static bool IsNumber(this string me)
        {
            // TBD: Use TryParse()???
            // ...

            var isNumber = false;
            try {
                var number = me.ToNumber();
                // no exception means conversion succeeded for at least one numeric type.
                isNumber = true;
            } catch (Exception) {
                // isNumber = false;
            }

            return isNumber;
        }

    }

}
