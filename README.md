# fuse
Fuse is a tiny package that let's you attach properties to .NET types dynamically at runtime.

It provides 3 extension methods that can be used with any `object?`:
1. SetFused
2. GetFused
3. Fused

## SetFused

The `SetFused` method can be used to associate or attach an arbitrary property to any .net object using a string key.
 
```csharp
var myObject = "A simple string";
var myClass = new MyClass { Foo = 42, Bar = "Success!" };
myObject.SetFused("foobar", myClass);

class MyClass 
{
    public int Foo {get; set; }
    public string Bar {get; set; }
}
```

## GetFused

The `GetFused` method can be used to retrieve properties that have been fused to an object.

```csharp
var luckyNumber = myObject.GetFused<MyClass>("foobar")!.Foo;
var result = myObject.GetFused<MyClass>("foobar")!.Bar;
```

## Fused

`Fused` is probably the most useful method. It lets you automatically bolt an additional property onto any object. You
can see an example of how this is used in the `Sample` application, but it looks something like this in action:

```csharp
// We start with a humble string
string brendan = "Brendan Eich (/ˈaɪk/; born July 4, 1961)";
UnpackDetails(brendan);
// And now our string has a aggregate Person property fused to it... no initialisation necessary - Person just
// has to have a parameterless constructor
var firstName = brendan.Fused<Person>().FirstName;

void UnpackDetails(string input)
{
    Regex regex = new Regex(@"(?<first>\w+) (?<last>\w+) \(.*; born (?<dob>.+)\)");
    Match match = regex.Match(input);

    var dob = match.Groups["dob"].Value;
    // Here we fuse some additional properties to the string so that we can use these later on
    input.Fused<Person>().FirstName = match.Groups["first"].Value;
    input.Fused<Person>().LastName = match.Groups["last"].Value;
    input.Fused<Person>().DateOfBirth = DateTime.ParseExact(dob, "MMMM d, yyyy", CultureInfo.InvariantCulture);
}

class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}
```