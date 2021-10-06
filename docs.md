# Carcass Documentation

Everything that you will need to learn how to program in Carcass will be written here(or atleast most of it.)

Here's the table of contents:

1. How does it work?
2. Printing some text...
3. Getting some input!
4. Variables.
5. Functions and calling them.
6. While If.
7. Importing your own files.
8. Exercises


# How does it work?

Carcass is made with a parser generator and lexer generator.
The order of process can be shown with this scheme:

``Input -> Lexing -> Parsing -> Evaluating.``

It supports stuff such as importation(non-preprocessor.) and parenthesis-less functions.

It's also a simple enough language to the point you can learn in about 30 minutes.

So why don't we start?..

# Printing some text...

Today, you'll be introduced to the ``print`` built-in function.

It accepts an expression argument(int, string, variable identifier, other expressions in Carcass...) and then writes it out to the console.

On the inside it's basically just something marked as a keyword that gets parsed into a Print Expression and then is evaluated in C# with ``Console.WriteLine();``

Let's try printing ``Hello, world!``:

First of all we need to start invoking the print function.
So just write ``print `` for now.

Now we need to write a string with Hello World. 
A string is a string of characters wrapped into one bunch.

Say 'C' is a character and "Cccc" is the string.
It contains 4 characters that are all C.
So that string is a string of 4 C characters.

Now in Carcass strings and characters are defined with double quotes like:

``"string content"``.

Using that we can learn that the string for our hello world would be:
``"Hello, world!"``

So now that we want to use the grammar ``PrintFunctionInvoke String``

We get that it would be
```print "Hello, world!"```

We can do it with numbers like ``123``

```print 123```

And so and so...

# Getting some input!

We will learn how to get some user input from the console.

We can do so through the input function.

It's EXTREMELY simple as it takes 0 arguments.

All you need is basically just mention the ``input`` function and you're done.

Like, ```print input``` would just say back what you said.

So this part was probably the simplest..

# Variables.

What are variables?

Well variables are sections in memory that contain data and are marked by an identifier.

In Carcass we define them with "var"

The syntax would be:

```VarKeyword Identifier = Value```

Example:
```var example = 123```

And we can print out variables aswell, just by mentioning their identifier:

```print example```

It's certaintly nothing hard, and you can use them to hold data.

# Functions and calling them.

Functions are big blocks of code that you can invoke from somewhere in your code instead of repeating that block of code everywhere.

To invoke a function you need to use this syntax: ``FunctionIdentifier(parameters)``

Say, we have a function a that takes in b and c. to invoke it we need to specify those parameters.

Like:
a(10,15), 10 and 15 will be the matches of b and c.

Now to define a function we need to follow another syntax: 
```
fn FunctionIdentifier(parametersWanted) {
 code to not repeat..
}
```
Now that we know, we can make a function ``test`` that writes the sum of parameters a and b(both userinput) divided by 2.

To turn the string input of the input function we can use the asInt function.

So to start, let's grab the two values of a and b.

```
var a = asInt(input)
var b = asInt(input)
```

Then we have to declare the function.
```
fn test(a,b){

}
```
And write it's contents:

```
fn test(a,b){
	print (a+b)/2
}
```

Then we can call it
```
test(a,b)
```

And there we have it.

Now functions can also return things.
We just need to use the return keyword.

Let's write the same function but with return.

```
fn test(a,b){
	return (a+b)/2
}
```

And now print it through a call.

```
print test(a,b)
```

And there, that's all for functions. Moving on...

# While If..

While and If statements run on comparison expressions.

If statements execute a block of code if the current expression resolves to true.
While statements continue executing that block of code until the current expression resolves to false.

Syntax for If statements: 
```if Expression { code }```
Syntax for While statements: 
```while Expression { code }```

So the possible operators for comparison are:

```>
<
>=
<=
==```

And it's pretty the same as in like basic math

1 < 2(resolves to true)
3 > 5(resolves to false)
and Et cetera...

This is simple aswell..

# Importing your own files.

Importing a file means just copying over the abstract syntax tree that is gotten from parsing the file into your main file, so you can use functions and variables defined in that file.

The syntax is ``import "FileName"``
And that will allow you to use the items defined in the file in your file.

Say you have a file

example.cx
```
var i = 5

fn example(a){
 return a*2
}
```

Then you had a file
main.cx
```
import "example.cx"

print example(i)
```

# Exercises

1. Try making 5 variables and add them together and divide by the first variable.
2. Make a small little math library.
3. Make something that can exponentiate a number by a selected number.
4. Make a small console adventure game.
5. Mathematical Game.
