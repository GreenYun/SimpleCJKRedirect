# SimpleCJKRedirect

This is a CSV style code chart for CJK character conversion, which is used to redirect the different forms to the correct one when working with Unicode.

`ConvertChart.csv` contains codes of varient and main forms of one character per line, seperated with a comma. Semicolon is used to be followed by comments. The main form is verified with _Kangxi Dictionary_.

`Program.cs` is an example parsing program, to print the character itself for error checking and to reformat the CSV file. 
