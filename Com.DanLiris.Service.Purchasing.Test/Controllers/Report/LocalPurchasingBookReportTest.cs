﻿using Com.DanLiris.Service.Purchasing.Lib.Facades.Report;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using Com.DanLiris.Service.Purchasing.WebApi.Controllers.v1.Report;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Com.DanLiris.Service.Purchasing.Test.Controllers.Report
{
    public class LocalPurchasingBookReportTest
    {
        protected int GetStatusCode(IActionResult response)
        {
            return (int)response.GetType().GetProperty("StatusCode").GetValue(response, null);
        }

        [Fact]
        public async Task Should_Success_GetLocalPurchasingBookReport()
        {
            var mockFacade = new Mock<ILocalPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ReturnsAsync(new LocalPurchasingBookReportViewModel());

            var controller = new LocalPurchasingBookReportController(mockFacade.Object);
            var response = await controller.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.OK, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Failed_GetLocalPurchasingBookReport_WithException()
        {
            var mockFacade = new Mock<ILocalPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ThrowsAsync(new Exception());

            var controller = new LocalPurchasingBookReportController(mockFacade.Object);
            var response = await controller.Get(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_GetLocalPurchasingBookReportXls()
        {
            var mockFacade = new Mock<ILocalPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ReturnsAsync(new MemoryStream());

            var controller = new LocalPurchasingBookReportController(mockFacade.Object);
            var response = await controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), false);

            Assert.NotNull(response);

            response = await controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), true);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Failed_GetLocalPurchasingBookReportXls_WithException()
        {
            var mockFacade = new Mock<ILocalPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GenerateExcel(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ThrowsAsync(new Exception());

            var controller = new LocalPurchasingBookReportController(mockFacade.Object);
            var response = await controller.GetXls(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }

        [Fact]
        public async Task Should_Success_GetLocalPurchasingBookReport_Pdf()
        {
            var mockFacade = new Mock<ILocalPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ReturnsAsync(new LocalPurchasingBookReportViewModel()
            {
                CategorySummaries = new List<Summary>() { new Summary() },
                CategorySummaryTotal = 1,
                CurrencySummaries = new List<Summary>() { new Summary() },
                GrandTotal = 1,
                Reports = new List<PurchasingReport>() { new PurchasingReport() }
            });

            var controller = new LocalPurchasingBookReportController(mockFacade.Object);
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";
            var response = await controller.GetPdf(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), false);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Success_GetLocalPurchasingBookReport_Pdf_ForeignCurrency()
        {
            var mockFacade = new Mock<ILocalPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ReturnsAsync(new LocalPurchasingBookReportViewModel()
            {
                CategorySummaries = new List<Summary>() { new Summary() },
                CategorySummaryTotal = 1,
                CurrencySummaries = new List<Summary>() { new Summary() },
                GrandTotal = 1,
                Reports = new List<PurchasingReport>() { new PurchasingReport() }
            });

            var controller = new LocalPurchasingBookReportController(mockFacade.Object);
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";
            var response = await controller.GetPdf(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), true);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task Should_Failed_GetLocalPurchasingBookReport_Pdf_WithException()
        {
            var mockFacade = new Mock<ILocalPurchasingBookReportFacade>();
            mockFacade.Setup(facade => facade.GetReport(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>())).ThrowsAsync(new Exception());

            var controller = new LocalPurchasingBookReportController(mockFacade.Object);
            controller.ControllerContext.HttpContext.Request.Headers["x-timezone-offset"] = "1";
            var response = await controller.GetPdf(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>());

            Assert.Equal((int)HttpStatusCode.InternalServerError, GetStatusCode(response));
        }
    }
}
