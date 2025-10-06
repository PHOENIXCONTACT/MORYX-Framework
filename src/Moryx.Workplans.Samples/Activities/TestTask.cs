// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using System.ComponentModel.DataAnnotations;

namespace Moryx.Workplans.Samples.Activities
{
    [Display(Name = "Test", Description = "Task which does something with a product")]
    public class TestTask : TaskStep<TestActivity, TestParameters>
    {
        private TestTask() { }

        public TestTask(string testString, int testInt)
        {
            TestInt = testInt;
            TestString = testString;
        }

        [Display(Name = "Test Integer Property", Description = "This is a Description of an Integer Property")]
        public int TestInt { get; set; }


        [Display(Name = "Test String Property", Description = "This is a Description of an String Property")]
        public string TestString { get; set; }
    }

    [Display(Name = "Test without Constructor and a very long name", Description = "Task which does something with a product")]
    public class TestTaskWithoutConstructor : TaskStep<TestActivity, TestParameters>
    {
        [Display(Name = "Test Integer Property", Description = "This is a Description of an Integer Property, which cannot be set by a constructor")]
        public int TestInt { get; set; }


        [Display(Name = "Test String Property", Description = "This is a Description of an String Property, which cannot be set by a constructor")]
        public string TestString { get; set; }
    }

}

