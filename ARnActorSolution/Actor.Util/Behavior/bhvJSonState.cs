using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Actor.Util
{

    public abstract class JSonValue
    {
        public abstract bool Parse(string aData) ;

        public static JSonValue Cast(string aData)
        {
            int i = 0;
            var json = JSonValue.CastFromToken(aData, ref i) ;
            json.Parse(aData);
            return json;
        }

        public static JSonValue CastFromToken(string aData,ref int pos)
        {
            // remove space
            while (aData[pos] == ' ')
                pos++;
            switch(aData[pos])
            {
                case '"' : { return new JSonString() ; } 
                case '{' : {return new JSonObject() ; }
                case '[' : {return new JSonList() ; }
                default :
                    {
                        if (aData.StartsWith("true")) {return new JSonTrue() ;}
                        if (aData.StartsWith("false")) {return new JSonFalse() ;}
                        if (aData.StartsWith("null")) {return new JSonNull() ;}
                        return null ;
                    }
            }
        }

        public int FindToken(string aData,char aDelimiter, int start)
        {
            int pos = start ;
            bool inAcc = false ;
            char Acc = ' ' ;
            while (pos < aData.Length)
            {
                char aDataPos = aData[pos];
                if (inAcc && (Acc != aDataPos))
                {
                    pos++ ;
                }
                else
                if (inAcc && (Acc == aDataPos))
                {
                    inAcc = false;
                    pos++;
                }
                else
                if (aDataPos == aDelimiter)
                {
                    return pos ;
                }
                else
                {
                    switch (aDataPos)
                    {
                        case '{': { inAcc = true; Acc = '}'; pos++ ; break; }
                        case '[': { inAcc = true; Acc = ']'; pos++; break; }
                        case '"': { inAcc = true; Acc = '"'; pos++; break; }
                        case '\\' : {pos = pos +2 ; break ; }
                        default:
                            { 
                                pos++;
                                break;
                            }
                    }
                }
            }
            return -1 ;
        }
    }

    public class JSonTrue : JSonValue
    {
        public override bool Parse(string aData) {return true ;}
    }

    public class JSonFalse : JSonValue
    {
        public override bool Parse(string aData) {return true ;}
    }

    public class JSonNull : JSonValue
    {
        public override bool Parse(string aData) {return true ;}
    }

    public class JSonObject : JSonValue
    {
        Dictionary<string, JSonValue> Members = new Dictionary<string, JSonValue>();
        public override bool Parse(string aData)
        {
            if (aData[0] != '{')
                return false ;
            int start = 1 ; // cause the {
            int pos= 1 ;
            while(pos >=0)
            {
                pos = FindToken(aData,',',start) ;
                string kv = aData.Substring(start, pos - 1) ;
                start = pos++;
                pos = FindToken(aData, ':', start);
                string Name = aData.Substring(start, pos - 1);
                start = pos++;
                JSonValue jv = JSonValue.CastFromToken(aData,ref pos) ;
                Members.Add(aData.Substring(start,pos-1),jv) ;
            }
            return true ;
        }
    }

    public class JSonList : JSonValue
    {
        List<JSonValue> Items = new List<JSonValue>();
        public override bool Parse(string aData) {return true ;}
    }

    public class JSonString : JSonValue
    {
        public string Value { get; set; }
        public override bool Parse(string aData) {return true;}
    }

    public class JSonLong : JSonValue
    {
        long Value;
        public override bool Parse(string aData) {return true;}
    }

    public class JSonDouble : JSonValue
    {
        double Value;
        public override bool Parse(string aData) {return true;}
    }


    //public class bhvJSonState : bhvStateBehavior<JSonData>
    //{
    //    JSonValue 
    //}
}
