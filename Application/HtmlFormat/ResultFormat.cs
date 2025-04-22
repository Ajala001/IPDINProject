using App.Core.Entities;
using App.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace App.Application.HtmlFormat
{
    public class ResultFormat(IExaminationService examinationService, UserManager<User> userManager) : IResultFormat
    {
        public async Task<string> HtmlContent(Result result)
        {
            var user = await userManager.FindByEmailAsync(result.User.Email);
            var fullname = $"{user.FirstName} {user.LastName}";
            var examination = await examinationService.GetExaminationAsync(result.BatchResult.ExaminationId);

            var courseScores = JsonConvert.DeserializeObject<Dictionary<string, int>>(result.Breakdown);
            double totalGradePoints = 0;
            int totalUnits = 0;
            int serialNumber = 1;

            string courseRows = "";
            foreach (var course in examination.Data.Courses)
            {
                int score = courseScores.ContainsKey(course.CourseCode) ? courseScores[course.CourseCode] : 0;
                var gradeData = CalculateGrade(score);
                int courseUnit = int.Parse(course.CourseUnit);

                totalGradePoints += gradeData.GradePoint * courseUnit;
                totalUnits += courseUnit;

                courseRows += $@"
        <tr>
            <td>{serialNumber++}</td>
            <td>{course.CourseCode}</td>
            <td>{course.CourseTitle}</td>
            <td>{course.CourseUnit}</td>
            <td>{gradeData.Grade}</td>
            <td>{gradeData.GradePoint}</td>
            <td>{score}</td>
        </tr>";
            }

            double gpa = totalUnits > 0 ? totalGradePoints / totalUnits : 0;

            string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Exam Results</title>
    <style>
        .container {{
            max-width: 800px;
            margin: auto;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            padding: 20px;
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 20px;
            background-color: #f9f9f9;
        }}
        header, footer {{
            text-align: center;
            margin-bottom: 20px;
        }}
        header h1 {{
            font-size: 25px;
            color: #003366;
        }}
        header h2 {{
            color: #003366;
        }}
        .student-info, .result-details {{
            margin-bottom: 20px;
        }}
        .student-info {{
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 20px;
            border: 1px solid #ddd;
            border-radius: 10px;
            padding: 15px;
            background-color: #003366; /* Dark Blue */
            color: #ffffff; /* White text */
        }}
        .student-image {{flex - shrink: 0;
            text-align: right;
        }}

        .student-image img {{width: 120px;
            height: 120px;
            border-radius: 10px;
            border: 3px solid #ffffff; /* White border for contrast */
            object-fit: cover;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
        }}
        th, td {{
            padding: 10px;
            border: 1px solid #ddd;
            text-align: center;
        }}
        th {{
            background-color: #003366;
            color: #fff;
        }}
        .summary {{
            text-align: center;
            font-weight: bold;
            margin-top: 20px;
        }}
        footer {{
            font-size: 12px;
            color: #555;
        }}
    </style>
</head>
    <div class=""container"">
        <header>
            <h1>THE FEDERAL POLYTECHNIC, ILARO</h1>
            <h2>Statement of Examination Results</h2>
            <p><strong>{examination.Data.ExamTitle} {examination.Data.ExamYear}</strong></p>
        </header>
        <div class=""student-info"">
            <div class=""student-details"">
                <p><strong>Name:</strong> {fullname}</p>
                <p><strong>Membership No:</strong> {user.MembershipNumber}</p>
                <p><strong>Email:</strong> {user.Email}</p>
                <p><strong>Phone:</strong> {user.PhoneNumber}</p>
            </div>
            <div class=""student-image"">
            <img src=""{user.ProfilePic ?? "default-image.jpg"}"" alt=""Profile Picture"">
            </div>
        </div>
        <div class=""result-details"">
            <table>
                <thead>
                    <tr>
                        <th>SN</th>
                        <th>Code</th>
                        <th>Title</th>
                        <th>Unit</th>
                        <th>Grade</th>
                        <th>Points</th>
                        <th>Score</th>
                    </tr>
                </thead>
                <tbody>
                    {courseRows}
                </tbody>
            </table>
        </div>
        <div class=""summary"">
            <p>Units Attempted: {totalUnits} | Grade Points: {totalGradePoints:F2} | GPA: {gpa:F2}</p>
        </div>
        <footer>
            <p>Registrar</p>
            <p><strong>Warning:</strong> Any alteration renders this result invalid.</p>
        </footer>
    </div>
</html>";

            return htmlContent;
        }

        private (string Grade, int GradePoint) CalculateGrade(int score)
        {
            if (score >= 70) return ("A", 5);
            if (score >= 60) return ("B", 4);
            if (score >= 50) return ("C", 3);
            if (score >= 45) return ("D", 2);
            return ("F", 0);
        }

    }
}
