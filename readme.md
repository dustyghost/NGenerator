# NGenerator (Narrative Generator)

NGenerator (the N stands for Narrative) is a template and tag substitution system. 
Supply the code some rules in the form of either Json or the `InputHolder` model, 
and it will convert the input into a random set of strings.
This is usful for building all sorts of procedurally generated narratives. 

## Code setup

Setup is really easy, just import the `NGenerator.Processors` namespace.

When you instantiate `NGenerratorProcessor`. 

You don't have to supply a `Random()` instance to the consructor, as the code will create one internally if not supplied. 
But if you want to control the seed, the example below shows how. 

Then you just call the `Process(string input)` method, passing your Json string. 

```csharp
using NGenerator.Processors;
       
 public void Generate()
{
    var rnd = new Random(1);

    var nGeneratorProcessor = new NGenerratorProcessor(rnd);
    string input = "{\"Template\":\"Hello {{sub}}!\",\"Tags\":{\"sub\":[\"World\",\"You\"]}}";

    var result = nGeneratorProcessor.Process(input);
    Console.Write(result); //Hello World!
}
```

If you wish, instead of using Json, you can pass an instance of `NGenerator.Models.InputHolder` model to the Processor.

```csharp
using NGenerator.Processors;
       
public void UseInputHolder()
{
    var rnd = new Random(1);
    var nGeneratorProcessor = new NGenerratorProcessor(rnd);

    InputHolder inputHolder = new InputHolder
    {
        Template = "Hello {{sub}}!",
        Tags = new Dictionary<string, string[]>
        {
            { "sub", new string[] {"World", "You"} }
        }
    };

    var result = nGeneratorProcessor.Process(inputHolder);

    Console.Write(result); //Hello World!
}
```

## Basic Inputs

This is made up of the `Template` and `Tags`. 

In the example below, the `{{sub}}` will be replaced with one of the strings in the `Tags.sub` array.

So the results of this could be `Hello World!` or `Hello You!`

For example:

```json
{
  "Template": "Hello {{sub}}!",
  "Tags": {
    "sub": [
      "World",
      "You"
    ]
  }
}
```


## Tags in Tags

You can even have tags in tags. These will be replaced recursively, allowing for very powerful string generation.

```json
{
  "Template": "My Name is {{name}}!",
  "Tags": {
    "surname": [
      "Johnson",
      "Smith"
    ],
    "name": [
      "John {{surname}}"
    ]
  }
}
```
So in the previous example, this would end up either `John Smith` or `John Johnson`

## Pluralization

You can tell the generator to pluralize a word by using the `.p` tag modifier

```json
{
  "Template": "{{name}} really {{feels.p}} oranges!",
  "Tags": {
    "surname": [
      "Smith"
    ],
    "name": [
      "John {{surname}}"
    ],
    "feels": [
      "love",
      "hate"
    ]
  }
}
```

In this example, `love` becomes `loves`, `hate` becomes `hates`
for example: `John Smith really loves oranges!` 

## Plural Definitions

You can also force the generator to use certain words for plurals, using the `::p.` definition. For example, the plural of fish is "fish".

```json
{
  "Template": "John really likes {{thing.p}}!",
  "Tags": {
    "thing": [
      "fish:p.fish"
    ]
  }
}
```
This will produce `John really likes fish!`, not fishes or fishs :)

## Upper and Lower Case

There are a few modifiers you can use to change the case of a word.

- u = uppercase first letter of word
- U = uppercase whole word
- l = lowercase whole word

for example, in the following json, `john` will be selected and become `John`:

```json
{
  "Template": "{{name.u}} really {{feels.p}} oranges!",
  "Tags": {
    "surname": [
      "smith"
    ],
    "name": [
      "john {{surname.u}}"
    ],
    "feels": [
      "love",
      "hate"
    ]
  }
}
```

## Add 'a' or 'an' Prefix

You can prefix a word with `a` or `an` using the `.a` modifier. 
This is limited to putting `an` in front of a, e, i, o and u. And will put `a` in front of the rest.

This is obviously not ideal for words like `unicorn` which is `a unicorn`

So you can use the `::a.` definition.

For example:

```json
{
  "Template": "John really wants {{thing.a}}!",
  "Tags": {
    "thing": [
      "unicorn:a.a"
    ]
  }
}
```

This will produce `John really wants a unicorn!`

So if it was `"unicorn:a.an"` then the result would have been `John really wants an unicorn!`. 
Obviously this is incorrect, but shows how this functionality works.

## Reserve Substitutions

So as an example, we want to ensure that once a substituted tag happens, we use the same word for the next instance of that tag. 
In this case we can use the number modifier. For example `.1` 

```json
{
  "Template": "{{sentance}}",
  "Tags": {
    "sentance": [
      "{{name.u.1}} was happy with {{thing.1.p}}, {{name.u.1}} was kind to one {{thing.1}}. The {{thing.1}} was always kind to {{name.u.2}}!"
    ],
    "name": [
      "john",
      "pete",
      "alph"
    ],
    "feels": [
      "happy"
    ],
    "thing": [
      "puppy",
      "cat"
    ]
  }
}
```

So in the previous example, if `{{name.1}}` returns john, the next time `{{name.1}}` is found then `john` would be used.

Note, other modifiers are applied after the reserve. So `{{name.1}}` which would make `john`, 
if `{{name.u.1}}` was the next to be encountered then `John` would be returned (uppercase first letter, but same word).

Also, in the previous example, if `John` was selected to be reserved for `{{name.1}}` and the next tag was `{{name.2}}`,
the process will select from the remaining unreserved names, so ` pete` or `alph` would be selected.
If there are no more unreserved names available, then the process will just pick a random one from all the names, reserved or not.

## Credit where it is due

This code was inspired by, but not a direct port of [Tracery](http://tracery.io/). There are differences in the way each of these function, but I felt it only right to attribute the inspiration for this project to [GalaxyKate](http://www.galaxykate.com/)
