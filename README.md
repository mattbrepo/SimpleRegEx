# SimpleRegEx
A [Regular Expression](https://en.wikipedia.org/wiki/Regular_expression) experiment

**Language: C#**

**Start: 2015**

## Why
I was trying to explain 'regular expression' to a collegue. In the meantime I built this tool to allow him to start composing some basic patterns.

## Example
A pattern to identify an email address composed by a name, a dot, another name and "@gmail.com":

```
string pattern = new SRex(EPat.LETTERS_LCASE, repetitionMin: 1, fromLineBeginning: true)
  .FollowedBy(".", 1, 1)
  .FollowedBy(EPat.LETTERS_LCASE, repetitionMin: 1)
          .FollowedBy("@gmail.com", 1, 1, true)
  .Pattern;
```

Result: _^([a-z])+(\\.)([a-z])+(@gmail\\.com)$_
