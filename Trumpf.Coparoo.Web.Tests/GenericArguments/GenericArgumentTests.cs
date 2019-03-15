// Copyright 2016, 2017, 2018 TRUMPF Werkzeugmaschinen GmbH + Co. KG.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Trumpf.Coparoo.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GenericArgumentTests
    {
        [TestMethod]
        public void WhenTryingToFindControlObjectWithGenericArgument_CorrectTypeIsReturned()
        {
            // during resolve it should not hit unrelated types like DialogControlObject
            var expected = new RootObject().Find<ListBoxControlObject<IListDataTemplateObject>>();
            var proc = new RootObject().Find<IListBoxControlObject<IListDataTemplateObject>>();

            Assert.AreEqual(expected.GetType(), proc.GetType());
            Assert.AreEqual(typeof(ListDataTemplateObject), proc.Value.GetType());
        }

        [TestMethod]
        public void WhenTryingToFindControlObjectWithGenericArgument_NoExceptionIsThrown()
            => new RootObject().Find<IListBoxControlObject<IListDataTemplateObject>>();
    }
}
