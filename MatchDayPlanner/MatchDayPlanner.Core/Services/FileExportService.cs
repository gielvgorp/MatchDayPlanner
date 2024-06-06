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
        public FileContentResult ExportToExcel(Scheme scheme)
        {
            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var matchdays = scheme.GetSchemeLayout();

                worksheet.Cells[2, 1].Value = "Date";
                worksheet.Cells[2, 1].Style.Font.Bold = true;
                worksheet.Cells[2, 1].Style.Font.Size = 20;

                worksheet.Cells["B2:D2"].Merge = true;
                worksheet.Cells["B2:D2"].Value = "Ingepland";
                worksheet.Cells["B2:D2"].Style.Font.Bold = true;
                worksheet.Cells["B2:D2"].Style.Font.Size = 20;
                worksheet.Cells["B2:D2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells["B2:D2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 5].Merge = true;
                worksheet.Cells[2, 5].Value = "Reserve";
                worksheet.Cells[2, 5].Style.Font.Bold = true;
                worksheet.Cells[2, 5].Style.Font.Size = 20;
                worksheet.Cells[2, 5].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[2, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 6].Merge = true;
                worksheet.Cells[2, 6].Value = "Locatie";
                worksheet.Cells[2, 6].Style.Font.Bold = true;
                worksheet.Cells[2, 6].Style.Font.Size = 20;
                worksheet.Cells[2, 6].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[2, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 7].Merge = true;
                worksheet.Cells[2, 7].Value = "Tijd";
                worksheet.Cells[2, 7].Style.Font.Bold = true;
                worksheet.Cells[2, 7].Style.Font.Size = 20;
                worksheet.Cells[2, 7].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells[2, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                for (int i = 0; i < matchdays.Count; i++)
                {
                    worksheet.Cells[(i+3), 1].Value = matchdays[i].matchDayDateTime.Date.ToString("dd/MM/yyyy");
                    worksheet.Cells[(i + 3), 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                   

                    for (int k = 0; k < matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Scheduled).ToList().Count; k++)
                    {
                        worksheet.Cells[(i + 3), (k + 2)].Value = matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Scheduled).ToList()[k].memberName;
                        worksheet.Cells[(i + 3), (k + 2)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    string reserve = "";
                    for (int k = 0; k < matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Reserve).ToList().Count; k++)
                    {
                        reserve += matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Reserve).ToList()[k].memberName + ", ";
                    }

                    worksheet.Cells[i + 3, 5].Value = reserve;

                    worksheet.Cells[i + 3, 6].Value = matchdays[i].location;
                    worksheet.Cells[i + 3, 7].Value = matchdays[i].matchDayDateTime.ToString("HH:mm");
                }

                worksheet.Column(1).Width = 16;
                worksheet.Column(2).AutoFit();
                worksheet.Column(3).AutoFit();
                worksheet.Column(4).AutoFit(); 
                worksheet.Column(5).AutoFit();   
                worksheet.Column(6).AutoFit();   
                worksheet.Column(7).AutoFit();   

                // Convert the Excel package to a byte array
                fileContents = package.GetAsByteArray();
            }

            // Return the file as a download
            return new FileContentResult(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = "Schedule.xlsx"
            };
        }

        public IActionResult ExportToPdf(Scheme scheme)
        {
            using (var ms = new MemoryStream())
            {
                var document = new iTextSharp.text.Document();
                var writer = PdfWriter.GetInstance(document, ms);
                // Attach the custom page event helper for background

                document.Open();

                // Adding title
                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                var titleParagraph = new Paragraph("Scheme", titleFont);
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
                    FileDownloadName = "Scheme.pdf"
                };
                return file;
            }
        }
    }
}
