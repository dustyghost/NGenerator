using NGenerator.Processors;
using System;
using Xunit;

namespace NGenerator.Tests
{
    public class NGeneratorProcessorTests
    {
        [Fact]
        public void RandomChoice()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\"Template\":\"Hello {{sub}}!\",\"Tags\":{\"sub\":[\"World\",\"You\"]}}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("Hello World!", result);
        }

        [Fact]
        public void SubItemStringReplace()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\n  \"Template\": \"My Name is {{name}}!\",\n  \"Tags\": {\n    \"surname\": [\n      \"Johnson\",\n      \"Smith\"\n    ],\n    \"name\": [\n      \"John {{surname}}\"\n    ]\n  }\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("My Name is John Johnson!", result);
        }

        [Fact]
        public void PluralizeWord()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\n  \"Template\": \"{{name}} really {{feels.p}} oranges!\",\n  \"Tags\": {\n    \"surname\": [\n      \"Smith\"\n    ],\n    \"name\": [\n      \"John {{surname}}\"\n    ],\n    \"feels\": [\n      \"love\",\n      \"hate\"\n    ]\n  }\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("John Smith really loves oranges!", result);
        }

        [Fact]
        public void PluralizeWord2()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\n  \"Template\": \"{{name}} really {{feels.p}} {{thing.p}}!\",\n  \"Tags\": {\n    \"surname\": [\n      \"Smith\"\n    ],\n    \"name\": [\n      \"John {{surname}}\"\n    ],\n    \"feels\": [\n      \"love\",\n      \"hate\"\n    ],\n    \"thing\": [\n      \"bus\",\n    ]\n  }\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("John Smith really loves buses!", result);
        }

        [Fact]
        public void UpperCaseFirstLetter()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\n  \"Template\": \"{{name.u}} really {{feels.p}} oranges!\",\n  \"Tags\": {\n    \"surname\": [\n      \"smith\"\n    ],\n    \"name\": [\n      \"john {{surname.u}}\"\n    ],\n    \"feels\": [\n      \"love\",\n      \"hate\"\n    ]\n  }\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("John Smith really loves oranges!", result);
        }

        [Fact]
        public void UpperCaseAll()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\n  \"Template\": \"{{name.u}} really {{feels.p.U}} oranges!\",\n  \"Tags\": {\n    \"surname\": [\n      \"smith\"\n    ],\n    \"name\": [\n      \"john {{surname.u}}\"\n    ],\n    \"feels\": [\n      \"love\",\n      \"hate\"\n    ]\n  }\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("John Smith really LOVES oranges!", result);
        }

        [Fact]
        public void LowerCaseAll()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\n  \"Template\": \"{{name.u}} really {{feels.p.l}} oranges!\",\n  \"Tags\": {\n    \"surname\": [\n      \"smith\"\n    ],\n    \"name\": [\n      \"john {{surname.u}}\"\n    ],\n    \"feels\": [\n      \"LOVE\",\n      \"HATE\"\n    ]\n  }\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("John Smith really loves oranges!", result);
        }

        [Fact]
        public void PluralDefinition()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\n  \"Template\": \"John really likes {{thing.p}}!\",\n  \"Tags\": {\n    \"thing\": [\n      \"fish::p.fish\"\n    ]\n  }\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("John really likes fish!", result);
        }

        [Fact]
        public void AnOrADefinition()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\n  \"Template\": \"John really wants {{thing.a}}!\",\n  \"Tags\": {\n    \"thing\": [\n      \"unicorn::a.a\"\n    ]\n  }\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("John really wants a unicorn!", result);
        }

        [Fact]
        public void ReserveMod()
        {
            //Arrange

            var rnd = new Random(1);
            var nGeneratorProcessor = new NGenerratorProcessor(rnd);
            string input = "{\r\n  \"Template\": \"{{sentance}}\",\r\n  \"Tags\": {\r\n    \"sentance\": [\r\n      \"{{name.u.1}} was happy with {{thing.1.p}}, {{name.u.1}} was kind to one {{thing.1}}. The {{thing.1}} was always kind to {{name.u.2}}!\"\r\n    ],\r\n    \"name\": [\r\n      \"john\",\r\n      \"pete\"\r\n    ],\r\n    \"feels\": [\r\n      \"happy\"\r\n    ],\r\n    \"thing\": [\r\n      \"puppy\",\r\n      \"cat\"\r\n    ]\r\n  }\r\n}";

            //Act

            var result = nGeneratorProcessor.Process(input);

            //Assert

            Assert.Equal("John was happy with puppies, John was kind to one puppy. The puppy was always kind to Pete!", result);
        }
    }
}