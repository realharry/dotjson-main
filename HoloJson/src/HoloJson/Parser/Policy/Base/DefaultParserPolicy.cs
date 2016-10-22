using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser.Policy.Base
{
    /// <summary>
    /// Base implementations of ParserPolicy.
    /// </summary>
    // [Serializable]
    public class DefaultParserPolicy : AbstractParserPolicy, ParserPolicy
    {
        // No leniency.
        public static readonly ParserPolicy STRICT = new DefaultParserPolicyAnonymousInnerClass1();
        private class DefaultParserPolicyAnonymousInnerClass1 : DefaultParserPolicy
        {
            public DefaultParserPolicyAnonymousInnerClass1()
            {
				isStrict = true;
				allowNonObjectOrNonArray = false;
				allowLeadingJsonMarker = false;
				allowTrailingComma = false;
				allowExtraCommas = false;
				allowEmptyObjectMemberValue = false;
				caseInsensitiveLiterals = false;
			}
        }

        // We use MINIJSON as default.
        public static readonly ParserPolicy MINIJSON = new DefaultParserPolicyAnonymousInnerClass2();
        private class DefaultParserPolicyAnonymousInnerClass2 : DefaultParserPolicy
        {
            public DefaultParserPolicyAnonymousInnerClass2()
            {
                isStrict = true;
                allowNonObjectOrNonArray = false;
                allowLeadingJsonMarker = false;
                allowTrailingComma = false;
                allowExtraCommas = false;
                allowEmptyObjectMemberValue = false;
                caseInsensitiveLiterals = false;
            }
        }

        // Ctor.
        public DefaultParserPolicy()
        {
            isStrict = true;
            allowNonObjectOrNonArray = false;
            allowLeadingJsonMarker = false;
            allowTrailingComma = false;
            allowExtraCommas = false;
            allowEmptyObjectMemberValue = false;
            caseInsensitiveLiterals = false;
        }

    }

}