ExoParse stands for ExpressionParser or math Expression Parser(the o makes it sound cooler)

It's a system for parsing mathematical expressions containing double precision floating point numbers (literals), variables, constants, and functions.

literals: 5 4.5 2.334 0.3 9e-4

varaibles: a b velocity

constants: pi e true false

functions: sin(x) round(x, decimals)

These elements are linked together by operators and modifiers.

Operators: 1 + 2   4 * 7

Modfiers: -2   5!

At the moment, ExoParse is set up as a command line tool (found in app.program). However, there's no reason this system couldn't be used for the backend or in other use interface components. Blender has a similar system for instance, which allows the user to end an expression into text fields expecting numbers.