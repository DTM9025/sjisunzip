sjisunzip
=========

This is a pretty braindead command line utility that simply forces the encoding to the right values to extract a non-ascii encoded zip file. By default the extracter is set to decode in Shift JIS ([Code page 932](http://en.wikipedia.org/wiki/Code_page_932)), but by passing the `-c` flag with a code page number, you can specify any other [code page in Windows](https://learn.microsoft.com/en-us/windows/win32/intl/code-page-identifiers).

[Download Here](https://github.com/DTM9025/sjisunzip/releases)

```
Usage: .\sjisunzip.exe [-c <Codepage Number>] someFile.zip [toFolder]
    -c: Sets the codepage the extracter will use (default 932). Common ones are 932 (JP), 936 (CN), and 1252 (EN)
Usage: .\sjisunzip.exe -r [-c <Codepage Number>] someFile.zip
    -r: Recode file to {filename}_utf8.zip
    -c: Sets the codepage the extracter will use (default 932). Common ones are 932 (JP), 936 (CN), and 1252 (EN)

Examples:
    .\sjisunzip.exe aFile.zip
    .\sjisunzip.exe aFile.zip MyNewFolder
    .\sjisunzip.exe -c 936 aFile.zip
    .\sjisunzip.exe -c 936 aFile.zip MyNewFolder
    .\sjisunzip.exe -r aFile.zip
    .\sjisunzip.exe -r -c 936 aFile.zip
```

You can also just drop a zip file onto the program since that'll pass it as the first argument and the contents will be extracted in the same directory.

If you've ever received a zip file from a friend, or the wrong damn gnu mirror or whatever that passed through Japan then you've probably seen garbled filenames
![example_1](https://cloud.githubusercontent.com/assets/2738686/5326938/37acc0de-7ce7-11e4-8259-06ef8b1f43a8.jpg)
---

Well this program forces the opened zip to the correct encoding then extracts the file to a more reasonable UTF encoding.
![example_2](https://cloud.githubusercontent.com/assets/2738686/5326978/712d7e50-7ce9-11e4-8f18-c885afc51055.jpg)
---

You can even just reencode the zip file to a less busted-ass one so you don't have this creeping horror issue in the future
![example_3](https://cloud.githubusercontent.com/assets/2738686/5326937/37ab2878-7ce7-11e4-9655-61b92a2b680d.jpg)
---

The filenames and paths should be untangled when done.
![example_4](https://cloud.githubusercontent.com/assets/2738686/5326940/37af9d72-7ce7-11e4-8ee2-3a9d11c6e669.jpg)
---

Bonus fact: When this type of transitive corruption occurs, the output characters are called [Mojibake](http://en.wikipedia.org/wiki/Mojibake). That's almost cute enough to not be awful anymore.
