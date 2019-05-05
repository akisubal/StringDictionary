# StringDictionary
## Usage

�쐬
            var builder = new StringDictionaryBuilder<int>();
            builder.Add("ab", 3);
            builder.Add("ac", 4);
            builder.Add("abc", 5);
            var dictionary = builder.Build();


���p
            Assert.IsNotNull(dictionary);

            Assert.IsTrue(dictionary.Contains("ab"));
            Assert.IsFalse(dictionary.Contains("efg"));
            Assert.IsFalse(dictionary.Contains("a"));

            Assert.IsTrue(dictionary["ab"] == 3);
            Assert.IsTrue(dictionary["ac"] == 4);
            Assert.IsFalse(dictionary.TryGet("a", out int a_value));
            Assert.IsTrue(dictionary.TryGet("abc", out int abc_value));
            Assert.IsTrue(abc_value == 5);