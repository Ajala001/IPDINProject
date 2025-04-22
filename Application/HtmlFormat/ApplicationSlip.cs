using App.Core.Entities;
using App.Core.Interfaces.Services;

namespace App.Application.HtmlFormat
{
    public class ApplicationSlip(IExaminationService examinationService) : IApplicationSlip
    {
        public async Task<string> HtmlContent(AppApplication appApplication)
        {
            var fullName = $"{appApplication.User.FirstName} {appApplication.User.LastName}";
            var examination = await examinationService.GetExaminationAsync(appApplication.ExaminationId);

            string courseRows = "";
            if (examination?.Data.Courses != null)
            {
                foreach (var course in examination.Data.Courses)
                {
                    courseRows += $@"
                    <tr>
                        <td>{course.CourseCode}</td>
                        <td>{course.CourseTitle}</td>
                        <td>{course.CourseUnit}</td>
                    </tr>";
                }
            }

            string htmlContent = $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Examination Slip</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        color: #333;
                    }}
                    .header {{
                        background-color: #003366;
                        color: #fff;
                        padding: 20px;
                        text-align: center;
                        border-bottom: 4px solid #003366;
                    }}
                    .header img {{
                        max-height: 80px;
                        vertical-align: middle;
                    }}
                    .header h1 {{
                        margin: 10px 0;
                        font-size: 28px;
                        font-weight: bold;
                    }}
                    .content {{
                        padding: 20px;
                        max-width: 800px;
                        margin: auto;
                    }}
                    .content h1 {{
                        font-size: 26px;
                        margin-bottom: 15px;
                        border-bottom: 2px solid #003366;
                        padding-bottom: 10px;
                        color: #003366;
                    }}
                    .content p {{
                        font-size: 18px;
                        margin: 10px 0;
                    }}
                    .table {{
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 20px;
                    }}
                    .table th, .table td {{
                        border: 1px solid #ddd;
                        padding: 12px;
                        text-align: left;
                        font-size: 16px;
                    }}
                    .table th {{
                        background-color: #f4f4f4;
                        color: #003366;
                        font-weight: bold;
                    }}
                    .table tr:nth-child(even) {{
                        background-color: #f9f9f9;
                    }}
                    .footer {{
                        margin-top: 30px;
                        padding: 15px;
                        background-color: #f4f4f4;
                        text-align: center;
                        border-top: 2px solid #003366;
                    }}
                    .footer p {{
                        font-size: 18px;
                        margin: 0;
                        color: #003366;
                    }}
                </style>
            </head>
            <body>
                <div class=""header"">
                   <img src=""school-logo.png"" alt=""School Logo"">
                   <h1>IPDIN Driving Institute</h1> 
                </div>
                <div class=""content"">
                    <h1>Examination Slip</h1>
                    <p><strong>Name:</strong> {fullName}</p>
                    <p><strong>Membership Number:</strong> {appApplication.User.MembershipNumber}</p>
                    <p><strong>Date:</strong> {appApplication.DateApplied:dd/MM/yyyy}</p>

                    <h2>Courses Offered:</h2>

                    <table class=""table"">
                        <thead>
                            <tr>
                                <th>Course Code</th>
                                <th>Course Title</th>
                                <th>Course Unit</th>
                            </tr>
                        </thead>
                        <tbody>
                            {courseRows}
                        </tbody>
                    </table>

                    <div class=""footer"">
                        <p><strong>Total Amount Paid:</strong> {examination!.Data!.Fee:n}</p>
                    </div>
                </div>
            </body>
            </html>
        ";

            return htmlContent;
        }
    }
}
