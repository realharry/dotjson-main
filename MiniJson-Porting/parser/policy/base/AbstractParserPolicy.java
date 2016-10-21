package org.minijson.parser.policy.base;

import java.io.Serializable;

import org.minijson.parser.policy.ParserPolicy;


/**
 * Base implementation for ParserPolicy.
 */
public abstract class AbstractParserPolicy implements ParserPolicy, Serializable
{
    private static final long serialVersionUID = 1L;

    // Note that we currently support only
    //  allowNonObjectOrNonArray, allowTrailingComma, allowExtraCommas, and caseInsensitiveLiterals.
    protected boolean strirct;
    protected boolean allowNonObjectOrNonArray;
    protected boolean allowLeadingJsonMarker;
    protected boolean allowTrailingComma;
    protected boolean allowExtraCommas;
    protected boolean allowEmptyObjectMemberValue;
    protected boolean caseInsensitiveLiterals;

    
    // Ctor.
    public AbstractParserPolicy()
    {
        // No values set...
    }

    @Override
    public boolean isStrirct()
    {
        return this.strirct;
    }
    public void setStrict(boolean strict)
    {
        this.strirct = strict;
    }

    @Override
    public boolean allowNonObjectOrNonArray()
    {
        if(this.strirct) {
            return false;
        } else {
            return this.allowNonObjectOrNonArray;
        }
    }
    public void setAllowNonObjectOrNonArray(boolean allowNonObjectOrNonArray)
    {
        this.allowNonObjectOrNonArray = allowNonObjectOrNonArray;
        if(this.allowNonObjectOrNonArray) {
            this.strirct = false;
        }
    }
    
    @Override
    public boolean allowLeadingJsonMarker()
    {
        if(this.strirct) {
            return false;
        } else {
            return this.allowLeadingJsonMarker;
        }
    }
    public void setAllowLeadingJsonMarker(boolean allowLeadingJsonMarker)
    {
        this.allowLeadingJsonMarker = allowLeadingJsonMarker;
        if(this.allowLeadingJsonMarker) {
            this.strirct = false;
        }
    }

    @Override
    public boolean allowTrailingComma()
    {
        if(this.strirct) {
            return false;
        } else {
            if(allowExtraCommas) {
                return true;
            } else {
                return this.allowTrailingComma;
            }
        }
    }
    public void setAllowTrailingComma(boolean allowTrailingComma)
    {
        this.allowTrailingComma = allowTrailingComma;
        if(this.allowTrailingComma) {
            this.strirct = false;
        }
    }

    @Override
    public boolean allowExtraCommas()
    {
        if(this.strirct) {
            return false;
        } else {
            return this.allowExtraCommas;
        }
    }
    public void setAllowExtraCommas(boolean allowExtraCommas)
    {
        this.allowExtraCommas = allowExtraCommas;
        if(this.allowExtraCommas) {
            this.strirct = false;
            this.allowTrailingComma = true;
        }
    }

    @Override
    public boolean allowEmptyObjectMemberValue()
    {
        if(this.strirct) {
            return false;
        } else {
            return this.allowEmptyObjectMemberValue;
        }
    }
    public void setAllowEmptyObjectMemberValue(boolean allowEmptyObjectMemberValue)
    {
        this.allowEmptyObjectMemberValue = allowEmptyObjectMemberValue;
        if(this.allowEmptyObjectMemberValue) {
            this.strirct = false;
        }
    }

    @Override
    public boolean caseInsensitiveLiterals()
    {
        if(this.strirct) {
            return false;
        } else {
            return this.caseInsensitiveLiterals;
        }
    }
    public void setCaseInsensitiveLiterals(boolean caseInsensitiveLiterals)
    {
        this.caseInsensitiveLiterals = allowNonObjectOrNonArray;
        if(this.caseInsensitiveLiterals) {
            this.strirct = false;
        }
    }


    @Override
    public String toString()
    {
        return "AbsttractParserPolicy [strirct=" + strirct
                + ", allowNonObjectOrNonArray=" + allowNonObjectOrNonArray
                + ", allowLeadingJsonMarker=" + allowLeadingJsonMarker
                + ", allowTrailingComma=" + allowTrailingComma
                + ", allowExtraCommas=" + allowExtraCommas
                + ", allowEmptyObjectMemberValue="
                + allowEmptyObjectMemberValue + ", caseInsensitiveLiterals="
                + caseInsensitiveLiterals + "]";
    }


}
