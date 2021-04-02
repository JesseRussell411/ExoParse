# ExoParse
Math expression parser written in C# for .netcore

﻿ExoParse stands for ExpressionParser or math Expression Parser.
 
In ExoParseV1, version 1 can be found. This version is now deprecated. It's more like the prototype for version 2, a proof of concept.

In ExoParseV2, version 2 can be found. This is the latest version. The program class and main method/entry point is in ExoParseV2/app/Program.cs
 More information can be found in ExoparseV2/readme.txt

I've actually found myself using this tool for homework because I know it better than anything else.



Some examples of V2:
# Find and count the primes between two numbers:
:def isPrime(num) = n = abs(num); if(n <= 2, true, if(n % 2 == 0, false, {sqr = int(ceil(n^0.5)); res = true; for(i = 3, i <= sqr, i += 2, if(n % i == 0, {res = false; i = sqr + 1})); res}))

:count = 0; for(i = 10000000000, i < 10000001000, ++i, if(isPrime(i), ++count; print(i))); count


# Factorial and recursion demonstation:
> :def factorial(n) = if(n > 1, n * factorial(n - 1), 1)
factorial(n) has been created.

> factorial(5)
factorial(5)

        120


> factorial(170)
factorial(170)

        7257415615307998967396728211129263114716991681296451376543577798900561843401706157852350749242617459511490991237838520776666022565442753025328900773207510902400430280058295603966612599658257104398558294257568966313439612262571094946806711205568880457193340212661452800000000000000000000000000000000000000000


> factorial(170) == 170!
factorial(170) == 170!

        1


>



