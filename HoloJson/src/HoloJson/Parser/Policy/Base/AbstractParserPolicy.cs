using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HoloJson.Parser.Policy.Base
{
    /// <summary>
    /// Base implementation for ParserPolicy.
    /// </summary>
    // [Serializable]
    public abstract class AbstractParserPolicy : ParserPolicy
    {
        // Note that we currently support only
        //  allowNonObjectOrNonArray, allowTrailingComma, allowExtraCommas, and caseInsensitiveLiterals.
        protected internal bool isStrict;
        protected internal bool allowNonObjectOrNonArray;
        protected internal bool allowLeadingJsonMarker;
        protected internal bool allowTrailingComma;
        protected internal bool allowExtraCommas;
        protected internal bool allowEmptyObjectMemberValue;
        protected internal bool caseInsensitiveLiterals;


        // Ctor.
        protected AbstractParserPolicy()
        {
            // No values set...
        }

        public virtual bool Strict
        {
            get
            {
                return this.isStrict;
            }
            set
            {
                this.isStrict = value;
            }
        }

        public virtual bool AllowNonObjectOrNonArray
        {
            get
            {
                if (this.isStrict) {
                    return false;
                } else {
                    return this.allowNonObjectOrNonArray;
                }
            }
            set
            {
                this.allowNonObjectOrNonArray = value;
                if (this.allowNonObjectOrNonArray) {
                    this.isStrict = false;
                }
            }
        }

        public virtual bool AllowLeadingJsonMarker
        {
            get
            {
                if (this.isStrict) {
                    return false;
                } else {
                    return this.allowLeadingJsonMarker;
                }
            }
            set
            {
                this.allowLeadingJsonMarker = value;
                if (this.allowLeadingJsonMarker) {
                    this.isStrict = false;
                }
            }
        }

        public virtual bool AllowTrailingComma
        {
            get
            {
                if (this.isStrict) {
                    return false;
                } else {
                    if (allowExtraCommas) {
                        return true;
                    } else {
                        return this.allowTrailingComma;
                    }
                }
            }
            set
            {
                this.allowTrailingComma = value;
                if (this.allowTrailingComma) {
                    this.isStrict = false;
                }
            }
        }

        public virtual bool AllowExtraCommas
        {
            get
            {
                if (this.isStrict) {
                    return false;
                } else {
                    return this.allowExtraCommas;
                }
            }
            set
            {
                this.allowExtraCommas = value;
                if (this.allowExtraCommas) {
                    this.isStrict = false;
                    this.allowTrailingComma = true;
                }
            }
        }

        public virtual bool AllowEmptyObjectMemberValue
        {
            get
            {
                if (this.isStrict) {
                    return false;
                } else {
                    return this.allowEmptyObjectMemberValue;
                }
            }
            set
            {
                this.allowEmptyObjectMemberValue = value;
                if (this.allowEmptyObjectMemberValue) {
                    this.isStrict = false;
                }
            }
        }

        public virtual bool CaseInsensitiveLiterals
        {
            get
            {
                if (this.isStrict) {
                    return false;
                } else {
                    return this.caseInsensitiveLiterals;
                }
            }
            set
            {
                this.caseInsensitiveLiterals = allowNonObjectOrNonArray;
                if (this.caseInsensitiveLiterals) {
                    this.isStrict = false;
                }
            }
        }


        public override string ToString()
        {
            return "AbsttractParserPolicy [strirct=" + isStrict + ", allowNonObjectOrNonArray=" + allowNonObjectOrNonArray + ", allowLeadingJsonMarker=" + allowLeadingJsonMarker + ", allowTrailingComma=" + allowTrailingComma + ", allowExtraCommas=" + allowExtraCommas + ", allowEmptyObjectMemberValue=" + allowEmptyObjectMemberValue + ", caseInsensitiveLiterals=" + caseInsensitiveLiterals + "]";
        }


    }

}