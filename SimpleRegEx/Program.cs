using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SimpleRegEx
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      MyTest();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainForm());
      return;
    }

    static void MyTest()
    {
            //-------------------------------------------------------- test 1
      string input = "name.family@domain.com";

      string pattern = new SRex(EPat.LETTERS_LCASE, repetitionMin: 1, fromLineBeginning: true)
        .FollowedBy(".", 1, 1)
        .FollowedBy(EPat.LETTERS_LCASE, repetitionMin: 1)
                .FollowedBy("@domain.com", 1, 1, true)
        .Pattern;

      Regex re = new Regex(pattern);
      bool mt1 = re.IsMatch(input);
      string[] names = re.Split(input);

      //-------------------------------------------------------- test 2
      string fileName = @"c:\test\vjkdf\bla\myfile_test.ext";
      string fileName2 = @"c:\test\vjkdf\bla\myfile2test.ext";
      string fileName3 = @"c:\test\vjkdf\bla\myfile2test.ext.";

      SRex letNumUndS = new SRex(EPat.LETTERS_LCASE).AndBy(EPat.NUMBERS).AndBy('_', 1);
      pattern = new SRex(@"\")
        .FollowedBy(letNumUndS)
        .FollowedBy(".", 1, 1)
        .FollowedBy(letNumUndS)
                .FollowedBy(EPat.LETTERS_LCASE, endsWith: true)
        .Pattern;

      re = new Regex(pattern, RegexOptions.IgnoreCase);
            bool mt2 = re.IsMatch(fileName);
            bool mt3 = re.IsMatch(fileName2);
            bool mt4 = re.IsMatch(fileName3);
      string[] groups = re.Split(fileName);

    }
  }
}
