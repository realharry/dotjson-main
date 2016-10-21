package org.minijson.parser.policy.base;

import org.minijson.parser.policy.ParserPolicy;


/**
 * Base implementations of ParserPolicy.
 */
public class DefaultParserPolicy extends AbstractParserPolicy implements ParserPolicy
{
    private static final long serialVersionUID = 1L;

    // No leniency.
    public static final ParserPolicy STRICT = new DefaultParserPolicy() {
        private static final long serialVersionUID = 1L;

        {  // Initializer
            strirct = true;
            allowNonObjectOrNonArray = false;
            allowLeadingJsonMarker = false;
            allowTrailingComma = false;
            allowExtraCommas = false;
            allowEmptyObjectMemberValue = false;
            caseInsensitiveLiterals = false;
        }
    };
    // We use MINIJSON as default.
    public static final ParserPolicy MINIJSON = new DefaultParserPolicy() {
        private static final long serialVersionUID = 1L;

        {  // Initializer
            strirct = false;
            allowNonObjectOrNonArray = true;
            allowLeadingJsonMarker = false;
            allowTrailingComma = true;
            allowExtraCommas = false;
            allowEmptyObjectMemberValue = false;
            caseInsensitiveLiterals = false;
        }
    };

    // Ctor.
    public DefaultParserPolicy()
    {
        strirct = true;
        allowNonObjectOrNonArray = false;
        allowLeadingJsonMarker = false;
        allowTrailingComma = false;
        allowExtraCommas = false;
        allowEmptyObjectMemberValue = false;
        caseInsensitiveLiterals = false;
    }

}
