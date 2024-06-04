using OfficeOpenXml;
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MatchDayPlanner.Core.SchemePlanner;
using System.Runtime.InteropServices.JavaScript;

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
                        reserve.CompareTo(matchdays[i].schemeLayout.Where(s => s.memberPrevention == TeamMember.PreventionStatus.Reserve).ToList()[k].memberName + ", ");
                    }

                    worksheet.Cells[i + 3, 5].Value = reserve;

                    worksheet.Cells[i + 3, 7].Value = matchdays[i].matchDayDateTime.Hour + ":" + matchdays[i].matchDayDateTime.Minute;
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
    }
}
