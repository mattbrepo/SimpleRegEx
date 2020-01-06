using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleRegEx
{
  /// <summary>
  /// Common Patterns
  /// </summary>
  enum EPat
  {
    ANY_CHAR,
    ANY_SPACE,
    SPACE,
    NEWLINE,
    CARRIAGE_RETURN,
    TAB,
    NUMBERS,
    LETTERS_UCASE,
    LETTERS_LCASE
  }

  class SRex
  {
    #region prop
    public string Pattern
    {
      get
      {
        string pattern = _patterns.Aggregate((item1, item2) => item1 + item2);
        return pattern;
      }
    }
    #endregion

    #region field
    private List<string> _patterns;
    #endregion

    #region ctor
    public SRex()
    {
      _patterns = new List<string>();
    }

    public SRex(EPat cp, int? repetitionMin = null, int? repetitionMax = null, bool fromLineBeginning = false, bool endsWith = false) : this()
    {
      string pattern = (fromLineBeginning ? "^" : "") + "(" + TranslateInRegEx(cp) + ")";
      string repetition = GetRepetition(repetitionMin, repetitionMax, endsWith);
      AddNewPattern(pattern, false, repetition);
    }

    public SRex(string words, int? repetitionMin = null, int? repetitionMax = null, bool fromLineBeginning = false, bool endsWith = false) : this()
    {
      string pattern = (fromLineBeginning ? "^" : "") + "(" + TranslateInRegEx(words) + ")";
      string repetition = GetRepetition(repetitionMin, repetitionMax, endsWith);
      AddNewPattern(pattern, false, repetition);
    }

    #endregion

    #region public func
    public SRex Or(params SRex[] srs)
    {
      string nextPattern = 
        "(" + 
        srs.Select(item => RemoveBracket(item.Pattern, true))
        .Aggregate((item1, item2) => "(" + item1 + ")|(" + item2 + ")") + 
        ")";
      _patterns.Add(nextPattern);
      return this;
    }

        public SRex FollowedBy(SRex obj, int? repetitionMin = null, int? repetitionMax = null, bool fromLineBeginning = false, bool endsWith = false)
    {
            string repetition = GetRepetition(repetitionMin, repetitionMax, endsWith);
            string pattern = (fromLineBeginning ? "^" : "") + "(" + obj.Pattern + ")";
            return AddNewPattern(pattern, false, repetition);
    }

        public SRex FollowedBy(char cStart, char cEnd, int? repetitionMin = null, int? repetitionMax = null, bool endsWith = false)
        {
            string repetition = GetRepetition(repetitionMin, repetitionMax, endsWith);
            return AddNewPattern("[" + cStart + "-" + cEnd + "]", true, repetition);
        }

    public SRex FollowedBy(EPat cp, int? repetitionMin = null, int? repetitionMax = null, bool endsWith = false)
    {
      string repetition = GetRepetition(repetitionMin, repetitionMax, endsWith);
      return AddNewPattern(TranslateInRegEx(cp), true, repetition);
    }

    public SRex FollowedBy(string words, int? repetitionMin = null, int? repetitionMax = null, bool endsWith = false)
    {
      string repetition = GetRepetition(repetitionMin, repetitionMax, endsWith);
      return AddNewPattern(TranslateInRegEx(words), true, repetition);
    }

    public SRex AndBy(EPat cp, int? repetitionMin = null, int? repetitionMax = null, bool endsWith = false)
    {
      string repetition = GetRepetition(repetitionMin, repetitionMax, endsWith);
      return AddToPrevPattern(TranslateInRegEx(cp), repetition);
    }

    public SRex AndBy(char c, int? repetitionMin = null, int? repetitionMax = null, bool endsWith = false)
    {
      string s = "" + c;
      string repetition = GetRepetition(repetitionMin, repetitionMax, endsWith);
      return AddToPrevPattern(TranslateInRegEx(s), repetition);
    }

    public SRex NotFollowedBy(EPat cp)
    {
      string nextPattern = TranslateInRegEx(cp);
      if (!nextPattern.StartsWith("[")) throw new Exception("'Not' isn't suitable for: " + cp.ToString()); 
      nextPattern = "[^" + nextPattern.Substring(1);
      return AddNewPattern(nextPattern, false, "");
    }

    public SRex NotFollowedBy(char c)
    {
      string nextPattern = "[^" + TranslateInRegEx("" + c) + "]";
      return AddNewPattern(nextPattern, false, "");
    } 
    #endregion

    #region private func

    private string RemoveBracket(string pattern, bool round)
    {
      string bracketStart = round ? "(" : "[";
      string bracketEnd = round ? ")" : "]";

      if (!pattern.StartsWith(bracketStart) || !pattern.EndsWith(bracketEnd)) return pattern;

      string res = pattern.Substring(1, pattern.Length - 2);
      return res;
    }

    private SRex AddToPrevPattern(string basePattern, string repetition)
    {
      if (_patterns.Count == 0) throw new Exception("No patterns!!!");
      string prevPattern = _patterns.Last();
      
      if (prevPattern.Last() != ')')  throw new Exception("No previous patterns!!!");

      if (prevPattern.Substring(prevPattern.Length - 2) == "])") prevPattern = prevPattern.Substring(0, prevPattern.Length - 2) + RemoveBracket(basePattern, false) + "])" + repetition;
      else prevPattern = prevPattern.Substring(0, prevPattern.Length - 1) + basePattern + ")" + repetition;

      _patterns[_patterns.Count - 1] = prevPattern;

      return this;
    }

    private SRex AddNewPattern(string basePattern, bool bracket, string repetition)
    {
      string nextPattern = (bracket ? "(" + basePattern + ")" : basePattern) + repetition;
      _patterns.Add(nextPattern);

      return this;
    }

    private string GetRepetition(int? repetitionMin, int? repetitionMax, bool endsWith)
    {
      string res = "";
      if (repetitionMin != null && repetitionMin >= 0)
      {
        if (repetitionMin == 0)
        {
          if (repetitionMax == null) res = "*"; //zero or more times
          else if (repetitionMax == 1) res = "?"; //zero or one time
          else res = "{" + repetitionMin + "," + repetitionMax + "}"; //ha senso da 0 a 2?!
        }
        else if (repetitionMin == 1 && repetitionMax == null) res = "+"; //one or more times
        else
        {
          if (repetitionMax == null) res = "{" + repetitionMin + ",}"; //a{2,}: two or more
          else if (repetitionMin == repetitionMax)
          {
            if (repetitionMin != 1) res = "{" + repetitionMin + "}"; //a{5}: exactly five
          }
          else res = "{" + repetitionMin + "," + repetitionMax + "}"; //a{1,3}: between one & three
        }
      }

      if (endsWith) res += "$";

      return res;
    }

    private string TranslateInRegEx(string words)
    {
      string res = words.Replace(@"\", @"\\");
      res = res.Replace(".", @"\.");
      res = res.Replace("^", @"\^");
      res = res.Replace("$", @"\$");
      res = res.Replace("*", @"\*");
      res = res.Replace("+", @"\+");
      res = res.Replace("?", @"\^");
      res = res.Replace("(", @"\(");
      res = res.Replace(")", @"\)");
      res = res.Replace("[", @"\[");
      res = res.Replace("]", @"\]");
      res = res.Replace("{", @"\{");
      res = res.Replace("}", @"\}");
      res = res.Replace("|", @"\|");

      return res;
    }

    private string TranslateInRegEx(EPat cp)
    {
      switch (cp)
      {
        case EPat.NUMBERS:
          return "[0-9]";
        case EPat.LETTERS_UCASE:
          return "[A-Z]";
        case EPat.LETTERS_LCASE:
          return "[a-z]";
        //case EPat.LETTERS:
        //    return "[A-Za-z]";
        //case EPat.LETTERS_NUMS:
        //    return "[A-Za-z0-9]";
        case EPat.SPACE:
          return "[ ]";
        case EPat.ANY_SPACE:
          return @"\s";
        case EPat.ANY_CHAR:
          return ".";
        case EPat.TAB:
          return @"\t";
        case EPat.NEWLINE:
          return @"\n";
        case EPat.CARRIAGE_RETURN:
          return @"\r";
        default:
          throw new Exception("Unrecognize " + cp.ToString());
      }
    }

    #endregion
  }
}
