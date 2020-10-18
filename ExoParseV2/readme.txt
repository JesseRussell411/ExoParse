ExoParse stands for ExpressionParser or math Expression Parser(the o makes it sound cooler, honestly though, I think I'll rename it in the future if it turns into something big, like that'll happen)

It's a system for parsing mathematical expressions containing double precision floating point numbers (literals), variables, constants, and functions.

literals: 5 4.5 2.334 0.3 9e-4

variables: a b velocity

constants: pi e true false

functions: sin(x) round(x, decimals)

These elements are linked together by operators and modifiers.

Operators: 1 + 2   4 * 7

Modifiers: -2   5!

*To find a full list of operators and modifiers, try looking in "/docs/list of operators and modifers.txt" in the source code.

At the moment, ExoParse is set up as a command line tool (found in app.program). However, there's no reason this system couldn't be adapted for back-end use or in other user interface components. Blender has a similar system for instance, which allows the user to enter an expression into text fields expecting numbers.


for example your could enter:
> 2+2
	4

> 2+2*3
	8

> a = 2
	2

> b := a

> a
	2

> b
	2

> a = 4
	4

> b
	4

you may have noticed the ":=" operator and it's effects on b in relation to a. This is the "set definition" operator; it sets the definition of a variable to the thing on the right; by contrast, the "=" operator is the "set equal" operator and it sets the variable's definition to the value of the thing on the right, ie: what gets returned when the thing on the right is executed. This can be thought of like a pointer. The value of what the variable points to get's returned when it's executed but other than that, the variable only points to it; it's not the same item. To dereference the variable and gain access to what it is pointing to, directly, precede it with the dereference modifier (*): *b returns a. Bare in mind that to you need to enter a space or some other whitespace to the beginning of the line if it starts with the dereference modifier. Without this whitespace, exoparse will insert ans (previouse answer) to the start and interpret the "*" as the multiplication operator.

for example:
b
	4

>  *b
a

You can take this further by using the dereferenced item in expressions:

>  *b = 42
	42

> c := *b
b

That last line sets c to b's definition.

a is now 42, b is still pointing to a, so its value is also 42. If we only entered: b = 42, without the *, then b would now be pointing to the literal: 42 and a would not have been touched.

Another feature of ExoParse is functions. It comes with a few standard built in functions like sin(x), cos(x), log(base, x), ln(x), ect. But what's more interesting is the ability to define new functions using the :def command.

:def sum(a, b, c) = a + b + c

Commands are a little like preprocessor directives in c, they tell ExoParse to do miscellaneous things like defining functions or just exiting. to see a list of available commands enter the command :help. note that to use a command, it has to start with ':'; This is known as the command operator and must be the first character in the statement for ExoParse to recognize the statement as a command. Additionally,  command names are not case sensitive so :HeLp works just a well as :help or :HELP

After defining sum(a, b, c), we can use it like any other functions. for example: sum(1,2,3) would return 6 and sum(3,3,3) would return 9.

Functions also support recursion allowing users to define more complex functions.Additionally, they can define yet more complex functions if they use ternary expressions and semicolon operators. These special operators allow for basic program flow. here's an example:

:def factorial(n) =    n <= 1   ?   1   :    n * factorial(n - 1)

This factorial function recursively calculates the factorial of n.

As of recently, three loops have been added in the form of functions: while, do while, and for.

I have also added a print function, so with both of these things, you can do some fun stuff.

such as:
--------

> :def fib(n) = n == 0 ? 0 : n == 1 ? 1 : fib(n - 1) + fib(n - 2)

> for(i = 0, i < 10, ++i, print($fib(i)))
for(i = 0, i < 10, ++i, print($fib(i)))
for(i = 0, i < 10, ++i, print($fib(i)))
0
1
1
2
3
5
8
13
21
34
        34