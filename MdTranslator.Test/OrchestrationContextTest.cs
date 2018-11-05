using System;
using System.Collections.Generic;
using Xunit;
using MdTranslator;

namespace MdTranslator.Test
{
    public class OrchestrationContextTest
    {
        [Fact]
        public void TestConvert_NormalCase()
        {
            var parentContext = new OrchestrationContext()
            {
                Owner = "foo",
                Repo = "bar",
                Language = "ja",
                SourceBranch = "baz",
                SourceSha = "ab01dd388ga",
                TargetBranch = "qux",
                TargetSha = "dc0319a395b",
                Paths = new string[] {"a", "b", "c"}
            };
            var ExpectedPath = "quux";
            var targetContext = parentContext.ConvertSubOrchestrationContext(ExpectedPath);
            Assert.Equal(parentContext.Owner, targetContext.Owner);
            Assert.Equal(parentContext.Repo, targetContext.Repo);
            Assert.Equal(parentContext.Language, targetContext.Language);
            Assert.Equal(parentContext.SourceBranch, targetContext.SourceBranch);
            Assert.Equal(parentContext.SourceSha, targetContext.SourceSha);
            Assert.Equal(parentContext.TargetBranch, targetContext.TargetBranch);
            Assert.Equal(parentContext.TargetSha, targetContext.TargetSha);
            Assert.Equal(ExpectedPath, targetContext.Path);


        }
    }
}
