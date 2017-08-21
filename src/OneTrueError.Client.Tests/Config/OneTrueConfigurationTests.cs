using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using OneTrueError.Client.Config;
using OneTrueError.Client.Uploaders;
using Xunit;

namespace OneTrueError.Client.Tests.Config
{
    public class OneTrueConfigurationTests
    {
        [Fact]
        public void
            ContextProviders_should_be_initialized_so_that_new_providers_can_Be_added_without_NullReference_checks()
        {

            var sut = new OneTrueConfiguration();

            sut.ContextProviders.Should().NotBeNull();
        }

        [Fact]
        public void
            FilterCollection_should_be_initialized_so_that_new_filters_can_Be_added_without_NullReference_checks()
        {

            var sut = new OneTrueConfiguration();

            sut.FilterCollection.Should().NotBeNull();
        }

        [Fact]
        public void default_number_of_properties_per_object_should_Be_100_to_avoid_too_large_error_reports()
        {

            var sut = new OneTrueConfiguration();

            sut.MaxNumberOfPropertiesPerCollection.Should().Be(100);
        }

        [Fact]
        public void should_per_default_ask_users_for_Details_to_make_it_easier_to_figure_out_why_the_exception_happened()
        {

            var sut = new OneTrueConfiguration();

            sut.UserInteraction.AskUserForDetails.Should().BeTrue();
        }

        [Fact]
        public void should_throw_library_exceptions_per_default_so_that_invalid_configurations_can_be_discovered()
        {

            var sut = new OneTrueConfiguration();

            sut.ThrowExceptions.Should().BeTrue();
        }

        [Fact]
        public void using_credentials_config_should_add_our_own_uploaded_with_the_Specified_information()
        {

            var sut = new OneTrueConfiguration();
            sut.Credentials(new Uri("http://localhost/abc"), "aaa", "bbb");
            var actual = (UploadToOneTrueError)sut.Uploaders.First();

            actual.ApiKey.Should().Be("aaa");
            actual.SharedSecret.Should().Be("bbb");
        }


    }
}
