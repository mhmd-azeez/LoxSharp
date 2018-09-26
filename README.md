# LoxSharp
Lox is a little programming language created by [Bob Nystrom](https://twitter.com/intent/user?screen_name=munificentbob) in his book [Crafting Interpreters](http://www.craftinginterpreters.com/). This is my attempt at recreating it in C#. I've published a web based version of the interpreter [here](https://encrypt0r.github.io/LoxSharp/).

## What's implemented
 The interprer is still a work in progress as I've not finished the book yet.
 
## Variables
Lox is a dynamically typed language, variables can have values of three types: `nil`, `boolean` and `string`.

```kotlin
var x = 5;
var y;
y = 10;
var z = x * y;
print(z);
```

## If conditional
```kotlin
var x = 250;
if (x % 2 == 0)
{
    print("Even");
}
else
{
    print("Odd");
}
```
## Loops

### While loop
```kotlin
var x = 0;
while(x < 10)
{
    if (x > 5)
        break;
        
    print(x);
    x = x + 1;
}
```
#### For loop
```kotlin
for (var i = 0; i < 10; i = i + 1)
{
    print("i is: " + i);
}
```
