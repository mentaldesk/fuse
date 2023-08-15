# fuse
Fuse is a tiny package that let's you attach properties to .NET types dynamically at runtime. This can be useful if you
want to associate data or meta-data with types that you can't change. 

The `SetFused` and `GetFused` methods can be used to set/get arbitrary properties on any .net object. In the following 
example an instance of `MyClass` is assigned to the fused property `foobar` on `myObject`:
 
```csharp
var myObject = "A simple string";
var myClass = new MyClass { Foo = 42, Bar = "Success!" };
myObject.SetFused("foobar", myClass);

var luckyNumber = myObject.GetFused<MyClass>("foobar")!.Foo;
var result = myObject.GetFused<MyClass>("foobar")!.Bar;

class `MyClass` 
{
    public int Foo {get; set; }
    public string Bar {get; set; }
}
```

## Generic overloads

Alternatively you can use the Generic overloads, `SetFused<T>` and `GetFused<T>`, which automatically assign a property
name based on the type of the value being stored/retrieved. In the following example an instance of `MyClass` is 
assigned to the fused property `MyClass` on `myObject`:

```csharp
var myObject = "A simple string";
var myClass = new MyClass { Foo = 42, Bar = "Success!" };
myObject.SetFused<MyClass>(myClass);

var luckyNumber = myObject.GetFused<MyClass>()!.Foo;
var result = myObject.GetFused<MyClass>()!.Bar;

class MyClass 
{
    public int Foo {get; set; }
    public string Bar {get; set; }
}
```

If you don't need to fuse more than one instance of a particular type with an object, you might find this syntax more 
concise.

## Fused

Finally, the `Fused` extension lets you automatically create and bolt an additional property onto any object. For this
to work, the Type you're fusing must have a parameterless constructor.

You can see an example of how this is used in the `Sample` application, but it looks something like this in action:

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