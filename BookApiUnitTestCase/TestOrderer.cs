using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BookApiUnitTestCase
{
    public class TestOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
        {
            return testCases.OrderBy(tc =>
            {
                var attr = tc.TestMethod.Method.GetCustomAttributes(typeof(TestPriorityAttribute))
                    .FirstOrDefault();

                return attr == null ? 0 : attr.GetNamedArgument<int>("Priority");
            });
        }
    }
}
