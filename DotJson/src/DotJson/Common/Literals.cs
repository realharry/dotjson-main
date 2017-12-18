using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DotJson.Common
{
	// Predefined string literals.
	public sealed class Literals
	{
		private Literals() {}
		
		public static readonly string NULL = "null";
		public static readonly string TRUE = "true";
		public static readonly string FALSE = "false";
		public static readonly int NULL_LENGTH = NULL.Length;
		public static readonly int TRUE_LENGTH = TRUE.Length;
		public static readonly int FALSE_LENGTH = FALSE.Length;

	}
}
