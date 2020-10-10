ExoParse stands for ExpressionParser or math Expression Parser(the o makes it sound cooler)

It's a system for parsing mathematical expressions containing double precision floating point numbers (literals), variables, constants, and functions.

literals: 5 4.5 2.334 0.3 9e-4

varaibles: a b velocity

constants: pi e true false

functions: sin(x) round(x, decimals)

These elements are linked together by operators and modifiers.

Operators: 1 + 2   4 * 7

Modfiers: -2   5!

At the moment, ExoParse is set up as a command line tool (found in app.program). However, there's no reason this system couldn't be used for the backend or in other use interface components. Blender has a similar system for instance, which allows the user to enter an expression into text fields expecting numbers.


for example your could enter:
2+2
	4

2+2*3
	8

a = 2
	2

b := a
	2

a
	2

b
	2

a = 4
	4

b
	4

you may have noticed the ":=" operator and it's effects on b in relation to a. This is the "set definition" operator; it sets the definition of a variable to the thing on the right; by contrast, the "=" operator is the "set equal" operator and it sets the variable's definition to the value of the thing on the right, ie: what gets returned when the thing on the right is executed. This can be thought of like a pointer. The value of what the variable points to get's returned when it's executed but other than that, the variable only points to it; it's not the same item. To dereference the variable and gain access to what it is pointing to, directly, precede it with the dereference modifier ($): $b returns a.

for example:
b
	4

$b
a <- there's a (normally b would be here, I've been excluding this line for brevity until now)
	4

You can take this further by using the dereferenced item in expressions:

$b = 42
	42

a is not 42, b is still pointing to a, so its value is also 42. If we only entered: b = 42, without the $, then b would now be pointing to the literal: 42 and a would not have been touched.