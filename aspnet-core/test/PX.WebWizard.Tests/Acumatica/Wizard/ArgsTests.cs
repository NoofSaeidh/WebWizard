using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using PX.WebWizard.Acumatica.Wizard;


namespace PX.WebWizard.Tests.Acumatica.Wizard
{
    public class ArgsTests
    {
        [Theory]
        [InlineData("Boolean", true)]
        [InlineData("String", "somevalue")]
        [InlineData("Int", 15)]
        public void Indexer_SetsAndGetsValue(string key, object value)
        {
            // Arrange
            var args = new Args();
            // Act
            args[key] = value;
            // Assert
            Assert.Equal(value, args[key]);
        }

        [Theory]
        [InlineData("boolean", true, "-boolean:\"True\"")]
        [InlineData("String", "somevalue", "-String:\"somevalue\"")]
        [InlineData("int", 15, "-int:\"15\"")]
        public void SerializeArgument_WorksForDictionary(string key, object value, string expectation)
        {
            // Arrange
            var args = new Args
            {
                [key] = value
            };
            // Act
            var result = args.SerializeArgument(key);

            // Assert
            Assert.Equal(expectation, result);
        }

        [Fact]
        public void SerializeArgument_WorksForProperty()
        {
            // Arrange
            var args = new MockArgs();
            args.Argument = "value";
            // Act
            var result = args.SerializeArgument(nameof(MockArgs.Argument));
            // Assert
            Assert.Equal("-argument:\"value\"", result);
        }

        [Fact]
        public void SerializeArgument_WorksForProperty_SetProperty_GetByKey()
        {
            // Arrange
            var args = new MockArgs();
            args.Argument = "value";
            // Act
            var result = args.SerializeArgument("argument");
            // Assert
            Assert.Equal("-argument:\"value\"", result);
        }

        [Fact]
        public void SerializeArgument_WorksForProperty_SetByKey_GetProperty()
        {
            // Arrange
            var args = new MockArgs();
            args["argument"] = "value";
            // Act
            var result = args.SerializeArgument(nameof(MockArgs.Argument));
            // Assert
            Assert.Equal("-argument:\"value\"", result);
        }

        [Fact]
        public void SerializeArgument_WorksForEnumerableProperty()
        {
            // Arrange
            var args = new MockArgs();
            args.Enumerable = new object[] { "a", 1, true };
            // Act
            var result = args.SerializeArgument(nameof(MockArgs.Enumerable));
            // Assert
            Assert.Equal("-enumerable:\"a\" -enumerable:\"1\" -enumerable:\"True\"", result);
        }

        [Fact]
        public void SerializeArgument_WorksForObjectProperty()
        {
            // Arrange
            var args = new MockArgs();
            args.Object = new MockArgs.MockObject
            {
                Int = 1,
                String = "a",
                Object = null
            };
            // Act
            var result = args.SerializeArgument(nameof(MockArgs.Object));
            // Assert
            Assert.Equal("-object:\"Int=1;String=a;\"", result);
        }

        [Fact]
        public void SerializeArgument_WorksForEnumerableObjectProperty()
        {
            // Arrange
            var args = new MockArgs();
            args.EnumerableObject = new MockArgs.MockObject[]
            {
                new MockArgs.MockObject
                {
                    Int = 1,
                    String = "a",
                    Object = null
                },
                new MockArgs.MockObject
                {
                    Int = 2,
                    String = "",
                    Object = false
                },
            };
            // Act
            var result = args.SerializeArgument(nameof(MockArgs.EnumerableObject));
            // Assert
            Assert.Equal("-enumerable_object:\"Int=1;String=a;\" -enumerable_object:\"Int=2;String=;Object=False;\"", result);
        }

        [Fact]
        public void Serialize_WorksForKeys()
        {
            // Arrange
            var args = new Args
            {
                ["boolean"] = true,
                ["String"] = "somevalue",
                ["int"] = 15
            };
            // Act
            var result = args.Serialize();
            // Assert
            Assert.Equal("-boolean:\"True\" -String:\"somevalue\" -int:\"15\"", result);
        }

        [Fact]
        public void Serialize_WorksForProperties()
        {
            // Arrange
            Args args = new MockArgs
            {
                Enumerable = new[] { "1", "a" },
                Object = new MockArgs.MockObject { Int = 1, String = "string" },
                Argument = false
            };
            // Act
            var result = args.Serialize();
            // Assert
            Assert.Contains("-argument:\"False\"", result);
            Assert.Contains("-enumerable:\"1\" -enumerable:\"a\"", result);
            Assert.Contains("-object:\"Int=1;String=string;\"", result);
        }

        [Fact]
        public void Serialize_WorksForKeysAndProperties()
        {
            // Arrange
            Args args = new MockArgs
            {
                ["boolean"] = true,
                ["String"] = "somevalue",
                ["int"] = 15,

                Enumerable = new[] { "1", "a" },
                Object = new MockArgs.MockObject { Int = 1, String = "string" },
                Argument = false
            };
            // Act
            var result = args.Serialize();
            // Assert
            Assert.Contains("-argument:\"False\"", result);
            Assert.Contains("-enumerable:\"1\" -enumerable:\"a\"", result);
            Assert.Contains("-object:\"Int=1;String=string;\"", result);
            Assert.Contains("-boolean:\"True\"", result);
            Assert.Contains("-String:\"somevalue\"", result);
            Assert.Contains("-int:\"15\"", result);
        }

        [Fact]
        public void ToListExtension_Works()
        {
            // Arrange
            Args args = new MockArgs
            {
                ["String"] = "somevalue",
                ["int"] = 15,

                Argument = false
            };
            // Act
            var result = args.ToList();
            // Asert
            Assert.Contains(NewPair("String", "somevalue"), result);
            Assert.Contains(NewPair("int", 15), result);
            Assert.Contains(NewPair("argument", false), result);
        }

        [Fact]
        public void Enumerator_WorksForKeysAndProperties()
        {
            // Arrange
            Args args = new MockArgs
            {
                ["String"] = "somevalue",
                ["int"] = 15,

                Argument = false
            };
            // Act
            var result = args.Select(x=>x).ToList();
            // Asert
            Assert.Contains(NewPair("String", "somevalue"), result);
            Assert.Contains(NewPair("int", 15), result);
            Assert.Contains(NewPair("argument", false), result);
        }

        private KeyValuePair<string, object> NewPair(string key, object value) => new KeyValuePair<string, object>(key, value);
    }

    public class MockArgs : Args
    {
        [Argument("argument")]
        public object Argument { get; set; }

        [Argument("enumerable", ArgumentType = ArgumentType.Enumerable)]
        public object[] Enumerable { get; set; }

        [Argument("object", ArgumentType = ArgumentType.Object)]
        public MockObject Object { get; set; }

        [Argument("enumerable_object", ArgumentType = ArgumentType.Object | ArgumentType.Enumerable)]
        public MockObject[] EnumerableObject { get; set; }

        public class MockObject
        {
            public int Int { get; set; }

            public string String { get; set; }

            public object Object { get; set; }
        }
    }
}
