/*
 * This sample shows how additional properties can be fused to existing objects. The properties will disappear when the
 * object they're fused to gets disposed or garbage collected.
 */

using System.Globalization;
using System.Text.RegularExpressions;
using MentalDesk.Fuse;

// We start with some simple strings
string brendan = "Brendan Eich (/ˈaɪk/; born July 4, 1961)";
string roberto = "Roberto Ierusalimschy (Brazilian Portuguese: [ʁoˈbɛʁtu jeɾuzaˈlĩski]; born May 21, 1960)";
string grace = "Grace Hopper (née Murray; born December 9, 1906)";
var people = new List<string>
{
    brendan, roberto, grace
};

foreach (var person in people)
{
    // The UnpackDetails function will fuse some additional properties onto our strings
    UnpackDetails(person);
    UnpackDetails(brendan);
}

// The UpdateRecords function will fuse a property onto the List<string>
UpdateRecords(people);

// And now we can use all those super powers that have been added to our humble types!
var youngest = people.Fused<Records>().Youngest!;
var oldest = people.Fused<Records>().Oldest!;
Console.WriteLine($"Our youngest computer legend is {youngest.FirstName}  {youngest.LastName}, born in {youngest.DateOfBirth.Year}");
Console.WriteLine($"Our oldest computer legend is {oldest.FirstName}  {oldest.LastName}, born in {oldest.DateOfBirth.Year}");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

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

void UpdateRecords(List<string> population)
{
    foreach (var person in population)
    {
        population.Fused<Records>().Youngest ??= person.Fused<Person>();
        population.Fused<Records>().Oldest ??= person.Fused<Person>();
        if (person.Fused<Person>().DateOfBirth > population.Fused<Records>().Youngest!.DateOfBirth)
        {
            population.Fused<Records>().Youngest = person.Fused<Person>();
        }
        if (person.Fused<Person>().DateOfBirth < population.Fused<Records>().Oldest!.DateOfBirth)
        {
            population.Fused<Records>().Oldest = person.Fused<Person>();
        }
    }
}

class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}

class Records
{
    public Person? Oldest { get; set; }
    public Person? Youngest { get; set; }
}