# LOXInterpreter

This project is a implementation of the Lox language using C# based on the book _Crafting Inpertpreters_ by Robert Nystrom 


The LoxInterpreter bin contains all the relevant code for this project. The GenerateAst bin quickly became irrelevant once the Expr and Stmt files were added to the main code. But for reference purposes it is included in this repo.


The rest of the repo is all of the test cases broken down into different bins for what they are testing. Each bin will have all the tests and then a result file giving what the interpreter returned. For example a test will be named "testfile" and what it gives will be named "testfile_result"


The benchmark file contains more complicated tests. The rest are mainly individual unit tests


If you wish to test it. You may compile the code and use the command prompt to test. 


Testing oddities 
The only file that is giving something other than what is expected is literals_strings in the strings bin. It is not giving non-ASCII symbols. I am unsure how to fix it.
