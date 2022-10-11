# SimpleRegEx
A regular expression experiment.

**Language: C#**

**Start: 2015**

## Why
I was trying to explain [regular expression](https://en.wikipedia.org/wiki/Regular_expression) to a colleague and I built this tool to allow him to start composing some basic patterns.

## Example
A pattern to identify an email address composed by a name, a dot, another name and "@gmail.com":

```
string pattern = new SRex(EPat.LETTERS_LCASE, repetitionMin: 1, fromLineBeginning: true) // first name
                  .FollowedBy(".", 1, 1) // a dot
                    .FollowedBy(EPat.LETTERS_LCASE, repetitionMin: 1) // second name
                      .FollowedBy("@gmail.com", 1, 1, true) // @gmail.com
                        .Pattern; // ^([a-z])+(\\.)([a-z])+(@gmail\\.com)$
```


