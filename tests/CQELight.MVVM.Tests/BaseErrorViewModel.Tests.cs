using CQELight.TestFramework;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace CQELight.MVVM.Tests
{
    public class BaseErrorViewModelTests : BaseUnitTestClass
    {
        #region Ctor & members

        private class TestErrorViewModel : BaseErrorViewModel
        {
            public void SimulateError() => AddError("test error", "testProperty");
            public void SimulateFix() => ClearErrors("testProperty");
        }
        #endregion

        #region GetErrors

        [Fact]
        public void BaseErrorViewModel_GetErrors_EmptyProperty()
        {
            var vm = new TestErrorViewModel();

            vm.GetErrors(null).Should().BeNull();
        }

        [Fact]
        public void BaseErrorViewModel_GetErrors_NoErrorsForProperty()
        {
            var vm = new TestErrorViewModel();

            vm.GetErrors("myProperty").Should().BeNull();
        }

        [Fact]
        public void BaseErrorViewModel_GetErrors_Should_Get_SimulatedError()
        {
            var vm = new TestErrorViewModel();
            vm.SimulateError();

            vm.HasErrors.Should().BeTrue();
            var e = vm.GetErrors("testProperty");
            e.Should().NotBeNull();
            e.Cast<string>().Should().HaveCount(1);
        }

        [Fact]
        public void BaseErrorViewModel_GetErrors_Should_NotGet_SimulatedError_AfterFix()
        {
            var vm = new TestErrorViewModel();
            vm.SimulateError();

            vm.HasErrors.Should().BeTrue();
            var e = vm.GetErrors("testProperty");
            e.Should().NotBeNull();
            e.Cast<string>().Should().HaveCount(1);

            vm.SimulateFix();
            vm.HasErrors.Should().BeFalse();

            e = vm.GetErrors("testProperty");
            e.Should().BeNull();
        }

        #endregion

    }
}
