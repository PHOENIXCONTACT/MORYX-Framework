// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.ControlSystem.Jobs;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationDataReportTests : OperationDataTestBase
    {
        [Test(Description = "Reporting amounts less than 0 should fail. Will test SuccessCount and FailureCount.")]
        public void ReportAmountsLessThanZero()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 10, 10);

            // Act - Assert: SuccessCount < 0
            var report = new OperationReport(ConfirmationType.Partial, -1, 5, User);
            Assert.Throws<ArgumentException>(() => operationData.Report(report), "Negative SuccessCount should not be reported");

            // Act - Assert: FailureCount < 0
            report = new OperationReport(ConfirmationType.Partial, 5, -1, User);
            Assert.Throws<ArgumentException>(() => operationData.Report(report), "Negative FailureCount should not be reported");

            // Act - Assert: SuccessCount < 0 && FailureCount < 0
            report = new OperationReport(ConfirmationType.Partial, -1, -1, User);
            Assert.Throws<ArgumentException>(() => operationData.Report(report), "Negative values should not be reported");
        }

        [TestCase(ConfirmationType.Partial, Description = "Tests the partial reporting while the operation is ready. " +
                                                          "Partial reporting should be not possible")]
        [TestCase(ConfirmationType.Final, Description = "Tests the final reporting while the operation is ready. " +
                                                        "After the report, the operation should be completed.")]
        public void ReportWhileReady(ConfirmationType confirmationType)
        {
            // Arrange
            var operationData = GetReadyOperation(10, false, 10, 10);

            var completedRaised = false;
            operationData.Completed += (_, _) => completedRaised = true;

            // Act
            var report = new OperationReport(confirmationType, 5, 5, User);

            switch (confirmationType)
            {
                case ConfirmationType.Partial:
                    // Act - Assert
                    Assert.Throws<InvalidOperationException>(() => operationData.Report(report), "A partial report is not possible for a Ready OperationData.");
                    Assert.That(completedRaised, Is.False);
                    break;
                case ConfirmationType.Final:
                    // Act
                    operationData.Report(report);

                    // Assert
                    // Check possible actions
                    Assert.That(completedRaised);
                    Assert.That(operationData.State.CanBegin, Is.False);
                    Assert.That(operationData.State.CanPartialReport, Is.False);
                    Assert.That(operationData.State.CanFinalReport, Is.False);
                    Assert.That(operationData.State.CanInterrupt, Is.False);
                    break;
            }
        }

        [TestCase(ConfirmationType.Partial, Description = "Tests the partial reporting while the operation is running. " +
                                                          "Should succeed.")]
        [TestCase(ConfirmationType.Final, Description = "Tests the final reporting while the operation is running. " +
                                                        "Final reporting should be not possible")]
        public void ReportWhileRunningByReport(ConfirmationType confirmationType)
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 10, 10);
            var partialReportRaised = false;
            operationData.PartialReport += (_, _) => partialReportRaised = true;

            var report = new OperationReport(confirmationType, 5, 5, User);

            switch (confirmationType)
            {
                case ConfirmationType.Partial:
                    // Act
                    operationData.Report(report);

                    // Assert
                    Assert.That(partialReportRaised);
                    break;
                case ConfirmationType.Final:
                    // Act - Assert
                    Assert.Throws<InvalidOperationException>(() => operationData.Report(report), "A final report is not possible for a running OperationData");

                    Assert.That(partialReportRaised, Is.False);
                    break;
            }
        }

        [TestCase(ConfirmationType.Partial, Description = "Tests the partial reporting while the operation is interrupting. " +
                                                          "Should succeed.")]
        [TestCase(ConfirmationType.Final, Description = "Tests the final reporting while the operation is interrupting. " +
                                                        "Final reporting should be not possible")]
        public void ReportWhileInterruptingByReport(ConfirmationType confirmationType)
        {
            // Arrange
            var operationData = GetInterruptingOperation(10, false, 10, 10);
            var partialReportRaised = false;
            operationData.PartialReport += (_, _) => partialReportRaised = true;

            var report = new OperationReport(confirmationType, 5, 5, User);

            switch (confirmationType)
            {
                case ConfirmationType.Partial:
                    // Act
                    operationData.Report(report);

                    // Assert
                    Assert.That(partialReportRaised);
                    break;
                case ConfirmationType.Final:
                    // Act - Assert
                    Assert.Throws<InvalidOperationException>(() => operationData.Report(report), "A final report is not possible for a running OperationData");

                    Assert.That(partialReportRaised, Is.False);
                    break;
            }
        }

        [TestCase(ConfirmationType.Partial, Description = "Tests interrupting the running operation. Only partial report should be possible.")]
        [TestCase(ConfirmationType.Final, Description = "Tests interrupting the running operation. Final reports should not be possible.")]
        public void ReportWhileRunningByInterrupt(ConfirmationType confirmationType)
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 10, 10);

            var partialReportRaised = false;
            operationData.PartialReport += (_, _) => partialReportRaised = true;

            var report = new OperationReport(confirmationType, 5, 5, User);

            switch (confirmationType)
            {
                case ConfirmationType.Partial:
                    // Act
                    operationData.Interrupt(report);

                    // Assert
                    // Raise partial report
                    Assert.That(partialReportRaised);
                    break;
                case ConfirmationType.Final:
                    // Final reports by interrupt are not allowed
                    // Act - Assert
                    Assert.Throws<InvalidOperationException>(() => operationData.Interrupt(report), "No final report while interrupting the OperationData");
                    Assert.That(partialReportRaised, Is.False);
                    break;
            }
        }

        [TestCase(ConfirmationType.Partial, false, true, Description = "Tests interrupting the amount reached operation. " +
                                                                       "Partial report should be possible. The operation should be interrupted.")]
        [TestCase(ConfirmationType.Final, true, false, Description = "Tests interrupting the amount reached operation. " +
                                                                     "By final reporting, the operation will be completed.")]
        public void ReportWhileAmountReachedByInterrupt(ConfirmationType confirmationType, bool expectedCompleted, bool expectedInterrupted)
        {
            // Arrange
            var operationData = GetAmountReachedOperation(10, false, 10, 10);

            var partialReportRaised = false;
            operationData.PartialReport += (_, _) => partialReportRaised = true;

            var interruptedRaised = false;
            operationData.Interrupted += (_, _) => interruptedRaised = true;

            var completedRaised = false;
            operationData.Completed += (_, _) => completedRaised = true;

            // Act
            var report = new OperationReport(confirmationType, 5, 5, User);
            operationData.Interrupt(report);

            // Assert
            Assert.That(partialReportRaised, Is.False);
            Assert.That(interruptedRaised, Is.EqualTo(expectedInterrupted));
            Assert.That(completedRaised, Is.EqualTo(expectedCompleted));
        }

        [TestCase(ConfirmationType.Partial, false, true, Description = "Tests reporting the amount reached operation. " +
                                                                       "Partial report should be possible. The operation should be interrupted.")]
        [TestCase(ConfirmationType.Final, true, false, Description = "Tests reporting the amount reached operation. " +
                                                                     "By final reporting, the operation will be completed.")]
        public void ReportWhileAmountReachedByReport(ConfirmationType confirmationType, bool expectedCompleted, bool expectedPartialReport)
        {
            // Arrange
            var operationData = GetAmountReachedOperation(10, false, 10, 10);

            var partialReportRaised = false;
            operationData.PartialReport += (_, _) => partialReportRaised = true;

            var completedRaised = false;
            operationData.Completed += (_, _) => completedRaised = true;

            // Act
            var report = new OperationReport(confirmationType, 5, 5, User);
            operationData.Report(report);

            // Assert
            Assert.That(partialReportRaised, Is.EqualTo(expectedPartialReport));
            Assert.That(completedRaised, Is.EqualTo(expectedCompleted));
        }

        [TestCase(ConfirmationType.Partial, Description = "Tests reporting the interrupted operation. Partial report should not be possible")]
        [TestCase(ConfirmationType.Final, Description = "Tests reporting the interrupted operation. " +
                                                        "By final reporting, the operation will be completed.")]
        public void ReportWhileInterruptedByReport(ConfirmationType confirmationType)
        {
            // Arrange
            var operationData = GetInterruptedOperation(10, false, 10, 10);

            var partialReportRaised = false;
            operationData.PartialReport += (_, _) => partialReportRaised = true;

            var completedRaised = false;
            operationData.Completed += (_, _) => completedRaised = true;

            var report = new OperationReport(confirmationType, 0, 0, User);

            switch (confirmationType)
            {
                case ConfirmationType.Partial:
                    // Act - Assert
                    Assert.Throws<InvalidOperationException>(() => operationData.Report(report), "No partial report possible for an interrupted OperationData");
                    Assert.That(partialReportRaised, Is.False);
                    Assert.That(completedRaised, Is.False);
                    break;
                case ConfirmationType.Final:
                    // Act
                    operationData.Report(report);

                    // Assert
                    Assert.That(partialReportRaised, Is.False);
                    Assert.That(completedRaised);
                    break;
            }
        }

        [Test(Description = "Reporting is not possible while creating the operation. " +
                            "Requesting the report context is not possible.")]
        public void GetReportContextThrowsWhileCreating()
        {
            // Arrange
            var operationData = InitializeOperationData(10, false, 11, 9);

            // Act - Assert
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport, Is.False);
            Assert.Throws<InvalidOperationException>(() => operationData.GetReportContext());

            // Arrange
            operationData.Assign();

            // Act - Assert
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport, Is.False);
            Assert.Throws<InvalidOperationException>(() => operationData.GetReportContext());

            // Arrange
            operationData.AssignCompleted(false);

            // Act - Assert
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport, Is.False);
            Assert.Throws<InvalidOperationException>(() => operationData.GetReportContext());
        }

        [Test(Description = "Reporting is not possible while operation is completed. " +
                            "Requesting the report context is not possible.")]
        public void GetReportContextThrowsWhileCompleted()
        {
            // Arrange
            var operationData = GetCompletedOperation(10, false, 11, 9);

            // Act - Assert
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport, Is.False);
            Assert.Throws<InvalidOperationException>(() => operationData.GetReportContext());
        }

        [Test(Description = "Validates the report context while the operation is ready")]
        public void ValidateReportContextWhileReady()
        {
            // Arrange
            var operationData = GetReadyOperation(10, false, 11, 9);

            // Act
            var reportContext = operationData.GetReportContext();

            // Assert
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport);
            Assert.That(reportContext.CanPartial, Is.False);
            Assert.That(reportContext.CanFinal, Is.False);
        }

        [Test(Description = "Validates the report context while the operation is running")]
        public void ValidateReportContextWhileRunning()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 11, 9);

            // Act
            var reportContext = operationData.GetReportContext();

            // Assert
            Assert.That(operationData.State.CanPartialReport);
            Assert.That(operationData.State.CanFinalReport, Is.False);
            Assert.That(reportContext.CanPartial, Is.False);
            Assert.That(reportContext.CanFinal, Is.False);
        }

        [Test(Description = "Validates the report context while the operation amount is reached")]
        public void ValidateReportContextWhileAmountReached()
        {
            // Arrange
            var operationData = GetAmountReachedOperation(10, false, 11, 9);

            // Act
            var reportContext = operationData.GetReportContext();

            // Assert
            Assert.That(operationData.State.CanPartialReport);
            Assert.That(operationData.State.CanFinalReport);
            Assert.That(reportContext.CanPartial, Is.False);
            Assert.That(reportContext.CanFinal, Is.False);
        }

        [Test(Description = "Validates the report context while the operation is interrupted")]
        public void ValidateReportContextWhileInterrupted()
        {
            // Arrange
            var operationData = GetInterruptedOperation(10, false, 11, 9);

            // Act
            var reportContext = operationData.GetReportContext();

            // Assert
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport);
            Assert.That(reportContext.CanPartial, Is.False);
            Assert.That(reportContext.CanFinal, Is.False);
        }

        [Test(Description = "Validates the report context if the operation have an unstarted job.")]
        public void ValidateReportContextWithUnstartedJob()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 11, 9);

            // Act
            var reportContext = operationData.GetReportContext();

            // Assert
            Assert.That(reportContext.CanPartial, Is.False);
            Assert.That(reportContext.CanFinal, Is.False);
            Assert.That(reportContext.SuccessCount, Is.EqualTo(0));
            Assert.That(reportContext.ScrapCount, Is.EqualTo(0));

            Assert.That(reportContext.ReportedSuccess, Is.EqualTo(0));
            Assert.That(reportContext.ReportedFailure, Is.EqualTo(0));
            Assert.That(reportContext.UnreportedSuccess, Is.EqualTo(0));
            Assert.That(reportContext.UnreportedFailure, Is.EqualTo(0));
        }

        [Test(Description = "Validates the report context if the operation have a producing job.")]
        public void ValidateReportContextWithStartedJob()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 11, 9);

            var initialJob = operationData.Operation.Jobs.First();
            initialJob.Classification = JobClassification.Running;
            initialJob.SuccessCount = 3;
            initialJob.FailureCount = 2;
            initialJob.ReworkedCount = 0;
            operationData.JobProgressChanged(initialJob);

            // Act
            var reportContext = operationData.GetReportContext();

            // Assert
            Assert.That(reportContext.SuccessCount, Is.EqualTo(3));
            Assert.That(reportContext.ScrapCount, Is.EqualTo(2));
            Assert.That(reportContext.UnreportedSuccess, Is.EqualTo(3));
            Assert.That(reportContext.UnreportedFailure, Is.EqualTo(2));
        }

        [Test(Description = "Validates the report context if the operation have a producing job and also a job with reworked parts.")]
        public void ValidateReportContextWithStartedReworkJob()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 11, 9);

            var initialJob = operationData.Operation.Jobs.First();
            initialJob.Classification = JobClassification.Running;
            initialJob.SuccessCount = 5;
            initialJob.FailureCount = 2;
            initialJob.ReworkedCount = 3; // one more that FailureCount to check if result will be 0
            operationData.JobProgressChanged(initialJob);

            // Act
            var reportContext = operationData.GetReportContext();

            // Asssert
            Assert.That(reportContext.SuccessCount, Is.EqualTo(5));
            Assert.That(reportContext.ScrapCount, Is.EqualTo(0));
            Assert.That(reportContext.UnreportedSuccess, Is.EqualTo(5));
            Assert.That(reportContext.UnreportedFailure, Is.EqualTo(0));
        }

        [Test(Description = "Validates the report context if the operation have already a reports")]
        public void ValidateReportContextWithStartedJobAfterReport()
        {
            // Arrange
            var operationData = GetRunningOperation(10, false, 11, 9);

            var initialJob = operationData.Operation.Jobs.First();
            initialJob.Classification = JobClassification.Running;
            initialJob.SuccessCount = 3;
            initialJob.FailureCount = 2;
            initialJob.ReworkedCount = 0;

            var report = new OperationReport(ConfirmationType.Partial, 3, 2, User);
            operationData.Report(report);

            // Act
            var reportContext = operationData.GetReportContext();

            // Assert
            Assert.That(reportContext.ReportedSuccess, Is.EqualTo(3));
            Assert.That(reportContext.ReportedFailure, Is.EqualTo(2));
            Assert.That(reportContext.UnreportedSuccess, Is.EqualTo(0));
            Assert.That(reportContext.UnreportedFailure, Is.EqualTo(0));
        }
    }
}

