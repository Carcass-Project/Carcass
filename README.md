![Nuget](https://img.shields.io/nuget/dt/Carcass?color=%2311FFF&label=NuGet%20Package%20Downloads)

# Carcass
Carcass is a basic programming language that's suppost to simple for beginners and not that hard on the person writing a program in it.
It was made using the Yoakke Lexer/Parser generator and some intense feelings.
If you'll be using the Carcass programming language for your project, please email me with your project, as I'm interested in seeing it!
If I do see your email, I will surely reply.

///INFO: If you're using CarcassHub, you NEED to have the .NET 5.0 runtime installed so it can work.///

# How to Build?
To build Carcass you just need to clone the repository and well run ``dotnet build``.
It's as simple as that.

# How do I contribute to the project?
You can always make a pull request with what changes you have made, I will check it at some point of time and approve it if everything great.

# Sample:

```
fn fib(n)
{
  if n < 2 
  {
    return 1
  }
  return fib(n - 1) + fib(n - 2)
}

fib(7)
```
