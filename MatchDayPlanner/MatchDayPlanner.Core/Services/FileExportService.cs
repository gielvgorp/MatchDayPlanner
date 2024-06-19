using OfficeOpenXml;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MatchDayPlanner.Core.SchemePlanner;
using System.Runtime.InteropServices.JavaScript;
using System.Reflection.Metadata;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Drawing;
using Font = iTextSharp.text.Font;

namespace MatchDayPlanner.Core.Services
{
    public class FileExportService
    {
        private readonly GameScheduleService _scheduleService;
        private readonly TeamService _teamService;

        public FileExportService(GameScheduleService c_scheduleService, TeamService c_teamService) 
        { 
            _scheduleService = c_scheduleService;
            _teamService = c_teamService;
        }
        public FileContentResult ExportToExcel(Scheme scheme)
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var schedule = _scheduleService.GetGameScheduleByID(scheme.GetSchemeID());
            var team = _teamService.GetTeamBySchemeID(scheme.GetSchemeID());

            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var matchdays = scheme.GetSchemeLayout();

                worksheet.Cells[2, 1].Value = "Date";
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 1].Style.Font.Size = 20;
                worksheet.Cells[2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                int scheduledCount = schedule.numberOfPresent;
                char endChar = (char)('B' + scheduledCount - 1);
                string cellRange = $"B2:{endChar}2";

                worksheet.Cells[cellRange].Merge = true;
                worksheet.Cells[cellRange].Value = "Ingepland";
                worksheet.Cells[cellRange].Style.Font.Bold = true;
                worksheet.Cells[cellRange].Style.Font.Size = 20;
                worksheet.Cells[cellRange].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[cellRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                endChar++;
                cellRange = $"{endChar}2";

                worksheet.Cells[cellRange].Merge = true;
                worksheet.Cells[cellRange].Value = "Reserve";
                worksheet.Cells[cellRange].Style.Font.Bold = true;
                worksheet.Cells[cellRange].Style.Font.Size = 20;
                worksheet.Cells[cellRange].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[cellRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                endChar++;
                cellRange = $"{endChar}2";

                worksheet.Cells[cellRange].Merge = true;
                worksheet.Cells[cellRange].Value = "Locatie";
                worksheet.Cells[cellRange].Style.Font.Bold = true;
                worksheet.Cells[cellRange].Style.Font.Size = 20;
                worksheet.Cells[cellRange].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[cellRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                endChar++;
                cellRange = $"{endChar}2";

                worksheet.Cells[cellRange].Merge = true;
                worksheet.Cells[cellRange].Value = "Tijd";
                worksheet.Cells[cellRange].Style.Font.Bold = true;
                worksheet.Cells[cellRange].Style.Font.Size = 20;
                worksheet.Cells[cellRange].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[cellRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                int colCount = 0;
                for (int i = 0; i < matchdays.Count; i++)
                {
                    colCount = 1 + matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Scheduled).ToList().Count;

                    // displaying matchday date
                    worksheet.Cells[(i+3), 1].Value = matchdays[i].matchDayDateTime.Date.ToString("dd/MM/yyyy");
                    worksheet.Cells[(i + 3), 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                   
                    // displaying scheduled members
                    for (int k = 0; k < matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Scheduled).ToList().Count; k++)
                    {
                        worksheet.Cells[(i + 3), (k + 2)].Value = matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Scheduled).ToList()[k].memberName;
                        worksheet.Cells[(i + 3), (k + 2)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    // dipslaying reserve members
                    string reserve = "";
                    for (int k = 0; k < matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Reserve).ToList().Count; k++)
                    {
                        reserve += matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Reserve).ToList()[k].memberName + ", ";
                    }

                    if (string.IsNullOrEmpty(reserve)) reserve = "Geen reserve";
                    if (string.IsNullOrEmpty(matchdays[i].location)) matchdays[i].location = "Locatie onbekend";

                    worksheet.Cells[i + 3, colCount+1].Value = reserve;
                    worksheet.Cells[i + 3, colCount + 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells[i + 3, colCount+2].Value = matchdays[i].location;
                    worksheet.Cells[i + 3, colCount+2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells[i + 3, colCount+3].Value = matchdays[i].matchDayDateTime.ToString("HH:mm");
                    worksheet.Cells[i + 3, colCount+3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                worksheet.Column(1).Width = 16;

                // set auto with to all columns
                for (int i = 2; i < (colCount + 3); i++)
                {
                    worksheet.Column(i).AutoFit();
                    worksheet.Column(i).Width += 2;     // add padding
                }

                fileContents = package.GetAsByteArray();
            }

            return new FileContentResult(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = string.Format("Schema van {0} - {1}.xlsx", team.teamName, schedule.scheduleName)
            };
        }

        public IActionResult ExportToPdf(Scheme scheme)
        {
            var schedule = _scheduleService.GetGameScheduleByID(scheme.GetSchemeID());
            var team = _teamService.GetTeamBySchemeID(scheme.GetSchemeID());

            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document();
                var writer = PdfWriter.GetInstance(document, ms);
                // Attach the custom page event helper for background

                document.Open();

                // Adding title
                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                var titleParagraph = new Paragraph("Schema van " + schedule.scheduleName, titleFont);
                titleParagraph.Alignment = Element.ALIGN_CENTER;
                document.Add(titleParagraph);

                // Adding space after the title
                document.Add(new Paragraph("\n"));

                // Adding a table
                var table = new PdfPTable(5)
                {
                    WidthPercentage = 100
                };

                // Adding headers
                var headerFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                table.AddCell(new PdfPCell(new Phrase("Datum", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Tijd", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Location", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Ingepland", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Reserve", headerFont)));

                // Adding data
                foreach (var matchDay in scheme.GetSchemeLayout())
                {
                    string scheduled = string.Empty;
                    for (int k = 0; k < matchDay.schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Scheduled).ToList().Count; k++)
                    {
                        scheduled += matchDay.schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Scheduled).ToList()[k].memberName + ", ";
                    }

                    string reserve = string.Empty;
                    for (int k = 0; k < matchDay.schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Reserve).ToList().Count; k++)
                    {
                        reserve += matchDay.schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Reserve).ToList()[k].memberName + ", ";
                    }

                    if (string.IsNullOrEmpty(reserve)) reserve = "Geen reserve";
                    if (string.IsNullOrEmpty(matchDay.location)) matchDay.location = "Locatie onbekend";

                    table.AddCell(matchDay.matchDayDateTime.ToString("yyyy-MM-dd"));
                    table.AddCell(matchDay.matchDayDateTime.ToString("HH:mm"));
                    table.AddCell(matchDay.location);
                    table.AddCell(scheduled);
                    table.AddCell(reserve);
                }

                document.Add(table);

                // Add footer to each page
                var footerFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
                var footerPhrase = new Phrase("Made by Matchday Planner", footerFont);
                for (int i = 1; i <= writer.PageNumber; i++)
                {
                    ColumnText.ShowTextAligned(writer.DirectContentUnder, Element.ALIGN_CENTER, footerPhrase, 297.5f, 30f, 0);
                }

                document.Close();

                var file = new FileContentResult(ms.ToArray(), "application/pdf")
                {
                    FileDownloadName = string.Format("Schema van {0} - {1}.xlsx", team.teamName, schedule.scheduleName)
                };

                return file;
            }
        }
    }
}
